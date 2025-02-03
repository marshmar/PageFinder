using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.AI;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;
using UnityEngine.Playables;

public class Enemy : Entity
{

    #region enum
    public enum EnemyType 
    { 
        Jiruru,
        Bansha,
        Witched
    };

    public enum State
    {
        IDLE,
        MOVE,
        ATTACK,
        DEBUFF,
        DIE
    }

    public enum IdleState
    {
        NONE,
        FIRSTWAIT, // 맨 처음 쉬는 동작(EX.STUN 이후) : 애니메이션 종료 후 Find로 넘어감
        PATROLWAIT,// 경로 도착 후 대기, Stun 이후 : 애니메이션 종료 후 Find로 넘어감
        ATTACKWAIT, // Perceive -> attack 넘어갈 때 0.5초 대기시 : 애니메이션 종료 후 Attack(Basic, Reinforce, Skill)으로 넘어감
    }

    public enum MoveState
    {
        NONE,
        PATROL,
        CHASE,
        ROTATE, // 인지거리에 플레이어가 있지만 앞에 없어서 회전해야하는 경우
        RUN
    }

    public enum AttackState
    {
        NONE,
        BASIC, //BASIC Attack
        REINFORCEMENT, //REINFORCEMENT Attack
        SKILL
    }

    // CircleRange 스크립트에서 접근하기 때문에 Public ex)
    public enum DebuffState
    {
        NONE,
        STIFF,
        KNOCKBACK,
        BINDING,
        STUN,
    }

    public enum PosType
    {
        GROUND,
        SKY
    }

    public enum Rank
    {
        MINION,
        NORMAL,
        ELITE,
        BOSS
    }

    public enum Personality
    {
        STATIC,
        CHASER,
        PATROL
    }

    public enum PatrolType
    {
        PATH,
        FIX
    }

    public enum AttackDistType
    {
        SHORT,
        LONG
    }
    #endregion

    #region Variables
    protected EnemyType enemyType;

    // State
    public State state; // 에너미의 현재 상태
    public IdleState idleState;
    public MoveState moveState;
    public AttackState attackState;
    public DebuffState debuffState;

    protected PosType posType = PosType.GROUND;
    [SerializeField]
    protected Rank rank;
    [SerializeField]
    protected Personality personality;
    protected PatrolType patrolType;

    [SerializeField]
    protected AttackDistType attackDistType;

    //Idle
    protected float maxFirstWaitTime;
    protected float maxPatrolWaitTime;
    protected float maxAttackWaitTime;

    protected float currFirstWaitTime;
    protected float currPatrolWaitTime;
    protected float currAttackWaitTime;

    // Debuff
    protected float maxDebuffTime;
    protected float currDebuffTime;

    // Patrol
    protected Vector3 currDestination;
    protected List<Vector3> patrolDestinations = new List<Vector3>();
    protected int patrolDestinationIndex;

    // AttackSpeed
    protected float oriAttackSpeed;
    protected float currAttackSpeed;

    protected float cognitiveDist;

    protected bool didPerceive; // 적 로직에서 적을 인지했는지 여부

    protected bool isDie;

    // Ink
    protected InkType inkType;
    protected int inkTypeResistance;

    // Stagger
    protected int staggerResistance;

    // Component
    protected Transform enemyTr;
    protected GameObject playerObj;
    protected EnemyUI enemyUI;

    // 강해담 수정: player -> playerState
    //protected Player playerScr;
    protected PlayerState playerState;
    protected NavMeshAgent agent;
    protected Rigidbody rb;


    #endregion

    #region Properties
    public override float MAXHP
    {
        get
        {
            return maxHP;
        }
        set
        {
            maxHP = value;
            enemyUI.SetMaxHPBarUI(maxHP);
        }
    }

    public override float HP
    {
        get
        {
            return currHP;
        }
        set
        {
            currHP = value;

            enemyUI.SetCurrHPBarUI(maxHP, currHP, currShield);

            if (currHP <= 0)
            {
                Die();
            }
        }
    }

    public override float MaxShield
    {
        get
        {
            return maxShield;
        }
        set
        {
            // 실드를 생성한 경우
            maxShield = value;
            enemyUI.SetMaxShieldUI(maxHP);
            CurrShield = maxShield;
        }
    }

    public override float CurrShield
    {
        get
        {
            return currShield;
        }
        set
        {
            currShield = value;

            // 쉴드를 다 사용했을 경우
            if (currShield <= 0)
                currShield = 0;

            enemyUI.SetCurrShieldUI(maxHP, currHP, currShield);
        }
    }

    protected virtual float OriAttackSpeed
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

    protected virtual float CurrAttackSpeed
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

    protected int PatrolDestinationIndex
    {
        get
        {
            return patrolDestinationIndex;
        }
        set
        {
            if (value >= patrolDestinations.Count || value < 0)
                patrolDestinationIndex = 0;
            else
                patrolDestinationIndex = value;
        }
    }

    public bool IsDie
    {
        get { return isDie; }
        set { isDie = value; }
    }
    #endregion

    /// <summary>
    /// Debuff용 Hit
    /// </summary>
    /// <param name="inkType"></param>
    /// <param name="damage"></param>
    /// <param name="debuffState"></param>
    /// <param name="debuffTime"></param>
    /// <param name="knockBackDir"></param>
    public void Hit(InkType inkType, float damage, DebuffState debuffState, float debuffTime, Vector3 subjectPos = default)
    {
        float diff = 0.0f;

        // 잉크 저항 적용
        if (this.inkType == inkType)
            damage = damage - (damage * inkTypeResistance / 100.0f);

        damage = damage - def; // 방어력 적용

        // def가 더 커서 회복되는 경우
        if (damage < 0)
            damage = 0;

        // 쉴드가 있는 경우
        if (currShield > 0)
        {
            diff = currShield - damage;
            CurrShield -= damage;

            if (diff < 0)
                HP += diff;
        }
        else
            HP -= damage;

        enemyUI.StartCoroutine(enemyUI.DamagePopUp(inkType, damage));

        if (HP <= 0)
            return;

        if (debuffState != DebuffState.NONE)
            SetDebuff(debuffState, debuffTime, (enemyTr.position - subjectPos).normalized);
        else
            Debug.LogWarning(debuffState);
    }

    /// <summary>
    /// 잉크용 Hit
    /// </summary>
    /// <param name="inkType"></param>
    /// <param name="damage"></param>
    public void Hit(InkType inkType, float damage)
    {
        float diff = 0.0f;

        // 잉크 저항 적용
        if (this.inkType == inkType)
            damage = damage - (damage * inkTypeResistance / 100.0f);

        damage = damage - def; // 방어력 적용

        // def가 더 커서 회복되는 경우
        if (damage < 0)
            damage = 0;

        // 쉴드가 있는 경우
        if (currShield > 0)
        {
            diff = currShield - damage;
            CurrShield -= damage;

            if (diff < 0)
                HP += diff;
        }
        else
            HP -= damage;

        enemyUI.StartCoroutine(enemyUI.DamagePopUp(inkType, damage));
    }

    public override void Start()
    {
        InitComponent();
        InitStat();
    }

    #region Init
    protected virtual void InitComponent()
    {
        enemyTr = DebugUtils.GetComponentWithErrorLogging<Transform>(transform, "Transform");
        playerObj = GameObject.FindWithTag("PLAYER");
        playerState = DebugUtils.GetComponentWithErrorLogging<PlayerState>(playerObj, "PlayerState");
        agent = DebugUtils.GetComponentWithErrorLogging<NavMeshAgent>(gameObject, "NavMeshAgent");
        rb = DebugUtils.GetComponentWithErrorLogging<Rigidbody>(gameObject, "Rigidbody");
        enemyUI = DebugUtils.GetComponentWithErrorLogging<EnemyUI>(gameObject, "EnemyUI");
    }

    /// <summary>
    /// 데이터 읽어와서 값 할당시 사용
    /// </summary>
    /// <param name="enemyData"></param>
    public virtual void InitStat(EnemyData enemyData)
    {
        rank = enemyData.rank;
        posType = enemyData.posType;
        personality = enemyData.personality;
        patrolType = enemyData.patrolType;
        inkType = enemyData.inkType;


        maxHP = enemyData.hp;
        atk = enemyData.atk;
        def = enemyData.def;
        cognitiveDist = enemyData.cognitiveDist;
        inkTypeResistance = enemyData.inkTypeResistance;
        staggerResistance = enemyData.staggerResistance;
        

        oriAttackSpeed = enemyData.atkSpeed;
        originalMoveSpeed = enemyData.moveSpeed;
        maxFirstWaitTime = enemyData.firstWaitTime;
        maxAttackWaitTime = enemyData.attackWaitTime;

        //dropItem = 

        transform.rotation = Quaternion.Euler(enemyData.spawnDir);
        patrolDestinations = enemyData.destinations;
    }

    /// <summary>
    /// 데이터 입력 후 기본적인 세팅
    /// </summary>
    protected virtual void InitStat()
    {
        Debug.Log("기본 초기화");
        // rank
        // personality
        // 성격에 따른 상태 설정
        SetPersonality();

        maxDebuffTime = 0;
        currDebuffTime = maxDebuffTime;
        
        currAttackSpeed = oriAttackSpeed;
        
        patrolDestinationIndex = 0;
        agent.stoppingDistance = 0;
        transform.position = patrolDestinations[patrolDestinationIndex];
        PatrolDestinationIndex += 1;
        
        currDestination = patrolDestinations[patrolDestinationIndex];
        // 체력에 대한 UI 세팅
        MAXHP = maxHP;
        HP = 100;
        MaxShield = 100;

        maxPatrolWaitTime = Random.Range(1, 2);

        currFirstWaitTime = maxFirstWaitTime;
        currPatrolWaitTime = maxPatrolWaitTime;
        currAttackWaitTime = maxAttackWaitTime;

        isDie = false;

        agent.acceleration = 1000f; // 적은 항상 최대 속도(agent.speed)로 이동하도록 설정
        agent.angularSpeed = 360f; // 플레이어의 이속에 상관없이 바로 회전할 수 있도록 설정
    }

    #endregion

    #region Change Stat

    /// <summary>
    /// 공속을 변경할 경우 이용하는 함수
    /// </summary>
    /// <param name="time">지속 시간</param>
    /// <param name="percentageToApply">적용할 퍼센테이지</param>
    /// <returns></returns>
    public IEnumerator ChangeAttackSpeed(float time, float percentageToApply)
    {
        CurrAttackSpeed = CurrAttackSpeed * percentageToApply;
        yield return new WaitForSeconds(time);
        CurrAttackSpeed = OriAttackSpeed;
    }

    /// <summary>
    /// 이속을 변경할 경우 이용하는 함수
    /// </summary>
    /// <param name="time">지속 시간</param>
    /// <param name="percentageToApply">적용할 퍼센테이지</param>
    /// <returns></returns>
    public IEnumerator ChangeMoveSpeed(float time, float percentageToApply)
    {
        MoveSpeed = MoveSpeed * percentageToApply;
        yield return new WaitForSeconds(time);
        MoveSpeed = OriginalMoveSpeed;
    }

    #endregion

    #region Set DetailState

    /// <summary>
    /// 상태 이상을 설정
    /// </summary>
    /// <param name="stateEffectName"></param>
    /// <param name="time"></param>
    /// <param name="pos"></param>
    public void SetDebuff(DebuffState type, float time, Vector3 dir = default)
    {
        state = State.DEBUFF;
        idleState = IdleState.NONE;
        moveState = MoveState.NONE;
        attackState = AttackState.NONE;

        agent.destination = enemyTr.position;
        agent.isStopped = true;

        maxDebuffTime = time;
        currDebuffTime = maxDebuffTime;

        switch (type)
        {
            case DebuffState.STIFF:
                debuffState = DebuffState.STIFF;
                break;

            case DebuffState.KNOCKBACK:
                debuffState = DebuffState.KNOCKBACK;

                // 코루틴이 동작중일 때 어떻게 처리할 것인지 생각해보기
                StartCoroutine(KnockBack(enemyTr.position + dir));
                break;

            case DebuffState.BINDING:
                debuffState = DebuffState.BINDING;
                break;

            case DebuffState.STUN:
                debuffState = DebuffState.STUN;
                break;

            default:
                debuffState = DebuffState.NONE;
                break;
        }
        //Debug.Log($"Enemy Debuff : {type} - {time}초");
    }

    private void SetPersonality()
    {
        switch(personality)
        {
            case Personality.STATIC:
                state = State.IDLE;
                idleState = IdleState.FIRSTWAIT;
                moveState = MoveState.NONE;
                attackState = AttackState.NONE;
                debuffState = DebuffState.NONE;

                didPerceive = false;
                break;

            case Personality.CHASER:
                state = State.MOVE;
                idleState = IdleState.NONE;
                moveState = MoveState.CHASE;
                attackState = AttackState.NONE;
                debuffState = DebuffState.NONE;

                didPerceive = true;
                break;

            // 아직 로직 구성 못함
            //case Personality.PATROL:
            //    

            //    state = State.IDLE;
            //    idleState = IdleState.FIRSTWAIT;
            //    moveState = MoveState.NONE;
            //    attackState = AttackState.NONE;
            //    debuffState = DebuffState.NONE;

            //    didPerceive = false;
            //    break;

            default:
                Debug.LogWarning(personality);
                break;

        }
    }

    #endregion

    private IEnumerator KnockBack(Vector3 targetPos)
    {
        // n초 동안 해당 지점까지 이동할 수 있도록 해야 함
        agent.enabled = false;
        //Debug.Log("넉백 코루틴 시작");

        //Debug.Log($"현재 적 위치 : {enemyTr.position}");
        //Debug.Log($"목표 적 위치 : {targetPos}");
        float dist = Vector3.Distance(enemyTr.position, targetPos);
        while (dist >= 0.1f)
        {
            transform.position = Vector3.MoveTowards(enemyTr.position, targetPos, Time.deltaTime / currDebuffTime);
            yield return null;
        }

        //Debug.Log("넉백 코루틴 끝");
        agent.enabled = true;
    }
}
