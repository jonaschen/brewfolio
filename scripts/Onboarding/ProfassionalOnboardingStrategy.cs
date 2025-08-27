// scripts/Onboarding/ProfessionalOnboardingStrategy.cs

/// <summary>
/// 針對 Isabella 的專業 Onboarding 策略。
/// 選擇 Persona 後，會彈出一個提示框詢問是否啟用專業模式。
/// </summary>
public class ProfessionalOnboardingStrategy : IOnboardingStrategy
{
    public void Start(OnboardingManager manager)
    {
        // 啟動時，要求 OnboardingManager 顯示專業模式提示框。
        manager.ShowProModePrompt(true);
    }

    public void HandleAction(OnboardingManager manager, string action)
    {
        // 根據使用者在提示框中的選擇，設定使用者設定檔。
        if (action == "enable_pro_mode")
        {
            manager.SetProMode(true);
        }
        else if (action == "disable_pro_mode")
        {
            manager.SetProMode(false);
        }

        // 無論選擇為何，完成 Onboarding 流程。
        manager.CompleteOnboarding();
    }
}
