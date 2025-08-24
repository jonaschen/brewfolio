// scripts/DataModels/BrewLogRecord.cs
using Godot;

public partial class BrewLogRecord
{
    // 客觀參數
    public float Dose { get; set; }
    public float Yield { get; set; }
    public float BrewTime { get; set; }

    // 主觀筆記
    public string RecipeName { get; set; }
    public string BeanName { get; set; }
    public string Notes { get; set; }

    public string ToJson()
    {
        var dict = new Godot.Collections.Dictionary
        {
            { "dose", Dose },
            { "yield", Yield },
            { "brewTime", BrewTime },
            { "recipeName", RecipeName },
            { "beanName", BeanName },
            { "notes", Notes }
        };
        return Json.Stringify(dict, "  ");
    }
}
