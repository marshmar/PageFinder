using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : Enemy
{
    public enum State
    {
        IDLE,
        MOVE,
        TRACE,
        ATTACK,
        DIE
    }


    // 에너미의 현재 상태
    public State state = State.IDLE;
    // 추적 사정거리
    public float traceDist = 10.0f;
    // 공격 사정거리
    protected float attackDist = 4.0f;
    // 인지 사정거리
    public float cognitiveDist = 10.0f;

    public Vector3[] posToMove = { Vector3.zero, Vector3.zero };
    protected int currentPosIndexToMove = 0;


    protected Transform monsterTr;
    private GameObject playerObj;
    protected Transform playerTr;
    private Player playerScr;
    private TokenManager tokenManager;
    protected NavMeshAgent agent;
    private Exp exp;
    private Palette palette;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        monsterTr = GetComponent<Transform>();

        GetPlayerScript();

        tokenManager = GameObject.Find("TokenManager").GetComponent<TokenManager>();
        agent = GetComponent<NavMeshAgent>();

        currentPosIndexToMove = 0;

        StartCoroutine(CheckEnemyState());
        StartCoroutine(EnemyAction());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        if(tokenManager != null)
            tokenManager.MakeToken(new Vector3(transform.position.x, 0.25f, transform.position.z));
        if (exp != null)
            exp.IncreaseExp(50);
    }

    // 플레이어 함수 가져오기
    public void GetPlayerScript()
    {
        playerObj = GameObject.FindWithTag("PLAYER");
        playerTr = playerObj.GetComponent<Transform>();
        playerScr = playerObj.GetComponent<Player>();
        exp = playerObj.GetComponent<Exp>();
        palette = playerObj.GetComponent<Palette>();
    }
    private void OnTriggerEnter(Collider coll)
    {
        Debug.Log(coll.name);
        if (coll.CompareTag("PLAYER"))
        {
            playerScr.HP -= atk;
            Debug.Log("PLAYER HP: " + playerScr.HP);
        }
        else if(coll.CompareTag("MAP") && moveType == 1) // 랜덤 이동시 맵에 닿았을 때 방향 다시 설정
        {
            Debug.Log("적과 맵이 닿음");
            SetCurrentPosIndexToMove();
            float distance = 0;
            // 이제 이동할 좌표를 랜덤하게 지정
            while (distance < cognitiveDist) // 이전 좌표와 인지 범위 내에서 새로 생성한 좌표의 거리가 최소 3이상 될 수 있게 설정
            {
                posToMove[currentPosIndexToMove] = new Vector3(originalPos.x + ReturnRandomValue(0, cognitiveDist - 1),
                                                            originalPos.y,
                                                            originalPos.z + ReturnRandomValue(0, cognitiveDist - 1));

                distance = Vector3.Distance(monsterTr.transform.position, posToMove[currentPosIndexToMove]);
            }
        }

        //meshRenderer.material.color = Color.magenta; //palette.ReturnCurrentColor();
        //state = State.DIE;
    }
    private void OnDrawGizmos()
    {
        if(state == State.TRACE)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, traceDist);
        }
        if (state == State.ATTACK)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackDist);
        }
    }
    IEnumerator CheckEnemyState()
    {
        while (!isDie)
        {
            //meshRenderer.material.color = Color.green;
            yield return new WaitForSeconds(0.3f);
            
            if (moveType == 0) // 경로 이동
                MovePath();
            else if (moveType == 1) // 랜덤 이동
                MoveRandom();
            else if (moveType == 2) // 추적 이동
                MoveTrace();
            else
                Debug.LogWarning(moveType);
        }

        // 동작 루틴
        /*
         *  1. 일정한 인지 범위 내에 이동 (경로 이동, 랜덤 이동, 추적 이동)
         *  2. 적 발견
         *  3. 추적
         *  4. 공격 (선공, 지속 선공, 회피, 수호)
         */ 
         
    }
    IEnumerator EnemyAction()
    {
        while (!isDie)
        {
            switch (state)
            {
                case State.IDLE:
                    //meshRenderer.material.color = Color.green;
                    //agent.SetDestination(playerTr.position);
                    //agent.isStopped = false;
                    //state = State.MOVE;
                    Debug.Log("Idle");
                    break;
                case State.MOVE:
                    //meshRenderer.material.color = Color.green;
                    agent.SetDestination(posToMove[currentPosIndexToMove]);
                    agent.isStopped = false;
                    break;
                case State.TRACE:
                    Debug.Log("Trace");
                    //meshRenderer.material.color = Color.blue;
                    agent.SetDestination(playerTr.position);
                    agent.isStopped = false;
                    break;
                case State.ATTACK:
                    Debug.Log("Attack");
                    //meshRenderer.material.color = Color.red;
                    break;
                case State.DIE:
                    Die();
                    break;
            }
            yield return new WaitForSeconds(0.3f);
        }
    }

    /// <summary>
    /// 경로 이동
    /// </summary>
    public virtual void MovePath()
    {
        float distance = Vector3.Distance(posToMove[currentPosIndexToMove], monsterTr.transform.position);

        state = State.MOVE;

        if (distance <= 1)
            SetCurrentPosIndexToMove();

        if (!CheckCognitiveDist())
        { 
            return;
        }

        
        distance = Vector3.Distance(playerTr.transform.position, monsterTr.transform.position);
        Debug.Log(distance);
        if (distance <= attackDist)
        {
            state = State.ATTACK;
            return;
        }
        else if (distance <= traceDist)
        {
            state = State.TRACE;
            return;
        }

    }

    /// <summary>
    /// 랜덤 이동
    /// </summary>
    public virtual void MoveRandom() 
    {
        float distance = Vector3.Distance(posToMove[currentPosIndexToMove], monsterTr.transform.position);

        state = State.MOVE;

        if (distance <= 1.5f)
        {
            SetCurrentPosIndexToMove();

            // 이제 이동할 좌표를 랜덤하게 지정
            while (distance < cognitiveDist || agent.pathPending) // 이전 좌표와 인지 범위 내에서 새로 생성한 좌표의 거리가 최소 3이상 될 수 있게 설정
            {
                posToMove[currentPosIndexToMove] = new Vector3(originalPos.x + ReturnRandomValue(0, cognitiveDist - 1),
                                                            originalPos.y,
                                                            originalPos.z + ReturnRandomValue(0, cognitiveDist - 1));

                distance = Vector3.Distance(monsterTr.transform.position, posToMove[currentPosIndexToMove]);
               
            }
        }

        Debug.Log(CheckCognitiveDist());
        Debug.Log(attackType);
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

    public void MoveTrace()
    {
        float distance = Vector3.Distance(playerTr.transform.position, monsterTr.transform.position);
        if (distance <= attackDist)
        {
            state = State.ATTACK;
        }
        else if (traceDist > 0) // 계속 추적하도록 설정
        {
            state = State.TRACE;
        }
        else
        {
            state = State.IDLE;
        }
    }

    protected bool CheckCognitiveDist()
    {
        float distance = Vector3.Distance(originalPos, playerTr.transform.position);

        if (attackType == 0) // 인지 범위 내에서만 공격
        {
            //Debug.Log(distance);
            if (distance <= cognitiveDist)
                return true;
            else
                return false;
        }
        else if(attackType == 1) // 인지 범위 바깥까지 공격
        {
            return true;
        }
        else
        {
            Debug.LogWarning(attackType);
            return false;
        }
    }

    protected void SetCurrentPosIndexToMove()
    {
           if (currentPosIndexToMove >= posToMove.Length - 1) // 최대 인덱스 값에 도달하기 전에 0으로 다시 리셋되도록 설정
                currentPosIndexToMove = 0;
            else
                currentPosIndexToMove++;
    }

    protected float ReturnRandomValue(float min, float max)
    {
        if(Random.Range(0,2) == 0)
            return -Random.Range(min, max);
        else 
            return Random.Range(min, max);
    }

}
