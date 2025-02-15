using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Fire_Jiruru : EnemyAction
{
    [SerializeField]
    private CircleRange circleRangeScr;

    protected override void SetDetailState()
    {
        switch (state)
        {
            case State.IDLE:
                moveState = MoveState.NONE;
                attackState = AttackState.NONE;
                debuffState = DebuffState.NONE;
                isFlee = false;

                SetIdleState();
                SetAgentData(transform.position);
                break;

            case State.MOVE:
                attackState = AttackState.NONE;
                debuffState = DebuffState.NONE;

                SetMoveState();
                SetMoveAction();
                break;

            // 공격시에 플레이어 따라가도록 설정
            case State.ATTACK:
                moveState = MoveState.NONE;
                debuffState = DebuffState.NONE;
                isFlee = false;

                Fire();
                SetAttackState();
                SetAgentData(transform.position);
                break;

            case State.DEBUFF:
                moveState = MoveState.NONE;
                attackState = AttackState.NONE;
                isFlee = false;

                SetAgentData(transform.position);
                break;

            case State.DIE:
                moveState = MoveState.NONE;
                attackState = AttackState.NONE;
                debuffState = DebuffState.NONE;
                isFlee = false;

                SetAgentData(transform.position);
                break;
        }
    }


    private void Fire()
    {
        ChangeMoveSpeed(3, 70);
        circleRangeScr.StartRangeCheck(1, Enemy.DebuffState.NONE, 1.5f, 0, ATK, 1);
        Debug.Log("Fire 시작");
    }
}
