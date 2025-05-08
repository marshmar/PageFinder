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
    public int buffLevel;
    public BuffData(BuffType buffType, int buffId, float buffValue, float duration = -1, List<Component> targets = null, int buffLevel = 0)
    {
        this.buffType = buffType;
        this.buffId = buffId;
        this.buffValue = buffValue;
        this.duration = duration;
        this.targets = targets;
        this.buffLevel = buffLevel;
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
        entityState.CurMoveSpeed.AddModifier(new StatModifier(BuffValue, StatModifierType.PercentAddTemporary, this));
        //entityState.CurMoveSpeed += BuffValue;
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
        entityState.CurMoveSpeed.RemoveAllFromSource(this);
        //entityState.CurMoveSpeed -= BuffValue;
    }
}

public class PermanentAttackSpeedBuff : BuffCommand
{
    private IEntityState entityState;
    public override void Execute()
    {
        entityState.CurAttackSpeed.AddModifier(new StatModifier(BuffValue, StatModifierType.FlatPermanent, this));
        //entityState.CurAttackSpeed += BuffValue;
    }

    public PermanentAttackSpeedBuff(IEntityState entityState, float value)
    {
        this.entityState = entityState;
        this.BuffValue = value;
    }

    public override void EndBuff()
    {
        entityState.CurAttackSpeed.RemoveAllFromSource(this);
        //entityState.CurAttackSpeed -= BuffValue;
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
        entityState.DmgBonus.AddModifier(new StatModifier(BuffValue, StatModifierType.FlatPermanent, this));
        //entityState.DmgBonus += BuffValue;
    }

    public override void EndBuff()
    {
        entityState.DmgBonus.RemoveAllFromSource(this);
        //entityState.DmgBonus -= BuffValue;
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
        entityState.DmgResist.AddModifier(new StatModifier(BuffValue, StatModifierType.FlatPermanent, this));
        //entityState.DmgResist += BuffValue;
    }
    public override void EndBuff()
    {
        entityState.DmgResist.RemoveAllFromSource(this);
        //entityState.DmgResist -= BuffValue;
    }
}

#region StatusEffect
public class BurnStatusEffect : BuffCommand, ITickable, ITemporary
{
    private PlayerState playerState;
    private Enemy enemy;
    public float ElapsedTime { get; set; } = 0f;
    public float TickThreshold { get; set; } = 0f;

    public float Duration { get; set; } = 0f;
    public float TickTimer { get; set; } = 0f;

    public BurnStatusEffect(PlayerState playerState, Enemy enemy, float duration, float tickThreshold, int buffId)
    {
        this.buffId = buffId;
        this.playerState = playerState;
        this.enemy = enemy;
        this.Duration = duration;
        this.TickThreshold = tickThreshold;

        if (playerState == null) Debug.LogError("PlayerState == null");
        if (enemy == null) Debug.LogError("Enemy == null");
    }

    
    public void Tick(float deltaTime)
    {
        TickTimer += deltaTime;
        if(TickTimer >= TickThreshold)
        {
            if(enemy == null)
            {
                Debug.LogError("Enemy is null");
            }

            if(playerState == null)
            {
                Debug.LogError("PlayerState is null");
            }

            enemy.Hit(InkType.FIRE, (playerState.CurAtk.Value * 0.1f + enemy.MaxHp.Value * 0.015f));
            TickTimer = 0f;
        }
    }

    public override void EndBuff() { }

    public override void Execute() { }

    public void Update(float deltaTime)
    {
        ElapsedTime += Time.deltaTime;
        if (ElapsedTime >= Duration)
        {
            active = false;
        }
    }
}

public class ConfusionStatusEffect : BuffCommand, ITemporary
{
    private EnemyAction enemyAction;
    private float val;

    public ConfusionStatusEffect(EnemyAction enemyAction, float duration, int buffId)
    {
        this.buffId = buffId;
        this.enemyAction = enemyAction;
        this.Duration = duration;
    }

    public float ElapsedTime { get; set; } = 0f;
    public float Duration { get; set; } = 0f;

    public override void EndBuff()
    {
        enemyAction.SetConfusionState(false);
    }

    public override void Execute() 
    {
        enemyAction.SetConfusionState(true);
    }

    public void Update(float deltaTime)
    {
        ElapsedTime += Time.deltaTime;
        if (ElapsedTime >= Duration)
        {
            active = false;
            EndBuff();
        }
    }
}
#endregion

#region InkMarkBuff
public class InkMarkSwampBuff : BuffCommand, ITickable, ILevelable
{
    private PlayerState playerState;
    public float TickTimer { get; set; } = 0f;
    public float TickThreshold { get; set; } = 0f;

    public int BuffLevel { get; set; } = 0;
    public int Level { get; set; } = 0;

    public InkMarkSwampBuff(PlayerState playerState, float tickThreshold, int buffLevel, int buffId)
    {
        this.buffId = buffId;
        this.playerState = playerState;
        this.TickThreshold = tickThreshold;
        this.BuffLevel = buffLevel;
    }

    public void Tick(float deltaTime)
    {

        TickTimer += deltaTime;
        if (TickTimer >= TickThreshold)
        {
            TickTimer = 0f;
            float hpRecoveryValue = 10f + (playerState.MaxHp.Value - playerState.CurHp) * (0.03f - 0.01f * BuffLevel);
            playerState.CurHp += hpRecoveryValue;

            Debug.Log($"hpRVal: {hpRecoveryValue}, buffLevel: {BuffLevel}");

        }
    }

    public override void EndBuff() { }

    public override void Execute() { }

    public void SetLevel(int level)
    {
        BuffLevel = level;
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
        entityState.CurAttackSpeed.AddModifier(new StatModifier(scriptValue, StatModifierType.FlatPermanent, this));
        //entityState.CurAttackSpeed -= scriptValue;
    }

    public override void Execute()
    {
        entityState.CurAttackSpeed.RemoveAllFromSource(this);
        //entityState.CurAttackSpeed += scriptValue;
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
        playerState.CurInkGain.AddModifier(new StatModifier(0.04f, StatModifierType.PercentAddTemporary, this));
        //playerState.CurInkGain -= 0.04f;
    }

    public override void Execute()
    {
        playerState.CurInkGain.RemoveAllFromSource(this);
        //playerState.CurInkGain += 0.04f;
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
