// scripts/MainSceneManager.cs
using Godot;

public partial class MainSceneManager : Control
{
	// --- UI 節點引用 ---
	[Export] private Label _welcomeLabel;

	// --- 服務 ---
	private UserProfileService _userProfileService;

	/// <summary>
	/// 當主場景載入時，此方法會被呼叫。
	/// </summary>
	public override async void _Ready()
	{
		// 取得全域的使用者設定服務
		_userProfileService = GetNode<UserProfileService>("/root/UserProfileService");

		// 根據使用者的設定檔來載入對應的內容
		LoadContentBasedOnProfile();
	}

	/// <summary>
	/// 讀取 UserProfile 並更新 UI，這是模組化 UI 的核心邏輯。
	/// </summary>
	private void LoadContentBasedOnProfile()
	{
		UserProfile profile = _userProfileService.CurrentUserProfile;
		string welcomeMessage;

		// 根據使用者在 Onboarding 中選擇的 Persona，顯示不同的歡迎詞
		switch (profile.SelectedPersona)
		{
			case PersonaType.Chloe:
				welcomeMessage = "嗨，Chloe！準備好探索今天的咖啡了嗎？";
				// 未來：在這裡載入 Chloe 專屬的 UI 模組 (Widgets)
				break;
			case PersonaType.Alex:
				welcomeMessage = "Alex，你好！讓我們開始今天的沖煮實驗吧。";
				// 未來：在這裡載入 Alex 專屬的 UI 模組
				break;
			case PersonaType.Isabella:
				string proModeStatus = profile.IsProModeEnabled ? "已啟用" : "未啟用";
				welcomeMessage = $"專業模式 {proModeStatus}。Isabella，請檢視您的品管數據。";
				// 未來：在這裡載入 Isabella 專屬的 UI 模組
				break;
			case PersonaType.Unknown:
			default:
				welcomeMessage = "歡迎來到 brewfolio！";
				// 顯示一個預設畫面
				break;
		}

		_welcomeLabel.Text = welcomeMessage;
	}
}
