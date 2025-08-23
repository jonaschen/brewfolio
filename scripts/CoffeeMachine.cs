
// scripts/CoffeeMachine.cs
using Godot;

public partial class CoffeeMachine : Node2D
{
	// State instances (Properties with PascalCase)
	public IState IdleState { get; private set; } = new IdleState();
	public IState GrindingState { get; private set; } = new GrindingState();
	public IState HeatingState { get; private set; } = new HeatingState();
	public IState BrewingState { get; private set; } = new BrewingState();
	public IState BrewCompleteState { get; private set; } = new BrewCompleteState();
	private IState _currentState;

	// Scene node references (private fields with _camelCase)
	[Export] private ColorRect _body;
	[Export] private ColorRect _heaterIndicator;
	[Export] private CpuParticles2D _brewingParticles; // Corrected class name
	[Export] private Label _statusLabel;
	[Export] private Button _startButton;
	[Export] private PanelContainer _saveRecipePanel;
	[Export] private LineEdit _recipeNameEdit;
	[Export] private LineEdit _beanNameEdit;
	[Export] private TextEdit _notesEdit;
	[Export] private Button _saveButton;

	public override void _Ready()
	{
		_startButton.Pressed += OnStartButtonPressed;
		_saveButton.Pressed += OnSaveButtonPressed; // 連接儲存按鈕的訊號
		TransitionToState(IdleState);
	}

	public override void _Process(double delta)
	{
		_currentState?.Execute(this);
	}
	
	public void TransitionToState(IState nextState)
	{
		_currentState?.Exit(this);
		_currentState = nextState;
		_currentState.Enter(this);
	}

	private void OnStartButtonPressed() // Method with PascalCase
	{
		if (_currentState == IdleState)
		{
			TransitionToState(GrindingState);
		}
	}

	private void OnSaveButtonPressed()
	{
		// Task 4: 實現儲存邏輯 (初步)
		var recipe = new BrewRecipe
		{
			RecipeName = _recipeNameEdit.Text,
			BeanName = _beanNameEdit.Text,
			Notes = _notesEdit.Text
		};

		string jsonOutput = recipe.ToJson();
		GD.Print("--- New Recipe Saved (Simulated) ---");
		GD.Print(jsonOutput);
		GD.Print("------------------------------------");

		// 儲存後，回到待機狀態
		TransitionToState(IdleState);
	}

	// --- Public methods for states to control visuals (PascalCase) ---
	public void UpdateStatusText(string text) => _statusLabel.Text = text;
	public void SetBrewingParticles(bool emitting) => _brewingParticles.Emitting = emitting;
	public void SetHeaterIndicatorColor(Color color) => _heaterIndicator.Color = color;
	public void SetStartButtonEnabled(bool enabled) => _startButton.Disabled = !enabled;
	public void SetSaveRecipePanelVisibility(bool visible) => _saveRecipePanel.Visible = visible;
}
