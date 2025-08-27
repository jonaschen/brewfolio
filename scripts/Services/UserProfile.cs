// scripts/Services/UserProfile.cs
public enum PersonaType { Unknown, Chloe, Alex, Isabella }

public partial class UserProfile
{
    public PersonaType SelectedPersona { get; set; } = PersonaType.Unknown;
    public bool IsProModeEnabled { get; set; } = false;
}
