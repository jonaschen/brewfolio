// scripts/BrewRecipe.cs
using Godot; // Godot.Variant is used for JSON serialization

public partial class BrewRecipe
{
    public string RecipeName { get; set; }
    public string BeanName { get; set; }
    public string Notes { get; set; }
    // 未來可以增加更多參數，例如粉量、水量等

    public string ToJson()
    {
        var dict = new Godot.Collections.Dictionary
        {
            { "recipeName", RecipeName },
            { "beanName", BeanName },
            { "notes", Notes }
        };
        return Json.Stringify(dict, "  "); // "  " for pretty print
    }
}
