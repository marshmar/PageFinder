using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class BossController : Enemy
{
    public enum State
    {
        IDLE,
        MOVE,
        TRACE,
        ATTACK,
        SKILL,
        DIE
    }


    // 에너미의 현재 상태
    public State state = State.IDLE;
    // 추적 사정거리
    public float traceDist = 10.0f;
    // 공격 사정거리
    public float attackDist = 4.0f;

    // 스킬 쿨타임
    protected float currentSkillCoolTime = 0;
    protected float maxSkillCoolTime = -1;
    protected bool usingSkill = false;

    // 맵 중앙
    public Vector3 mapCenterPos = Vector3.zero;

    protected Transform monsterTr;
    private GameObject playerObj;
    protected Transform playerTr;
    protected Player playerScr;
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

        // 하위 클래스에서 초기화하지 않았을 경우 10으로 초기화
        if(maxSkillCoolTime == -1)
            maxSkillCoolTime = 10;

        currentSkillCoolTime = maxSkillCoolTime;
        usingSkill = false;


        StartCoroutine(CheckEnemyMoveState());
        StartCoroutine(EnemyAction());
    }

    // Update is called once per frame
    void Update()
    {
        SetSkillCoolTime();
    }

    private void OnDestroy()
    {
        if (tokenManager != null)
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
    protected virtual void OnTriggerEnter(Collider coll)
    {
        if (coll.CompareTag("PLAYER"))
        {
            playerScr.HP -= atk;
            Debug.Log("PLAYER HP: " + playerScr.HP);
        }

        //meshRenderer.material.color = Color.magenta; //palette.GetCurrentColor();
    }

    protected virtual IEnumerator CheckEnemyMoveState()
    {
        while (!isDie)
        {
            yield return new WaitForSeconds(0.3f);

            // 맵 중앙과 플레이어 위치 비교
            float distance = Vector3.Distance(playerTr.transform.position, mapCenterPos);

            // 플레이어가 보스 구역에 들어오지 않은 경우 움직이지 않도록 한다.
            if (distance > 50) // 맵 지름 : 50
                continue;

            distance = Vector3.Distance(playerTr.transform.position, monsterTr.transform.position);

            if (!CheckSkillCoolTimeIsEnded()) // 스킬 쿨타임이 끝나지 않은 경우
            {
                state = State.TRACE;
                if (distance <= attackDist)
                {
                    state = State.ATTACK;
                }
            }
            else // 스킬 쿨타임이 끝난 경우
            {
                if (CheckIfSkillIsUsing()) // 스킬이 사용중인 경우
                    continue;

                // 처음 스킬 사용하는 경우
                usingSkill = true;

                // 스킬이 끝나는 곳에 아래 코드 추가하기
                //currentSkillCoolTime = 0;

                state = State.SKILL;
            }
        }
    }


    protected virtual IEnumerator EnemyAction()
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
                    break;
                case State.DIE:
                    Die();
                    break;
            }
            yield return new WaitForSeconds(0.3f);
        }
    }

    void SetSkillCoolTime()
    {
        if (CheckIfSkillIsUsing())
            return;

        currentSkillCoolTime += Time.deltaTime;
        //Debug.Log(currentSkillCoolTime);
    }

    protected bool CheckSkillCoolTimeIsEnded()
    {
        return currentSkillCoolTime >= maxSkillCoolTime ? true : false;
    }

    protected bool CheckIfSkillIsUsing()
    {
        return usingSkill ? true : false;
    }

}
