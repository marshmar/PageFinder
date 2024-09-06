using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;

public class MediumBossEnemy : HighEnemy
{
    [Header("Reinforcement Default Attack")]

    [SerializeField]
    protected GameObject ReinforcementAttack_Prefab;

    [SerializeField]
    protected int maxDefaultAtkCnt = 4;
    protected int currDefaultAtkCnt = 0;

    public override void Start()
    {
        base.Start();

        currDefaultAtkCnt = 0;
    }

    protected override void SetAllCoolTime()
    {
        if (state == State.IDLE)
        {
            if (idleState == IdleState.DEFAULT)
                SetCurrFindCoolTime();
        }
        else if (state == State.MOVE)
        {
            if (moveState == MoveState.TRACE)
                SetAttackCooltime();
        }
        else if (state == State.ATTACK)
        {
            if (attackState == AttackState.ATTACKWAIT || attackState == AttackState.DEFAULT || attackState == AttackState.REINFORCEMENT)
                SetAttackCooltime();
        }
    }

    protected override void AttackAction()
    {
        switch (attackState)
        {
            case AttackState.ATTACKWAIT:
                break;

            case AttackState.DEFAULT:
                DefaultAttackAction();
                break;

            case AttackState.SKILL:
                SkillAction();
                break;

            case AttackState.REINFORCEMENT:
                ReinforcementAttackAction();
                break;

            default:
                Debug.LogWarning(attackState);
                break;
        }
    }

    protected override void SetAttackState()
    {
        int attackValue = GetTypeOfAttackToUse();

        // 사용할 스킬이 있는 경우
        if (attackValue >= 0)
            attackState = AttackState.SKILL;
        else if (attackValue == -1)
            attackState = AttackState.DEFAULT;
        else if (attackValue == -2)
            attackState = AttackState.REINFORCEMENT;
        else
            attackState = AttackState.ATTACKWAIT;
    }

    private void ReinforcementAttackAction()
    {
        agent.isStopped = true;
    }

    protected override void AttackAni()
    {
        switch (attackState)
        {
            case AttackState.ATTACKWAIT:
                AttackWaitAni();
                break;

            case AttackState.DEFAULT:
                DefaultAttackAni();
                break;

            case AttackState.REINFORCEMENT:
                ReinforcementAttackAni();
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
    /// 강화 공격 애니메이션 Events에서 호출하는 함수
    /// </summary>
    protected virtual void ReinforcementAttack()
    {
        // 강화 기본 공격은 기본 공격 쿨타임이 돌았을 때 + 기본 공격 횟수가 N번째일 때 실행한다.
    }

    protected override int CheckIfThereAreAnySkillsAvailable()
    {
        /* <적 등급 별 루틴>
         *  하급 : skillCoolTime.Count == 0 => 실행 false
         *  상급 : 스킬 1개 => 해당 스킬 쿨타임 체크 => 0이면 true 아니면 false
         *  중간보스 : 스킬 2개 => 상황에 따라 어떤 스킬을 사용할지 체크 
         */

        /// 기본 공격을 하고 있는 중일 경우
        if (attackState == AttackState.DEFAULT || attackState == AttackState.REINFORCEMENT)
            return -1;

        // 스킬 우선도에 따라 비교
        for (int priority = 0; priority < skillNames.Count; priority++)
        {
            // 우선 순위가 높은 스킬 순서대로 쿨타임이 돌았는지 체크
            for (int indexToCheck = 0; indexToCheck < skillPriority.Count; indexToCheck++)
            {
                // 우선도 
                if (skillPriority[indexToCheck] != priority)
                    continue;

                // 해당 스킬 쿨타임과 사용 조건 체크
                if (currSkillCoolTimes[indexToCheck] <= 0 && skillCondition[indexToCheck])
                {
                    currSkillName = skillNames[indexToCheck];
                    return indexToCheck;
                }
                break;
            }
        }

        return -1;
    }

    /// <summary>
    /// 공격과 스킬 사용을 결정한다. 
    /// </summary>
    /// <returns> -2 :전부 사용 X    -1 :Attack     0~n :Skill N </returns>
    protected override int GetTypeOfAttackToUse()
    {
        int skillIndexToUse = -1;

        // 우선순위가 높은 순서대로 쿨타임이 돌은 스킬의 인덱스 확인
        skillIndexToUse = CheckIfThereAreAnySkillsAvailable();

        // 사용할 스킬이 있는 경우
        if (skillIndexToUse >= 0)
            return skillIndexToUse;

        // 기본 공격 쿨타임이 돌은 경우
        if (currDefaultAtkCoolTime <= 0)
        {
            if (currDefaultAtkCnt == maxDefaultAtkCnt)
                return -2;
            return -1;
        }

        return -3;
    }

    protected override void DefaultAttackAniEnd()
    {
        base.DefaultAttackAniEnd();

        currDefaultAtkCnt++;
    }


    #region 애니메이션 관련 함수

    protected override void DefaultIdleAni()
    {
        base.DefaultIdleAni();

        ani.SetBool("isReinforcementAttack", false);
    }

    protected override void FindAni()
    {
        base.FindAni();

        ani.SetBool("isReinforcementAttack", false);
    }

    protected override void TraceAni()
    {
        base.TraceAni();

        ani.SetBool("isReinforcementAttack", false);
    }

    protected override void AttackWaitAni()
    {
        base.AttackWaitAni();

        ani.SetBool("isReinforcementAttack", false);
    }

    protected override void DefaultAttackAni()
    {
        base.DefaultAttackAni();

        ani.SetBool("isReinforcementAttack", false);
    }

    protected override void SkillAni()
    {
        base.SkillAni();

        ani.SetBool("isReinforcementAttack", false);
    }

    protected override void StunAni()
    {
        base.StunAni();

        ani.SetBool("isReinforcementAttack", false);
    }

    protected override void DieAni()
    {
        base.DieAni();

        ani.SetBool("isReinforcementAttack", false);
    }

    protected virtual void ReinforcementAttackAni()
    {
        ani.SetBool("isIdle", false);
        ani.SetBool("isMove", false);
        ani.SetBool("isAttack", true);
        ani.SetBool("isAbnormal", false);

        ani.SetBool("isFind", false);
        ani.SetBool("isTrace", false);

        ani.SetBool("isAttackWait", false);
        ani.SetBool("isDefaultAttack", false);
        ani.SetBool("isReinforcementAttack", true);
        ani.SetBool("isSkill", false);
    }

    /// <summary>
    /// 강화 공격 애니메이션이 끝났을 때 호출하는 함수
    /// </summary>
    private void ReinforcementAttackAniEnd()
    {
        currDefaultAtkCnt = 0;
        currDefaultAtkCoolTime = maxDefaultAtkCoolTime;
    }

    #endregion
}
