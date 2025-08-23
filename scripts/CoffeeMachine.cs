// CoffeeMachine.cs
using Godot;

public partial class CoffeeMachine : Node
{
    private IState currentState;

    private OffState offState = new OffState();
    private IdleState idleState = new IdleState();
    private GrindingState grindingState = new GrindingState();

    public override void _Ready()
    {
        // Set the initial state when the scene starts
        TransitionToState(offState);
    }

    public override void _Input(InputEvent @event)
    {
        // A simple trigger for state transition
        if (@event.IsActionPressed("ui_accept")) // "ui_accept" is Spacebar by default
        {
            if (currentState == offState)
            {
                TransitionToState(idleState);
            }
            else if (currentState == idleState)
            {
                TransitionToState(grindingState);
            }
            else if (currentState == grindingState)
            {
                TransitionToState(offState);
            }
        }
    }

    public void TransitionToState(IState nextState)
    {
        currentState?.Exit(); // Call Exit on the current state if it exists
        currentState = nextState;
        currentState.Enter(this);
    }
}
