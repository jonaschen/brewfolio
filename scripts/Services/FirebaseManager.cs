////// scripts/Services/FirebaseManager.cs
////using Godot;
////using System;
////using System.Threading.Tasks;
////using Firebase.Auth; // 這是我們從 NuGet 下載的 SDK
////
////public partial class FirebaseManager : Node
////{
////    // --- Firebase 設定 ---
////    // !! 重要 !! 請將此處的 "YOUR_WEB_API_KEY" 替換為您自己的金鑰
////    private const string FirebaseApiKey = "AIzaSyDDk9ULI9qw-TT0C8UxsAL2F62tW4sJoSA";
////    // --- 服務實例 ---
////    public FirebaseAuthProvider AuthProvider { get; private set; }
////    public FirebaseAuthLink CurrentUser { get; private set; }
////
////    /// <summary>
////    /// 當這個單例節點被載入時，Godot 會自動呼叫此方法。
////    /// </summary>
////    public override void _Ready()
////    {
////        InitializeFirebase();
////    }
////
////    /// <summary>
////    /// 初始化 Firebase 相關服務。
////    /// </summary>
////    private void InitializeFirebase()
////    {
////        try
////        {
////            var config = new FirebaseConfig(FirebaseApiKey);
////            AuthProvider = new FirebaseAuthProvider(config);
////            GD.Print("Firebase Auth Provider 初始化成功！");
////        }
////        catch (Exception ex)
////        {
////            GD.PrintErr($"Firebase 初始化失敗: {ex.Message}");
////        }
////    }
////
////    /// <summary>
////    /// 執行匿名登入以驗證與 Firebase 的連線。
////    /// 這是一個非同步方法，因为它需要進行網路請求。
////    /// </summary>
////    public async Task<bool> SignInAnonymouslyAsync()
////    {
////        if (AuthProvider == null)
////        {
////            GD.PrintErr("匿名登入失敗：AuthProvider 尚未初始化。");
////            return false;
////        }
////
////        try
////        {
////            GD.Print("正在嘗試匿名登入...");
////            // 呼叫 SDK 的匿名登入 API
////            CurrentUser = await AuthProvider.SignInAnonymouslyAsync();
////            
////            GD.Print("--- Hello Firebase! ---");
////            GD.Print("匿名登入成功！");
////            GD.Print($"使用者 UID: {CurrentUser.User.LocalId}");
////            GD.Print("-----------------------");
////            return true;
////        }
////        catch (Exception ex)
////        {
////            GD.PrintErr($"匿名登入失敗: {ex.Message}");
////            CurrentUser = null;
////            return false;
////        }
////    }
////}
//
//// scripts/Services/FirebaseManager.cs
//using Godot;
//using System;
//using System.Threading.Tasks;
//using Firebase.Auth;
//
//public partial class FirebaseManager : Node
//{
//    // --- Firebase 設定 ---
//    private const string FirebaseApiKey = "YOUR_WEB_API_KEY"; // Make sure this is filled in
//
//    // --- 服務實例 ---
//    public FirebaseAuthProvider AuthProvider { get; private set; }
//    public FirebaseAuthLink CurrentUser { get; private set; }
//
//    public override void _Ready()
//    {
//        InitializeFirebase();
//    }
//
//    private void InitializeFirebase()
//    {
//        try
//        {
//            var config = new FirebaseConfig(FirebaseApiKey);
//            AuthProvider = new FirebaseAuthProvider(config);
//            GD.Print("Firebase Auth Provider 初始化成功！");
//        }
//        catch (Exception ex)
//        {
//            GD.PrintErr($"Firebase 初始化失敗: {ex.Message}");
//        }
//    }
//
//    /// <summary>
//    /// Creates a new user with the specified email and password.
//    /// </summary>
//    public async Task<bool> CreateUserWithEmailPasswordAsync(string email, string password)
//    {
//        if (AuthProvider == null) return false;
//        try
//        {
//            GD.Print($"正在註冊新使用者: {email}");
//            CurrentUser = await AuthProvider.CreateUserWithEmailAndPasswordAsync(email, password);
//            GD.Print($"使用者註冊成功！ UID: {CurrentUser.User.LocalId}");
//            return true;
//        }
//        catch (FirebaseAuthException ex)
//        {
//            GD.PrintErr($"註冊失敗: {ex.Reason}");
//            CurrentUser = null;
//            return false;
//        }
//    }
//
//    /// <summary>
//    /// Signs in an existing user with the specified email and password.
//    /// </summary>
//    public async Task<bool> SignInWithEmailPasswordAsync(string email, string password)
//    {
//        if (AuthProvider == null) return false;
//        try
//        {
//            GD.Print($"正在登入使用者: {email}");
//            CurrentUser = await AuthProvider.SignInWithEmailAndPasswordAsync(email, password);
//            GD.Print($"使用者登入成功！ UID: {CurrentUser.User.LocalId}");
//            return true;
//        }
//        catch (FirebaseAuthException ex)
//        {
//            GD.PrintErr($"登入失敗: {ex.Reason}");
//            CurrentUser = null;
//            return false;
//        }
//    }
//}
// scripts/Services/FirebaseManager.cs
using Godot;
using System;
using System.Threading.Tasks;
using Firebase.Auth;

public partial class FirebaseManager : Node
{
    // --- Firebase 設定 ---
    private const string FirebaseApiKey = "AIzaSyDDk9ULI9qw-TT0C8UxsAL2F62tW4sJoSA";

    // --- 服務實例 ---
    public FirebaseAuthProvider AuthProvider { get; private set; }
    public FirebaseAuthLink CurrentUser { get; private set; }

    public override void _Ready()
    {
        InitializeFirebase();
    }

    private void InitializeFirebase()
    {
        try
        {
            var config = new FirebaseConfig(FirebaseApiKey);
            AuthProvider = new FirebaseAuthProvider(config);
            GD.Print("Firebase Auth Provider 初始化成功！");
        }
        catch (Exception ex)
        {
            GD.PrintErr($"Firebase 初始化失敗: {ex.Message}");
        }
    }

    /// <summary>
    /// Creates a new user with the specified email and password.
    /// </summary>
    public async Task<bool> CreateUserWithEmailPasswordAsync(string email, string password)
    {
        if (AuthProvider == null) return false;
        try
        {
            GD.Print($"正在註冊新使用者: {email}");
            CurrentUser = await AuthProvider.CreateUserWithEmailAndPasswordAsync(email, password);
            GD.Print($"使用者註冊成功！ UID: {CurrentUser.User.LocalId}");
            return true;
        }
        catch (FirebaseAuthException ex)
        {
            // **FIXED**: Changed ex.Reason to ex.Message for more detailed error logging.
            GD.PrintErr($"註冊失敗: {ex.Message}");
            CurrentUser = null;
            return false;
        }
    }

    /// <summary>
    /// Signs in an existing user with the specified email and password.
    /// </summary>
    public async Task<bool> SignInWithEmailPasswordAsync(string email, string password)
    {
        if (AuthProvider == null) return false;
        try
        {
            GD.Print($"正在登入使用者: {email}");
            CurrentUser = await AuthProvider.SignInWithEmailAndPasswordAsync(email, password);
            GD.Print($"使用者登入成功！ UID: {CurrentUser.User.LocalId}");
            return true;
        }
        catch (FirebaseAuthException ex)
        {
            // **FIXED**: Changed ex.Reason to ex.Message for more detailed error logging.
            GD.PrintErr($"登入失敗: {ex.Message}");
            CurrentUser = null;
            return false;
        }
    }
}

