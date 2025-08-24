
// scripts/CoffeeMachine.cs
using Godot;
using System; // 需要這個來使用 Action

public partial class CoffeeMachine : Node2D
{
	// --- 事件定義 ---
	// 當狀態轉換時觸發
	public event Action<IState> OnStateChanged;
	// 當沖煮完成時觸發
	public event Action<SimulationParameters> OnBrewCompleted;

	// State instances (Properties with PascalCase)
	public IState IdleState { get; private set; } = new IdleState();
	public IState GrindingState { get; private set; } = new GrindingState();
	public IState HeatingState { get; private set; } = new HeatingState();
	public IState BrewingState { get; private set; } = new BrewingState();
	public IState BrewCompleteState { get; private set; } = new BrewCompleteState();
	private IState _currentState;

	// Scene node references (private fields with _camelCase)
	//[Export] private ColorRect _body;
	//[Export] private ColorRect _heaterIndicator;
	//[Export] private CpuParticles2D _brewingParticles; // Corrected class name
	//[Export] private Label _statusLabel;
	//[Export] private Button _startButton;
	//[Export] private PanelContainer _saveRecipePanel;
	//[Export] private LineEdit _recipeNameEdit;
	//[Export] private LineEdit _beanNameEdit;
	//[Export] private TextEdit _notesEdit;
	//[Export] private Button _saveButton;

	// 用於儲存當前模擬的即時數據
	private SimulationParameters _currentSimParams;

	public override void _Ready()
	{
	//	_startButton.Pressed += OnStartButtonPressed;
	//	_saveButton.Pressed += OnSaveButtonPressed; // 連接儲存按鈕的訊號
		TransitionToState(IdleState);
	}

	// 提供一個公開的方法讓外部 (UIManager) 啟動流程
	public void StartBrewing()
	{
		GD.Print("CoffeeMachine: Start Brewing!"); 
		if (_currentState == IdleState)
		{
			GD.Print("CoffeeMachine: Current state is idle!"); 
			_currentSimParams = new SimulationParameters();
			
			// 使用新定義的 TargetBrewTime 屬性
			// 總時長是固定的狀態時間加總
			_currentSimParams.TargetBrewTime = 2.0f + 3.0f + 5.0f; // Grinding(2) + Heating(3) + Brewing(5)
			
			TransitionToState(GrindingState);
		}
	}

	public override void _Process(double delta)
	{
		_currentState?.Execute(this);
	}
	
	public void TransitionToState(IState nextState)
	{
		_currentState?.Exit(this);
		_currentState = nextState;

		//// 在狀態轉換時，根據下一個狀態來更新 UI
		//UpdateVisualsForState(nextState);

		_currentState.Enter(this);
		OnStateChanged?.Invoke(_currentState);
	}

	//private void UpdateVisualsForState(IState state)
	//{
	//    if (state is IdleState)
	//    {
	//        _heaterIndicator.Color = new Color(0.5f, 0.5f, 1.0f); // Blueish
	//    }
	//    else
	//    {
	//        // 預設在非待機狀態時，禁用開始按鈕

	//        if (state is HeatingState)
	//        {
	//            _heaterIndicator.Color = new Color(1.0f, 0.5f, 0.5f); // Reddish
	//        }
	//        else if (state is BrewingState)
	//        {
	//        }
	//    }
	//}

	//private void OnStartButtonPressed() // Method with PascalCase
	//{
	//	if (_currentState == IdleState)
	//	{
	//		TransitionToState(GrindingState);
	//	}
	//}

	//private void OnSaveButtonPressed()
	//{
	//	// Task 4: 實現儲存邏輯 (初步)
	//	var recipe = new BrewRecipe
	//	{
	//		RecipeName = _recipeNameEdit.Text,
	//		BeanName = _beanNameEdit.Text,
	//		Notes = _notesEdit.Text
	//	};

	//	string jsonOutput = recipe.ToJson();
	//	GD.Print("--- New Recipe Saved (Simulated) ---");
	//	GD.Print(jsonOutput);
	//	GD.Print("------------------------------------");

	//	// 儲存後，回到待機狀態
	//	TransitionToState(IdleState);
	//}

	//private void OnStartButtonPressed()
	//{
	//    if (_currentState == IdleState)
	//    {
	//        // 在開始流程時，建立一個新的模擬數據實例
	//        _currentSimParams = new SimulationParameters();
	//        TransitionToState(GrindingState);
	//    }
	//}
	//
	// BrewingState 完成後會呼叫這個方法
	public void FinishBrewing()
	{
		GD.Print("Brewing finished. Emitting OnBrewCompleted event.");
		// 觸發事件，將最終的數據廣播出去
		OnBrewCompleted?.Invoke(_currentSimParams);

		// 流程結束，回到待機狀態
		TransitionToState(IdleState);
	}
	
	//// --- Public methods for states to control visuals (PascalCase) ---
	//public void UpdateStatusText(string text)
	//{
	//    _statusLabel.Text = text;
	//}

}
