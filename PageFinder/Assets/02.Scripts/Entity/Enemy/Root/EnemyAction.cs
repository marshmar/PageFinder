using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.LowLevel;

public class EnemyAction : EnemyAnimation
{
    protected bool isUpdaterCoroutineWorking = false;

    public override float HP
    {
        get
        {
            return currHP; //+ currShield;  // 100 + 50   - 55
        }
        set
        {
            // 감소시켜도 쉴드가 남아있는 경우
            //if (value > currHP)
            //{
            //    CurrShield = value - currHP;
            //}
            //else // 감소시켜도 쉴드가 남아있지 않은 경우
            //{
            //    CurrShield = 0;
            //    currHP = value;
            //}
            currHP = value;

            Hit();
            hpBar.SetCurrValueUI(currHP);
            if (currHP <= 0)
            {
                if (gameObject.name.Contains("Jiruru"))
                    playerState.Coin += 50;
                else if (gameObject.name.Contains("Bansha"))
                    playerState.Coin += 100;
                else
                    playerState.Coin += 250;

                isDie = true;
                // <해야할 처리>
                EnemyManager.Instance.DestroyEnemy("enemy", gameObject);
                //Debug.Log("적 비활성화");
                // 플레이어 경험치 획득
                // 토큰 생성 
                //Die();
            }

        }
    }

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        if(!isUpdaterCoroutineWorking)
            StartCoroutine(Updater());
    }

    /// <summary>
    /// 적이 피해를 입을 때 플레이어 쪽에서 호출하는 함수
    /// </summary>
    /// <param name="damage"></param>
    protected virtual void Hit()
    {
        //SetStateEffect("Stun", 0.2f, Vector3.zero);
        //Debug.Log("Hit");
    }

    private void OnTriggerEnter(Collider coll)
    {
        if (coll.CompareTag("MAP")) // 맵에 닿았을 때 방향 다시 설정 
        {
            SetCurrentPosIndexToMove();
        }
    }

    private void OnDrawGizmos()
    {
        if (state == State.MOVE)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, cognitiveDist);
        }
        if (state == State.ATTACK)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, atkDist);
        }
    }
    protected IEnumerator Updater()
    {
        isUpdaterCoroutineWorking = true;

        while (!isDie)
        {
            if (playerObj == null)
                break;

            SetAllCoolTime();
            SetRootState();

            switch (state)
            {
                case State.IDLE:
                    moveState = MoveState.NONE;
                    attackState = AttackState.NONE;

                    SetIdleState();
                    IdleAction();
                    break;

                case State.ABNORMAL:
                    idleState = IdleState.NONE;
                    moveState = MoveState.NONE;
                    attackState = AttackState.NONE;

                    AbnormalState();
                    break;

                case State.MOVE:
                    idleState = IdleState.NONE;
                    attackState = AttackState.NONE;

                    SetMoveState();
                    MoveAction();
                    break;

                case State.ATTACK:
                    idleState = IdleState.NONE;
                    moveState = MoveState.NONE;

                    SetAttackState();
                    AttackAction();
                    break;

                case State.DIE:
                    idleState = IdleState.NONE;
                    moveState = MoveState.NONE;
                    attackState = AttackState.NONE;

                    Die();
                    break;
            }
            yield return null;
        }
    }

    #region State 관련 함수

    protected virtual void SetRootState()
    {
        float distance;
        distance = Vector3.Distance(playerObj.transform.transform.position, enemyTr.position);

        // 상태이상일 경우
        if (state == State.ABNORMAL)
            return;

        // 공격 상태일 때 애니메이션이 끝나고 동작할 수 있도록 함 
        // 애니메이션의 Event에서 애니메이션이 끝났을 경우 ~AniEnd()를 호출할 때 AttackState.None으로 변경해놓는 것을 이용
        if (state == State.ATTACK && attackState != AttackState.NONE)
            return;

        if (distance <= atkDist)
        {
            // 적이 플레이어 앞에 있는 경우
            if (CheckIfThereIsPlayerInFrontOfEnemy())
                state = State.ATTACK;
            else// 플레이어를 바라보도록 회전하게 함 
                state = State.MOVE;
        }
        else if (distance <= cognitiveDist)
        {
            if (abnormalState == AbnomralState.BINDING)
                state = State.IDLE;
            else
                state = State.MOVE;
        }
        else // 인지 범위 바깥인 경우
        {
            if (abnormalState == AbnomralState.BINDING)
            {
                state = State.IDLE;
                return;
            }
                
            // 지속 선공형이 플레이어를 추적중일 때
            if (attackPattern == AttackPattern.SUSTAINEDPREEMPTIVE && playerRecognitionStatue)
            {
                state = State.MOVE;
                return;
            }

            if (currFindCoolTime > 0)
                state = State.IDLE;
            else
                state = State.MOVE;
        }
    }

    protected void SetIdleState()
    {
        // 나중에 Idle 에서 여러가지 종류가 있으면 그때 따로 설정하기
        idleState = IdleState.DEFAULT;
    }


    protected void SetMoveState()
    {
        float distance = Vector3.Distance(playerObj.transform.transform.position, enemyTr.position);

        switch(attackPattern)
        {
            // 인지 거리에 들어오지 않았을 경우 탐색
            // 인지 거리에 들어왔을 경우 추적
            case AttackPattern.PREEMPTIVE:
            case AttackPattern.AVOIDANCE:
            case AttackPattern.GUARD:

                if (distance > cognitiveDist)
                    moveState = MoveState.FIND;
                else
                    moveState = MoveState.TRACE;

                break;

            case AttackPattern.SUSTAINEDPREEMPTIVE:

                // 플레이어를 한 번이라도 인지했을 경우
                if (playerRecognitionStatue)
                    moveState = MoveState.TRACE;
                else
                {
                    if (distance > cognitiveDist)
                        moveState = MoveState.FIND;
                    else
                        moveState = MoveState.TRACE;
                }

                break;
            default:
                Debug.LogWarning(attackPattern);
                break;
        }

        // 플레이어가 움직이지 않고 회전만 해야해야하는 경우
        if (distance <= atkDist)
        {
            agent.destination = transform.position;

            // 플레이어가 자기 자신 앞에 있지 않을 경우
            if (!CheckIfThereIsPlayerInFrontOfEnemy())
                moveState = MoveState.ROTATE;
        } 

        switch (moveState)
        {
            case MoveState.FIND:
                FindState();
                break;

            case MoveState.TRACE:
                break;

            case MoveState.ROTATE:
                break;

            default:
                Debug.LogWarning(moveState);
                break;
        }
    }

    protected virtual void SetAttackState()
    {
        if (currDefaultAtkCoolTime <= 0)
            attackState = AttackState.DEFAULT;
        else // 쿨타임이 돌지 않았을 경우 Idle -> Attack Wait
            attackState = AttackState.ATTACKWAIT;
    }

    private void FindState()
    {
        switch (findPattern)
        {
            case FindPattern.PATH:
                MovePath();
                break;
            case FindPattern.FIX:
                break;
            default:
                Debug.LogWarning(findPattern);
                break;
        }
    }

    #endregion

    #region Action 관련 함수

    protected void IdleAction()
    {
        agent.destination = transform.position;
        agent.isStopped = true;
    }

    protected void MoveAction()
    {
        switch(moveState)
        {
            case MoveState.NONE:
                break;

            case MoveState.FIND:
                FindAction();
                break;

            case MoveState.TRACE:
                TraceAction();
                break;

            case MoveState.ROTATE:
                RotateAction();
                break;

            default:
                Debug.LogWarning(moveState);
                break;

        }
    }

    protected virtual void AttackAction()
    {
        switch(attackState)
        {
            case AttackState.NONE:
                break;

            case AttackState.ATTACKWAIT:
                AttackWaitAction();
                break;

            case AttackState.DEFAULT:
                DefaultAttackAction();
                break;

            default:
                Debug.LogWarning(attackState);
                break;
        }
    }

    private void FindAction()
    {
        switch(findPattern)
        {
            case FindPattern.PATH:
                agent.destination = posToMove[currentPosIndexToMove];
                agent.isStopped = false;
                agent.updateRotation = true;
                break;

            case FindPattern.FIX:
                agent.isStopped = true;
                break;
        }
       
    }

    private void TraceAction()
    {
        float distance = Vector3.Distance(playerObj.transform.transform.position, enemyTr.position);

        if (distance <= atkDist)
            agent.destination = transform.position;
        else if (distance < cognitiveDist)
        {
            Vector3 pos = playerObj.transform.position - (playerObj.transform.position - enemyTr.position).normalized * (atkDist - 0.2f);
            pos.y = transform.position.y;

            agent.destination = pos;  // 공격 사거리 전까지의 위치
        }
            

        // agent 세팅 값
        agent.isStopped = false;
        agent.updateRotation = true;
    }

    private void RotateAction()
    {
        SetEnemyDir();
    }

    protected void DefaultAttackAction()
    {
        agent.isStopped = true;
    }

    protected void AttackWaitAction()
    {
        if (!CheckIfThereIsPlayerInFrontOfEnemy())
            SetEnemyDir();
    }

    private void AbnormalState()
    {
        agent.isStopped = true;

        switch (abnormalState)
        {
            case AbnomralState.STUN:
                break;

            case AbnomralState.KNOCKBACK:
                enemyTr.position = Vector3.MoveTowards(enemyTr.position, stateEffectPos, Time.deltaTime * 3);
                break;

            case AbnomralState.AIR:
                Debug.Log("Air 이동중");
                // 애니메이션 자체에서 하늘로 띄워지는 움직임을 보여주는 것으로 하는게 나은듯
                // 직접 구현하는 것보다는 그 편이 나아보임. 
                // 이유 : 적들을 지상으로부터 띄우면 NavMeshAgent 오류가 많이떠서 설정하기 번거로움.

                enemyTr.position = Vector3.MoveTowards(enemyTr.position, stateEffectPos, Time.deltaTime * 3);
                break;
        }
    }

    #endregion

    #region 시간 관련 함수
    protected virtual void SetAllCoolTime()
    {
        if (abnormalState != AbnomralState.NONE)
            SetAbnormalTime();

        if (state == State.IDLE)
        {
            if(idleState == IdleState.DEFAULT)
                SetCurrFindCoolTime();
        }
        else if (state == State.MOVE)
        {
            if (moveState == MoveState.TRACE)
                SetAttackCooltime();
        }
        else if (state == State.ATTACK)
        {
            if (attackState == AttackState.ATTACKWAIT || attackState == AttackState.DEFAULT)
                SetAttackCooltime();
        }
    }

    protected virtual void SetAttackCooltime()
    {
        SetCurrDefaultAtkCoolTime();
    }

    protected void SetCurrDefaultAtkCoolTime()
    {
        if (currDefaultAtkCoolTime < 0)
            return;

        currDefaultAtkCoolTime -= Time.deltaTime;
    }

    protected void SetCurrFindCoolTime()
    {
        if (currFindCoolTime < 0)
            return;

        currFindCoolTime -= Time.deltaTime;
    }

    protected void SetAbnormalTime()
    {
        if (currAbnormalTime < 0)
        {
            state = State.IDLE;
            abnormalState = AbnomralState.NONE;
            return;
        }

        currAbnormalTime -= Time.deltaTime;
    }

    #endregion

    #region 경로 이동 관련 함수
    /// <summary>
    /// 경로 이동
    /// </summary>
    private void MovePath()
    {
        float distance = Vector3.Distance(posToMove[currentPosIndexToMove], enemyTr.position);

        // 미세한 차이로 도달하지 않을 경우도 있기 때문에 0이 아니라 0.5f로 설정
        if (distance > 1f)
            return;

        // 목표지점에 도달했을 경우
        SetCurrentPosIndexToMove();
        currFindCoolTime = maxFindCoolTime;
    }

    /// <summary>
    /// 현재 posIndexToMove 값을 설정한다.
    /// </summary>
    private void SetCurrentPosIndexToMove()
    {
        if (currentPosIndexToMove >= posToMove.Length - 1) // 최대 인덱스 값에 도달하기 전에 0으로 다시 리셋되도록 설정
            currentPosIndexToMove = 0;
        else
            currentPosIndexToMove++;
    }

    #endregion

    private void SetEnemyDir()
    {
        //Debug.Log("목적지 도착 ---------------------------------> 회전 값 변경");

        Vector3 dir = playerObj.transform.position - new Vector3(enemyTr.position.x, playerObj.transform.position.y, enemyTr.position.z);
        agent.updateRotation = false;
        enemyTr.rotation = Quaternion.Slerp(enemyTr.rotation, Quaternion.LookRotation(dir), 3f * Time.deltaTime);
    }

    protected bool CheckIfThereIsPlayerInFrontOfEnemy()
    {
        Vector3 pos = new Vector3(enemyTr.position.x, playerObj.transform.position.y, enemyTr.position.z); 

        // 적의 정면에 플레이어가 존재할 경우
        if (Physics.Raycast(pos, enemyTr.forward, atkDist, LayerMask.GetMask("PLAYER")))
            return true;
        else
            return false;
    }

    private void DefaultAttack()
    {
        float distance = Vector3.Distance(playerObj.transform.position, enemyTr.position);

        if(distance <= atkDist)
            playerState.CurHp -= atk * (defaultAtkPercent / 100);
    }
}
