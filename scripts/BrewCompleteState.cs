// scripts/BrewCompleteState.cs
using Godot;

public partial class BrewCompleteState : IState
{
    public void Enter(CoffeeMachine machine)
    {
        machine.FinishBrewing();
    }

    public void Execute(CoffeeMachine machine) { }

    public void Exit(CoffeeMachine machine) 
    {
    }
}
