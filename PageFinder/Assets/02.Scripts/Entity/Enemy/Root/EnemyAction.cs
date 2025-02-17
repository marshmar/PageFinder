using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class EnemyAction : EnemyAnimation
{
    /// <summary>
    /// Debuff�� Hit
    /// </summary>
    /// <param name="inkType"></param>
    /// <param name="damage"></param>
    /// <param name="debuffState"></param>
    /// <param name="debuffTime"></param>
    /// <param name="knockBackDir"></param>
    public override void Hit(InkType inkType, float damage, DebuffState debuffState, float debuffTime, Vector3 subjectPos = default)
    {
        float diff = 0.0f;

        // ��ũ ���� ����
        if (this.inkType == inkType)
            damage = damage - (damage * inkTypeResistance / 100.0f);

        // ���尡 �ִ� ���
        if (currShield > 0)
        {
            diff = currShield - damage;
            CurrShield -= damage;

            // ���忡 ���̸� �ְ� ���� �������� Hp���� ����
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
            // �÷��̾ �׾��� ���
            if (playerState.CurHp <= 0)
            {
                Debug.Log("Player Die -> Enemy Coroutine ����");
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

        // ����(didPerceive �״��), �˹�(didPerceive �״��)
        // �ӹ�(didPerceive �״��), ����(didPerceive = false)
        if (state == State.DEBUFF && debuffState != DebuffState.NONE)
            return;

        // idle ���¿����� Idle �ִϸ��̼��� ������ ��� ���·� ����
        // �׷��Ƿ� Idle �ִϸ��̼� ���¿����� ��� �������� ������ �ʵ��� ��
        if (state == State.IDLE && idleState != IdleState.NONE)
            return;

        // ���� ���°� �Ǹ� ������ ������ attackState�� None���� �����
        // ���� �ִϸ��̼� ���� �������� �ʵ��� �����ϱ� ����
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
                // �÷��̾ �������� ���
                if (didPerceive)
                {
                    if (distance <= cognitiveDist)
                    {
                        // �÷��̾ �տ� �ִ� ���
                        if (CheckPlayerInFrontOfEnemy(cognitiveDist))
                            state = State.ATTACK;
                        else
                            state = State.MOVE; // ȸ��
                    }
                    else
                        state = State.MOVE;
                }
                else
                {
                    if (distance <= cognitiveDist)
                    {
                        // �÷��̾ �տ� �ִ� ���
                        if (CheckPlayerInFrontOfEnemy(cognitiveDist))
                        {
                            didPerceive = true;
                            state = State.IDLE; // ���� ���
                        }
                        else
                            state = State.MOVE; // ȸ��

                        //Debug.Log("RootState didPerceive False : �����Ÿ� �ȿ� ����");
                    }
                    else
                    {
                        distance = Vector3.Distance(enemyTr.position, currDestination);

                        if (distance <= 1)
                            state = State.IDLE; // ���� ���
                        else
                            state = State.MOVE; // ����

                        //Debug.Log($"RootState didPerceive False  Diestance{distance}: {state}");
                    }
                }
                break;

            // ���Ÿ� ��
            case AttackDistType.LONG:

                if(isOnEdge)
                {
                    if (CheckPlayerInFrontOfEnemy(Vector3.Distance(enemyTr.position, playerObj.transform.position)))
                        state = State.ATTACK;
                    else
                        state = State.MOVE;
                    return;
                }

                // �÷��̾ �������� ���
                if (didPerceive)
                {
                    if (IsEnemyInCamera())
                    {
                        // ���� ������ ���
                        if(moveState == MoveState.FLEE)
                        {
                            if (Vector3.Distance(currDestination, enemyTr.position) > 1)
                            {
                                state = State.MOVE;
                                return;
                            }
                        }

                        // �÷��̾ �����Ÿ� ���� ������ ���
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
                            state = State.MOVE; // ȸ��
                    }
                    else
                        state = State.MOVE;

                    //Debug.Log($"���� �÷��̾������� ��Ȳ {state}");
                }
                else
                {
                    if (IsEnemyInCamera())
                    {
                        // �÷��̾ �տ� �ִ� ���
                        if (CheckPlayerInFrontOfEnemy(Vector3.Distance(enemyTr.position, playerObj.transform.position)))
                        {
                            didPerceive = true;
                            state = State.IDLE; // ���� ���
                        }
                        else
                            state = State.MOVE; // ȸ��

                        //Debug.Log($"���� ī�޶� �ȿ� ���� {state}");
                    }
                    else
                    {
                        distance = Vector3.Distance(enemyTr.position, currDestination);

                        if (distance <= 1)
                            state = State.IDLE; // ���� ���
                        else
                            state = State.MOVE; // Flee

                        //Debug.Log($"���� ī�޶� �ۿ� ����");
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
        // IdleState.First : �� ó�� ���۽�, Stun�� ���� �Ŀ� ����
        // �׷��Ƿ� �� �Լ� ���η� ������ �����Ƿ� ���� ó������ �ʴ´�.
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
                // �÷��̾ �������� ���
                if (didPerceive)
                {
                    if (distance <= cognitiveDist)
                    {
                        // �÷��̾ �տ� ���� ���� ���
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
                        // �÷��̾ �տ� ���� ���� ���
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

                // �÷��̾ �������� ���
                if (didPerceive)
                {
                    if (IsEnemyInCamera())
                    {
                        // ���� ������ ���
                        if (moveState == MoveState.FLEE)
                        {
                            if (Vector3.Distance(currDestination, enemyTr.position) > 1)
                            {
                                moveState = MoveState.FLEE;
                                return;
                            }
                        }

                        // �÷��̾ �տ� �ִ� ���
                        if (CheckPlayerInFrontOfEnemy(fleeDist))
                            moveState = MoveState.FLEE;
                        else
                            moveState = MoveState.ROTATE;
                    }
                    else
                    {
                        Debug.Log("Enemy�� ī�޶� �ȿ� ���� �ʽ��ϴ�.");
                        moveState = MoveState.NONE;
                    }
                        
                }
                else
                {
                    if (IsEnemyInCamera())
                    {
                        // �÷��̾ �տ� ���� ���� ���
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
        // �ϱ� ���� �⺻���ݸ� ����
        attackState = AttackState.BASIC;
    }

    #endregion

    #region Action ����

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
                    Debug.LogError($"�� �����÷��� ��ġ Ž�� ����");

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
        // ��Ʈ �ڽ� Ȱ��ȭ �ڵ�� �ٲٱ�

        float distance = Vector3.Distance(playerObj.transform.position, enemyTr.position);

        if (distance <= cognitiveDist)
            playerState.CurHp -= atk;
    }

    /// <summary>
    /// �⺻ ���� �ִϸ��̼� ���� �� ����
    /// </summary>
    protected virtual void BasicAttackEnd()
    {
        attackState = AttackState.NONE;
        //Debug.Log($"���� �� : {attackState}");
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
        // �ð��� ���� ȸ�� �ӵ����� �÷��̾� ������ �ӵ��� ����Ͽ� �ٷ� �÷��̾� ���� �ٶ� �� �ֵ��� ���߿� �����ϱ�
        // ����� �÷��̾ ��� ���ۺ��� ���� ���ݸ��ϰ� ȸ���� ��, �̰� Slerp�� ������ ���� ���� ��
    }

    protected bool CheckPlayerInFrontOfEnemy(float dist)
    {
        Vector3 pos = new Vector3(enemyTr.position.x, playerObj.transform.position.y, enemyTr.position.z); 

        switch(attackDistType)
        {
            case AttackDistType.SHORT:
                // ���� ���鿡 �÷��̾ ������ ���
                if (Physics.Raycast(pos, enemyTr.forward, dist, LayerMask.GetMask("PLAYER")))
                    return true;
                else
                    return false;

            case AttackDistType.LONG:
                // ���� ���鿡 �÷��̾ ������ ���
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
        GameObject bullet = Instantiate(bulletPrefab, spawnPos, Quaternion.identity); // Witched �տ� �� ����(-60,0,60)�� �Ѿ� ����
        Bullet bulletScr = DebugUtils.GetComponentWithErrorLogging<Bullet>(bullet, "Bullet");
        bulletScr.Damage = damage;
        bulletScr.bulletSpeed = speed;

        Vector3 targetPos = enemyTr.position + targetDir * 100; // �ش� �������� �� �ٱ����� ������ ������ �� �ֵ��� *100���� ����
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

        // Nav Mesh Agent Ȱ��ȭ�� ���¿��� �����ϸ� ���� �ٶ󺸰� �ִ� ������ �ٲ�� ������ ��Ȱ��ȭ
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
                didPerceive = false; // ���� patrol ���� �ٽ� ������ ����
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
            //Debug.Log("�ʰ� �浹 flee -> Attack���� ��ȯ");
        }
    }

}