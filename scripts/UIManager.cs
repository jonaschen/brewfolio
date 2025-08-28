// scripts/UIManager.cs
using Godot;
using System.Globalization;
using System.Collections.Generic;

public partial class UIManager : CanvasLayer
{
    [Export] private CoffeeMachine _coffeeMachine;
    
    // --- UI Node References ---
    [Export] private ColorRect _heaterIndicator;
    [Export] private CpuParticles2D _brewingParticles;
    [Export] private Label _statusLabel;
    [Export] private Button _startButton;
    private Button _logoutButton;
    private VBoxContainer _recipeListContainer; // To display recipes

    // --- Save Form Nodes ---
    [Export] private PanelContainer _saveRecipePanel;
    [Export] private LineEdit _recipeNameEdit;
    [Export] private LineEdit _beanNameEdit;
    [Export] private TextEdit _notesEdit;
    [Export] private LineEdit _doseEdit;
    [Export] private LineEdit _yieldEdit;
    [Export] private LineEdit _timeEdit;
    [Export] private Button _saveButton;

    // --- Services ---
    private RecipeRepository _recipeRepository;
    private UserProfileService _userProfileService;
    private FirebaseManager _firebaseManager;

    // --- State ---
    private BrewLogRecord _currentLogRecord;

    public override async void _Ready()
    {
        // Get service singletons
        _recipeRepository = GetNode<RecipeRepository>("/root/RecipeRepository");
        _userProfileService = GetNode<UserProfileService>("/root/UserProfileService");
        _firebaseManager = GetNode<FirebaseManager>("/root/FirebaseManager");

        // Subscribe to machine and repository events
        _coffeeMachine.OnStateChanged += HandleStateChanged;
        _coffeeMachine.OnBrewCompleted += HandleBrewCompleted;
        _recipeRepository.RecipesUpdated += UpdateRecipeList;
        
        // Connect UI signals
        _startButton.Pressed += OnStartButtonPressed;
        _saveButton.Pressed += OnSaveButtonPressed;
        
        _saveRecipePanel.Visible = false;

        // Create and configure the logout button
        _logoutButton = new Button();
        _logoutButton.Text = "Logout";
        _logoutButton.Position = new Vector2(GetViewportRect().Size.X - 120, 20);
        AddChild(_logoutButton);
        _logoutButton.Pressed += OnLogoutButtonPressed;

        // Create the recipe list container
        _recipeListContainer = new VBoxContainer();
        _recipeListContainer.Position = new Vector2(20, 60);
        _recipeListContainer.Size = new Vector2(300, 400);
        AddChild(_recipeListContainer);

        // Initial UI update and data fetch
        UpdateRecipeList();
        await _recipeRepository.FetchRecipesFromServer();
    }

    private void UpdateRecipeList()
    {
        // Clear existing recipe entries
        foreach (Node child in _recipeListContainer.GetChildren())
        {
            child.QueueFree();
        }

        // Add a header
        var header = new Label();
        header.Text = "Your Recipes:";
        _recipeListContainer.AddChild(header);

        // Get recipes from the cache and display them
        List<BrewLogRecord> recipes = _recipeRepository.GetCachedRecipes();
        if (recipes.Count == 0)
        {
            var emptyLabel = new Label();
            emptyLabel.Text = "No recipes saved yet.";
            _recipeListContainer.AddChild(emptyLabel);
        }
        else
        {
            foreach (var recipe in recipes)
            {
                var recipeLabel = new Label();
                recipeLabel.Text = $"- {recipe.RecipeName} ({recipe.Dose}g -> {recipe.Yield}g)";
                _recipeListContainer.AddChild(recipeLabel);
            }
        }
    }

    private void HandleStateChanged(IState newState)
    {
        if (newState is IdleState) _statusLabel.Text = "Idle. Press Start.";
        else if (newState is GrindingState) _statusLabel.Text = "Grinding...";
        else if (newState is HeatingState) _statusLabel.Text = "Heating...";
        else if (newState is BrewingState) _statusLabel.Text = "Brewing...";

        _startButton.Disabled = !(newState is IdleState);
        _heaterIndicator.Color = (newState is HeatingState) ? new Color(1.0f, 0.5f, 0.5f) : new Color(0.5f, 0.5f, 1.0f);
        _brewingParticles.Emitting = (newState is BrewingState);
    }
    
    private void HandleBrewCompleted(SimulationParameters finalSimParams)
    {
        _currentLogRecord = new BrewLogRecord
        {
            Dose = finalSimParams.Dose,
            Yield = finalSimParams.TargetYield,
            BrewTime = finalSimParams.CurrentBrewTime,
        };
        
        _doseEdit.Text = _currentLogRecord.Dose.ToString(CultureInfo.InvariantCulture);
        _yieldEdit.Text = _currentLogRecord.Yield.ToString(CultureInfo.InvariantCulture);
        _timeEdit.Text = _currentLogRecord.BrewTime.ToString(CultureInfo.InvariantCulture);
        
        _recipeNameEdit.Text = "";
        _beanNameEdit.Text = "";
        _notesEdit.Text = "";

        _saveRecipePanel.Visible = true;
    }

    private void OnStartButtonPressed()
    {
        _coffeeMachine.StartBrewing();
    }
    
    private async void OnSaveButtonPressed()
    {
        if (_currentLogRecord == null) return;
        
        _currentLogRecord.RecipeName = _recipeNameEdit.Text;
        _currentLogRecord.BeanName = _beanNameEdit.Text;
        _currentLogRecord.Notes = _notesEdit.Text;
        _currentLogRecord.Dose = float.Parse(_doseEdit.Text, CultureInfo.InvariantCulture);
        _currentLogRecord.Yield = float.Parse(_yieldEdit.Text, CultureInfo.InvariantCulture);
        _currentLogRecord.BrewTime = float.Parse(_timeEdit.Text, CultureInfo.InvariantCulture);

        GD.Print("Saving recipe...");
        await _recipeRepository.SaveRecipe(_currentLogRecord);
        
        _saveRecipePanel.Visible = false;
        _currentLogRecord = null;
    }

    private void OnLogoutButtonPressed()
    {
        GD.Print("Logging out...");
        _recipeRepository.ClearLocalCache();
        _userProfileService.ClearProfile();
        _firebaseManager.SignOut();
        GetTree().ChangeSceneToFile("res://scenes/auth_scene.tscn");
    }
}
