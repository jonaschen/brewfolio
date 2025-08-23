// GrindingState.cs
using Godot;

public partial class GrindingState : IState
{
    public void Enter(CoffeeMachine machine)
    {
        GD.Print("Entered GRINDING state. Grinding beans... Press 'Space' to turn off.");
    }

    public void Execute() { }

    public void Exit()
    {
        GD.Print("...Stopping grinding and turning off.");
    }
}
