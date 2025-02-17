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

        // ����(didPerceive �״��), �˹�(didPerceive �״��)
        // �ӹ�(didPerceive �״��), ����(didPerceive = false)
        if (state == State.DEBUFF && debuffState != DebuffState.NONE)
            return;

        // idle ���¿����� Idle �ִϸ��̼��� ������ ��� ���·� ����
        // �׷��Ƿ� Idle �ִϸ��̼� ���¿����� ��� �������� ������ �ʵ��� ��
        if (state == State.IDLE && idleState != IdleState.NONE)
            return;

        // ���� ���°� �Ǹ� ������ ������ attackState�� None���� �����
        // ���� �ִϸ��̼� ���� �������� �ʵ��� �����ϱ� ����
        if (state == State.ATTACK && attackState != AttackState.NONE)
        {
            // Fire ���۽� �÷��̾� ���󰡵��� ���� ������Ʈ
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

            // ���ݽÿ� �÷��̾� ���󰡵��� ����
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
        Debug.Log("Fire ����");
    }
}
