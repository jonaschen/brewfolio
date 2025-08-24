// scripts/OffState.cs
using Godot;

public partial class OffState : IState
{
    public void Enter(CoffeeMachine machine)
    {
        
    }

    public void Execute(CoffeeMachine machine) 
    { 
        // In the off state, we don't do anything per frame.
    }

    public void Exit(CoffeeMachine machine) 
    { 
        // Logic for what happens when the machine turns on would go here.
    }
}
