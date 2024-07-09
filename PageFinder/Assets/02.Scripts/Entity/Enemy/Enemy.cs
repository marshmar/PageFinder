using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.UI;

public class Enemy : Entity
{
    [SerializeField]
    private Slider hpBar;

    // 원래 초기 좌표
    public Vector3 originalPos;

    public enum PosType
    {
        GROUND,
        SKY
    }

    public enum MoveType
    {
        PATH, // 경로 이동
        RANDOM, // 랜덤 이동
        TRACE, // 추적 이동
        FIX // 고정
    }

    public enum AttackType
    {
        PREEMPTIVE, // 선제 공격 (인지범위 내에서만)
        SUSTAINEDPREEMPTIVE, // 지속 선제 공격 (인지범위 바깥까지)
        AVOIDANCE, // 회피
        GUARD // 수호 
    }

    public enum AttackRangeType
    {
        SHORT,
        LONG
    }


    [SerializeField] // 포지션 : 육상, 비행
    protected PosType posType = PosType.GROUND; 
    [SerializeField] // 행동 패턴 : 경로이동, 랜덤이동, 추적이동, 고정
    protected MoveType moveType = MoveType.RANDOM; 
    [SerializeField] // 공격 성향 : 선공, 지속 선공, 회피, 수호
    protected AttackType attackType = AttackType.PREEMPTIVE;
    [SerializeField] // 공격 범위 : 근거리, 원거리
    protected AttackRangeType attackRangeType = AttackRangeType.SHORT;

    protected MeshRenderer meshRenderer;
    // 에너미의 사망 여부
    protected bool isDie = false;

    public override float HP
    {
        get { return currHP; }
        set
        {
            currHP = value;
            hpBar.value = currHP;
            //Debug.Log(name + " : " + HP);
            if (currHP <= 0)
            {
                Die();
            }
        }
    }
    public override void Start()
    {
        base.Start();

        currHP = maxHP;
        hpBar.maxValue = maxHP;
        hpBar.value = maxHP;

        meshRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();
    }
}
