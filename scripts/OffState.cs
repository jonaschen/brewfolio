// OffState.cs
using Godot;

public partial class OffState : IState
{
    public void Enter(CoffeeMachine machine)
    {
        GD.Print("Coffee Machine is OFF. Press 'Space' to turn on.");
    }

    public void Execute() { }

    public void Exit()
    {
        GD.Print("...Turning on.");
    }
}
