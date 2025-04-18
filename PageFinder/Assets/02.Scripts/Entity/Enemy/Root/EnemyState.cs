using System.Collections.Generic;
using UnityEngine;

public class EnemyState : IEntityState
{
    protected float maxHp;
    protected float curHp;
    protected float curAtk;
    protected float curMoveSpeed;
    protected float curAttackSpeed;
    protected float curAttackRange;
    protected float dmgResist;
    protected float dmgBonus;
    protected float inkTypeResistance;

    public float MaxHp { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public float CurHp { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public float CurAtk { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public float CurMoveSpeed { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public float CurAttackSpeed { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public float DmgResist { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public float DmgBonus { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    public Dictionary<EntityState, float> Multipliers => throw new System.NotImplementedException();

    public float CurAttackRange { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
}
