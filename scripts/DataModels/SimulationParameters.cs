// scripts/DataModels/SimulationParameters.cs
public partial class SimulationParameters
{
    // 在模擬開始時設定的參數
    public float Dose { get; set; } = 18.0f; // 粉量 (g)
    public float TargetYield { get; set; } = 36.0f; // 目標液重 (g)
    
    // === 新增的屬性 ===
    public float TargetBrewTime { get; set; } = 28.0f; // 目標總時間 (s)

    // 在模擬過程中動態變化的參數
    public float CurrentYield { get; set; } = 0.0f; // 當前液重 (g)
    public float CurrentBrewTime { get; set; } = 0.0f; // 當前累計時間 (s)
}
