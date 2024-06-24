using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shark : BossController
{
    Vector3 rushPos = Vector3.zero;



    // Start is called before the first frame update
    public override void Start()
    {
        // 기본 능력치 세팅
        // 이속, 최대 체력, 현재 체력, 공격력, 방어력
        moveSpeed = 0.7f;
        maxHP = 40.0f;
        atk = 10.0f;

        // 타입 세팅
        posType = 0;
        moveType = 2; // 추적 이동
        attackType = 0;

        // 위치 세팅
        originalPos = new Vector3(-70, 0.86f, -82);

        base.Start();

        //Debug.Log((new Vector3(2, 0 , 0) - new Vector3(5, 0 , 0)).normalized);
    }

    protected override IEnumerator EnemyAction()
    {
        while (!isDie)
        {
            switch (state)
            {
                case State.IDLE:
                    break;
                case State.MOVE:
                    meshRenderer.material.color = Color.green;
                    agent.SetDestination(playerTr.position);
                    agent.isStopped = false;
                    break;
                case State.TRACE:
                    Debug.Log("Trace");
                    meshRenderer.material.color = Color.blue;
                    agent.SetDestination(playerTr.position);
                    agent.isStopped = false;
                    break;
                case State.ATTACK:
                    Debug.Log("Attack");
                    meshRenderer.material.color = Color.red;
                    break;
                case State.SKILL:
                    meshRenderer.material.color = Color.yellow;
                    agent.SetDestination(rushPos);
                    break;
                case State.DIE:
                    Die();
                    break;
            }
            yield return new WaitForSeconds(0.3f);
        }
    }


    protected override IEnumerator CheckEnemyState()
    {
        while (!isDie)
        {
            //meshRenderer.material.color = Color.green;
            yield return new WaitForSeconds(0.3f);

            float distance = Vector3.Distance(playerTr.transform.position, monsterTr.transform.position);


            if (!CheckSkillCoolTimeIsEnded() && !usingSkill) // 스킬 쿨타임이 끝나지 않은 경우
            {
                if (distance <= attackDist)
                {
                    state = State.ATTACK;
                }
                else if (distance <= traceDist)
                {
                    state = State.TRACE;
                }
            }
            else // 스킬 쿨타임이 끝난 경우
            {
                if (CheckIfSkillIsUsing()) // 스킬이 사용중인 경우
                    continue;

                // 처음 스킬 사용하는 경우
                usingSkill = true;

                //Debug.Log((playerTr.position - monsterTr.position).Normalize());
                rushPos = (playerTr.position - monsterTr.position).normalized * 45; // 정규화된 벡터 * 맵 최대 크기(원의 지름 이상) -> 즉 플레이어가 있던 방향의 맵 부분
                Debug.Log("돌진 위치 : " + (playerTr.position - monsterTr.position).normalized);
                state = State.SKILL;

                // 돌진시 플레이어를 통고하여 갈 것 인지 아니면 멈출 것인지 결정해보기
            }
        }
    }
}
