using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class MediumBossEnemy : HighEnemy, IShield
{
    [Header("Reinforcement Default Attack")]

    [SerializeField]
    protected GameObject ReinforcementAttack_Prefab;

    [SerializeField]
    protected int maxDefaultAtkCnt = 4;
    protected int currDefaultAtkCnt = 0;

    [Header("Shield")]
    [SerializeField]
    private Gradation grdation;
    [SerializeField]
    private ShieldBar shieldBar;

    float maxShield;
    float currShield;

    public override float HP
    {
        get 
        { 
            return currHP + currShield;  // 100 + 50   - 55
        }
        set
        {
            // 감소시켜도 쉴드가 남아있는 경우
            if(value > currHP)
            {
                CurrShield = value - currHP;
            }
            else // 감소시켜도 쉴드가 남아있지 않은 경우
            {
                CurrShield = 0;
                currHP = value;
            }

            Hit();
            hpBar.SetCurrValueUI(currHP);
            
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

    public override float MAXHP
    {
        get
        {
            return maxHP;
        }
        set
        {
            maxHP = value;

            hpBar.SetMaxValueUI(maxHP);
            grdation.SetGradation(maxHP);
        }
    }

    #region 쉴드 관련

    public float MaxShield
    {
        get
        {
            return maxShield;
        }
        set
        {
            // 실드를 생성한 경우

            maxShield = value;
            hpBar.SetMaxValueUI(maxHP + maxShield);
            
            grdation.SetGradation(maxHP + maxShield);

            
            shieldBar.SetMaxShieldValueUI(maxHP, currHP, maxShield);
            CurrShield = maxShield;
        }
    }

    public float CurrShield
    {
        get
        {
            return currShield;
        }
        set
        {
            currShield = value;

            shieldBar.SetCurrValueUI(currShield);

            // 쉴드를 다 사용했을 경우
            if (currShield <= 0)
            {
                currShield = 0;
                grdation.SetGradation(maxHP);
            }
        }
    }

    #endregion


    public override void Start()
    {
        AddAnivariableNames("isReinforcementAttack");

        base.Start();
        currDefaultAtkCnt = 0;

        grdation.SetGradation(maxHP);

        MaxShield = 50;
    }

    #region 상태 관련 함수

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

    #endregion

    #region 액션 관련 함수
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

    private void ReinforcementAttackAction()
    {
        agent.isStopped = true;
    }

    #endregion

    #region 쿨타임 관련 함수

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

    #endregion

    #region 강화 기본 공격 관련 함수

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

            case AttackState.REINFORCEMENT:
                SetAniVariableValue("isAttack", "isReinforcementAttack");
                break;

            case AttackState.SKILL:
                SkillAni();
                break;

            default:
                Debug.LogWarning(attackState);
                break;
        }
    }

    protected override void DefaultAttackAniEnd()
    {
        base.DefaultAttackAniEnd();

        currDefaultAtkCnt++;
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
