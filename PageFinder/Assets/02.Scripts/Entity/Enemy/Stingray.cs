using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Stingray : EnemyController
{
    // Start is called before the first frame update
    public override void Start()
    {
        // 기본 능력치 세팅
        // 이속, 최대 체력, 현재 체력, 공격력, 방어력
        moveSpeed = 0.8f;
        maxHP = 40.0f;
        atk = 10.0f;

        // 타입 세팅
        posType = 1; // 공중
        moveType = 1; // 랜덤 이동
        attackType = 0;

        cognitiveDist = 5f;

        base.Start();
    }

    /// <summary>
    /// 랜덤 이동
    /// </summary>
    public override void MoveRandom()
    {
        
        float distance = Vector3.Distance(posToMove[currentPosIndexToMove], monsterTr.transform.position);

        state = State.MOVE;

        Debug.Log(distance);
        if (distance <= 1.5f)
        {
            SetCurrentPosIndexToMove();

            // 이제 이동할 좌표를 랜덤하게 지정
            while (distance < cognitiveDist) // 이전 좌표와 인지 범위 내에서 새로 생성한 좌표의 거리가 최소 3이상 될 수 있게 설정
            {
                posToMove[currentPosIndexToMove] = new Vector3(originalPos.x + ReturnRandomValue(0, cognitiveDist - 1),
                                                            originalPos.y,
                                                            originalPos.z + ReturnRandomValue(0, cognitiveDist - 1));

                distance = Vector3.Distance(monsterTr.transform.position, posToMove[currentPosIndexToMove]);

            }
        }


        if (!CheckCognitiveDist())
            return;

        distance = Vector3.Distance(playerTr.transform.position, monsterTr.transform.position);
        if (distance <= attackDist)
        {
            state = State.ATTACK;
        }
        else if (distance <= traceDist)
        {
            state = State.TRACE;
        }

        Debug.Log(state);
    }

    // 투사체 발사 로직 

    /* <가오리 공격 루틴>
     * 1. 공격 사정거리 이내
     * 2. 투사체 1회 발사 (일직선) 
     * 3. 1초 후 2번으로 
     * 
     * 
     * 
     * 
     * 
     * 
     * 
     * 
     */
}
