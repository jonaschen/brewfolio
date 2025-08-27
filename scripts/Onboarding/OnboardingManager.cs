// scripts/Onboarding/OnboardingManager.cs
using Godot;

public partial class OnboardingManager : Control
{
	// --- UI 節點引用 ---
	[Export] private VBoxContainer _personaSelectionContainer; // 包含三個主要按鈕的容器
	[Export] private Button _chloeButton;
	[Export] private Button _alexButton;
	[Export] private Button _isabellaButton;
	
	[Export] private PanelContainer _proModePrompt; // 專業模式提示框
	[Export] private Button _enableProModeButton;
	[Export] private Button _disableProModeButton;

	// --- 服務與策略 ---
	private UserProfileService _userProfileService;
	private IOnboardingStrategy _currentStrategy;

	public override void _Ready()
	{
		// 取得全域的使用者設定服務
		_userProfileService = GetNode<UserProfileService>("/root/UserProfileService");

		// --- 連接按鈕訊號 ---
		_chloeButton.Pressed += () => OnPersonaSelected(PersonaType.Chloe);
		_alexButton.Pressed += () => OnPersonaSelected(PersonaType.Alex);
		_isabellaButton.Pressed += () => OnPersonaSelected(PersonaType.Isabella);
		
		_enableProModeButton.Pressed += () => _currentStrategy?.HandleAction(this, "enable_pro_mode");
		_disableProModeButton.Pressed += () => _currentStrategy?.HandleAction(this, "disable_pro_mode");

		// --- 初始 UI 狀態 ---
		_proModePrompt.Visible = false;
		_personaSelectionContainer.Visible = true;
	}

	private void OnPersonaSelected(PersonaType persona)
	{
		// 1. 更新使用者設定檔
		_userProfileService.CurrentUserProfile.SelectedPersona = persona;
		
		// 2. 根據選擇，指派對應的策略
		if (persona == PersonaType.Isabella)
		{
			_currentStrategy = new ProfessionalOnboardingStrategy();
		}
		else
		{
			_currentStrategy = new DefaultOnboardingStrategy();
		}
		
		// 3. 隱藏 Persona 選擇畫面，並啟動策略
		_personaSelectionContainer.Visible = false;
		_currentStrategy.Start(this);
	}

	// --- 公開給策略呼叫的方法 ---

	public void ShowProModePrompt(bool show)
	{
		_proModePrompt.Visible = show;
	}

	public void SetProMode(bool isEnabled)
	{
		_userProfileService.CurrentUserProfile.IsProModeEnabled = isEnabled;
	}

	public void CompleteOnboarding()
	{
		// 儲存最終的使用者設定
		_userProfileService.SaveProfile();
		
		GD.Print("Onboarding 完成！使用者設定已儲存。");
		GD.Print($"使用者類型: {_userProfileService.CurrentUserProfile.SelectedPersona}");
		GD.Print($"專業模式: {_userProfileService.CurrentUserProfile.IsProModeEnabled}");
		
		// 切換到主場景
		GetTree().ChangeSceneToFile("res://scenes/main_scene.tscn");
	}
}
