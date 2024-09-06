using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : Enemy
{
    public enum State
    {
        IDLE,
        MOVE,
        TRACE,
        ATTACK,
        STUN,
        SKILL,
        DIE
    }


    // 에너미의 현재 상태
    public State state = State.MOVE;

    [SerializeField] // 추적 사정거리
    protected float traceDist = 10.0f;
    [SerializeField] // 공격 사정거리
    protected float attackDist = 2.0f;
    [SerializeField] // 인지 사정거리
    protected float cognitiveDist = 10.0f;

    [SerializeField] // 이동 위치
    protected Vector3[] posToMove = { Vector3.zero, Vector3.zero };
    protected int currentPosIndexToMove = 0;


    protected Transform monsterTr;
    private GameObject playerObj;
    protected Transform playerTr;
    public Player playerScr;
    private TokenManager tokenManager;
    protected NavMeshAgent agent;
    private Exp exp;
    protected Animator ani;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        monsterTr = GetComponent<Transform>();

        GetPlayerScript();

        tokenManager = GameObject.Find("TokenManager").GetComponent<TokenManager>();
        agent = GetComponent<NavMeshAgent>();
        ani = GetComponent<Animator>();

        // 값 세팅 
        currentPosIndexToMove = 0;
        agent.stoppingDistance = 0;
        ani.SetFloat("stunTime", stunTime);

        StartCoroutine(CheckEnemyMoveState());
        StartCoroutine(EnemyAction());
    }

    private void OnDestroy()
    {
        if(tokenManager != null)
            tokenManager.MakeToken(new Vector3(transform.position.x, 0.25f, transform.position.z));
        if (exp != null)
            exp.IncreaseExp(50);
    }

    // 플레이어 함수 가져오기
    public void GetPlayerScript()
    {
        playerObj = GameObject.FindWithTag("PLAYER");
        playerTr = playerObj.GetComponent<Transform>();
        playerScr = playerObj.GetComponent<Player>();
        exp = playerObj.GetComponent<Exp>();
    }

    private void OnTriggerEnter(Collider coll)
    {
        if(coll.CompareTag("MAP")) // 맵에 닿았을 때 방향 다시 설정 
        {
            SetCurrentPosIndexToMove();
            float distance = 0;
            // 앞으로 이동할 좌표를 랜덤하게 지정
            while (distance < cognitiveDist) // 이전 좌표와 인지 범위 내에서 새로 생성한 좌표의 거리가 최소 인지범위 거리 이상 될 수 있게 설정
            {
                posToMove[currentPosIndexToMove] = new Vector3(originalPos.x + ReturnRandomValue(0, cognitiveDist - 1),
                                                            originalPos.y,
                                                            originalPos.z + ReturnRandomValue(0, cognitiveDist - 1));

                distance = Vector3.Distance(monsterTr.transform.position, posToMove[currentPosIndexToMove]);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if(state == State.TRACE)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, traceDist);
        }
        if (state == State.ATTACK)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackDist);
        }
    }

    IEnumerator CheckEnemyMoveState()
    {
        while (!isDie)
        {
            //meshRenderer.material.color = Color.green;
            yield return null;

            if (state != State.MOVE && state != State.TRACE)
                continue;

            if (moveType == MoveType.PATH) // 경로 이동
                MovePath();
            else if (moveType == MoveType.TRACE) // 추적 이동
                MoveTrace();
            else
                Debug.LogWarning(moveType);
        }

        // 동작 루틴
        /*
         *  1. 일정한 인지 범위 내에 이동 (경로 이동, 추적 이동)
         *  2. 적 발견
         *  3. 추적
         *  4. 공격 (선공, 지속 선공, 회피, 수호)
         */ 
         
    }
    protected virtual IEnumerator EnemyAction()
    {
        while (!isDie)
        {
            SetCurrDefaultAtkCoolTime();
            SetCurrSkillCoolTime();
            ChangeCurrentStateToSkillState();
            //Debug.Log(state);
            switch (state)
            {
                case State.IDLE:
                    //meshRenderer.material.color = Color.green;
                    ani.SetBool("isIdle", true);
                    ani.SetBool("isMove", false);
                    ani.SetBool("isAttack", false);
                    ani.SetBool("isStun", false);
                    Debug.Log("Enemy Action Idle");
                    break;

                case State.MOVE:
                    ani.SetBool("isIdle", false);
                    ani.SetBool("isMove", true);
                    ani.SetBool("isAttack", false);
                    ani.SetBool("isStun", false);

                    //meshRenderer.material.color = Color.green;
                    agent.SetDestination(posToMove[currentPosIndexToMove]);
                    agent.stoppingDistance = 0;
                    agent.isStopped = false;
                    //Debug.Log("Enemy Action Move");
                    break;

                case State.TRACE:
                    ani.SetBool("isIdle", false);
                    ani.SetBool("isMove", true);
                    ani.SetBool("isAttack", false);
                    ani.SetBool("isStun", false);

                    //meshRenderer.material.color = Color.blue;
                    agent.SetDestination(playerTr.position);
                    agent.stoppingDistance = attackDist;
                    agent.isStopped = false;
                    break;

                case State.ATTACK:
                    ani.SetBool("isIdle", false);
                    ani.SetBool("isMove", false);
                    ani.SetBool("isAttack", true);
                    ani.SetBool("isStun", false);

                    agent.SetDestination(playerTr.position);
                    agent.isStopped = true;
                    //meshRenderer.material.color = Color.red;
                    break;

                case State.STUN:
                    ani.SetFloat("stunTime", stunTime);
                    ani.SetBool("isIdle", false);
                    ani.SetBool("isMove", false);
                    ani.SetBool("isAttack", false);
                    ani.SetBool("isStun", true);

                    agent.isStopped = true;
                    break;

                case State.SKILL:
                    // 해당 적 클래스에서 재정의하여 원하는 스킬을 호출한다. 
                    Debug.Log("Skill 사용");
                    break;

                case State.DIE:
                    Die();
                    break;
            }
            yield return null;
        }
    }

    /// <summary>
    /// 경로 이동
    /// </summary>
    public virtual void MovePath()
    {
        float distance = Vector3.Distance(posToMove[currentPosIndexToMove], monsterTr.transform.position);

        state = State.MOVE;

        if (distance <= 1)
            SetCurrentPosIndexToMove();

        distance = Vector3.Distance(playerTr.transform.position, monsterTr.transform.position);

        if (!CheckCognitiveDist(distance)) // 적이 플레이어를 인지했는지 확인한다. 
            return;

        if (distance <= attackDist)
        {
            // 공격이나 스킬을 사용하는 경우
            if (DecideUseOfAttackAndSkill() != -2)
                state = State.ATTACK;
            else
                state = State.IDLE;
        }
        else if (distance <= traceDist)
            state = State.TRACE;
        else
            state = State.IDLE;
    }

    /// <summary>
    /// 추적 이동
    /// </summary>
    public void MoveTrace()
    {
        float distance = Vector3.Distance(playerTr.transform.position, monsterTr.transform.position);

        if (distance <= attackDist)
        {
            // 공격이나 스킬을 사용하는 경우
            if (DecideUseOfAttackAndSkill() != -2)
                state = State.ATTACK;
            else
                state = State.IDLE;
        }
        else if (traceDist > 0) // 계속 추적하도록 설정
            state = State.TRACE;
        else
            state = State.IDLE;
    }

    /// <summary>
    /// Attack Type에 따라 플레이어를 인지했는지를 확인한다. 
    /// </summary>
    /// <returns></returns>
    protected bool CheckCognitiveDist(float distance)
    {
        if (attackType == AttackType.PREEMPTIVE) // 인지 범위 내에서만 공격
        {
            if (distance <= cognitiveDist)
                return true;
            else
                return false;
        }
        else if(attackType == AttackType.SUSTAINEDPREEMPTIVE) // 인지 범위 바깥까지 공격
            return true;
        else
        {
            Debug.LogWarning(attackType);
            return false;
        }
    }

    /// <summary>
    /// 현재 posIndexToMove 값을 설정한다.
    /// </summary>
    protected void SetCurrentPosIndexToMove()
    {
           if (currentPosIndexToMove >= posToMove.Length - 1) // 최대 인덱스 값에 도달하기 전에 0으로 다시 리셋되도록 설정
                currentPosIndexToMove = 0;
           else
                currentPosIndexToMove++;
    }

    /// <summary>
    /// 랜덤 값을 리턴한다. 
    /// </summary>
    /// <param name="min">최소값</param>
    /// <param name="max">최대값</param>
    /// <returns>음수 or 양수</returns>
    protected float ReturnRandomValue(float min, float max)
    {
        if(Random.Range(0,2) == 0)
            return -Random.Range(min, max);
        else 
            return Random.Range(min, max);
    }

    /// <summary>
    /// 적이 피해를 입을 때 플레이어 쪽에서 호출하는 함수
    /// </summary>
    /// <param name="damage"></param>
    public void Hit(float damage)
    {
        HP -= damage;
        state = State.STUN;
    }

    /// <summary>
    /// 플레이어가 자신의 공격범위에 있는지 확인한다.
    /// </summary>
    /// <returns></returns>
    public bool CheckIfPlayerIsWithInAttackRange()
    {
        if (Vector3.Distance(playerObj.transform.position, monsterTr.transform.position) <= attackDist)
            return true;
        else
            return false;
    }

    protected void ChangeCurrentStateToSkillState()
    {
        if (!CheckSkillCoolTime())
            return;

        // 스킬 쿨타임이 0이 되어서 사용할 준비가 된 경우
        state = State.SKILL;
    }

    public bool CheckSkillCoolTime()
    {
        /* <적 등급 별 루틴>
         *  하급 : skillCoolTime.Count == 0 => 실행 false
         *  상급 : 스킬 1개 => 해당 스킬 쿨타임 체크 => 0이면 true 아니면 false
         *  중간보스 : 스킬 2개 => 상황에 따라 어떤 스킬을 사용할지 체크 
         */

        for (int i=0; i < skillUsageStatus.Count; i++)
        {
            if (skillUsageStatus[i])
            {
                return true;
            }
        }

        return false;
    }

    public void SetCurrSkillCoolTime()
    {
        for(int i=0; i< currSkillCoolTimes.Count; i++)
        {
            if (skillUsageStatus[i]) // 해당 스킬 사용중인 상태
                continue;

            if (currSkillCoolTimes[i] > 0)
            {
                currSkillCoolTimes[i] -= Time.deltaTime;
                continue;
            }

            // 스킬 쿨타임이 다 돌은 경우
            if (state == State.IDLE || state == State.MOVE)
            {
                currSkillCoolTimes[i] = maxSkillCoolTimes[i];
                skillUsageStatus[i] = true;
            }
        }
    }

    public void SetCurrDefaultAtkCoolTime()
    {
        if(currDefaultAtkCoolTime > 0)
        {
            currDefaultAtkCoolTime -= Time.deltaTime;
            return;
        }

        currDefaultAtkCoolTime = 0;
    }


    /// <summary>
    /// 공격과 스킬 사용을 결정한다. 
    /// </summary>
    /// <returns> -2 : 공격,스킬 전부 사용 X  -1 : Attack  0~n : Skill N </returns>
    public int DecideUseOfAttackAndSkill()
    {
        int mostTimeSkillCoolTimeIndex = -1;

        // 우선순위 : 스킬 > 기본 공격

        for(int i=0; i < maxSkillCoolTimes.Count; i++)
        {
            // 나중에 스킬별 사용 조건까지 비교해야 함
            if (currSkillCoolTimes[i] <= 0)
            {
                if(i==0)
                {
                    mostTimeSkillCoolTimeIndex = i;
                    continue;
                }

                // 현재 사용가능한 스킬들 중 스킬 쿨타임이 가장 긴 스킬을 찾기 위함.
                if (maxSkillCoolTimes[i] > maxSkillCoolTimes[mostTimeSkillCoolTimeIndex])
                    mostTimeSkillCoolTimeIndex = i;
            }
        }

        // 스킬들 중 쿨타임이 돌은 스킬이 있는 경우
        if (mostTimeSkillCoolTimeIndex != -1)
            return mostTimeSkillCoolTimeIndex;

        if (currDefaultAtkCoolTime <= 0)
            return -1;

        return -2;
    }
}
