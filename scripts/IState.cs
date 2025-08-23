// IState.cs
// This is not a Node, so it doesn't need to inherit from Godot.Node
// It's a pure C# interface.
public interface IState
{
    void Enter(CoffeeMachine machine); // State entry logic
    void Execute(); // Logic to run every frame (optional for this spike)
    void Exit(); // State exit logic
}
