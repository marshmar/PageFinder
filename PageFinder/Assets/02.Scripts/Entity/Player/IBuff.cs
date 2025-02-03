using UnityEngine;

public enum BuffState
{
    BuffState_MaxHp,
    BuffState_CurHp,
    BuffState_MaxInk,
    BuffState_CurInk,
    BuffState_CurInkGain,
    BuffState_CurAttackSpeed,
    BuffState_CurAttackRange,
    BuffState_CurAtk,
    BuffState_CurDef,
    BuffState_CurMoveSpeed,
    BuffState_CurCritical,
    BuffState_CurImag,
    BuffState_MaxShield,
    BuffState_CurShield
}
public enum BuffType
{
    BuffType_PermanentBuff,
    BuffType_PermanentMultiplier,
    BuffType_PermanentDebuff,
    BuffType_TemporaryBuff,
    BuffType_TemporaryDebuff
}

public interface IBuff
{
    public BuffType Type { get; set; }
    public BuffState State { get; set; }
    public float Value { get; set; }
    public float Duration { get; set; } // -1: 영구적 버프
}
