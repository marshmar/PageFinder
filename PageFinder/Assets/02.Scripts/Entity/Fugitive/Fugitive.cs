using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Fugitive : EnemyAction
{
    static List<bool> destinationUsageInfo = new List<bool>(); // ���� ��ġ�� ����ϰ� �ִ��� Ȯ���ϴ� �뵵

    public override void InitStatValue()
    {
        base.InitStatValue();

        PatrolDestinationIndex = Random.Range(0, patrolDestinations.Count);

        destinationUsageInfo.Clear();
        for (int i = 0; i < patrolDestinations.Count; i++)
            destinationUsageInfo.Add(false);

        SetDestination();
        SetStartPos();

        state = State.MOVE;
    }

    protected override void SetAllCoolTime()
    {
        SetDebuffTime();
    }

    protected override void SetRootState()
    {
        state = State.MOVE;
    }

    protected override void SetMoveState()
    {
        float distance = Vector3.Distance(currDestination, enemyTr.position);

        moveState = MoveState.PATROL;

        // �������� �������� ���
        if (distance < 1f)
            SetDestination();
    }

    private void SetDestination()
    {
        int tmpIndex = PatrolDestinationIndex;
        while (tmpIndex == PatrolDestinationIndex || isUsingDestination(tmpIndex))
        {
            tmpIndex = Random.Range(0, patrolDestinations.Count);
        }

        // ���� ������ ������ ����
        destinationUsageInfo[PatrolDestinationIndex] = false;

        // ���ο� ������ ������ ����
        destinationUsageInfo[tmpIndex] = true;
        PatrolDestinationIndex = tmpIndex;
        currDestination = patrolDestinations[patrolDestinationIndex];
    }

    private void SetStartPos()
    {
        int tmpIndex = 0;
        while (isUsingDestination(tmpIndex))
        {
            tmpIndex = Random.Range(0, patrolDestinations.Count);
        }
        transform.position = patrolDestinations[tmpIndex];
        agent.Warp(patrolDestinations[tmpIndex]);
    }

    /// <summary>
    /// ���� �������� ����ϰ� �ִ��� Ȯ��
    /// </summary>
    /// <param name="i">�ε���</param>
    /// <returns>����� : true</returns>
    private bool isUsingDestination(int i)
    {
        if (i < 0 || i >= patrolDestinations.Count)
            return false;

        return destinationUsageInfo[i];
    }

    protected override void Dead()
    {
        EnemySetter.Instance.RemoveAllEnemies(enemyType);
    }
}
