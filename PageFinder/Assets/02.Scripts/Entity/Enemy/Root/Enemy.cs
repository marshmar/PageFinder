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

    #region enum
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
        ABNORMAL,
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
        TRACE,
        ROTATE
    }

    protected enum AttackState
    {
        NONE,
        ATTACKWAIT,
        DEFAULT, //DEFAULT Attack
        REINFORCEMENT, //REINFORCEMENT Attack
        SKILL
    }

    // CircleRange 스크립트에서 접근하기 때문에 Public ex)
    public enum AbnomralState
    {
        NONE,
        STUN,
        KNOCKBACK,
        BINDING,
        AIR,
    }

    #endregion

    #region Variables
    [Header("State")]
    protected State state = State.IDLE; // 에너미의 현재 상태
    protected IdleState idleState; // 에너미의 현재 상태
    protected MoveState moveState;
    protected AttackState attackState;
    protected AbnomralState abnormalState;

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

    // 공속
    protected float oriAttackSpeed = 1;
    protected float currAttackSpeed = 1;

    // 컴포넌트
    protected Transform enemyTr;
    protected GameObject playerObj;

    // 강해담 수정: player -> playerState
    //protected Player playerScr;
    protected PlayerState playerState;
    protected NavMeshAgent agent;

    protected Rigidbody rb;

    // 에너미의 사망 여부
    protected bool isDie = false;

    #endregion

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

    public virtual float OriAttackSpeed
    {
        get
        {
            return oriAttackSpeed;
        }
        set
        {
            oriAttackSpeed = value;
        }
    }

    public virtual float CurrAttackSpeed
    {
        get
        {
            return currAttackSpeed;
        }
        set
        {
            currAttackSpeed = value;
        }
    }

    public IEnumerator ChangeAttackSpeed(float time, float value)
    {
        yield return new WaitForSeconds(time);
        CurrAttackSpeed = value;
    }

    public IEnumerator ChangeMoveSpeed(float time, float value)
    {
        yield return new WaitForSeconds(time);
        CurrAttackSpeed = value;
    }

    /// <summary>
    /// 상태 이상을 설정
    /// </summary>
    /// <param name="stateEffectName"></param>
    /// <param name="time"></param>
    /// <param name="pos"></param>
    public void SetStateEffect(AbnomralState type, float time, Vector3 pos)
    {
        state = State.ABNORMAL;
        switch (type)
        {
            case AbnomralState.STUN:
                abnormalState = AbnomralState.STUN;
                break;

            case AbnomralState.KNOCKBACK:
                abnormalState = AbnomralState.KNOCKBACK;
                break;

            case AbnomralState.BINDING:
                abnormalState = AbnomralState.BINDING;
                break;

            case AbnomralState.AIR:
                abnormalState = AbnomralState.AIR;
                break;

            default:
                abnormalState = AbnomralState.NONE;
                break;
        }

        stateEffectPos = pos;
        maxAbnormalTime = time;
        currAbnormalTime = maxAbnormalTime;
    }


    public override void Start()
    {
        enemyTr = DebugUtils.GetComponentWithErrorLogging<Transform>(transform, "Transform");
        playerObj = GameObject.FindWithTag("PLAYER");
        playerState = DebugUtils.GetComponentWithErrorLogging<PlayerState>(playerObj, "PlayerState");
        //playerScr = DebugUtils.GetComponentWithErrorLogging<Player>(playerObj, "Player");
        agent = DebugUtils.GetComponentWithErrorLogging<NavMeshAgent>(gameObject, "NavMeshAgent");
        rb = DebugUtils.GetComponentWithErrorLogging<Rigidbody>(gameObject, "Rigidbody");

        // 값 세팅
        isDie = false;
        posToMove[0] = transform.position;
        posToMove[1] = posToMove[0] + transform.TransformDirection(Vector3.forward) * moveDist; //posToMove[0] + transform.TransformDirection(Vector3.forward) * moveDist

        currDefaultAtkCoolTime = maxDefaultAtkCoolTime;
        currentPosIndexToMove = 1;
        agent.stoppingDistance = 0;

        currHP = maxHP;

        // Slider Bar
        hpBar = GetComponentInChildren<SliderBar>();
        shieldBar = GetComponentInChildren<ShieldBar>();
        hpBar.SetMaxValueUI(maxHP);
        hpBar.SetCurrValueUI(currHP);
        //MaxShield = 0;

        abnormalState = AbnomralState.NONE;

        currFindCoolTime = maxFindCoolTime;


        
        agent.acceleration = 1000f; // 적은 항상 최대 속도(agent.speed)로 이동하도록 설정
        agent.angularSpeed = 360f; // 플레이어의 이속에 상관없이 바로 회전할 수 있도록 설정
    }

    public void SetStatus(BattlePage page, int index)
    {
        moveDist = page.moveDist[index];
        maxHP = page.maxHP[index];
        atk = page.atk[index];
        currAttackSpeed = page.atkSpeed[index]; 
    }
}
