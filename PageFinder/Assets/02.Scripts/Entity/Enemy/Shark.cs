using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shark : BossController
{
    Vector3 rushPos = Vector3.zero;


    /*
     * <능력치>
     * 이속 : 0.7
     * 최대체력 : 200
     * 공격력 : 10
     * 
     * <타입 세팅>
     * posType : x -> 0으로 자동세팅됨
     * moveType : x -> 0으로 자동세팅됨
     * attackType : x -> 0으로 자동세팅됨
     * 
     * <스킬 쿨타임>
     * 스킬 : 돌진 
     * 최대 스킬 쿨타임 : 10초
     * 
     * <동작 루틴>
     * 플레이어가 보스 구역에 들어오지 않을 경우 -> 움직이지 않음
     * 플레이어가 보수 구역에 들어왔을 경우 -> 동작 시작
     * 
     * 스킬 쿨타임이 되지 않은 경우 -> 플레이어를 계속 추적하여 공격
     * 스킬 쿨타임이 되었을 경우 -> 돌진 : 플레이어가 있던 방향으로 맵에 닿을때까지 1회 돌진한다. 돌진 후 다시 추적 이동
     * 
     *
     */


    // Start is called before the first frame update
    public override void Start()
    {
        // 기본 능력치 세팅
        // 이속, 최대 체력, 현재 체력, 공격력, 방어력
        moveSpeed = 0.7f;
        maxHP = 200.0f;
        atk = 10.0f;

        // 타입 세팅
        posType = 0;
        moveType = 2; // 추적 이동
        attackType = 0;

        // 위치 세팅
        mapCenterPos = new Vector3(-25, 1, -25);

        // 스킬 쿨타임
        maxSkillCoolTime = 5;

        base.Start();
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
                    //Debug.Log("Trace");
                    meshRenderer.material.color = Color.blue;
                    agent.SetDestination(playerTr.position);
                    agent.isStopped = false;
                    break;
                case State.ATTACK:
                    //Debug.Log("Attack");
                    meshRenderer.material.color = Color.red;
                    break;
                case State.SKILL:
                    //Debug.Log("SKILL");
                    //Debug.Log(rushPos);
                    meshRenderer.material.color = Color.yellow;
                    agent.SetDestination(playerTr.position);
                    agent.isStopped = false;

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
            yield return null;

            float distance = Vector3.Distance(playerTr.transform.position, mapCenterPos);

            // 플레이어가 보스 구역에 들어오지 않은 경우 움직이지 않도록 한다.
            if (distance > 50) // 맵 지름 : 50
            {
                Debug.Log("플레이어는 보스 구역에 있지 않다.");
                continue;
            }

            distance = Vector3.Distance(playerTr.transform.position, monsterTr.transform.position);

            if (!CheckSkillCoolTimeIsEnded()) // 스킬 쿨타임이 끝나지 않은 경우
            {
                //Debug.Log("스킬 쿨타임이 끝나지 않은 경우");
                state = State.TRACE;
                if (distance <= attackDist)
                {
                    state = State.ATTACK;
                }
            }
            else // 스킬 쿨타임이 끝난 경우
            {
                if (CheckIfSkillIsUsing()) // 스킬이 사용중인 경우
                {
                    if(CheckIfItCollWithMap()) // 벽에 닿은 경우
                    {
                        Debug.Log("벽에 닿음");
                        currentSkillCoolTime = 0;
                        usingSkill = false;
                        moveSpeed = 3.5f;
                        agent.speed = moveSpeed;
                        state = State.TRACE;
                    }
                    //Debug.Log("스킬 사용중");
                    continue;
                }
                    
                Debug.Log("스킬 사용 시작");
                // 처음 스킬 사용하는 경우
                usingSkill = true;
                moveSpeed = 15;
                agent.speed = moveSpeed;
                state = State.SKILL;
            }
        }
    }

    bool CheckIfItCollWithMap()
    {
        return Physics.Raycast(monsterTr.position, monsterTr.forward, 5, LayerMask.GetMask("MAP")) ? true : false;
    }
}
