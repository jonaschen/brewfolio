// scripts/BrewingState.cs
using Godot;

public partial class BrewingState : IState
{
    public async void Enter(CoffeeMachine machine)
    {
        machine.UpdateStatusText("Brewing...");
        machine.SetBrewingParticles(true);
        
        // Start a 5-second timer.
        await machine.ToSignal(machine.GetTree().CreateTimer(5.0), SceneTreeTimer.SignalName.Timeout);
        
        // Transition back to the idle state using the PascalCase property
        machine.TransitionToState(machine.IdleState);
    }

    public void Execute(CoffeeMachine machine) { }

    public void Exit(CoffeeMachine machine) 
    {
        machine.SetBrewingParticles(false);
    }
}
