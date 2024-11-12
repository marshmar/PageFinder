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

    public override void Start()
    {
        base.Start();

        ani = GetComponent<Animator>();

        AddAnivariableNames("isIdle", "isMove", "isAttack", "isStun", "isFind", "isTrace", "isAttackWait", "isDefaultAttack");

        if (!isAnimationCoroutineWorking)
            StartCoroutine(Animation());
    }


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

                case State.STUN:
                    SetAniVariableValue("isStun");
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
        switch(moveState)
        {
            case MoveState.FIND:
                SetAniVariableValue("isMove", "isFind");
                break;

            case MoveState.TRACE:
                SetAniVariableValue("isMove", "isTrace");
                break;
        }
    }

    protected virtual void AttackAni()
    {
        switch(attackState)
        {
            case AttackState.ATTACKWAIT:
                SetAniVariableValue("isAttack", "isAttackWait");
                break;

            case AttackState.DEFAULT:
                SetAniVariableValue("isAttack", "isDefaultAttack");
                break;

            default:
                Debug.LogWarning(attackState);
                break;

        }
    }

    /// <summary>
    /// Animator에서 사용하는 변수의 이름을 추가하는 함수
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

    protected virtual void DefaultAttackAniEnd()
    {
        currDefaultAtkCoolTime = maxDefaultAtkCoolTime;
    }
}
