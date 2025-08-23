// scripts/HeatingState.cs
using Godot;

public partial class HeatingState : IState
{
    public async void Enter(CoffeeMachine machine)
    {
        machine.UpdateStatusText("Heating...");
        machine.SetHeaterIndicatorColor(new Color(1.0f, 0.5f, 0.5f)); // Reddish
        
        // Start a 3-second timer.
        await machine.ToSignal(machine.GetTree().CreateTimer(3.0), SceneTreeTimer.SignalName.Timeout);

        // Transition to the next state using the PascalCase property
        machine.TransitionToState(machine.BrewingState);
    }

    public void Execute(CoffeeMachine machine) { }
    
    public void Exit(CoffeeMachine machine) { }
}
