using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.AI;
using UnityEngine.UIElements;


public class Enemy : Entity, IObserver, IListener
{

    #region enum
    public enum EnemyType 
    { 
        Jiruru,
        Bansha,
        Witched,
        Fugitive,
        Target_Fugitive,
        Chaser_Jiruru,
        Fire_Jiruru
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
        FLEE
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
        STAGGER,
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

    // Debuff
    protected float maxDebuffTime;
    protected float currDebuffTime;

    //knock back
    protected Vector3 knockBackPos;

    // Stagger
    protected int staggerResistance;

    // 원거리 적
    protected float fleeDist = 4;
    protected bool isFlee = false;
    protected bool isOnEdge = false;

    // Component
    protected Transform enemyTr;
    protected GameObject playerObj;
    protected EnemyUI enemyUI;

    // 강해담 수정: player -> playerState
    //protected Player playerScr;
    protected PlayerState playerState;
    protected NavMeshAgent agent;
    protected Rigidbody rb;

    protected ShieldManager shieldManager;

    protected bool control = false;
    #endregion

    #region Properties
    public override float MAXHP
    {
        get => maxHP;
        set
        {
            // maxHP가 늘어날 경우, 늘어난 만큼의 체력을 현재 hp에서 더해주기
            float hpInterval = value - maxHP;

            maxHP = value;
            enemyUI.SetMaxHPBarUI(maxHP);

            HP += hpInterval;
        }
    }

    public override float HP
    {
        get => currHP;
        set
        {
            // 데미지 계산 공식 적용 필요
            float inputDamage = currHP - value;

            if (inputDamage < 0)
            {
                currHP = currHP + -inputDamage;
                if (currHP > maxHP) currHP = maxHP;
            }
            else
            {
                float damage = shieldManager.CalculateDamageWithDecreasingShield(inputDamage);
                if (damage <= 0)
                {
                    enemyUI.SetStateBarUIForCurValue(maxHP, currHP, CurrShield);
                    return;
                }

                enemyUI.StartDamageFlash(currHP, damage, maxHP);
                currHP -= damage;
            }

            enemyUI.SetStateBarUIForCurValue(maxHP, currHP, CurrShield);

            if (currHP <= 0)
                IsDie = true;
        }
    }

    public override float CurrShield
    {
        get => shieldManager.CurShield;
        set
        {
            currShield = Mathf.Max(0, value);
            enemyUI.SetStateBarUIForCurValue(MAXHP, HP, value);
        }
    }
    public override float MaxShield
    {
        get => shieldManager.MaxShield;
        set
        {
            shieldManager.MaxShield = value; 
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

    public virtual void Hit(InkType inkType, float damage, DebuffState debuffState, float debuffTime, Vector3 subjectPos = default) {}

    private void Awake()
    {
        InitComponent();
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

        //쉴드
        shieldManager = DebugUtils.GetComponentWithErrorLogging<ShieldManager>(this.gameObject, "ShieldManager");
        shieldManager.Attach(this);
        EventManager.Instance.AddListener(EVENT_TYPE.Generate_Shield_Enemy, this);
    }

    /// <summary>
    /// 데이터 읽어와서 값 할당시 사용
    /// </summary>
    /// <param name="enemyData"></param>
    public virtual void InitStat(EnemyData enemyData)
    {
        rank = enemyData.rank;
        enemyType = enemyData.enemyType;
        posType = enemyData.posType;
        personality = enemyData.personality;
        patrolType = enemyData.patrolType;
        attackDistType = enemyData.attackDistType;
        inkType = enemyData.inkType;

        MAXHP = enemyData.hp;
        atk = enemyData.atk;
        cognitiveDist = enemyData.cognitiveDist;
        inkTypeResistance = enemyData.inkTypeResistance;
        staggerResistance = enemyData.staggerResistance;

        oriAttackSpeed = enemyData.atkSpeed;
        originalMoveSpeed = enemyData.moveSpeed;
        maxFirstWaitTime = enemyData.firstWaitTime;
        maxAttackWaitTime = enemyData.attackWaitTime;

        transform.rotation = Quaternion.Euler(enemyData.spawnDir);
        patrolDestinations = enemyData.destinations;
    }

    /// <summary>
    /// 데이터 입력 후 기본적인 세팅
    /// </summary>
    public virtual void InitStatValue()
    {
        // rank
        // personality
        // 성격에 따른 상태 설정
        SetPersonality();

        currAttackSpeed = 0.8f; //oriAttackSpeed
        CurrAttackSpeed = currAttackSpeed;
        patrolDestinationIndex = 0;
        agent.stoppingDistance = 0;

        transform.position = patrolDestinations[patrolDestinationIndex];
        agent.Warp(transform.position);

        //Debug.Log($"{gameObject.name} : {transform.position}에 생성됨");
        //Debug.Log($"Nav Mesh is On : {agent.isOnNavMesh}");
        //agent.SamplePathPosition();
        //if(!agent.isOnNavMesh)
        //{
        //    NavMeshHit hit;
        //    // SamplePosition을 사용하여 해당 위치에서 유효한 NavMesh 지점 찾기
        //    if (NavMesh.SamplePosition(transform.position, out hit, 50, NavMesh.AllAreas))
        //    {
        //        //Debug.Log($"유효한 위치 존재 : {hit.position}");
        //        agent.Warp(hit.position);
        //        transform.position = hit.position;  // 유효한 위치
        //    }
        //    else
        //    {
        //        //Debug.Log("유효한 위치 없음");
        //    }
        //    //Debug.Log($"Re - Nav Mesh is On : {agent.isOnNavMesh}");
        //}

        PatrolDestinationIndex += 1;
        
        currDestination = patrolDestinations[patrolDestinationIndex];
        // 체력에 대한 UI 세팅
        MaxShield = MAXHP;
        HP = maxHP;

        maxPatrolWaitTime = Random.Range(1, 2);

        currFirstWaitTime = maxFirstWaitTime;
        currPatrolWaitTime = maxPatrolWaitTime;
        currAttackWaitTime = maxAttackWaitTime;

        isDie = false;

        agent.acceleration = 1000f; // 적은 항상 최대 속도(agent.speed)로 이동하도록 설정
        agent.angularSpeed = 360f; // 플레이어의 이속에 상관없이 바로 회전할 수 있도록 설정

        //원거리적
        isOnEdge = false;
        isFlee = false;

        control = true;
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

    public IEnumerator ChangeInkTypeResistance(float time, int percentageToApply)
    {
        inkTypeResistance = percentageToApply;
        //Debug.Log($"잉크 저항 : {inkTypeResistance}");
        yield return new WaitForSeconds(time);
        inkTypeResistance = 0;
    }

    #endregion

    #region Set DetailState

    /// <summary>
    /// 상태 이상을 설정
    /// </summary>
    /// <param name="stateEffectName"></param>
    /// <param name="time"></param>
    /// <param name="pos"></param>

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

                enemyUI.ActivatePerceiveImg();
                break;

            case Personality.PATROL:


                state = State.IDLE;
                idleState = IdleState.FIRSTWAIT;
                moveState = MoveState.NONE;
                attackState = AttackState.NONE;
                debuffState = DebuffState.NONE;

                didPerceive = false;
                break;

            default:
                Debug.LogWarning(personality);
                break;

        }
    }

    #endregion

    public void Notify(Subject subject)
    {
        enemyUI.SetStateBarUIForCurValue(maxHP, HP, CurrShield);
    }

    private void DebuffEnd()
    {
        if (debuffState == DebuffState.STUN)
        {
            state = State.IDLE;
            idleState = IdleState.FIRSTWAIT;
        }
        else
            debuffState = DebuffState.NONE;
    }

    private void DieEnd()
    {
        isDie = true;
    }



    protected void Dead()
    {
        switch(enemyType)
        {
            case EnemyType.Jiruru:
            case EnemyType.Bansha:
            case EnemyType.Fire_Jiruru:
            case EnemyType.Chaser_Jiruru:
                EnemyPooler.Instance.ReleaseEnemy(enemyType, gameObject);
                break;

            case EnemyType.Fugitive:
            case EnemyType.Target_Fugitive:
            case EnemyType.Witched:
                EnemyPooler.Instance.ReleaseAllEnemy(enemyType);
                break;
        }
    }


    public void OnEvent(EVENT_TYPE eventType, UnityEngine.Component Sender, object Param)
    {
        switch (eventType)
        {
            case EVENT_TYPE.Generate_Shield_Enemy:
                System.Tuple<float, float> shildData = ((System.Tuple<System.Tuple<float, float>, GameObject>)Param).Item1;
                GameObject enemyObj = ((System.Tuple<System.Tuple<float, float>, GameObject>)Param).Item2;
                float shieldAmount = shildData.Item1;
                float shieldDuration = shildData.Item2;

                // 쉴드를 사용하는 객체가 아닌 경우
                if (!enemyObj.Equals(this.gameObject))
                    return;
                   
                //Debug.Log($"{gameObject.name} : 쉴드 추가 ({shieldAmount}, {shieldDuration})");
                shieldManager.GenerateShield(shieldAmount, shieldDuration);
                enemyUI.SetStateBarUIForCurValue(maxHP, HP, CurrShield);
                break;
        }
    }
}
