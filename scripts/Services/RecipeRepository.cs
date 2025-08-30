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

	private FirebaseManager _firebaseManager;
	private readonly Dictionary<string, BrewLogRecord> _recipeCache = new();
	private const string FirestoreProjectId = "brewfolio-f5a11"; // 請換成您自己的 Project ID
	
	// 明確指定使用 System.Net.Http.HttpClient 來避免命名衝突
	private readonly System.Net.Http.HttpClient _httpClient = new();

	public override void _Ready()
	{
		_firebaseManager = GetNode<FirebaseManager>("/root/FirebaseManager");
	}

	public List<BrewLogRecord> GetCachedRecipes()
	{
		return new List<BrewLogRecord>(_recipeCache.Values);
	}

	public async Task<bool> SaveRecipeAsync(BrewLogRecord record)
	{
		if (_firebaseManager.CurrentUser == null)
		{
			GD.PrintErr("RecipeRepository: 無法儲存，使用者未登入。");
			return false;
		}

		if (string.IsNullOrEmpty(record.Id))
		{
			record.Id = Guid.NewGuid().ToString();
		}

		_recipeCache[record.Id] = record;
		EmitSignal(SignalName.RecipesUpdated);

		try
		{
			string userId = _firebaseManager.CurrentUser.User.LocalId;
			string token = _firebaseManager.CurrentUser.FirebaseToken;
			// Firestore REST API v1 的路徑格式
			string url = $"https://firestore.googleapis.com/v1/projects/{FirestoreProjectId}/databases/(default)/documents/users/{userId}/recipes/{record.Id}";

			var firestoreDoc = record.ToFirestoreDocument();
			var jsonPayload = JsonConvert.SerializeObject(firestoreDoc);

			var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
			// 使用 PATCH 進行寫入或更新
			var request = new HttpRequestMessage(new HttpMethod("PATCH"), url);
			request.Content = content;
			request.Headers.Add("Authorization", $"Bearer {token}");

			var response = await _httpClient.SendAsync(request);

			if (response.IsSuccessStatusCode)
			{
				GD.Print($"配方 '{record.RecipeName}' 已成功儲存至 Firestore。");
				return true;
			}
			else
			{
				var errorBody = await response.Content.ReadAsStringAsync();
				GD.PrintErr($"儲存配方至 Firestore 失敗。狀態: {response.StatusCode}, 內容: {errorBody}");
				return false;
			}
		}
		catch (Exception ex)
		{
			GD.PrintErr($"儲存配方時發生例外狀況: {ex.Message}");
			return false;
		}
	}

	public async Task FetchRecipesFromServerAsync()
	{
		if (_firebaseManager.CurrentUser == null)
		{
			GD.PrintErr("RecipeRepository: 無法獲取配方，使用者未登入。");
			return;
		}

		try
		{
			string userId = _firebaseManager.CurrentUser.User.LocalId;
			string token = _firebaseManager.CurrentUser.FirebaseToken;
			string url = $"https://firestore.googleapis.com/v1/projects/{FirestoreProjectId}/databases/(default)/documents/users/{userId}/recipes";

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
						if (record != null)
						{
							_recipeCache[record.Id] = record;
						}
					}
				}
				GD.Print($"從 Firestore 成功獲取 {_recipeCache.Count} 筆配方。");
				EmitSignal(SignalName.RecipesUpdated);
			}
			else
			{
				var errorBody = await response.Content.ReadAsStringAsync();
				GD.PrintErr($"從 Firestore 獲取配方失敗。狀態: {response.StatusCode}, 內容: {errorBody}");
			}
		}
		catch (Exception ex)
		{
			GD.PrintErr($"獲取配方時發生例外狀況: {ex.Message}");
		}
	}

	public void ClearLocalCache()
	{
		_recipeCache.Clear();
		GD.Print("RecipeRepository: 本地快取已清除。");
		EmitSignal(SignalName.RecipesUpdated);
	}
}

// 用於 Firestore REST API 反序列化的輔助類別
public class FirestoreCollectionResponse
{
	public List<FirestoreDocument> documents { get; set; }
}
