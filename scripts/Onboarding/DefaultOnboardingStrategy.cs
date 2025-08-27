// scripts/Onboarding/DefaultOnboardingStrategy.cs

/// <summary>
/// 針對 Chloe 和 Alex 的預設 Onboarding 策略。
/// 選擇 Persona 後直接進入主場景。
/// </summary>
public class DefaultOnboardingStrategy : IOnboardingStrategy
{
    public void Start(OnboardingManager manager)
    {
        // 這個策略沒有額外步驟，直接完成。
        manager.CompleteOnboarding();
    }

    public void HandleAction(OnboardingManager manager, string action)
    {
        // 不需要處理任何額外動作。
    }
}
