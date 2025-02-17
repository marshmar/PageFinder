using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Fugitive : EnemyAction
{
    static List<bool> destinationUsageInfo = new List<bool>(); // 현재 위치를 사용하고 있는지 확인하는 용도

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

        // 목적지에 도달했을 경우
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

        // 기존 목적지 데이터 변경
        destinationUsageInfo[PatrolDestinationIndex] = false;

        // 새로운 목적지 데이터 변경
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
    /// 현재 목적지를 사용하고 있는지 확인
    /// </summary>
    /// <param name="i">인덱스</param>
    /// <returns>사용중 : true</returns>
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
