using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
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

    [SerializeField]
    protected Gradation gradation;
    #endregion

    #region Init

    protected override void InitStat()
    {
        base.InitStat();

        currBasicAtkCnt = 0;
        reinforcementAtkCnt = 4;
    }

    #endregion

    #region State

    protected override void SetAttackState()
    {
        int attackValue = GetTypeOfAttackToUse();
        Debug.Log($"실행할 공격 종류 : {attackValue}");
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
        Debug.Log($"{attackValue}   {attackState}");
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
