using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Fugitive : EnemyAction
{
    public override void InitStatValue()
    {
        base.InitStatValue();
        PatrolDestinationIndex = Random.Range(0, patrolDestinations.Count);

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
        {
            int tmpIndex = PatrolDestinationIndex;
            while (tmpIndex == PatrolDestinationIndex)
            {
                tmpIndex = Random.Range(0, patrolDestinations.Count);
            }
            PatrolDestinationIndex = tmpIndex;

           currDestination = patrolDestinations[patrolDestinationIndex];
        }
    }

    protected override void Dead()
    {
        EnemySetter.Instance.RemoveAllEnemies(enemyType);
    }
}
