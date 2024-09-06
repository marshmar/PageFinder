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
        get { return currHP; }
        set
        {
            currHP -= value;
            Hit();
            hpBar.value = currHP;
            
            //Debug.Log(name + " : " + HP);
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

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        if(!isUpdaterCoroutineWorking)
            StartCoroutine(Updater());
    }


    private void OnDestroy()
    {
        if (tokenManager != null)
            tokenManager.MakeToken(new Vector3(transform.position.x, 0.25f, transform.position.z));
        if (exp != null)
            exp.IncreaseExp(50);
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
            SetAllCoolTime();
            SetAllState();

            switch (state)
            {
                case State.IDLE:
                    abnormalState = AbnormalState.NONE;
                    moveState = MoveState.NONE;
                    attackState = AttackState.NONE;

                    SetIdleState();
                    IdleAction();
                    break;

                case State.ABONORMAL:
                    idleState = IdleState.NONE;
                    moveState = MoveState.NONE;
                    attackState = AttackState.NONE;

                    SetAbnormalState();
                    AbnormalAction();
                    break;

                case State.MOVE:
                    idleState = IdleState.NONE;
                    abnormalState = AbnormalState.NONE;
                    attackState = AttackState.NONE;

                    SetMoveState();
                    MoveAction();
                    break;

                case State.ATTACK:
                    idleState = IdleState.NONE;
                    abnormalState = AbnormalState.NONE;
                    moveState = MoveState.NONE;

                    SetAttackState();
                    AttackAction();
                    break;

                case State.DIE:
                    idleState = IdleState.NONE;
                    abnormalState = AbnormalState.NONE;
                    moveState = MoveState.NONE;
                    attackState = AttackState.NONE;

                    Die();
                    break;
            }
            yield return null;
        }
    }

    protected virtual void SetAllState()
    {
        float distance;
        distance = Vector3.Distance(playerObj.transform.transform.position, enemyTr.position);

        if (distance <= atkDist)
        {
            // 적이 플레이어 앞에 있는 경우
            if (CheckIfThereIsPlayerInFrontOfEnemy())
                state = State.ATTACK;
            else
                state = State.MOVE;

        }
        else if (distance <= cognitiveDist)
            state = State.MOVE;
        else // 인지 범위 바깥인 경우
        {
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

    #region State 관련 함수

    protected void SetIdleState()
    {
        // 나중에 Idle 에서 여러가지 종류가 있으면 그때 따로 설정하기
        idleState = IdleState.DEFAULT;
    }

    protected void SetAbnormalState()
    {
        switch (abnormalState)
        {
            case AbnormalState.BINDING:
                break;

            case AbnormalState.STUN:
                StunAction();
                break;
        }
    }

    protected void SetMoveState()
    {
        float distance = Vector3.Distance(playerObj.transform.transform.position, enemyTr.position);

        switch(attackPattern)
        {
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

        switch (moveState)
        {
            case MoveState.FIND:
                FindState();
                break;

            case MoveState.TRACE:
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

    protected void AbnormalAction()
    {
        switch (abnormalState)
        {
            case AbnormalState.BINDING:
                break;

            case AbnormalState.STUN:
                StunAction();
                break;

        }
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
        if (!ani.GetCurrentAnimatorStateInfo(0).IsName("Find"))
            return;

        agent.destination = posToMove[currentPosIndexToMove];
        agent.isStopped = false;
        agent.updateRotation = true;
    }

    private void TraceAction()
    {
        float distance = Vector3.Distance(playerObj.transform.transform.position, enemyTr.position);

        if (!ani.GetCurrentAnimatorStateInfo(0).IsName("Trace"))
            return;

        if (distance <= atkDist)
        {
            agent.destination = transform.position;

            // 플레이어가 자기 자신 앞에 있지 않을 경우
            if (!CheckIfThereIsPlayerInFrontOfEnemy())
                SetEnemyDir();
        }
        else if (distance < cognitiveDist)
            agent.destination = playerObj.transform.position - (playerObj.transform.position - enemyTr.position).normalized * atkDist;  //  공격 사거리 전까지의 위치


        // agent 세팅 값
        agent.isStopped = false;
        agent.updateRotation = true;
    }

    protected void DefaultAttackAction()
    {
        agent.isStopped = true;
    }

    private void StunAction()
    {
        agent.isStopped = true;
    }

    #endregion

    #region 쿨타임 관련 함수
    protected virtual void SetAllCoolTime()
    {
        if(state == State.IDLE)
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
    #endregion

    #region 경로 이동 관련 함수
    /// <summary>
    /// 경로 이동
    /// </summary>
    private void MovePath()
    {
        float distance = Vector3.Distance(posToMove[currentPosIndexToMove], enemyTr.position);

        // 미세한 차이로 도달하지 않을 경우도 있기 때문에 0이 아니라 0.5f로 설정
        if (distance > 0.5f)
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

    #region 잡
    /// <summary>
    /// 적이 피해를 입을 때 플레이어 쪽에서 호출하는 함수
    /// </summary>
    /// <param name="damage"></param>
    protected virtual void Hit()
    {
        state = State.ABONORMAL;
    }

    private void SetEnemyDir()
    {
        //Debug.Log("목적지 도착 ---------------------------------> 회전 값 변경");

        Vector3 dir = playerObj.transform.position - new Vector3(enemyTr.position.x, playerObj.transform.position.y, enemyTr.position.z);
        agent.updateRotation = false;
        enemyTr.rotation = Quaternion.Slerp(enemyTr.rotation, Quaternion.LookRotation(dir), 1.8f * Time.deltaTime);
    }

    protected bool CheckIfThereIsPlayerInFrontOfEnemy()
    {
        // 적의 정면에 플레이어가 존재할 경우
        if (Physics.Raycast(enemyTr.position, enemyTr.forward, atkDist, LayerMask.GetMask("PLAYER")))
            return true;
        else
            return false;
    }

    #endregion
}
