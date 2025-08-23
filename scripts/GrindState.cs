// scripts/GrindingState.cs
using Godot;

public partial class GrindingState : IState
{
    public async void Enter(CoffeeMachine machine)
    {
        machine.UpdateStatusText("Grinding...");
        
        // start a 2-second timer. when it finishes, transition to the next state.
        await machine.ToSignal(machine.GetTree().CreateTimer(2.0), SceneTreeTimer.SignalName.Timeout);
        
        machine.TransitionToState(machine.HeatingState);
    }

    public void Execute(CoffeeMachine machine) { }

    public void Exit(CoffeeMachine machine)
    {
    }
}

