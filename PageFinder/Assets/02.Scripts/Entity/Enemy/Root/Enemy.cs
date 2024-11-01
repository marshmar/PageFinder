using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.UI;

public class Enemy : Entity
{
    protected enum PosType
    {
        GROUND,
        SKY
    }

    protected enum FindPattern
    {
        PATH, // 경로 이동
        FIX // 고정
    }

    protected enum AttackPattern
    {
        PREEMPTIVE, // 선제 공격 (인지범위 내에서만)
        SUSTAINEDPREEMPTIVE, // 지속 선제 공격 (인지범위 바깥까지)
        AVOIDANCE, // 회피
        GUARD // 수호 
    }

    protected enum Rank
    { 
        LOW,
        HIGH,
        MEDIUMBOSS,
        FINALBOSS
    }


    protected enum State
    {
        IDLE,
        STUN,
        MOVE,
        ATTACK,
        DIE
    }

    protected enum IdleState
    {
        NONE,
        DEFAULT
    }

    protected enum MoveState
    {
        NONE,
        FIND,
        TRACE
    }

    protected enum AttackState
    {
        NONE,
        ATTACKWAIT,
        DEFAULT,
        REINFORCEMENT,
        SKILL
    }

    protected enum StateEffect
    {
        NONE,
        STUN,
        KNOCKBACK,
        BINDING,
        AIR,
    }

    [Header("State")]
    protected State state = State.IDLE; // 에너미의 현재 상태
    protected IdleState idleState; // 에너미의 현재 상태
    protected MoveState moveState;
    protected AttackState attackState;
    protected StateEffect stateEffect;

    [SerializeField] // 포지션 : 육상, 비행
    protected PosType posType = PosType.GROUND;
    protected Rank rank;

    [Header("Pattern")]
    [SerializeField] // 행동 패턴 : 경로이동, 추적이동, 고정
    protected FindPattern findPattern = FindPattern.PATH; 
    [SerializeField] // 공격 패턴 : 선공, 지속 선공, 회피, 수호
    protected AttackPattern attackPattern = AttackPattern.PREEMPTIVE;

    // 상태 관련 함수
    protected bool playerRecognitionStatue = false; // 플레이어를 한 번이라도 인지했는지 확인하는 변수

    [Header("Abnormal")]
    [SerializeField]
    protected float maxAbnormalTime;
    protected float currAbnormalTime = 0;
    protected Vector3 stateEffectPos = Vector3.zero;

    [Header("Find")]
    [SerializeField]
    protected float maxFindCoolTime;
    protected float currFindCoolTime;

    [Header("DefaultAttack")]
    [SerializeField]
    protected int defaultAtkPercent = 100; // 기본 공격 적용 퍼센트
    // 기본 공격
    [SerializeField]
    protected float maxDefaultAtkCoolTime;
    protected float currDefaultAtkCoolTime;

    [Header("Dist")]
    [SerializeField] // 공격 사정거리
    protected float atkDist = 2.0f;
    [SerializeField] // 인지 사정거리
    protected float cognitiveDist = 10.0f;

    [Header("Move")]
    [SerializeField] // 이동 위치
    protected Vector3[] posToMove = { Vector3.zero, Vector3.zero };
    protected int currentPosIndexToMove = 0;
    private int moveDist = 0;

    [Header("Stun")]


    // 컴포넌트
    protected Transform enemyTr;
    protected GameObject playerObj;
    protected Player playerScr;
    protected NavMeshAgent agent;

    protected MeshRenderer meshRenderer;
    protected Rigidbody rb;

    // 에너미의 사망 여부
    protected bool isDie = false;

    public virtual int DefaultAtkPercent
    {
        get { return defaultAtkPercent; }
        set
        {
            currHP = value;
        }
    }

    public int MoveDist
    {
        get { return moveDist; }
        set
        {
            moveDist = value;
        }
    }

    public void SetStateEffect(string stateEffectName, float time, Vector3 pos)
    {
        switch (stateEffectName)
        {
            case "Stun":
                state = State.STUN;
                stateEffect = StateEffect.STUN;
                break;
            case "KnockBack":
                state = State.STUN;
                stateEffect = StateEffect.KNOCKBACK;
                stateEffectPos = pos;
                Debug.Log("KnockBack" + pos);
                break;
            case "Binding":
                stateEffect = StateEffect.BINDING;
                break;
            case "Air":
                state = State.STUN;
                stateEffect = StateEffect.AIR;
                stateEffectPos = pos;
                Debug.Log("Air" + pos);
                break;
        }

        maxAbnormalTime = time;
        currAbnormalTime = maxAbnormalTime;
    }


    public override void Start()
    {
        enemyTr = GetComponent<Transform>();
        playerObj = GameObject.FindWithTag("PLAYER");
        playerScr = playerObj.GetComponent<Player>();
        agent = GetComponent<NavMeshAgent>();
        enemyTr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        meshRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();

        // 값 세팅
        isDie = false;
        posToMove[0] = transform.position;
        posToMove[1] = posToMove[0] + transform.TransformDirection(Vector3.forward) * moveDist;

        currDefaultAtkCoolTime = maxDefaultAtkCoolTime;
        currentPosIndexToMove = 1;
        agent.stoppingDistance = 0;

        currHP = maxHP;

        // Slider Bar
        hpBar = GetComponentInChildren<SliderBar>();
        shieldBar = GetComponentInChildren<ShieldBar>();
        hpBar.SetMaxValueUI(maxHP);
        hpBar.SetCurrValueUI(currHP);
        MaxShield = 0;

        stateEffect = StateEffect.NONE;

        currFindCoolTime = maxFindCoolTime;
    }
}
