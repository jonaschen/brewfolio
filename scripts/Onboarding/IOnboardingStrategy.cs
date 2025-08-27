// scripts/Onboarding/IOnboardingStrategy.cs

/// <summary>
/// 定義 Onboarding 流程策略的合約。
/// 每個策略都負責引導使用者完成特定的引導步驟。
/// </summary>
public interface IOnboardingStrategy
{
    /// <summary>
    /// 啟動此策略的流程。
    /// </summary>
    /// <param name="manager">對 OnboardingManager 的引用，用於控制 UI 和流程轉換。</param>
    void Start(OnboardingManager manager);

    /// <summary>
    /// 處理來自 UI 的動作回饋（例如，按鈕點擊）。
    /// </summary>
    /// <param name="manager">對 OnboardingManager 的引用。</param>
    /// <param name="action">一個代表使用者動作的字串。</param>
    void HandleAction(OnboardingManager manager, string action);
}

