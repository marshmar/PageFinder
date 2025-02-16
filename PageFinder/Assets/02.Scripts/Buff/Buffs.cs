using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public enum BuffType
{
    BuffType_Permanent,
    BuffType_Temporary,
    BuffType_Script
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

public class TemporaryMovementBuff : TemporaryBuffCommand
{
    private IEntityState entityState;

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

    public override void Tick(float deltaTime)
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
