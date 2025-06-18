using System.Collections.Generic;
using UnityEngine;

public abstract class Command
{
    public abstract void Execute();
}


public abstract class BuffCommand : Command
{
    public int buffId;
    public bool isActive = true;
    private float buffValue;
    protected BuffType buffType;
    public BuffType BuffType {  get; set; }
    public virtual float BuffValue 
    {
        get
        {
            return buffValue;
        }
        set
        {
            buffValue = value;
        }
    }

    public abstract void EndBuff();
}

public interface ITemporary
{
    public float ElapsedTime { get; set; }
    public float Duration { get; set; }

    public void Update(float deltaTime);
    public void Reset();
}

public interface ITickable
{
    public float TickTimer { get; set; }
    public float TickThreshold { get; set; }
    public void Tick(float deltaTime);
}

public interface ILevelable
{
    public int Level { get; set; }
    public void SetLevel(int level);
}

#region InputCommand
public enum InputType
{
    BASICATTACK,
    DASH,
    SKILL
}

[System.Serializable]
public abstract class InputCommand : Command
{
    public InputType inputType;
    public float Timestamp; // Input Time
    public float ExpirationTimeSec;
    public int Priority;
    public bool IsExpired = false;

    public override void Execute() { }

    public abstract bool IsExcuteable();
}

public class BasicAttackCommand : InputCommand
{
    private PlayerAttackController playerAttackContorller;

    public BasicAttackCommand(PlayerAttackController playerAttackContorller, float timeStamp)
    {
        this.playerAttackContorller = playerAttackContorller;
        this.Priority = 0;
        this.ExpirationTimeSec = 0.2f;
        this.Timestamp = timeStamp;
        this.inputType = InputType.BASICATTACK;
    }

    public override bool IsExcuteable()
    {
        return playerAttackContorller.CanExcuteBehaviour();
    }

    public override void Execute()
    {
        if (playerAttackContorller.IsAnimatedBasedAttack())
            playerAttackContorller.ExcuteAnim();
        else
            playerAttackContorller.ExcuteBehaviour();
    }
}
public class DashCommand : InputCommand
{
    private NewPlayerDashController playerDashContorller;
    public DashCommand(NewPlayerDashController playerDashContorller, float timeStamp)
    {
        this.playerDashContorller = playerDashContorller;
        this.Priority = 2;
        this.ExpirationTimeSec = 0.3f;
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

public class SkillCommand : InputCommand
{
    private NewPlayerSkillController playerSkillController;
    public SkillCommand(NewPlayerSkillController playerSkillController, float timeStamp)
    {
        this.playerSkillController = playerSkillController;
        this.Priority = 1;
        this.ExpirationTimeSec = 0.5f;
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
#endregion
