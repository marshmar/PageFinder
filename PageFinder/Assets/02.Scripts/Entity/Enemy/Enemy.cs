using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.UI;

public class Enemy : Entity
{
    public enum PosType
    {
        GROUND,
        SKY
    }

    public enum MoveType
    {
        PATH, // 경로 이동
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

    [SerializeField] // 포지션 : 육상, 비행
    protected PosType posType = PosType.GROUND;
    [SerializeField] // 행동 패턴 : 경로이동, 추적이동, 고정
    protected MoveType moveType = MoveType.PATH; 
    [SerializeField] // 공격 성향 : 선공, 지속 선공, 회피, 수호
    protected AttackType attackType = AttackType.PREEMPTIVE;

    [SerializeField]
    private Slider hpBar;

    // 원래 초기 좌표
    public Vector3 originalPos;


    [SerializeField]
    protected int defaultAtkPercent = 100; // 기본 공격 적용 퍼센트
    [SerializeField]
    protected float stunTime = 0.2f; // 경직 시간

    // 기본 공격
    protected float maxDefaultAtkCoolTime = 2f;
    protected float currDefaultAtkCoolTime = 0;

    // 스킬
    [SerializeField]
    protected List<float> maxSkillCoolTimes = new List<float>(); // 스킬 쿨타임 - 인스펙터 창에서 설정 
    protected List<float> currSkillCoolTimes = new List<float>(); // 현재 스킬 쿨타임 
    protected List<bool> skillUsageStatus =  new List<bool>();



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
                // 해야할 처리 
                // 플레이어 경험치 획득
                // 토큰 생성 
                Die();
            }
        }
    }

    public virtual int DefaultAtkPercent
    {
        get { return defaultAtkPercent; }
        set
        {
            currHP = value;
        }
    }

    public virtual float MaxDefaultAtkCoolTime
    {
        get { return maxDefaultAtkCoolTime; }
        set
        {
            maxDefaultAtkCoolTime = value;
        }
    }

    public virtual float CurrDefaultAtkCoolTime
    {
        get { return currDefaultAtkCoolTime; }
        set
        {
            currDefaultAtkCoolTime = value;
        }
    }

    public override void Start()
    {
        base.Start();

        isDie = false;

        currHP = maxHP;
        hpBar.maxValue = maxHP;
        hpBar.value = maxHP;

        for (int i = 0; i < maxSkillCoolTimes.Count; i++)
            currSkillCoolTimes.Add(maxSkillCoolTimes[i]);

        for (int i = 0; i < maxSkillCoolTimes.Count; i++)
            skillUsageStatus.Add(false);

        meshRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();
    }
}
