// scripts/AuthManager.cs
using Godot;
using System;
using System.Threading.Tasks;

public partial class AuthManager : Control
{
    // --- UI Node References ---
    [Export] private Label _titleLabel;
    [Export] private LineEdit _emailEdit;
    [Export] private LineEdit _passwordEdit;
    [Export] private Label _feedbackLabel;
    [Export] private Button _submitButton;
    [Export] private Button _toggleButton;

    // --- Services ---
    private FirebaseManager _firebaseManager;

    // --- State ---
    private bool _isRegisterMode = false;

    public override void _Ready()
    {
        // Get the global FirebaseManager instance
        _firebaseManager = GetNode<FirebaseManager>("/root/FirebaseManager");

        // Connect button signals
        _submitButton.Pressed += OnSubmitPressed;
        _toggleButton.Pressed += OnToggleModePressed;

        // Set initial UI state
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (_isRegisterMode)
        {
            _titleLabel.Text = "註冊 (Register)";
            _submitButton.Text = "註冊";
            _toggleButton.Text = "已經有帳號了？點此登入";
        }
        else
        {
            _titleLabel.Text = "登入 (Login)";
            _submitButton.Text = "登入";
            _toggleButton.Text = "還沒有帳號？點此註冊";
        }
        _feedbackLabel.Text = ""; // Clear feedback on mode switch
    }

    private void OnToggleModePressed()
    {
        _isRegisterMode = !_isRegisterMode;
        UpdateUI();
    }

    private async void OnSubmitPressed()
    {
        string email = _emailEdit.Text;
        string password = _passwordEdit.Text;

        // Basic validation
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            _feedbackLabel.Text = "電子郵件和密碼不能為空。";
            return;
        }

        _feedbackLabel.Text = "處理中...";
        _submitButton.Disabled = true;

        bool success = false;
        if (_isRegisterMode)
        {
            success = await _firebaseManager.CreateUserWithEmailPasswordAsync(email, password);
        }
        else
        {
            success = await _firebaseManager.SignInWithEmailPasswordAsync(email, password);
        }

        if (success)
        {
            _feedbackLabel.Text = _isRegisterMode ? "註冊成功！" : "登入成功！";
            // On success, transition to the main scene
            GetTree().ChangeSceneToFile("res://scenes/main_scene.tscn");
        }
        else
        {
            // FirebaseManager will print the detailed error, we show a generic message
            _feedbackLabel.Text = "操作失敗，請檢查您的輸入或網路連線。";
        }

        _submitButton.Disabled = false;
    }
}

