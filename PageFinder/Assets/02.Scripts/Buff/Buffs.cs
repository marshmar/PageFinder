using NUnit.Framework;
using UnityEngine;

public enum BuffType
{
    BuffType_Permanent,
    BuffType_Temporary
}

[System.Serializable]
public struct BuffData
{
    public BuffType buffType;
    public int buffId;
    public float buffValue;
    public float duration;
    public Component target;

    public BuffData(BuffType buffType, int buffId, float buffValue, float duration = -1, Component target= null)
    {
        this.buffType = buffType;
        this.buffId = buffId;
        this.buffValue = buffValue;
        this.duration = duration;
        this.target = target;
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