
// scripts/CoffeeMachine.cs
using Godot;

public partial class CoffeeMachine : Node2D
{
	// State instances (Properties with PascalCase)
	public IState IdleState { get; private set; } = new IdleState();
	public IState GrindingState { get; private set; } = new GrindingState();
	public IState HeatingState { get; private set; } = new HeatingState();
	public IState BrewingState { get; private set; } = new BrewingState();
	private IState _currentState;

	// Scene node references (private fields with _camelCase)
	[Export] private ColorRect _body;
	[Export] private ColorRect _heaterIndicator;
	[Export] private CpuParticles2D _brewingParticles; // Corrected class name
	[Export] private Label _statusLabel;
	[Export] private Button _startButton;

	public override void _Ready()
	{
		_startButton.Pressed += OnStartButtonPressed;
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

	// --- Public methods for states to control visuals (PascalCase) ---
	public void UpdateStatusText(string text) => _statusLabel.Text = text;
	public void SetBrewingParticles(bool emitting) => _brewingParticles.Emitting = emitting;
	public void SetHeaterIndicatorColor(Color color) => _heaterIndicator.Color = color;
	public void SetStartButtonEnabled(bool enabled) => _startButton.Disabled = !enabled;
}
