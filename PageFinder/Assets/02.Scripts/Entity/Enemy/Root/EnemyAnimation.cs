using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using static UnityEngine.Rendering.DebugUI;

public class EnemyAnimation : Enemy
{
    #region Variables
    List<string> aniVariableNames = new List<string>();
    private string[] stateTypeNames = {"moveType", "attackType"};
    protected bool debuffIsEnd;

    protected Animator ani;
    #endregion

    #region Properties
    public override float MoveSpeed
    {
        get
        {
            return moveSpeed;
        }
        set
        {
            moveSpeed = value;
            ani.SetFloat("runSpeed", moveSpeed/3.5f); // 3.5 : nav mesh agent 기본속도
            agent.speed = moveSpeed;
        }
    }


    protected override float CurrAttackSpeed
    {
        get
        {
            return currAttackSpeed;
        }
        set
        {
            currAttackSpeed = value;
            ani.SetFloat("attackSpeed", currAttackSpeed);
        }
    }

    protected bool DebuffIsEnd
    {
        get { return debuffIsEnd; }
        set 
        { 
            debuffIsEnd = value;
            ani.SetBool("DebuffIsEnd", debuffIsEnd);
        }
    }

    #endregion

    public override void Start()
    {
        InitStatValue();
        StartCoroutine(EnemyCoroutine());
    }

    #region EnemyCoroutine

    protected virtual IEnumerator EnemyCoroutine()
    {
        while (!isDie)
        {
            // 플레이어가 죽었을 경우
            if (playerObj == null)
                break;

            Animation();
            yield return null;
        }

        Dead();
    }

    /// <summary>
    /// 전체 애니메이션 동작 관리
    /// </summary>
    /// <returns></returns>
    protected void Animation()
    {
        switch (state)
        {
            case State.IDLE:
                IdleAni();
                break;

            case State.DEBUFF:
                DebuffAni();
                break;

            case State.MOVE:
                MoveAni();
                break;

            case State.ATTACK:
                AttackAni();
                break;

            case State.DIE:
                SetAniVariableValue(State.DIE);
                break;

            default:
                Debug.LogWarning(state);
                break;
        }
    }

    #endregion

    #region Init
    protected override void InitComponent()
    {
        base.InitComponent();
        ani = DebugUtils.GetComponentWithErrorLogging<Animator>(gameObject, "Animator");
    }

    protected override void InitStatValue()
    {
        base.InitStatValue();

        CurrAttackSpeed = currAttackSpeed;
        MoveSpeed = moveSpeed;

        debuffIsEnd = false;

        AddAnivariableNames("isIdle", "isMove", "isAttack", "isDebuff", "isDie");
        MoveSpeed = 3.5f;

        SetStateTypeVariables(-1); // 전부 None을 의미하는 0값으로 세팅
    }

    #endregion

    #region Animations
    protected void IdleAni()
    {
        switch (idleState)
        {
            case IdleState.NONE:
                break;

            case IdleState.FIRSTWAIT:
                SetAniVariableValue(IdleState.FIRSTWAIT);
                break;

            case IdleState.PATROLWAIT:
                SetAniVariableValue(IdleState.PATROLWAIT);
                break;

            case IdleState.ATTACKWAIT:
                SetAniVariableValue(IdleState.ATTACKWAIT);
                break;

            default:
                Debug.LogWarning(idleState);
                break;
        }
    }

    protected void MoveAni()
    {
        switch (moveState)
        {
            case MoveState.NONE:
                ani.SetInteger(stateTypeNames[0], 0);
                break;

            case MoveState.PATROL:
                SetAniVariableValue(MoveState.PATROL);
                break;

            case MoveState.CHASE:
                SetAniVariableValue(MoveState.CHASE);
                break;

            case MoveState.ROTATE:
                SetAniVariableValue(MoveState.ROTATE);
                break;

            case MoveState.FLEE:
                SetAniVariableValue(MoveState.FLEE);
                break;

            default:
                Debug.LogWarning(moveState);
                break;
        }
    }

    protected virtual void AttackAni()
    {
        switch (attackState)
        {
            case AttackState.NONE:
                ani.SetInteger(stateTypeNames[1], 0);
                Debug.Log("AttackState == None  Ani ");
                break;

            case AttackState.BASIC:
                SetAniVariableValue(AttackState.BASIC);
                break;

            default:
                Debug.LogWarning(attackState);
                break;
        }
    }

    protected void DebuffAni()
    {
        switch (debuffState)
        {
            case DebuffState.NONE:
                break;

            case DebuffState.STAGGER:
                SetAniVariableValue(DebuffState.STAGGER);
                break;

            case DebuffState.KNOCKBACK:
                SetAniVariableValue(DebuffState.KNOCKBACK);
                break;

            case DebuffState.BINDING:
                SetAniVariableValue(DebuffState.BINDING);
                break;

            case DebuffState.STUN:
                SetAniVariableValue(DebuffState.STUN);
                break;

            default:
                break;
        }
    }

    #endregion

    #region Set Variable

    /// <summary>
    /// Animator에서 사용하는 Parameters 변수의 이름을 추가하는 함수
    /// </summary>
    /// <param name="names"></param>
    protected void AddAnivariableNames(params string[] names)
    {
        for (int i = 0; i < names.Length; i++)
            aniVariableNames.Add(names[i]);
    }

    /// <summary>
    /// Animator의 변수 값을 변경하는 함수
    /// </summary>
    /// <param name="names">true로 변경시킬 변수 이름</param>
    private void SetAniVariableValueToTrue(params string[] names)
    {
        for (int i = 0; i < aniVariableNames.Count; i++)
        {
            ani.SetBool(aniVariableNames[i], false);
        }

        for (int nameIndex = 0; nameIndex < names.Length; nameIndex++)
        {
            for (int i = 0; i < aniVariableNames.Count; i++)
            {
                if (names[nameIndex].Equals(aniVariableNames[i]))
                {
                    ani.SetBool(aniVariableNames[i], true);
                    break;
                }
            }
        }
    }

    protected void SetAniVariableValue<T>(T t)
    {
        int value = GetStateTypeToInt(t);
        switch (state)
        {
            case State.IDLE:
                SetAniVariableValueToTrue("isIdle");
                break;

            case State.MOVE:
                SetAniVariableValueToTrue("isMove");
                SetStateTypeVariables(0, value);
                // MoveType - none : 0     patrol : 1   chase : 2   rotate : 3
                break;

            case State.ATTACK:
                SetAniVariableValueToTrue("isAttack");
                SetStateTypeVariables(1, value);
                // AttackType - none : 0     basicAttack : 1  reinforcementAttack : 2     Skill0 : 3      Skill1 :4   ... 
                break;

            case State.DEBUFF:
                SetAniVariableValueToTrue("isDebuff");
                // DebuffType - none : 0     stiff : 1    knockback : 2   binding : 3     stun : 4
                break;

            case State.DIE:
                SetAniVariableValueToTrue("isDie");
                break;

            default:
                break;
        }
    }

    private void SetStateTypeVariables(int typeNum, int value = 0)
    {
        for (int i = 0; i < stateTypeNames.Length; i++)
        {
            if(i == typeNum)
                ani.SetInteger(stateTypeNames[i], value);
            else
                ani.SetInteger(stateTypeNames[i], 0); // None을 의미하는 0의 값으로 변경
        }
    }

    /// <summary>
    /// Idle,Move,Attack,Debuff State들의 상태값을 int형으로 얻는다.
    /// </summary>
    /// <typeparam name="T">Idle, Move, Attack, Debuff State</typeparam>
    /// <param name="t"></param>
    /// <returns></returns>
    private int GetStateTypeToInt<T>(T t)
    {
        // enum을 int형으로 변경할 수 있는 것을 이용
        switch(t)
        {
            case IdleState.NONE:
            case MoveState.NONE:
            case AttackState.NONE:
            case DebuffState.NONE:
                return 0;

            // None
            case IdleState.FIRSTWAIT:
            case MoveState.PATROL:
            case AttackState.BASIC:
            case DebuffState.STAGGER:
                return 1;

            case IdleState.PATROLWAIT:
            case MoveState.CHASE:
            case AttackState.REINFORCEMENT:
            case DebuffState.KNOCKBACK:
                return 2;

            case IdleState.ATTACKWAIT:
            case MoveState.ROTATE:
            case AttackState.SKILL:
            case DebuffState.BINDING:
                return 3;

            case AttackState.SKILL + 1:
            case MoveState.FLEE:
            case DebuffState.STUN:
                return 4;

            case AttackState.SKILL + 2:
                return 5;

            default:
                Debug.LogWarning(t);
                return 0;
        }
    }
    #endregion
}
