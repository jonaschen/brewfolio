// IdleState.cs
using Godot;

public partial class IdleState : IState
{
    public void Enter(CoffeeMachine machine)
    {
        GD.Print("Entered IDLE state. Ready to brew. Press 'Space' to start grinding.");
    }

    public void Execute() { }

    public void Exit()
    {
        GD.Print("...Starting grinding process.");
    }
}
