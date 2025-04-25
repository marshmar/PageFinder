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
    protected int reinforcementAtkCnt; // 강화 공격 횟수
    protected int currBasicAtkCnt;
    #endregion

    public override float CurHp
    {
        get => curHp;
        set
        {
            // 데미지 계산 공식 적용 필요
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
    //            // 플레이어를 인지했을 경우
    //            if (didPerceive)
    //            {
    //                if (distance <= cognitiveDist)
    //                {
    //                    // 플레이어가 앞에 있는 경우
    //                    if (CheckPlayerInFrontOfEnemy(cognitiveDist))
    //                        state = State.ATTACK;
    //                    else
    //                        state = State.MOVE; // 회전
    //                }
    //                else
    //                    state = State.MOVE;
    //            }
    //            else
    //            {
    //                if (distance <= cognitiveDist)
    //                {
    //                    // 플레이어가 앞에 있는 경우
    //                    if (CheckPlayerInFrontOfEnemy(cognitiveDist))
    //                    {
    //                        didPerceive = true;
    //                        state = State.IDLE; // 공격 대기
    //                    }
    //                    else
    //                        state = State.MOVE; // 회전

    //                    //Debug.Log("RootState didPerceive False : 인지거리 안에 있음");
    //                }
    //                else
    //                {
    //                    distance = Vector3.Distance(enemyTr.position, currDestination);

    //                    if (distance <= 1)
    //                        state = State.IDLE; // 순찰 대기
    //                    else
    //                        state = State.MOVE; // 순찰

    //                    //Debug.Log($"RootState didPerceive False  Diestance{distance}: {state}");
    //                }
    //            }
    //            break;

    //        // 원거리 적
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
    //                    state = State.MOVE; // 회전
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
    //            // 플레이어를 인지했을 경우
    //            if (didPerceive)
    //            {
    //                if (distance <= cognitiveDist)
    //                {
    //                    // 플레이어가 앞에 있지 않은 경우
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
    //                    // 플레이어가 앞에 있지 않은 경우
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
    //                // 플레이어가 앞에 있지 않은 경우
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
        //Debug.Log($"실행할 공격 종류 : {attackValue}   {attackState}   {attackValue}");
    }

    #endregion

    #region Reinforcement Attack
    /// <summary>
    /// 강화 공격 애니메이션 Events에서 호출하는 함수
    /// </summary>
    protected virtual void ReinforcementAttack()
    {
        // 강화 기본 공격은 기본 공격 쿨타임이 돌았을 때 + 기본 공격 횟수가 N번째일 때 실행한다.
    }

    #endregion

    #region Skill

    protected override int GetTypeOfAttackToUse()
    {
        /* <적 등급 별 루틴>
             *  하급 : skillCoolTime.Count == 0 => 실행 false
             *  상급 : 스킬 1개 => 해당 스킬 쿨타임 체크 => 0이면 true 아니면 false
             *  중간보스 : 스킬 2개 => 상황에 따라 어떤 스킬을 사용할지 체크 
             */

        // 스킬 우선도에 따라 비교
        // 우선 순위가 높은 스킬 순서대로 쿨타임이 돌았는지 체크 : 숫자가 낮을 수록 우선 순위가 높음
        for (int priority = 0; priority < skillPriority.Count; priority++)
        {
            int skillNum = skillPriority[priority];

            // 해당 스킬 쿨타임과 사용 조건 체크
            if (currSkillCoolTimes[skillNum] <= 0 && skillConditions[skillNum])
            {
                currSkillNum = skillNum;
                return skillNum;
            }
        }

        // 강화 공격인 경우
        if (currBasicAtkCnt == reinforcementAtkCnt)
            return -2;

        // 기본 공격
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
    /// 기본 공격이 끝날 때 애니메이션 Event에서 호출하는 함수
    /// </summary>
    protected override void BasicAttackEnd()
    {
        base.BasicAttackEnd();

        currBasicAtkCnt++;
    }

    /// <summary>
    /// 강화 공격이 끝날 때 애니메이션 Event에서 호출하는 함수
    /// </summary>
    private void ReinforcementAttackEnd()
    {
        currBasicAtkCnt = 0;
        attackState = AttackState.NONE;
    }

    #endregion

}
