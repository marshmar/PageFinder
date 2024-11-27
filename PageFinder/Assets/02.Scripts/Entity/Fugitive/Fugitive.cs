using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Fugitive : Entity
{
    private enum State
    {
        MOVE,
        STUN
    }

    private enum MoveState
    {
        MOVETORALLY, // 랠리 포인트로 이동
        WALKAWAYFROMFUGITIVE, // 도망자로부터 도망
        RUNFROMPLAYER // 플레이어로부터 도망
    }

    private enum TargetPosType
    {
        RALLY,
        FUGITIVE,
        PLAYER
    }

    private State state = State.MOVE; // 에너미의 현재 상태
    private MoveState moveState = MoveState.MOVETORALLY; // 에너미의 이동 상태

    private float playerCognitiveDist = 5; // 플레이어 인지 거리
    private float fugitiveCognitiveDist = 3; // 도망자 인지 거리

    private NavMeshPath path;

    private Vector3 targetPos; // 이동할 좌표
    private bool[] canChangeTargetPos = { true, true, true};


    int currPosIndex;
    private float moveDistance = 5;

    private bool isDie = false;

    private Vector3 otherFugitivePos;

    Vector3[] rallyPoints;

    float currStunTime;

    protected Transform playerTr;
    protected NavMeshAgent agent;
    private RallyPoints rallyData;
    /*
     * <시작>
     * 1. EnemyMananger에서 랠리 포인트 받기 
     * 2. 해당 랠리 포인트들에서 랜덤으로 랠리포인트 하나 고르고 해당 좌표로 이동 좌표 초기화
     * 3. state == MOVETORALLY
     *      
     * <조건>
     * 1. 플레이어 인지 범위 내에 플레이어가 들어오면 플레이어->자신 방향으로 n만큼 떨어진 거리에 랜덤 좌표 이동
     * 2. 도망자 인지 범위 내에 도망자가 들어오면 도망자->자신 방향으로 n만큼 떨어진 거리에 랜덤 좌표로 이동
     * 3. 1,2에 해당하지 않는다면 랠리 포인트로 이동
     */

    public override float MAXHP
    {
        get
        {
            return maxHP;
        }
        set
        {
            maxHP = value;
            if (hpBar != null)
                hpBar.SetMaxValueUI(maxHP);
            HP = value;
        }
    }

    public override float HP
    {
        get
        {
            return currHP;
        }
        set
        {
            currHP = value; //def
            //Debug.Log("Hit!\n 남은 체력: " + currHP);
            // Tatget인 경우
            if (hpBar != null)
                hpBar.SetCurrValueUI(currHP);

            if (currHP <= 0)
            {
                EnemyManager.Instance.DestroyEnemy("fugitive", gameObject);
            }
        }
    }

    public float PlayerCognitiveDist
    {
        get
        {
            return playerCognitiveDist;
        }
        set
        {
            playerCognitiveDist = value;
        }
    }

    public float FugitiveCognitiveDist
    {
        get
        {
            return fugitiveCognitiveDist;
        }
        set
        {
            fugitiveCognitiveDist = value;
        }
    }

    public float MoveDistance
    {
        get
        {
            return moveDistance;
        }
        set
        {
            moveDistance = value;
        }
    }

    public override void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        path = new NavMeshPath();
        playerTr = GameObject.FindWithTag("PLAYER").transform;
        rallyData = GameObject.Find("RallyPoints").GetComponent<RallyPoints>();

        agent.speed = moveSpeed * 3.5f;

        //if (name.Contains("Target"))
        //{
        //    hpBar = GetComponentInChildren<SliderBar>();
        //    shieldBar = GetComponentInChildren<ShieldBar>();
        //    hpBar.SetMaxValueUI(maxHP);
        //    hpBar.SetCurrValueUI(currHP);
        //    MaxShield = 0;
        //}

        isDie = false;

        StartCoroutine(Updater());
    }

    protected IEnumerator Updater()
    {
        while (!isDie)
        {
            setCoolTime();

            switch (state)
            {
                case State.MOVE:
                    //SetMoveState();
                    MoveAction();
                    break;

                //case State.STUN:
                //    StunAction();
                //    break;

                default:
                    Debug.LogWarning(state);
                    break;
            }
            yield return null;
        }
    }


    private void MoveAction()
    {
        switch (moveState)
        {
            case MoveState.MOVETORALLY:
                //agent.isStopped = false;
                // 랜덤한 랠리 포인트 설정
                SetRandomRallyPoint();
                break;

            //case MoveState.WALKAWAYFROMFUGITIVE:
            //    agent.isStopped = false;
            //    // 다른 도망자 -> 적 방향벡터로 설정
            //    setTargetPos(1, otherFugitivePos);
            //    agent.destination = targetPos;
            //    break;

            //case MoveState.RUNFROMPLAYER:
            //    agent.isStopped = false;
            //    // 플레이어 -> 적 방향벡터로 설정
            //    setTargetPos(0, playerTr.position);
            //    agent.destination = targetPos;
            //    break;

            default:
                Debug.LogWarning(moveState);
                break;
        }
    }

    private void SetMoveState()
    {
        float dist = Vector3.Distance(playerTr.position, transform.position);
        
        //// 플레이어가 인지 범위안에 들어왔을 경우
        //if (dist < playerCognitiveDist)
        //{
        //    Debug.Log("플레이어가 인지 범위안에 들어왔습니다.");
        //    moveState = MoveState.RUNFROMPLAYER;
        //    return;
        //}

        //// 범위 내에 도망자가 있는 경우
        //if (checkIfOtherFugitiveIsInCoginitiveRange())
        //{
        //    Debug.Log("도망자 인지 범위안에 들어왔습니다.");
        //    moveState = MoveState.WALKAWAYFROMFUGITIVE;
        //    return;
        //}

        if (!canChangeTargetPos[0] || !canChangeTargetPos[1])
            return;

        // 랠리 포인트로 이동
        moveState = MoveState.MOVETORALLY;
    }

    private void SetRandomRallyPoint()
    {
        // 다 도달했을 경우
        if(!canChangeTargetPos[2])
        {
            if (Vector3.Distance(targetPos, transform.position) < 1f)
            {
                rallyData.SetUseState(currPosIndex, false);
                canChangeTargetPos[2] = true;
            }
            return;
        }

        
        canChangeTargetPos[2] = false;

        int randomRallyPointIndex = Random.Range(0, rallyPoints.Length); //rallyPoints.Length

        while (targetPos == rallyPoints[randomRallyPointIndex] && rallyData.CheckIfCanUseRallyPoint(randomRallyPointIndex))
        {
            targetPos = rallyPoints[Random.Range(0, rallyPoints.Length)];
        }

        targetPos = rallyPoints[randomRallyPointIndex];
        currPosIndex = randomRallyPointIndex;
        rallyData.SetUseState(randomRallyPointIndex, true);
        agent.destination = targetPos;
    }

    //private void setTargetPos(int index, Vector3 pos)
    //{
    //    float dist = Vector3.Distance(targetPos, transform.position);
    //    // 다 도달했을 경우
    //    if (!canChangeTargetPos[index])
    //    {
    //        if(dist < 1f)
    //        {
    //            canChangeTargetPos[index] = true;
    //            Debug.Log("목적지 도달!");
    //        }
                
    //        return;
    //    }

    //    canChangeTargetPos[index] = false;

    //    Vector3 dir = (transform.position - pos).normalized;
    //    Vector3 tmpPos = transform.position + new Vector3(dir.x, 0, dir.z) * moveDistance;

    //    agent.destination = tmpPos;
    //    NavMeshPathStatus status = agent.pathStatus;

    //    GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
    //    obj.transform.position = tmpPos;

    //    if (status == NavMeshPathStatus.PathComplete)
    //        Debug.Log("PathComplete");
    //    else if (status == NavMeshPathStatus.PathPartial)
    //        Debug.Log("PathPartial");
    //    else if (status == NavMeshPathStatus.PathInvalid)
    //        Debug.Log("PathInvalid");

    //    // 움직임 세팅
    //    // 움직일 수 있는 위치인 경우
    //    if (NavMesh.CalculatePath(transform.position, tmpPos, NavMesh.AllAreas, path)) //
    //    {
    //        targetPos = tmpPos;
    //        Debug.Log("플레이어 반대방향으로 설정 :" + targetPos);
    //    }
    //    else
    //    {
    //        targetPos = getNearestRallyPos(); // 가장 가까운 랠리 포인트로 설정
    //        Debug.Log("플레이어 반대방향으로 설정했는데 범위 바깥이라 가장 가까운 랠리 포인트로 설정" + targetPos);
    //    }
           
    //}

    private void StunAction()
    {
        agent.isStopped = true;
    }

    private void setCoolTime()
    {
        if (state != State.STUN)
            return;

        currStunTime -= Time.deltaTime;
        if (currStunTime < 0)
        {
            currStunTime = 0;
            state = State.MOVE;
        }
    }

    //bool checkIfOtherFugitiveIsInCoginitiveRange()
    //{
    //    Collider[] objs = Physics.OverlapSphere(transform.position, fugitiveCognitiveDist, LayerMask.GetMask("ENEMY"));
    //    float maxDist = 0;
    //    int maxDistObjIndex = -1;
    //    float dist = 0;

    //    for (int i = 0; i < objs.Length; i++)
    //    {
    //        dist = Vector3.Distance(transform.position, objs[i].transform.position);
    //        if (dist > maxDist)
    //        {
    //            maxDistObjIndex = i;
    //            maxDist = dist;
    //        }
    //    }

    //    // 범위 내에 다른 도망자들이 없는 경우
    //    if (maxDistObjIndex == -1)
    //    {
    //        otherFugitivePos = Vector3.zero;
    //        return false;
    //    }

    //    // 범위 내에 다른 도망자들이 있는 경우
    //    otherFugitivePos = objs[maxDistObjIndex].transform.position;
    //    return true;
    //}

    public void SetRallyPoints(Vector3[] pos)
    {
        rallyPoints = pos;
    }

    // 가장 가까운 랠리 포인트 얻기
    private Vector3 getNearestRallyPos()
    {
        float dist;
        float maxDist = 0;
        int rallyPointIndex = 0;

        for (int i = 0; i < rallyPoints.Length; i++)
        {
            dist = Vector3.Distance(transform.position, rallyPoints[i]);
            if (dist > maxDist)
            {
                rallyPointIndex = i;
                maxDist = dist;
            }
        }

        return rallyPoints[rallyPointIndex];
    }

    public void SetStatus(RiddlePage page, int index)
    {
        playerCognitiveDist = page.playerCognitiveDist[index];
        fugitiveCognitiveDist = page.fugitiveCognitiveDist[index];
        moveDistance = page.moveDistance[index];
        SetRallyPoints(page.rallyPoints);
        moveSpeed = page.moveSpeed[index];
        MAXHP = page.maxHp[index];
    }
}
