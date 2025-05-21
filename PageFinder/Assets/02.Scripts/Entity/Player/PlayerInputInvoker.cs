using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum InputType
{
    BASICATTACK,
    DASH,
    SKILL
}

public class NewBasicAttackCommand : InputCommand
{
    private NewPlayerAttackController playerAttackContorller;

    public NewBasicAttackCommand(NewPlayerAttackController playerAttackContorller, float timeStamp)
    {
        this.playerAttackContorller = playerAttackContorller;
        this.Priority = 0;
        this.ExpirationTime = 0.2f;
        this.Timestamp = timeStamp;
        this.inputType = InputType.BASICATTACK;
    }

    public override bool IsExcuteable()
    {
        return playerAttackContorller.CanExcuteBehaviour();
    }

    public override void Execute()
    {
        playerAttackContorller.ExcuteAnim();
    }
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

public class NewDashCommand : InputCommand
{
    private NewPlayerDashController playerDashContorller;
    public NewDashCommand(NewPlayerDashController playerDashContorller, float timeStamp)
    {
        this.playerDashContorller = playerDashContorller;
        this.Priority = 2;
        this.ExpirationTime = 0.3f;
        this.Timestamp = timeStamp;
        this.inputType = InputType.DASH;
    }

    public override bool IsExcuteable()
    {
        return playerDashContorller.CanExcuteBehaviour();
    }

    public override void Execute()
    {
        playerDashContorller.ExcuteBehaviour();
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

public class NewSkillCommand : InputCommand
{
    private NewPlayerSkillController playerSkillController;
    public NewSkillCommand(NewPlayerSkillController playerSkillController, float timeStamp)
    {
        this.playerSkillController = playerSkillController;
        this.Priority = 1;
        this.ExpirationTime = 0.5f;
        this.Timestamp = timeStamp;
        this.inputType = InputType.SKILL;
    }

    public override bool IsExcuteable()
    {
        return playerSkillController.CanExcuteBehaviour();
    }

    public override void Execute()
    {
        playerSkillController.ExcuteBehaviour();
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
    [SerializeField] private Queue<InputCommand> inputs = new Queue<InputCommand>();
    private Queue<InputCommand> addQueue = new Queue<InputCommand>();
    private int removeCounts = 0; 

    private void Update()
    {
        UpdateCommands();

        EnqueueCommand();
        RemoveCommand();
    }

    private void RemoveCommand()
    {
        if(removeCounts == inputs.Count)
        {
            inputs.Clear();
            removeCounts = 0;
        }
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
            if (Time.time > input.Timestamp + input.ExpirationTime)
            {
                removeCounts++;
                continue;
            }
            if (!input.IsExcuteable()) continue;


            if(excuteCommand == null || input.Priority > excuteCommand.Priority)
                excuteCommand = input;
        }

        if(excuteCommand != null)
        {
            excuteCommand.Execute();
            inputs.Clear();
            removeCounts = 0;
        }
    }

    private void EnqueueCommand()
    {
        while (addQueue.Count > 0)
        {
            var input = addQueue.Dequeue();
            inputs.Enqueue(input);
        }
        addQueue.Clear();
    }
}
