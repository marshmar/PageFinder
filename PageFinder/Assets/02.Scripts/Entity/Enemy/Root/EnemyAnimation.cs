using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class EnemyAnimation : Enemy
{
    protected Animator ani;
    protected bool isAnimationCoroutineWorking = false;

    List<string> aniVariableNames = new List<string>();


    public override float MoveSpeed
    {
        get
        {
            return moveSpeed;
        }
        set
        {
            moveSpeed = value;
            ani.SetFloat("moveSpeed", moveSpeed);
            agent.speed = moveSpeed;
        }
    }


    public override float CurrAttackSpeed
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

    public override void Start()
    {
        base.Start();

        ani = DebugUtils.GetComponentWithErrorLogging<Animator>(gameObject, "Animator");
        ani.SetFloat("attackSpeed", currAttackSpeed);
        ani.SetFloat("moveSpeed", moveSpeed);
        AddAnivariableNames("isIdle", 
                            "isMove", "isFind", "isTrace", "isRotate",
                            "isAttack", "isAttackWait", "isDefaultAttack",
                            "isStun");

        MoveSpeed = 1f;

        if (!isAnimationCoroutineWorking)
            StartCoroutine(Animation());
    }

    /// <summary>
    /// 전체 애니메이션 동작 관리
    /// </summary>
    /// <returns></returns>
    protected IEnumerator Animation()
    {
        isAnimationCoroutineWorking = true;
        while (!isDie)
        {
            if (playerObj == null)
                break;

            switch (state)
            {
                case State.IDLE:
                    IdleAni();
                    break;

                case State.ABNORMAL:
                    AbnormalAni();
                    break;

                case State.MOVE:
                    MoveAni();
                    break;

                case State.ATTACK:
                    AttackAni();
                    break;

                case State.DIE:
                    //SetAniVariableValue("isDie");
                    break;

                default:
                    Debug.LogWarning(state);
                    break;
            }
            yield return null;
        }
    }

    protected void AbnormalAni()
    {
        switch (abnormalState)
        {
            case AbnomralState.STUN:
                SetAniVariableValue("isAbnormal", "isStun");
                break;

            case AbnomralState.KNOCKBACK:
                SetAniVariableValue("isAbnormal", "isKnockBack");
                break;

            case AbnomralState.BINDING:
                SetAniVariableValue("isAbnormal", "isBinding");
                break;

            case AbnomralState.AIR:
                SetAniVariableValue("isAbnormal", "isAir");
                break;

            default:
                break;
        }
    }

    protected void IdleAni()
    {
        switch (idleState)
        {
            case IdleState.NONE:
                break;

            case IdleState.DEFAULT:
                SetAniVariableValue("isIdle");
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
                break;

            case MoveState.FIND:
                SetAniVariableValue("isMove", "isFind");
                break;

            case MoveState.TRACE:
                SetAniVariableValue("isMove", "isTrace");
                break;

            case MoveState.ROTATE:
                SetAniVariableValue("isMove", "isRotate");
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
            case AttackState.ATTACKWAIT:
                SetAniVariableValue("isAttack", "isAttackWait");
                break;

            case AttackState.DEFAULT:
                SetAniVariableValue("isAttack", "isDefaultAttack");
                break;

            default:
                break;
        }
    }

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
    protected void SetAniVariableValue(params string[] names)
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

    /// <summary>
    /// 기본 공격 애니메이션 종료 후 동작
    /// </summary>
    protected virtual void DefaultAttackAniEnd()
    {
        currDefaultAtkCoolTime = maxDefaultAtkCoolTime;
        attackState = AttackState.NONE;
    }
}
