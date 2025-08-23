// scripts/OffState.cs
using Godot;

public partial class OffState : IState
{
    public void Enter(CoffeeMachine machine)
    {
        // 更新UI文字，並禁用開始按鈕
        machine.UpdateStatusText("Machine is Off.");
        machine.SetStartButtonEnabled(false);
        
        // 確保加熱器和粒子效果是關閉的
        machine.SetHeaterIndicatorColor(new Color(0.2f, 0.2f, 0.2f)); // Dark grey
        machine.SetBrewingParticles(false);
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
