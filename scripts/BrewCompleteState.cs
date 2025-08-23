// scripts/BrewCompleteState.cs
using Godot;

public partial class BrewCompleteState : IState
{
    public void Enter(CoffeeMachine machine)
    {
        machine.UpdateStatusText("沖煮完成！請儲存您的配方。");
        // 顯示儲存表單
        machine.SetSaveRecipePanelVisibility(true);
    }

    public void Execute(CoffeeMachine machine) { }

    public void Exit(CoffeeMachine machine) 
    {
        // 隱藏儲存表單
        machine.SetSaveRecipePanelVisibility(false);
    }
}
