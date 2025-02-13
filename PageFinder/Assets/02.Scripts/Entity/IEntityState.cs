using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 그냥 스탯 데이터 클래스 하나 만들어서 참조로 가지고 있는게 나을것 같다.
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
/// 공통 프로퍼티 지정 필요
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
