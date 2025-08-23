// IdleState.cs
using Godot;

public partial class IdleState : IState
{
    public void Enter(CoffeeMachine machine)
    {
        machine.UpdateStatusText("idle. press start.");
        machine.SetHeaterIndicatorColor(new Color(0.5f, 0.5f, 1.0f)); // blueish
        machine.SetBrewingParticles(false);
        machine.SetStartButtonEnabled(true);
    }

    public void Execute(CoffeeMachine machine) { }

    public void Exit(CoffeeMachine machine)
    {
        machine.SetStartButtonEnabled(false);
    }
}


