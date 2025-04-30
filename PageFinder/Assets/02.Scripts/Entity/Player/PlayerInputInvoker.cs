using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum InputType
{
    BASICATTACK,
    DASH,
    SKILL
}

public class BasicAttackCommand : InputCommand
{
    private PlayerAttackController playerAttackContorller;

    public BasicAttackCommand(PlayerAttackController playerAttackContorller, float timeStamp)
    {
        this.playerAttackContorller = playerAttackContorller;
        this.Priority = 0;
        this.ExpirationTime = 0.2f;
        this.Timestamp = timeStamp;
        this.inputType = InputType.BASICATTACK;
    }

    public override bool IsExcuteable()
    {
        return playerAttackContorller.CheckAttackExcutable();
    }

    public override void Execute()
    {
        playerAttackContorller.Attack();
    }
}

public class DashCommand : InputCommand
{
    private PlayerDashController playerDashContorller;
    public DashCommand(PlayerDashController playerDashContorller, float timeStamp)
    {
        this.playerDashContorller = playerDashContorller;
        this.Priority = 2;
        this.ExpirationTime = 0.3f;
        this.Timestamp = timeStamp;
        this.inputType = InputType.DASH;
    }

    public override bool IsExcuteable()
    {
        return playerDashContorller.CheckDashExcutable();
    }

    public override void Execute()
    {
        playerDashContorller.ExcuteDash();
    }
}

public class SkillCommand : InputCommand
{
    private PlayerSkillController playerSkillController;
    public SkillCommand(PlayerSkillController playerSkillController, float timeStamp)
    {
        this.playerSkillController = playerSkillController;
        this.Priority = 1;
        this.ExpirationTime = 0.5f;
        this.Timestamp = timeStamp;
        this.inputType = InputType.SKILL;
    }

    public override bool IsExcuteable()
    {
        return playerSkillController.CheckSkillExcutable();
    }

    public override void Execute()
    {
        playerSkillController.ExcuteSkill();
    }

}
public class PlayerInputInvoker : MonoBehaviour
{
    private Queue<InputCommand> inputs = new Queue<InputCommand>();
    private Queue<InputCommand> addQueue = new Queue<InputCommand>();

    private void Update()
    {
        UpdateCommands();

        EnqueueCommand();
    }

    public void AddInputCommand(InputCommand input)
    {
        addQueue.Enqueue(input);
    }

    private void UpdateCommands()
    {
        InputCommand excuteCommand = null;
        foreach(InputCommand input in inputs)
        {
            if (!input.IsExcuteable()) continue;
            if (Time.time > input.Timestamp + input.ExpirationTime) continue;

            if(excuteCommand == null || input.Priority > excuteCommand.Priority)
                excuteCommand = input;
        }

        if(excuteCommand != null)
        {
            excuteCommand.Execute();
            inputs.Clear();
        }
    }

    private void EnqueueCommand()
    {
        while (addQueue.Count > 0)
        {
            var input = addQueue.Dequeue();
            inputs.Enqueue(input);
        }
    }
}
