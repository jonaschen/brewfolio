// scripts/UIManager.cs
using Godot;
using System.Globalization;

public partial class UIManager : CanvasLayer
{
    [Export] private CoffeeMachine _coffeeMachine;
    
    // --- 所有 UI 節點都由 UIManager 管理 ---
    [Export] private ColorRect _heaterIndicator;
    [Export] private CpuParticles2D _brewingParticles;
    [Export] private Label _statusLabel;
    [Export] private Button _startButton;

    // --- 儲存表單相關節點 ---
    [Export] private PanelContainer _saveRecipePanel;
    [Export] private LineEdit _recipeNameEdit;
    [Export] private LineEdit _beanNameEdit;
    [Export] private TextEdit _notesEdit;
    [Export] private LineEdit _doseEdit;
    [Export] private LineEdit _yieldEdit;
    [Export] private LineEdit _timeEdit;
    [Export] private Button _saveButton;

    private BrewLogRecord _currentLogRecord;

    public override void _Ready()
    {
        // 訂閱 CoffeeMachine 的事件
        _coffeeMachine.OnStateChanged += HandleStateChanged;
        _coffeeMachine.OnBrewCompleted += HandleBrewCompleted;
        
        // 連接 UI 按鈕事件
        _startButton.Pressed += OnStartButtonPressed;
        _saveButton.Pressed += OnSaveButtonPressed;
        
        _saveRecipePanel.Visible = false;
        //// 手動初始化第一次的狀態顯示
        HandleStateChanged(_coffeeMachine.IdleState); 
    }

    // 當 CoffeeMachine 狀態改變時，這個方法會被呼叫
    private void HandleStateChanged(IState newState)
    {
        // 更新狀態文字
        if (newState is IdleState) _statusLabel.Text = "Idle. Press Start.";
        else if (newState is GrindingState) _statusLabel.Text = "Grinding...";
        else if (newState is HeatingState) _statusLabel.Text = "Heating...";
        else if (newState is BrewingState) _statusLabel.Text = "Brewing...";

        // 更新視覺元素
        _startButton.Disabled = !(newState is IdleState);
        _heaterIndicator.Color = (newState is HeatingState) ? new Color(1.0f, 0.5f, 0.5f) : new Color(0.5f, 0.5f, 1.0f);
        _brewingParticles.Emitting = (newState is BrewingState);
    }
    
    // 當沖煮流程結束時，這個方法會被呼叫
    private void HandleBrewCompleted(SimulationParameters finalSimParams)
    {
        GD.Print("UIManager received OnBrewCompleted event.");
        // 收到事件後，建立一個新的日誌紀錄
        _currentLogRecord = new BrewLogRecord
        {
            // 單向數據傳遞：從 SimulationParameters 填入初始值
            Dose = finalSimParams.Dose,
            Yield = finalSimParams.TargetYield, // 假設萃取完成，使用目標值
            BrewTime = finalSimParams.CurrentBrewTime, // 這裡我們還沒模擬計時，先用固定值
        };
        
        // 將數據填入 UI 表單
        _doseEdit.Text = _currentLogRecord.Dose.ToString(CultureInfo.InvariantCulture);
        _yieldEdit.Text = _currentLogRecord.Yield.ToString(CultureInfo.InvariantCulture);
        _timeEdit.Text = _currentLogRecord.BrewTime.ToString(CultureInfo.InvariantCulture);
        
        // 清空主觀筆記欄位
        _recipeNameEdit.Text = "";
        _beanNameEdit.Text = "";
        _notesEdit.Text = "";

        // 顯示表單
        _saveRecipePanel.Visible = true;
    }

    private void OnStartButtonPressed()
    {
        GD.Print("UIManager: Start Button was pressed!"); 
        // UIManager 告訴 CoffeeMachine 開始工作
        _coffeeMachine.StartBrewing();
    }
    
    private void OnSaveButtonPressed()
    {
        GD.Print("OnSaveButtonPressed method was called!");
        if (_currentLogRecord == null) {
            GD.Print("Save failed: _currentLogRecord is null.");
            return;
        }
        
        // 使用者在表單上的修改，只會更新 BrewLogRecord
        _currentLogRecord.RecipeName = _recipeNameEdit.Text;
        _currentLogRecord.BeanName = _beanNameEdit.Text;
        _currentLogRecord.Notes = _notesEdit.Text;
        _currentLogRecord.Dose = float.Parse(_doseEdit.Text, CultureInfo.InvariantCulture);
        // ... 其他欄位 ...

        GD.Print("--- Final Log Record to be Saved ---");
        GD.Print(_currentLogRecord.ToJson());
        
        _saveRecipePanel.Visible = false;
        _currentLogRecord = null; // 清除當前紀錄
    }
}

