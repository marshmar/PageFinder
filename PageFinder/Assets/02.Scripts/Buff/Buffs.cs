using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;


public enum BuffType
{
    BuffType_Permanent,
    BuffType_Temporary,
    BuffType_Script,
    BuffType_Tickable
}

[System.Serializable]
public struct BuffData
{
    public BuffType buffType;
    public int buffId;
    public float buffValue;
    public float duration;
    public List<Component> targets;
    public BuffData(BuffType buffType, int buffId, float buffValue, float duration = -1, List<Component> targets = null)
    {
        this.buffType = buffType;
        this.buffId = buffId;
        this.buffValue = buffValue;
        this.duration = duration;
        this.targets = targets;
    }
}

public class TemporaryMovementBuff : BuffCommand, ITemporary
{
    private IEntityState entityState;

    public float Duration { get; set; }
    public float ElapsedTime { get; set; }

    public TemporaryMovementBuff(IEntityState entityState, float value, float duration)
    {
        this.entityState = entityState;
        this.BuffValue = value;
        this.Duration = duration;
        this.ElapsedTime = 0;
    }

    public override void Execute()
    {
        entityState.CurMoveSpeed += BuffValue;
    }

    public void Update(float deltaTime)
    {
        this.ElapsedTime += Time.deltaTime;
        if(this.ElapsedTime >= Duration)
        {
            this.active = false;
            EndBuff();
        }
    }

    public override void EndBuff()
    {
        entityState.CurMoveSpeed -= BuffValue;
    }
}

public class PermanentAttackSpeedBuff : BuffCommand
{
    private IEntityState entityState;
    public override void Execute()
    {
        entityState.CurAttackSpeed += BuffValue;
    }

    public PermanentAttackSpeedBuff(IEntityState entityState, float value)
    {
        this.entityState = entityState;
        this.BuffValue = value;
    }

    public override void EndBuff()
    {
        entityState.CurAttackSpeed -= BuffValue;
    }
}

public class PermanentDamageBonusBuff : BuffCommand
{
    private IEntityState entityState;

    public PermanentDamageBonusBuff(IEntityState entityState, float value)
    {
        this.entityState = entityState;
        this.BuffValue = value;
    }

    public override void Execute()
    {
        entityState.DmgBonus += BuffValue;
    }

    public override void EndBuff()
    {
        entityState.DmgBonus -= BuffValue;
    }
}

public class PemanentDamageResistBuff : BuffCommand
{
    private IEntityState entityState;

    public PemanentDamageResistBuff(IEntityState entityState, float value)
    {
        this.entityState = entityState;
        this.BuffValue = value;
    }
    public override void Execute()
    {
        entityState.DmgResist += BuffValue;
    }
    public override void EndBuff()
    {
        entityState.DmgResist -= BuffValue;
    }
}

#region InkMarkBuff
public class InkMarkFireBuff : BuffCommand, ITickable
{
    private PlayerState playerState;
    private Enemy enemy;
    private float elapsedTime = 0f;
    private float tickThreshold = 0f;
    public float ElapsedTime { get => elapsedTime; set => elapsedTime = value; }
    public float TickThreshold { get => tickThreshold; set => tickThreshold = value; }

    public InkMarkFireBuff(PlayerState playerState, Enemy enemy, float tickThreshold, int buffId)
    {
        this.buffId = buffId;
        this.playerState = playerState;
        this.enemy = enemy;
        this.tickThreshold = tickThreshold;
    }

    
    public void Tick(float deltaTime)
    {
        this.elapsedTime += deltaTime;
        if(this.elapsedTime >= TickThreshold)
        {
            enemy.Hit(InkType.FIRE, (playerState.CurAtk * 0.05f + enemy.MaxHp * 0.015f));
            this.elapsedTime = 0f;
        }
    }

    public override void EndBuff() { }

    public override void Execute() { }
}

public class InkMarkSwampBuff : BuffCommand, ITickable
{
    private PlayerState playerState;
    private float elapsedTime = 0f;
    private float tickThreshold = 0f;
    public float ElapsedTime { get => elapsedTime; set => elapsedTime = value; }
    public float TickThreshold { get => tickThreshold; set => tickThreshold = value; }

    public InkMarkSwampBuff(PlayerState playerState, float tickThreshold, int buffId)
    {
        this.buffId = buffId;
        this.playerState = playerState;
        this.tickThreshold = tickThreshold;
    }

    public void Tick(float deltaTime)
    {

        this.elapsedTime += deltaTime;
        if (this.elapsedTime >= TickThreshold)
        {
            this.elapsedTime = 0f;
            if (playerState.CurHp >= playerState.MaxHp * 0.7f) return;
            playerState.CurHp += (playerState.MaxHp - playerState.CurHp) * 0.03f;
            this.elapsedTime = 0f;
        }
    }

    public override void EndBuff() { }

    public override void Execute() { }
}

public class InkMarkMistBuff : BuffCommand
{
    private IEntityState entityState;
    private float val;

    public InkMarkMistBuff(IEntityState entityState, float buffValue, int buffId)
    {
        this.buffId = buffId;
        this.entityState = entityState;
        this.BuffValue = buffValue;
    }

    public override void EndBuff()
    {
        entityState.CurAttackRange += val;
    }

    public override void Execute()
    {
        val = entityState.CurAttackRange * 0.3f;
        entityState.CurAttackRange -= val;
    }
}
#endregion
#region PlayerPassiveBuff
public class FlameStrike : BuffCommand
{
    private IEntityState entityState;
    private float scriptValue;
    public FlameStrike(IEntityState entityState, float scriptValue)
    {
        this.entityState = entityState;
        this.scriptValue = scriptValue;
    }
    public override void EndBuff()
    {
        entityState.CurAttackSpeed -= scriptValue;
    }

    public override void Execute()
    {
        entityState.CurAttackSpeed += scriptValue;
    }
}

public class DeepWell : BuffCommand
{
    private PlayerState playerState;

    public DeepWell(PlayerState playerState)
    {
        this.playerState = playerState;
    }

    public override void EndBuff()
    {
        playerState.CurInkGain -= 0.04f;
    }

    public override void Execute()
    {
        playerState.CurInkGain += 0.04f;
    }
}

public class WaterConservation : BuffCommand
{
    private PlayerDashController playerDashController;
    private PlayerSkillController PlayerSkillController;

    private float defaultDashCost;
    private float defaultSkillCost;
    private float scriptValue;
    public WaterConservation(PlayerDashController playerDashController, PlayerSkillController playerSkillController, float scriptValue)
    {
        this.playerDashController = playerDashController;
        this.PlayerSkillController = playerSkillController;
        this.scriptValue = scriptValue;
        defaultDashCost = playerDashController.DashCost;
        defaultSkillCost = playerSkillController.CurrSkillData.skillCost;
    }

    public override void EndBuff()
    {
        playerDashController.DashCost = defaultDashCost;
        PlayerSkillController.CurrSkillData.skillCost = defaultSkillCost;
    }

    public override void Execute()
    {
        playerDashController.DashCost = defaultDashCost * (1-scriptValue);
        PlayerSkillController.CurrSkillData.skillCost = defaultSkillCost * (1 - scriptValue);
    }
}

public class ThickVine : BuffCommand
{
    private PlayerState playerState;
    

    public ThickVine(PlayerState playerState, float scriptValue)
    {
        this.playerState = playerState;
        if (playerState is null) Debug.Log("Null");
        this.BuffValue = scriptValue;
    }

    public override void EndBuff()
    {
        playerState.ThickVine = false;
        playerState.thickVineValue = 0;
    }

    public override void Execute()
    {
        Debug.Log("¾ï¼¾ µ¢Äð ½ÇÇà");
        playerState.ThickVine = true;
        playerState.thickVineValue = BuffValue;
    }
}
#endregion
