// scripts/Services/UserProfileService.cs
using Godot;
using System.Text.Json; // 引入 System.Text.Json 來處理 JSON 序列化

public partial class UserProfileService : Node
{
    // 公開的屬性，讓 App 的任何部分都能讀取當前的使用者設定
    public UserProfile CurrentUserProfile { get; private set; }

    // 使用 Godot 的 "user://" 路徑，這會指向 App 在裝置上安全的私有儲存空間
    private const string SavePath = "user://user_profile.json";

    /// <summary>
    /// 當這個單例節點被載入到場景樹時，Godot 會自動呼叫此方法。
    /// 我們的首要任務就是嘗試從本地讀取已儲存的設定檔。
    /// </summary>
    public override void _Ready()
    {
        LoadProfile();
    }

    /// <summary>
    /// 將目前的 UserProfile 物件儲存到本地檔案。
    /// </summary>
    public void SaveProfile()
    {
        // 使用 System.Text.Json 將 C# 物件序列化為 JSON 字串
        // JsonSerializerOptions 用於產生格式化 (pretty-print) 的 JSON，方便除錯
        var options = new JsonSerializerOptions { WriteIndented = true };
        string jsonString = JsonSerializer.Serialize(CurrentUserProfile, options);

        // 使用 Godot 的 FileAccess API 來安全地寫入檔案
        using var file = FileAccess.Open(SavePath, FileAccess.ModeFlags.Write);
        if (file != null)
        {
            file.StoreString(jsonString);
            GD.Print($"使用者設定已成功儲存至: {ProjectSettings.GlobalizePath(SavePath)}");
        }
        else
        {
            GD.PrintErr($"儲存使用者設定失敗！無法開啟檔案: {SavePath}");
        }
    }

    /// <summary>
    /// 從本地檔案讀取使用者設定，如果檔案不存在，則建立一個新的預設設定。
    /// </summary>
    public void LoadProfile()
    {
        if (FileAccess.FileExists(SavePath))
        {
            using var file = FileAccess.Open(SavePath, FileAccess.ModeFlags.Read);
            if (file != null)
            {
                // **FIXED**: Changed GetContentAsString() to the correct Godot 4 method, GetAsText()
                string jsonString = file.GetAsText();
                // 使用 System.Text.Json 將 JSON 字串反序列化回 C# 物件
                CurrentUserProfile = JsonSerializer.Deserialize<UserProfile>(jsonString);
                GD.Print("已成功從本地載入使用者設定。");
            }
            else
            {
                 GD.PrintErr($"讀取使用者設定失敗！無法開啟檔案: {SavePath}");
                 CreateDefaultProfile();
            }
        }
        else
        {
            // 這是使用者第一次開啟 App，或設定檔遺失
            GD.Print("找不到本地使用者設定檔，將建立一個新的預設設定。");
            CreateDefaultProfile();
        }
    }

    /// <summary>
    /// 建立一個新的、預設的 UserProfile 物件。
    /// </summary>
    private void CreateDefaultProfile()
    {
        CurrentUserProfile = new UserProfile
        {
            SelectedPersona = PersonaType.Unknown,
            IsProModeEnabled = false
        };
    }

    /// <summary>
    /// Clears the user profile from the device and resets it to default.
    /// </summary>
    public void ClearProfile()
    {
        if (FileAccess.FileExists(SavePath))
        {
            var err = FileAccess.Open(SavePath, FileAccess.ModeFlags.Read);
            if (err != null)
            {
                // Godot 4 requires a more complex way to delete files.
                // You can't delete a file directly, you have to use the DirAccess object.
                var dir = DirAccess.Open("user://");
                dir.Remove(SavePath.Replace("user://", ""));
                GD.Print("User profile deleted.");
            }
        }
        // Reset the in-memory profile to default.
        CreateDefaultProfile();
    }
}

