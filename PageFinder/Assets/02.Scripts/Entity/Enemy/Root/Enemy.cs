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
        ABONORMAL,
        MOVE,
        ATTACK,
        DIE
    }

    protected enum IdleState
    {
        NONE,
        DEFAULT
    }

    protected enum AbnormalState
    {
        NONE,
        STUN,
        BINDING
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

    [Header("State")]
    
    [SerializeField]
    protected State state = State.IDLE; // 에너미의 현재 상태
    [SerializeField]
    protected IdleState idleState; // 에너미의 현재 상태
    [SerializeField]
    protected AbnormalState abnormalState;
    [SerializeField]
    protected MoveState moveState;
    [SerializeField]
    protected AttackState attackState;
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

    [Header("Stun")]
    [SerializeField]
    protected float stunTime = 0.2f; // 경직 시간

    [SerializeField]
    protected Slider hpBar;

    // 컴포넌트
    protected Transform enemyTr;
    protected GameObject playerObj;
    protected Player playerScr;
    protected TokenManager tokenManager;
    protected NavMeshAgent agent;
    protected Exp exp;
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

            if (currHP <= 0)
            {
                // <해야할 처리>

                // 플레이어 경험치 획득
                // 토큰 생성 
                isDie = true;
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

    public override void Start()
    {
        base.Start();

        enemyTr = GetComponent<Transform>();
        playerObj = GameObject.FindWithTag("PLAYER");
        playerScr = playerObj.GetComponent<Player>();
        exp = playerObj.GetComponent<Exp>();
        tokenManager = GameObject.Find("TokenManager").GetComponent<TokenManager>();
        agent = GetComponent<NavMeshAgent>();
        enemyTr = GetComponent<Transform>();
        meshRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();

        // 값 세팅
        isDie = false;
        currHP = maxHP;
        hpBar.maxValue = maxHP;
        hpBar.value = maxHP;
        currDefaultAtkCoolTime = maxDefaultAtkCoolTime;
        currentPosIndexToMove = 0;
        agent.stoppingDistance = 0;

        currFindCoolTime = maxFindCoolTime;

    }
}
