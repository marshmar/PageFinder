using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyAnimation : Enemy
{
    protected Animator ani;
    protected bool isAnimationCoroutineWorking = false;


    public override void Start()
    {
        base.Start();

        ani = GetComponent<Animator>();

        if (!isAnimationCoroutineWorking)
            StartCoroutine(Animation());
    }

    protected IEnumerator Animation()
    {
        isAnimationCoroutineWorking = true;
        while (!isDie)
        {
            switch (state)
            {
                case State.IDLE:
                    IdleAni();
                    break;

                case State.ABONORMAL:
                    AbnormalAni();
                    break;

                case State.MOVE:
                    MoveAni();
                    break;

                case State.ATTACK:
                    AttackAni();
                    break;

                case State.DIE:
                    DieAni();
                    break;

                default:
                    Debug.LogWarning(state);
                    break;
            }
            yield return null;
        }
    }

    protected void IdleAni()
    {
        switch (idleState)
        {
            case IdleState.NONE:
                break;

            case IdleState.DEFAULT:
                DefaultIdleAni();
                break;

            default:
                Debug.LogWarning(idleState);
                break;
        }
    }

    protected void AbnormalAni()
    {
        switch(abnormalState)
        {
            case AbnormalState.NONE:
                break;

            case AbnormalState.BINDING:
                break;

            case AbnormalState.STUN:
                StunAni();
                break;
            default:
                Debug.LogWarning(abnormalState);
                break;
        }
    }

    protected void MoveAni()
    {
        switch(moveState)
        {
            case MoveState.FIND:
                FindAni();
                break;

            case MoveState.TRACE:
                TraceAni();
                break;
        }
    }

    protected virtual void AttackAni()
    {
        switch(attackState)
        {
            case AttackState.ATTACKWAIT:
                AttackWaitAni();
                break;

            case AttackState.DEFAULT:
                DefaultAttackAni();
                break;

            default:
                Debug.LogWarning(attackState);
                break;

        }
    }

    protected virtual void DefaultIdleAni()
    {
        ani.SetBool("isIdle", true);
        ani.SetBool("isMove", false);
        ani.SetBool("isAttack", false);
        ani.SetBool("isAbnormal", false);

        ani.SetBool("isFind", false);
        ani.SetBool("isTrace", false);

        ani.SetBool("isAttackWait", false);
        ani.SetBool("isDefaultAttack", false);
    }

    protected virtual void FindAni()
    {
        ani.SetBool("isIdle", false);
        ani.SetBool("isMove", true);
        ani.SetBool("isAttack", false);
        ani.SetBool("isAbnormal", false);

        ani.SetBool("isFind", true);
        ani.SetBool("isTrace", false);

        ani.SetBool("isAttackWait", false);
        ani.SetBool("isDefaultAttack", false);
    }

    protected virtual void TraceAni()
    {
        ani.SetBool("isIdle", false);
        ani.SetBool("isMove", true);
        ani.SetBool("isAttack", false);
        ani.SetBool("isAbnormal", false);

        ani.SetBool("isFind", false);
        ani.SetBool("isTrace", true);

        ani.SetBool("isAttackWait", false);
        ani.SetBool("isDefaultAttack", false);
    }

    protected virtual void AttackWaitAni()
    {
        ani.SetBool("isIdle", false);
        ani.SetBool("isMove", false);
        ani.SetBool("isAttack", true);
        ani.SetBool("isAbnormal", false);

        ani.SetBool("isFind", false);
        ani.SetBool("isTrace", false);

        ani.SetBool("isAttackWait", true);
        ani.SetBool("isDefaultAttack", false);
    }

    protected virtual void DefaultAttackAni()
    {
        ani.SetBool("isIdle", false);
        ani.SetBool("isMove", false);
        ani.SetBool("isAttack", true);
        ani.SetBool("isAbnormal", false);

        ani.SetBool("isFind", false);
        ani.SetBool("isTrace", false);

        ani.SetBool("isAttackWait", false);
        ani.SetBool("isDefaultAttack", true);
    }

    protected virtual void StunAni()
    {
        ani.SetBool("isIdle", false);
        ani.SetBool("isMove", false);
        ani.SetBool("isAttack", false);
        ani.SetBool("isAbnormal", true);

        ani.SetBool("isFind", false);
        ani.SetBool("isTrace", false);

        ani.SetBool("isAttackWait", false);
        ani.SetBool("isDefaultAttack", false);
    }

    protected virtual void DieAni()
    {
        //ani.SetBool("isDie");
    }

    protected virtual void DefaultAttackAniEnd()
    {
        currDefaultAtkCoolTime = maxDefaultAtkCoolTime;
    }
}
