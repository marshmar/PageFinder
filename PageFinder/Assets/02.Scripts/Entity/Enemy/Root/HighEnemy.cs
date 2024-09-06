using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighEnemy : EnemyAction
{
    [Header("Skill")]
    [SerializeField]
    protected List<string> skillNames = new List<string>();
    [SerializeField]
    protected List<float> maxSkillCoolTimes = new List<float>(); // 스킬 쿨타임 - 인스펙터 창에서 설정 
    protected List<float> currSkillCoolTimes = new List<float>(); // 현재 스킬 쿨타임 
    [SerializeField]
    protected List<int> skillPriority = new List<int>(); // 스킬 우선도
    [SerializeField]
    protected string currSkillName = "";
    protected List<bool> skillCondition = new List<bool>(); // 스킬 조건

    public override void Start()
    {
        base.Start();

        for (int i = 0; i < maxSkillCoolTimes.Count; i++)
        {
            currSkillCoolTimes.Add(maxSkillCoolTimes[i]);
            skillCondition.Add(false);
        }

        // 스킬 관련 값들을 전부 세팅을 했는지 체크
        if (!(maxSkillCoolTimes.Count == currSkillCoolTimes.Count
            && maxSkillCoolTimes.Count == skillNames.Count
            && maxSkillCoolTimes.Count == skillPriority.Count)
            && maxSkillCoolTimes.Count == skillCondition.Count)
        {
            Debug.LogError("maxSkillCoolTimes : " + maxSkillCoolTimes.Count);
            Debug.LogError("currSkillCoolTimes : " + currSkillCoolTimes.Count);
            Debug.LogError("skillNames : " + skillNames.Count);
            Debug.LogError("skillPriority : " + skillPriority.Count);
            Debug.LogError("skillCondition : " + skillCondition.Count);
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
            default:
                Debug.LogWarning(attackState);
                break;
        }
    }

    protected void SkillAction()
    {
        agent.isStopped = true;
    }


    #region State 관련 함수

    protected override void SetAllState()
    {
        float distance;
        distance = Vector3.Distance(playerObj.transform.transform.position, enemyTr.position);

        // 스킬을 진행하고 있는 경우 Attack - SkillN 애니메이션을 유지하도록 하기 위함
        if (!currSkillName.Equals(""))
            return;

        if (distance <= atkDist)
        {
            // 적이 플레이어 앞에 있는 경우
            if (CheckIfThereIsPlayerInFrontOfEnemy())
                state = State.ATTACK;
            else
            {
                if (skillCondition[0])
                    return;

                state = State.MOVE;
            }
                

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

    protected override void SetAttackState()
    {
        int attackValue = GetTypeOfAttackToUse();

        // 사용할 스킬이 있는 경우
        if (attackValue >= 0)
            attackState = AttackState.SKILL;
        else if(attackValue == -1)
            attackState = AttackState.DEFAULT;
        else
            attackState = AttackState.ATTACKWAIT;
    }

    #endregion


    protected override void SetAttackCooltime()
    {
        SetCurrDefaultAtkCoolTime();
        SetCurrSkillCoolTime();
        CheckSkillsCondition();
    }

    #region 스킬 관련 함수

    protected virtual int CheckIfThereAreAnySkillsAvailable()
    {
        /* <적 등급 별 루틴>
         *  하급 : skillCoolTime.Count == 0 => 실행 false
         *  상급 : 스킬 1개 => 해당 스킬 쿨타임 체크 => 0이면 true 아니면 false
         *  중간보스 : 스킬 2개 => 상황에 따라 어떤 스킬을 사용할지 체크 
         */

        /// 기본 공격을 하고 있는 중일 경우
        if (attackState == AttackState.DEFAULT)
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
    protected virtual int GetTypeOfAttackToUse()
    {
        int skillIndexToUse = -1;

        // 우선순위가 높은 순서대로 쿨타임이 돌은 스킬의 인덱스 확인
        skillIndexToUse = CheckIfThereAreAnySkillsAvailable();

        // 사용할 스킬이 있는 경우
        if (skillIndexToUse >= 0)
            return skillIndexToUse;

        // 기본 공격 쿨타임이 돌은 경우
        if (currDefaultAtkCoolTime <= 0)
            return -1;

        return -2;
    }

    private void SetCurrSkillCoolTime()
    {
        for (int i = 0; i < currSkillCoolTimes.Count; i++)
        {
            if (currSkillName.Equals(skillNames[i])) // 해당 스킬 사용중인 상태
                continue;

            if (currSkillCoolTimes[i] < 0)
                continue;

            currSkillCoolTimes[i] -= Time.deltaTime;
        }
    }

    protected virtual void CheckSkillsCondition()
    {

    }

    /// <summary>
    /// 현재 스킬 쿨타임을 리셋한다.
    /// </summary>
    private void ResetCurrSkillCoolTime()
    {
        for (int i = 0; i < skillNames.Count; i++)
        {
            if (currSkillName.Equals(skillNames[i]))
            {
                currSkillCoolTimes[i] = maxSkillCoolTimes[i];
                break;
            }
        }
    }

    #endregion

    #region 애니메이션 관련 함수

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

            case AttackState.SKILL:
                SkillAni();
                break;

            default:
                Debug.LogWarning(attackState);
                break;
        }
    }

    protected override void DefaultIdleAni()
    {
        base.DefaultIdleAni();

        ani.SetBool("isSkill", false);
    }

    protected override void FindAni()
    {
        base.FindAni();

        ani.SetBool("isSkill", false);
    }

    protected override void TraceAni()
    {
        base.TraceAni();

        ani.SetBool("isSkill", false);
    }

    protected override void AttackWaitAni()
    {
        base.AttackWaitAni();
        ani.SetBool("isSkill", false);
    }

    protected override void DefaultAttackAni()
    {
        base.DefaultAttackAni();
        ani.SetBool("isSkill", false);
    }

    protected override void StunAni()
    {
        base.StunAni();
        ani.SetBool("isSkill", false);
    }

    protected override void DieAni()
    {
        base.DieAni();
        ani.SetBool("isSkill", false);
    }

    protected virtual void SkillAni()
    {
        ani.SetBool("isIdle", false);
        ani.SetBool("isMove", false);
        ani.SetBool("isAttack", true);
        ani.SetBool("isAbnormal", false);

        ani.SetBool("isFind", false);
        ani.SetBool("isTrace", false);
        ani.SetBool("isAttackWait", false);
        ani.SetBool("isDefaultAttack", false);
        ani.SetBool("isSkill", true);

        if(!currSkillName.Equals(""))
        {
            for(int i=0; i<skillNames.Count; i++)
            {
                if (currSkillName.Equals(skillNames[i]))
                    ani.SetBool(skillNames[i], true); // 스킬 인덱스에 따라 string값 변경하도록 수정하기
                else
                    ani.SetBool(skillNames[i], false);
            }
        }
    }

    /// <summary>
    /// Skill 애니메이션이 끝나고 호출되는 함수 (Inspector - Events)
    /// </summary>
    protected void SkillAniEnd()
    {
        // 스킬 조건
        for (int i = 0; i < skillNames.Count; i++)
        {
            // 현재 사용한 스킬 
            if (currSkillName.Equals(skillNames[i]))
            {
                if(rank == Rank.MEDIUMBOSS)
                    skillCondition[i] = false;
                break;
            }
        }

        // 스킬 쿨타임
        ResetCurrSkillCoolTime();
        currDefaultAtkCoolTime = maxDefaultAtkCoolTime;

        // 애니메이션
        for (int i = 0; i < skillNames.Count; i++)
        {
            ani.SetBool(skillNames[i], false);
        }

        // 현재 스킬 이름
        currSkillName = "";
    }

    #endregion



    /// <summary>
    /// 적이 피해를 입을 때 플레이어 쪽에서 호출하는 함수
    /// </summary>
    /// <param name="damage"></param>
    protected override void Hit()
    {
        // 상급, 보스, 중간보스 부터는 플레이어에게 공격당했을 때 스턴이 걸리지 않기 때문에 비워둔다.
    }
}
