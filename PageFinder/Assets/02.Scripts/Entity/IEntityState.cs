using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// �׳� ���� ������ Ŭ���� �ϳ� ���� ������ ������ �ִ°� ������ ����.
/// </summary>
public enum EntityState
{
    MaxHp,
    CurHp,
    CurAtk,
    CurMoveSpeed,
    CurAttackSpeed,
    EntityState,
    MaxInk,
    CurInk,
    CurInkGain,
    CurAttackRange,
    CurCritical,
    CurCriticalDmg,
    PlayerState
}

/// <summary>
/// ���� ������Ƽ ���� �ʿ�
/// </summary>
public interface IEntityState
{
    public float MaxHp { get; set; }
    public float CurHp { get; set; }
    public float CurAtk { get; set; }
    public float CurMoveSpeed { get; set; }
    public float CurAttackSpeed { get; set; }

    public float DmgResist { get; set; }

    public float DmgBonus { get; set; }

    public Dictionary<EntityState, float> Multipliers { get; }
}
