using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class EnemyAction : EnemyAnimation
{
    /// <summary>
    /// Debuff용 Hit
    /// </summary>
    /// <param name="inkType"></param>
    /// <param name="damage"></param>
    /// <param name="debuffState"></param>
    /// <param name="debuffTime"></param>
    /// <param name="knockBackDir"></param>
    public override void Hit(InkType inkType, float damage, DebuffState debuffState, float debuffTime, Vector3 subjectPos = default)
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

            // 쉴드에 데미를 주고도 남은 데미지를 Hp에도 적용
            if (diff < 0)
                HP += diff;
        }
        else
            HP -= damage;

        //enemyUI.StartCoroutine(enemyUI.DamagePopUp(inkType, damage));

        if (HP <= 0)
            return;

        if (debuffState != DebuffState.NONE)
        {
            Vector3 dir = (enemyTr.position - subjectPos).normalized;
            dir.y = 0;
            knockBackPos = enemyTr.position + dir * 1;
            SetDebuff(debuffState, debuffTime);
        }
        else
            Debug.LogWarning(debuffState);
    }


    #region Enemy Coroutine
    public override IEnumerator EnemyCoroutine()
    {
        while (!isDie)
        {
            // 플레이어가 죽었을 경우
            if (playerState.CurHp <= 0)
            {
                Debug.Log("Player Die -> Enemy Coroutine 종료");
                break;
            }
               

            SetAllCoolTime();
            Action();
            Animation();
            yield return null;
        }

        Dead();
    }

    protected virtual void Action()
    {
        if (state == State.DIE)
            return;

        // 경직(didPerceive 그대로), 넉백(didPerceive 그대로)
        // 속박(didPerceive 그대로), 기절(didPerceive = false)
        if (state == State.DEBUFF && debuffState != DebuffState.NONE)
            return;

        // idle 상태에서는 Idle 애니메이션이 끝나고 어떠한 상태로 변함
        // 그러므로 Idle 애니메이션 상태에서는 어떠한 동작으로 변하지 않도록 함
        if (state == State.IDLE && idleState != IdleState.NONE)
            return;

        // 공격 상태가 되면 공격이 끝나야 attackState가 None으로 변경됨
        // 공격 애니메이션 도중 움직이지 않도록 설정하기 위함
        if (state == State.ATTACK && attackState != AttackState.NONE)
            return;

        SetRootState();
        SetDetailState();
    }
    #endregion

    #region State

    protected virtual void SetRootState()
    {
        if(currHP <= 0)
        {
            state = State.DIE;
            return;
        }

        float distance = Vector3.Distance(playerObj.transform.transform.position, enemyTr.position);

        switch (attackDistType)
        {
            case AttackDistType.SHORT:
                // 플레이어를 인지했을 경우
                if (didPerceive)
                {
                    if (distance <= cognitiveDist)
                    {
                        // 플레이어가 앞에 있는 경우
                        if (CheckPlayerInFrontOfEnemy(cognitiveDist))
                            state = State.ATTACK;
                        else
                            state = State.MOVE; // 회전
                    }
                    else
                        state = State.MOVE;
                }
                else
                {
                    if (distance <= cognitiveDist)
                    {
                        // 플레이어가 앞에 있는 경우
                        if (CheckPlayerInFrontOfEnemy(cognitiveDist))
                        {
                            didPerceive = true;
                            state = State.IDLE; // 공격 대기
                        }
                        else
                            state = State.MOVE; // 회전

                        //Debug.Log("RootState didPerceive False : 인지거리 안에 있음");
                    }
                    else
                    {
                        distance = Vector3.Distance(enemyTr.position, currDestination);

                        if (distance <= 1)
                            state = State.IDLE; // 순찰 대기
                        else
                            state = State.MOVE; // 순찰

                        //Debug.Log($"RootState didPerceive False  Diestance{distance}: {state}");
                    }
                }
                break;

            // 원거리 적
            case AttackDistType.LONG:

                if(isOnEdge)
                {
                    if (CheckPlayerInFrontOfEnemy(Vector3.Distance(enemyTr.position, playerObj.transform.position)))
                        state = State.ATTACK;
                    else
                        state = State.MOVE;
                    return;
                }

                // 플레이어를 인지했을 경우
                if (didPerceive)
                {
                    if (IsEnemyInCamera())
                    {
                        // 도망 상태일 경우
                        if(moveState == MoveState.FLEE)
                        {
                            if (Vector3.Distance(currDestination, enemyTr.position) > 1)
                            {
                                state = State.MOVE;
                                return;
                            }
                        }

                        // 플레이어가 도망거리 내에 들어왔을 경우
                        if (CheckPlayerInFrontOfEnemy(fleeDist))
                        {
                            if (isOnEdge)
                                state = State.ATTACK; 
                            else
                                state = State.MOVE; // FLEE
                        }
                        else if (CheckPlayerInFrontOfEnemy(Vector3.Distance(enemyTr.position, playerObj.transform.position)))
                            state = State.ATTACK;
                        else
                            state = State.MOVE; // 회전
                    }
                    else
                        state = State.MOVE;

                    //Debug.Log($"적이 플레이어인지한 상황 {state}");
                }
                else
                {
                    if (IsEnemyInCamera())
                    {
                        // 플레이어가 앞에 있는 경우
                        if (CheckPlayerInFrontOfEnemy(Vector3.Distance(enemyTr.position, playerObj.transform.position)))
                        {
                            didPerceive = true;
                            state = State.IDLE; // 공격 대기
                        }
                        else
                            state = State.MOVE; // 회전

                        //Debug.Log($"적이 카메라 안에 있음 {state}");
                    }
                    else
                    {
                        distance = Vector3.Distance(enemyTr.position, currDestination);

                        if (distance <= 1)
                            state = State.IDLE; // 순찰 대기
                        else
                            state = State.MOVE; // Flee

                        //Debug.Log($"적이 카메라 밖에 있음");
                    }
                }
                break;
        }
    }

    protected virtual void SetDetailState()
    {
        switch (state)
        {
            case State.IDLE:
                moveState = MoveState.NONE;
                attackState = AttackState.NONE;
                debuffState = DebuffState.NONE;
                isFlee = false;

                SetIdleState();
                SetAgentData(transform.position);
                break;

            case State.MOVE:
                attackState = AttackState.NONE;
                debuffState = DebuffState.NONE;

                SetMoveState();
                SetMoveAction();
                break;

            case State.ATTACK:
                moveState = MoveState.NONE;
                debuffState = DebuffState.NONE;
                isFlee = false;

                SetAttackState();
                SetAgentData(transform.position);
                break;

            case State.DEBUFF:
                moveState = MoveState.NONE;
                attackState = AttackState.NONE;
                isFlee = false;

                SetAgentData(transform.position);
                break;

            case State.DIE:
                moveState = MoveState.NONE;
                attackState = AttackState.NONE;
                debuffState = DebuffState.NONE;
                isFlee = false;

                SetAgentData(transform.position);
                break;
        }
    }

    protected void SetIdleState()
    {
        // IdleState.First : 맨 처음 시작시, Stun이 끝난 후에 동작
        // 그러므로 이 함수 내부로 들어오지 않으므로 따로 처리하지 않는다.
        if (didPerceive)
        {
            enemyUI.ActivatePerceiveImg();
            idleState = IdleState.ATTACKWAIT;
        }
        else
            idleState = IdleState.PATROLWAIT;
    }

    protected virtual void SetMoveState()
    {
        float distance = Vector3.Distance(playerObj.transform.transform.position, enemyTr.position);
        switch (attackDistType)
        {
            case AttackDistType.SHORT:
                // 플레이어를 인지했을 경우
                if (didPerceive)
                {
                    if (distance <= cognitiveDist)
                    {
                        // 플레이어가 앞에 있지 않은 경우
                        if (!CheckPlayerInFrontOfEnemy(cognitiveDist))
                            moveState = MoveState.ROTATE;
                    }
                    else
                        moveState = MoveState.CHASE;
                }
                else
                {
                    if (distance <= cognitiveDist)
                    {
                        // 플레이어가 앞에 있지 않은 경우
                        if (!CheckPlayerInFrontOfEnemy(cognitiveDist))
                            moveState = MoveState.ROTATE;
                    }
                    else
                    {
                        distance = Vector3.Distance(enemyTr.position, currDestination);

                        if (distance > 1)
                            moveState = MoveState.PATROL;
                    }
                }
                break;

            case AttackDistType.LONG:
                if(isOnEdge)
                {
                    moveState = MoveState.ROTATE;
                    return;
                }

                // 플레이어를 인지했을 경우
                if (didPerceive)
                {
                    if (IsEnemyInCamera())
                    {
                        // 도망 상태일 경우
                        if (moveState == MoveState.FLEE)
                        {
                            if (Vector3.Distance(currDestination, enemyTr.position) > 1)
                            {
                                moveState = MoveState.FLEE;
                                return;
                            }
                        }

                        // 플레이어가 앞에 있는 경우
                        if (CheckPlayerInFrontOfEnemy(fleeDist))
                            moveState = MoveState.FLEE;
                        else
                            moveState = MoveState.ROTATE;
                    }
                    else
                    {
                        Debug.Log("Enemy가 카메라 안에 있지 않습니다.");
                        moveState = MoveState.NONE;
                    }
                        
                }
                else
                {
                    if (IsEnemyInCamera())
                    {
                        // 플레이어가 앞에 있지 않은 경우
                        if (!CheckPlayerInFrontOfEnemy(Vector3.Distance(enemyTr.position, playerObj.transform.position)))
                            moveState = MoveState.ROTATE;
                    }
                    else
                    {
                        distance = Vector3.Distance(enemyTr.position, currDestination);

                        if (distance > 1)
                            moveState = MoveState.PATROL;
                    }
                }
                break;
        }
       
    }


    protected virtual void SetAttackState()
    {
        // 하급 적은 기본공격만 존재
        attackState = AttackState.BASIC;
    }

    #endregion

    #region Action 관련

    protected void SetMoveAction()
    {
        switch (moveState)
        {
            case MoveState.NONE:
                isFlee = false;
                break;

            case MoveState.PATROL:
                isFlee = false;
                currDestination = patrolDestinations[patrolDestinationIndex];
                SetAgentData(currDestination, false);
                break;

            case MoveState.ROTATE:
                isFlee = false;
                currDestination = transform.position;
                SetAgentData(currDestination, false, false);
                Rotate();
                break;

            case MoveState.CHASE:
                isFlee = false;
                currDestination = playerObj.transform.position;
                SetAgentData(currDestination, false);
                break;

            case MoveState.FLEE:
                if (isFlee)
                    return;

                isFlee = true;
                Vector3 dir = (enemyTr.position - playerObj.transform.position).normalized;
                dir.y = 0;

                NavMeshHit hit;
                if (NavMesh.SamplePosition(enemyTr.position + dir * fleeDist, out hit, 1000, NavMesh.AllAreas))
                    currDestination = new Vector3(hit.position.x, enemyTr.position.y, hit.position.z);
                else
                    Debug.LogError($"적 도망시랜덤 위치 탐색 실패");

                SetAgentData(currDestination, false);
                break;

            default:
                SetAgentData(currDestination, false);
                Debug.LogWarning(moveState);
                break;
        }
    }

    protected virtual void BasicAttack()
    {
        // 히트 박스 활성화 코드로 바꾸기

        float distance = Vector3.Distance(playerObj.transform.position, enemyTr.position);

        if (distance <= cognitiveDist)
            playerState.CurHp -= atk;
    }

    /// <summary>
    /// 기본 공격 애니메이션 종료 후 동작
    /// </summary>
    protected virtual void BasicAttackEnd()
    {
        attackState = AttackState.NONE;
        //Debug.Log($"공격 끝 : {attackState}");
    }

    #endregion

    #region CoolTime
    protected virtual void SetAllCoolTime()
    {
        SetIdleTime();
        SetDebuffTime();
    }

    private void SetIdleTime()
    {
        if (state == State.IDLE && idleState == IdleState.NONE)
            return;

        switch (idleState)
        {
            case IdleState.FIRSTWAIT:
                if(currFirstWaitTime > 0)
                {
                    currFirstWaitTime -= Time.deltaTime;
                    return;
                }
                currFirstWaitTime = maxFirstWaitTime;
                state = State.MOVE; // patrol
                break;

            case IdleState.PATROLWAIT:
                if (currPatrolWaitTime > 0)
                {
                    currPatrolWaitTime -= Time.deltaTime;
                    return;
                }
                PatrolDestinationIndex += 1;
                currDestination = patrolDestinations[PatrolDestinationIndex];
                currPatrolWaitTime = maxPatrolWaitTime;
                state = State.MOVE; // patrol
                break;

            case IdleState.ATTACKWAIT:
                if (currAttackWaitTime > 0)
                {
                    currAttackWaitTime -= Time.deltaTime;
                    return;
                }
               
                currAttackWaitTime = maxAttackWaitTime;
                state = State.ATTACK; // attack
                break;
        }
        idleState = IdleState.NONE;
    }

    #endregion

    #region Rotate
    private void Rotate()
    {
        Vector3 dir = (playerObj.transform.position - 
            new Vector3(enemyTr.position.x, playerObj.transform.position.y, enemyTr.position.z)).normalized;

        if(dir != Vector3.zero)
            enemyTr.rotation = Quaternion.Slerp(enemyTr.rotation, Quaternion.LookRotation(dir), 7f * Time.deltaTime); 
        // 시간에 따른 회전 속도값을 플레이어 움직임 속도에 비례하여 바로 플레이어 쪽을 바라볼 수 있도록 나중에 설정하기
        // 현재는 플레이어가 계속 빙글빙글 돌면 공격못하고 회전만 함, 이게 Slerp의 문제일 수도 있을 듯
    }

    protected bool CheckPlayerInFrontOfEnemy(float dist)
    {
        Vector3 pos = new Vector3(enemyTr.position.x, playerObj.transform.position.y, enemyTr.position.z); 

        switch(attackDistType)
        {
            case AttackDistType.SHORT:
                // 적의 정면에 플레이어가 존재할 경우
                if (Physics.Raycast(pos, enemyTr.forward, dist, LayerMask.GetMask("PLAYER")))
                    return true;
                else
                    return false;

            case AttackDistType.LONG:
                // 적의 정면에 플레이어가 존재할 경우
                if (Physics.Raycast(pos, enemyTr.forward, dist, LayerMask.GetMask("PLAYER")))
                    return true;
                else
                    return false;

            default:
                return false;
        }
    }
    #endregion

    #region Long Distance Attack Enemy
    protected void SetBullet(GameObject bulletPrefab, int angle, float damage, float speed)
    {
        Vector3 targetDir = Quaternion.AngleAxis(angle, Vector3.up) * enemyTr.forward;
        Vector3 spawnPos = enemyTr.position + targetDir;
        spawnPos.y = playerObj.transform.position.y + 1;
        GameObject bullet = Instantiate(bulletPrefab, spawnPos, Quaternion.identity); // Witched 앞에 세 갈래(-60,0,60)로 총알 생성
        Bullet bulletScr = DebugUtils.GetComponentWithErrorLogging<Bullet>(bullet, "Bullet");
        bulletScr.Damage = damage;
        bulletScr.bulletSpeed = speed;

        Vector3 targetPos = enemyTr.position + targetDir * 100; // 해당 방향으로 맵 바깥까지 지점이 설정될 수 있도록 *100으로 설정
        bulletScr.Fire(targetPos);
    }

    protected bool IsEnemyInCamera()
    {
        Vector3 enemyUIPos =  Camera.main.WorldToViewportPoint(new Vector3(enemyTr.position.x, 0, enemyTr.position.z));

        if (enemyUIPos.x >= 0 && enemyUIPos.x <= 1 && enemyUIPos.y >= 0 && enemyUIPos.y <= 1 && enemyUIPos.z > 0)
            return true;
        return false;
    }

    #endregion

    protected void SetAgentData(Vector3 pos, bool isStop = true, bool isRotate = true)
    {
        agent.destination = pos;
        agent.isStopped = isStop;
        agent.updateRotation = isRotate;
    }

    protected void SetDebuffTime()
    {
        if (state != State.DEBUFF)
            return;

        if (debuffState == DebuffState.NONE)
            return;

        if (currDebuffTime < 0 && !DebuffIsEnd)
        {
            DebuffIsEnd = true;

            if(debuffState == DebuffState.KNOCKBACK)
                agent.enabled = true;

            return;
        }

        switch (debuffState)
        {
            case DebuffState.STAGGER:
                break;

            case DebuffState.BINDING:
                break;

            case DebuffState.KNOCKBACK:
                transform.position = Vector3.MoveTowards(enemyTr.position, knockBackPos, Time.deltaTime);
                break;

            case DebuffState.STUN:
                break;
        }
        currDebuffTime -= Time.deltaTime;
    }

    public void SetDebuff(DebuffState type, float time)
    {
        state = State.DEBUFF;
        idleState = IdleState.NONE;
        moveState = MoveState.NONE;
        attackState = AttackState.NONE;

        // Nav Mesh Agent 활성화된 상태에서 동작하면 적이 바라보고 있는 방향이 바뀌기 때문에 비활성화
        if (agent.enabled)
        {
            agent.destination = enemyTr.position;
            agent.isStopped = true;
        }

        switch (type)
        {
            // Stagger -> Attack or  Move(Chase)
            case DebuffState.STAGGER:
                debuffState = DebuffState.STAGGER;

                break;

            // KnockBack -> Attack or  Move(Chase)
            case DebuffState.KNOCKBACK:
                debuffState = DebuffState.KNOCKBACK;
                agent.enabled = false;
                break;

            // BINDING -> Attack or Move(Chase)
            case DebuffState.BINDING:
                debuffState = DebuffState.BINDING;

                break;

            // Stun -> Idle(FirstWait)
            case DebuffState.STUN:
                debuffState = DebuffState.STUN;
                didPerceive = false; // 적의 patrol 로직 다시 돌도록 설정
                break;

            default:
                debuffState = DebuffState.NONE;
                break;
        }

        maxDebuffTime = time;
        currDebuffTime = maxDebuffTime;
        DebuffIsEnd = false;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MAP"))
        {
            isFlee = false;
            isOnEdge = true;
            //Debug.Log("맵과 충돌 flee -> Attack으로 전환");
        }
    }

}