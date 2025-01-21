using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighEnemy : EnemyAction
{
    [Header("Skill")]
    [SerializeField]
    protected string[] skillNames;
    [SerializeField]
    protected bool[] moveSkillTypeData;
    [SerializeField]
    protected float[] maxSkillCoolTimes; // 스킬 쿨타임 - 인스펙터 창에서 설정 
    protected List<float> currSkillCoolTimes = new List<float>(); // 현재 스킬 쿨타임 
    [SerializeField]
    protected float[] skillPriority; // 스킬 우선도
    [SerializeField]
    protected string currSkillName = "";
    protected List<bool> skillCondition = new List<bool>(); // 스킬 조건

    public override void Start()
    {
        // 상급부터는 스킬 1개 사용하기 때문에 애니메이션 추가
        AddAnivariableNames("isSkill", "Skill0");

        base.Start();
        
        for (int i = 0; i < maxSkillCoolTimes.Length; i++)
        {
            currSkillCoolTimes.Add(maxSkillCoolTimes[i]);
            skillCondition.Add(false);
        }

        // 스킬 관련 값들을 전부 세팅을 했는지 체크
        if (!(maxSkillCoolTimes.Length == currSkillCoolTimes.Count
            && maxSkillCoolTimes.Length == skillNames.Length
            && maxSkillCoolTimes.Length == skillPriority.Length)
            && maxSkillCoolTimes.Length == skillCondition.Count)
        {
            Debug.LogError($"maxSkillCoolTimes : {maxSkillCoolTimes.Length}" +
                $"currSkillCoolTimes : {currSkillCoolTimes.Count}" +
                $"skillNames : {skillNames.Length}" +
                $"skillPriority : {skillPriority.Length}" +
                $"skillCondition : {skillCondition.Count}");
        }
    }

    /// <summary>
    /// 적이 피해를 입을 때 플레이어 쪽에서 호출하는 함수
    /// </summary>
    /// <param name="damage"></param>
    protected override void Hit()
    {
        // 상급, 보스, 중간보스 부터는 플레이어에게 공격당했을 때 스턴이 걸리지 않기 때문에 비워둔다.
    }

    #region State 관련 함수

    protected override void SetRootState()
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
            else
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

    #region 액션 관련 함수

    protected override void AttackAction()
    {
        switch (attackState)
        {
            case AttackState.ATTACKWAIT:
                AttackWaitAction();
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

    #endregion

    #region 시간 관련 함수

    protected override void SetAttackCooltime()
    {
        SetCurrDefaultAtkCoolTime();
        SetCurrSkillCoolTime();
        CheckSkillsCondition();
    }

    /// <summary>
    /// 현재 스킬 쿨타임을 리셋한다.
    /// </summary>
    private void ResetCurrSkillCoolTime()
    {
        for (int i = 0; i < skillNames.Length; i++)
        {
            if (currSkillName.Equals(skillNames[i]))
            {
                if (currSkillName.Equals("Skill2"))
                {
                    maxSkillCoolTimes[i] = 200;
                    //Debug.Log("최대값 변경 : " + maxSkillCoolTimes[i]);
                }
                   

                currSkillCoolTimes[i] = maxSkillCoolTimes[i];
                break;
            }
        }
    }

    #endregion

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
        for (int priority = 0; priority < skillNames.Length; priority++)
        {
            // 우선 순위가 높은 스킬 순서대로 쿨타임이 돌았는지 체크 : 숫자가 낮을 수록 우선 순위가 높음
            for (int indexToCheck = 0; indexToCheck < skillPriority.Length; indexToCheck++)
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
        if(abnormalState == AbnomralState.BINDING)
        {
            for(int i=0; i<skillCondition.Count; i++)
            {
                // 움직임 스킬인 경우
                if (moveSkillTypeData[i])
                {
                    skillCondition[i] = false;
                    break;
                }
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
                SetAniVariableValue("isAttack", "isAttackWait");
                break;

            case AttackState.DEFAULT:
                SetAniVariableValue("isAttack", "isDefaultAttack");
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
        if (currSkillName.Equals(""))
            return;

        for (int i = 0; i < skillNames.Length; i++)
        {
            if (!currSkillName.Equals(skillNames[i]))
                continue;
            //Debug.Log(skillNames[i] + "애니메이션 활성화");
            SetAniVariableValue("isAttack", "isSkill", skillNames[i]); // 스킬 인덱스에 따라 string값 변경하도록 수정하기
            break;
        }
    }

    /// <summary>
    /// Skill 애니메이션이 끝나고 호출되는 함수 (Inspector - Events)
    /// </summary>
    protected void SkillAniEnd()
    {
        // 스킬 조건
        for (int i = 0; i < skillNames.Length; i++)
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
        for (int i = 0; i < skillNames.Length; i++)
        {
            ani.SetBool(skillNames[i], false);
        }

        // 현재 스킬 이름
        currSkillName = "";

        attackState = AttackState.NONE;
    }

    #endregion
}
