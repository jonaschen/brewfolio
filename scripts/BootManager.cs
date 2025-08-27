// scripts/BootManager.cs
using Godot;

public partial class BootManager : Control
{
    private UserProfileService _userProfileService;
    private FirebaseManager _firebaseManager;

    public override void _Ready()
    {
        // 取得所有需要的全域服務
        _userProfileService = GetNode<UserProfileService>("/root/UserProfileService");
        _firebaseManager = GetNode<FirebaseManager>("/root/FirebaseManager");

        // 開始執行啟動決策流程
        DecideNextScene();
    }

    private void DecideNextScene()
    {
        // 為了確保服務都已載入完成，延遲一幀執行
        CallDeferred(nameof(ExecuteNavigationLogic));
    }

    private void ExecuteNavigationLogic()
    {
        // 情境 A: 全新使用者 (本地沒有設定檔)
        if (_userProfileService.CurrentUserProfile.SelectedPersona == PersonaType.Unknown)
        {
            GD.Print("啟動流程：偵測到全新使用者，導向 Onboarding 場景。");
            GetTree().ChangeSceneToFile("res://scenes/onboarding_scene.tscn");
            return;
        }

        // 情境 C: 已登入使用者
        if (_firebaseManager.CurrentUser != null)
        {
            GD.Print("啟動流程：偵測到已登入使用者，導向主場景。");
            GetTree().ChangeSceneToFile("res://scenes/main_scene.tscn");
            return;
        }

        // 情境 B: 已登出使用者 (有設定檔但未登入)
        GD.Print("啟動流程：偵測到已登出使用者，導向認證場景。");
        GetTree().ChangeSceneToFile("res://scenes/auth_scene.tscn");
    }
}

