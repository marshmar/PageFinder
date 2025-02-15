using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighEnemy : EnemyAction
{
    #region Variables
    [Header("Skill")]
    protected List<float> maxSkillCoolTimes = new List<float>(); // 스킬 쿨타임 
    protected List<float> currSkillCoolTimes = new List<float>(); // 현재 스킬 쿨타임 
    protected List<int> skillPriority = new List<int>(); // 스킬 우선도
    protected List<bool> skillConditions = new List<bool>(); // 스킬 조건
    protected int currSkillNum; // -1 : 아무 스킬도 사용하지 않는 상태   0 : 스킬0    1: 스킬1   2: 스킬2

    #endregion

    #region Init

    protected override void InitStatValue()
    {
        base.InitStatValue();

        foreach(var coolTime in maxSkillCoolTimes)
            currSkillCoolTimes.Add(coolTime);

        currSkillNum = -1; // 아무 스킬도 사용하지 않는 상태
    }

    public override void InitStat(EnemyData enemyData)
    {
        base.InitStat(enemyData);

        maxSkillCoolTimes = enemyData.skillCoolTimes;
        skillPriority = enemyData.skillPriority;

        for (int i = 0; i < skillPriority.Count; i++)
            skillConditions.Add(false);
    }

    #endregion

    #region State

    protected override void SetAttackState()
    {
        int attackValue = GetTypeOfAttackToUse();

        switch(attackValue)
        {
            case -1:
                attackState = AttackState.BASIC;
                break;

            // 상급 : 스킬이 1개
            case 0:
                attackState = AttackState.SKILL;
                break;
        }
    }

    #endregion

    #region Cool Time

    protected override void SetAllCoolTime()
    {
        base.SetAllCoolTime();
        SetSkillCooltime();
    }

    private void SetSkillCooltime()
    {
        SetCurrSkillCoolTime();
        CheckSkillsCondition();
    }


    #endregion

    #region Skill

    protected virtual int GetTypeOfAttackToUse()
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
            if (currSkillCoolTimes[skillNum] <=0 && skillConditions[skillNum])
            {
                currSkillNum = skillNum;
                return skillNum;
            }
        }

        // 기본 공격
        return -1;
    }


    private void SetCurrSkillCoolTime()
    {
        for (int i = 0; i < currSkillCoolTimes.Count; i++)
        {
            if (i == currSkillNum) // 해당 스킬 사용중인 상태
                continue;

            if (currSkillCoolTimes[i] < 0)
                continue;

            currSkillCoolTimes[i] -= Time.deltaTime;
        }
    }

    protected virtual void CheckSkillsCondition()
    {

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

            case AttackState.SKILL:
                SkillAni();
                break;

            default:
                break;
        }
    }

    protected virtual void SkillAni()
    {
        // 아무 스킬도 사용하지 않는 상태
        if (currSkillNum == -1)
            return;

        SetAniVariableValue(AttackState.SKILL + currSkillNum); // 스킬 인덱스에 따라 애니메이션 값 변경하도록 함
    }

    /// <summary>
    /// Skill 애니메이션이 끝나고 호출되는 함수 (Inspector - Events)
    /// </summary>
    public virtual void SkillEnd() // public : CircleRange와 같이 다른 스크립트에서도 사용
    {
        // 사용한 스킬 조건 : 상급은 스킬 사용조건이 없기 때문에 false로 초기화하지 않는다.
        if (rank != Rank.ELITE)
            skillConditions[currSkillNum] = false;

        // 스킬 쿨타임
        currSkillCoolTimes[currSkillNum] = maxSkillCoolTimes[currSkillNum];

        // 현재 스킬 
        currSkillNum = -1;

        attackState = AttackState.NONE;

        //Debug.Log("스킬 끝");
    }

    #endregion
}
