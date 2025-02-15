using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class Fugitive : EnemyAction
{
    protected override void InitStatValue()
    {
        base.InitStatValue();

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
        if (distance < 0.5f)
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
}
