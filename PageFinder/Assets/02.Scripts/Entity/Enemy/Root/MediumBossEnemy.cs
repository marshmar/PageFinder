using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class MediumBossEnemy : HighEnemy
{
    #region Variables
    [Header("Reinforcement Default Attack")]

    [SerializeField]
    protected GameObject ReinforcementAttack_Prefab;

    [SerializeField]
    protected int reinforcementAtkCnt; // ��ȭ ���� Ƚ��
    protected int currBasicAtkCnt;
    #endregion

    public override float CurHp
    {
        get => curHp;
        set
        {
            // ������ ��� ���� ���� �ʿ�
            float inputDamage = curHp - value;

            if (inputDamage < 0)
            {
                curHp = curHp + -inputDamage;
                if (curHp > maxHp) curHp = maxHp;
            }
            else
            {
                float damage = shieldManager.CalculateDamageWithDecreasingShield(inputDamage);
                if (damage <= 0)
                {
                    enemyUI.SetStateBarUIForCurValue(maxHp, curHp, CurShield);
                    return;
                }

                enemyUI.StartDamageFlash(curHp, damage, maxHp);
                curHp -= damage;
            }

            enemyUI.SetStateBarUIForCurValue(maxHp, curHp, CurShield);

            if (curHp <= 0)
                IsDie = true;
        }
    }


    #region Init

    public override void InitStatValue()
    {
        base.InitStatValue();

        currBasicAtkCnt = 0;
        reinforcementAtkCnt = 3;
        confusionCoolTime = 12.0f;
    }

    #endregion

    #region State

    //protected override void SetRootState()
    //{
    //    float distance = Vector3.Distance(playerObj.transform.transform.position, enemyTr.position);

    //    switch (attackDistType)
    //    {
    //        case AttackDistType.SHORT:
    //            // �÷��̾ �������� ���
    //            if (didPerceive)
    //            {
    //                if (distance <= cognitiveDist)
    //                {
    //                    // �÷��̾ �տ� �ִ� ���
    //                    if (CheckPlayerInFrontOfEnemy(cognitiveDist))
    //                        state = State.ATTACK;
    //                    else
    //                        state = State.MOVE; // ȸ��
    //                }
    //                else
    //                    state = State.MOVE;
    //            }
    //            else
    //            {
    //                if (distance <= cognitiveDist)
    //                {
    //                    // �÷��̾ �տ� �ִ� ���
    //                    if (CheckPlayerInFrontOfEnemy(cognitiveDist))
    //                    {
    //                        didPerceive = true;
    //                        state = State.IDLE; // ���� ���
    //                    }
    //                    else
    //                        state = State.MOVE; // ȸ��

    //                    //Debug.Log("RootState didPerceive False : �����Ÿ� �ȿ� ����");
    //                }
    //                else
    //                {
    //                    distance = Vector3.Distance(enemyTr.position, currDestination);

    //                    if (distance <= 1)
    //                        state = State.IDLE; // ���� ���
    //                    else
    //                        state = State.MOVE; // ����

    //                    //Debug.Log($"RootState didPerceive False  Diestance{distance}: {state}");
    //                }
    //            }
    //            break;

    //        // ���Ÿ� ��
    //        case AttackDistType.LONG:

    //            if (isOnEdge)
    //            {
    //                if (CheckPlayerInFrontOfEnemy(Vector3.Distance(enemyTr.position, playerObj.transform.position)))
    //                    state = State.ATTACK;
    //                else
    //                    state = State.MOVE;
    //                return;
    //            }

    //            if (IsEnemyInCamera())
    //            {
    //                if (CheckPlayerInFrontOfEnemy(Vector3.Distance(enemyTr.position, playerObj.transform.position)))
    //                    state = State.ATTACK;
    //                else
    //                    state = State.MOVE; // ȸ��
    //            }
    //            else
    //                state = State.MOVE;
    //            break;
    //    }
    //}

    //protected override void SetMoveState()
    //{
    //    float distance = Vector3.Distance(playerObj.transform.transform.position, enemyTr.position);
    //    switch (attackDistType)
    //    {
    //        case AttackDistType.SHORT:
    //            // �÷��̾ �������� ���
    //            if (didPerceive)
    //            {
    //                if (distance <= cognitiveDist)
    //                {
    //                    // �÷��̾ �տ� ���� ���� ���
    //                    if (!CheckPlayerInFrontOfEnemy(cognitiveDist))
    //                        moveState = MoveState.ROTATE;
    //                }
    //                else
    //                    moveState = MoveState.CHASE;
    //            }
    //            else
    //            {
    //                if (distance <= cognitiveDist)
    //                {
    //                    // �÷��̾ �տ� ���� ���� ���
    //                    if (!CheckPlayerInFrontOfEnemy(cognitiveDist))
    //                        moveState = MoveState.ROTATE;
    //                }
    //                else
    //                {
    //                    distance = Vector3.Distance(enemyTr.position, currDestination);

    //                    if (distance > 1)
    //                        moveState = MoveState.PATROL;
    //                }
    //            }
    //            break;

    //        case AttackDistType.LONG:
    //            if(isOnEdge)
    //            {
    //                moveState = MoveState.ROTATE;
    //                return;
    //            }

    //            if (IsEnemyInCamera())
    //            {
    //                // �÷��̾ �տ� ���� ���� ���
    //                if (!CheckPlayerInFrontOfEnemy(Vector3.Distance(enemyTr.position, playerObj.transform.position)))
    //                    moveState = MoveState.ROTATE;
    //            }
    //            else
    //            {
    //                moveState = MoveState.NONE;
    //            }
    //            break;
    //    }
    //}


    protected override void SetAttackState()
    {
        int attackValue = GetTypeOfAttackToUse();

        switch (attackValue)
        {
            case -2:
                attackState = AttackState.REINFORCEMENT;
                break;

            case -1:
                attackState = AttackState.BASIC;
                break;

            case 0:
            case 1:
            case 2:
                attackState = AttackState.SKILL;
                break;
        }
        //Debug.Log($"������ ���� ���� : {attackValue}   {attackState}   {attackValue}");
    }

    #endregion

    #region Reinforcement Attack
    /// <summary>
    /// ��ȭ ���� �ִϸ��̼� Events���� ȣ���ϴ� �Լ�
    /// </summary>
    protected virtual void ReinforcementAttack()
    {
        // ��ȭ �⺻ ������ �⺻ ���� ��Ÿ���� ������ �� + �⺻ ���� Ƚ���� N��°�� �� �����Ѵ�.
    }

    #endregion

    #region Skill

    protected override int GetTypeOfAttackToUse()
    {
        /* <�� ��� �� ��ƾ>
             *  �ϱ� : skillCoolTime.Count == 0 => ���� false
             *  ��� : ��ų 1�� => �ش� ��ų ��Ÿ�� üũ => 0�̸� true �ƴϸ� false
             *  �߰����� : ��ų 2�� => ��Ȳ�� ���� � ��ų�� ������� üũ 
             */

        // ��ų �켱���� ���� ��
        // �켱 ������ ���� ��ų ������� ��Ÿ���� ���Ҵ��� üũ : ���ڰ� ���� ���� �켱 ������ ����
        for (int priority = 0; priority < skillPriority.Count; priority++)
        {
            int skillNum = skillPriority[priority];

            // �ش� ��ų ��Ÿ�Ӱ� ��� ���� üũ
            if (currSkillCoolTimes[skillNum] <= 0 && skillConditions[skillNum])
            {
                currSkillNum = skillNum;
                return skillNum;
            }
        }

        // ��ȭ ������ ���
        if (currBasicAtkCnt == reinforcementAtkCnt)
            return -2;

        // �⺻ ����
        return -1;
    }

    #endregion

    #region Animation
    protected override void AttackAni()
    {
        switch (attackState)
        {
            case AttackState.BASIC:
                SetAniVariableValue(AttackState.BASIC);
                break;

            case AttackState.REINFORCEMENT:
                SetAniVariableValue(AttackState.REINFORCEMENT);
                break;

            case AttackState.SKILL:
                SkillAni();
                break;

            default:
                Debug.LogWarning(attackState);
                break;
        }
    }

    /// <summary>
    /// �⺻ ������ ���� �� �ִϸ��̼� Event���� ȣ���ϴ� �Լ�
    /// </summary>
    protected override void BasicAttackEnd()
    {
        base.BasicAttackEnd();

        currBasicAtkCnt++;
    }

    /// <summary>
    /// ��ȭ ������ ���� �� �ִϸ��̼� Event���� ȣ���ϴ� �Լ�
    /// </summary>
    private void ReinforcementAttackEnd()
    {
        currBasicAtkCnt = 0;
        attackState = AttackState.NONE;
    }

    #endregion

}
