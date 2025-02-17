using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Android;

public class Fire_Jiruru : EnemyAction
{
    [SerializeField]
    private CircleRange circleRangeScr;

    protected override void Action()
    {
        if (state == State.DIE)
            return;

        // 경직(didPerceive 그대로), 넉백(didPerceive 그대로)
        // 속박(didPerceive 그대로), 기절(didPerceive = false)
        if (state == State.DEBUFF && debuffState != DebuffState.NONE)
            return;

        // idle 상태에서는 Idle 애니메이션이 끝나고 어떠한 상태로 변함
        // 그러므로 Idle 애니메이션 상태에서는 어떠한 동작으로 변하지 않도록 함
        if (state == State.IDLE && idleState != IdleState.NONE)
            return;

        // 공격 상태가 되면 공격이 끝나야 attackState가 None으로 변경됨
        // 공격 애니메이션 도중 움직이지 않도록 설정하기 위함
        if (state == State.ATTACK && attackState != AttackState.NONE)
        {
            // Fire 시작시 플레이어 따라가도록 정보 업데이트
            agent.destination = playerObj.transform.position;
            return;
        }

        SetRootState();
        SetDetailState();
    }

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
                SetAgentData(currDestination, false);
                agent.stoppingDistance = 1;
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
