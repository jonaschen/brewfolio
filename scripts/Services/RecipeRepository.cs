// scripts/Services/RecipeRepository.cs
using Godot;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

public partial class RecipeRepository : Node
{
    [Signal]
    public delegate void RecipesUpdatedEventHandler();

    // --- Services ---
    private FirebaseManager _firebaseManager;

    // --- State ---
    private readonly Dictionary<string, BrewLogRecord> _recipeCache = new();
    private const string FirestoreProjectId = "YOUR_PROJECT_ID"; // IMPORTANT: User needs to fill this in.
    private readonly HttpClient _httpClient = new();

    public override void _Ready()
    {
        _firebaseManager = GetNode<FirebaseManager>("/root/FirebaseManager");
    }

    public List<BrewLogRecord> GetCachedRecipes()
    {
        return new List<BrewLogRecord>(_recipeCache.Values);
    }

    public async Task<bool> SaveRecipe(BrewLogRecord record)
    {
        if (_firebaseManager.CurrentUser == null)
        {
            GD.PrintErr("RecipeRepository: Cannot save recipe, user not logged in.");
            return false;
        }

        // If the record is new, generate an ID.
        if (string.IsNullOrEmpty(record.Id))
        {
            record.Id = Guid.NewGuid().ToString();
        }

        // Update local cache first for immediate UI feedback.
        _recipeCache[record.Id] = record;
        EmitSignal(SignalName.RecipesUpdated);

        // Then, save to Firestore asynchronously.
        try
        {
            string userId = _firebaseManager.CurrentUser.User.LocalId;
            string token = _firebaseManager.CurrentUser.FirebaseToken;
            string url = $"https://firestore.googleapis.com/v1/projects/{FirestoreProjectId}/databases/(default)/documents/users/{userId}/recipes/{record.Id}?key={_firebaseManager.GetApiKey()}";

            var firestoreDoc = record.ToFirestoreDocument();
            var jsonPayload = JsonConvert.SerializeObject(firestoreDoc);

            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(new HttpMethod("PATCH"), url);
            request.Content = content;
            request.Headers.Add("Authorization", $"Bearer {token}");

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                GD.Print($"Recipe '{record.RecipeName}' saved to Firestore successfully.");
                return true;
            }
            else
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                GD.PrintErr($"Failed to save recipe to Firestore. Status: {response.StatusCode}, Body: {errorBody}");
                // Optional: Revert local cache change on failure.
                // _recipeCache.Remove(record.Id);
                // EmitSignal(SignalName.RecipesUpdated);
                return false;
            }
        }
        catch (Exception ex)
        {
            GD.PrintErr($"An exception occurred while saving recipe: {ex.Message}");
            return false;
        }
    }

    public async Task FetchRecipesFromServer()
    {
        if (_firebaseManager.CurrentUser == null)
        {
            GD.PrintErr("RecipeRepository: Cannot fetch recipes, user not logged in.");
            return;
        }

        try
        {
            string userId = _firebaseManager.CurrentUser.User.LocalId;
            string token = _firebaseManager.CurrentUser.FirebaseToken;
            string url = $"https://firestore.googleapis.com/v1/projects/{FirestoreProjectId}/databases/(default)/documents/users/{userId}/recipes?key={_firebaseManager.GetApiKey()}";

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Authorization", $"Bearer {token}");

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var firestoreResponse = JsonConvert.DeserializeObject<FirestoreCollectionResponse>(jsonString);

                _recipeCache.Clear();
                if (firestoreResponse?.documents != null)
                {
                    foreach (var doc in firestoreResponse.documents)
                    {
                        var record = BrewLogRecord.FromFirestoreDocument(doc);
                        _recipeCache[record.Id] = record;
                    }
                }
                GD.Print($"Fetched {_recipeCache.Count} recipes from Firestore.");
                EmitSignal(SignalName.RecipesUpdated);
            }
            else
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                GD.PrintErr($"Failed to fetch recipes from Firestore. Status: {response.StatusCode}, Body: {errorBody}");
            }
        }
        catch (Exception ex)
        {
            GD.PrintErr($"An exception occurred while fetching recipes: {ex.Message}");
        }
    }

    public void ClearLocalCache()
    {
        _recipeCache.Clear();
        GD.Print("RecipeRepository: Local cache cleared.");
        EmitSignal(SignalName.RecipesUpdated);
    }
}

// Helper classes for Firestore REST API deserialization
public class FirestoreCollectionResponse
{
    public List<FirestoreDocument> documents { get; set; }
}
