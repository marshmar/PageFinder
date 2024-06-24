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
    protected float maxSkillCoolTime = 1;
    protected bool usingSkill = false;

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

        currentSkillCoolTime = 10;
        maxSkillCoolTime = 10;
        usingSkill = false;


        StartCoroutine(CheckEnemyState());
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
    private void OnTriggerEnter(Collider coll)
    {
        if (coll.CompareTag("PLAYER"))
        {
            playerScr.HP -= atk;
            Debug.Log("PLAYER HP: " + playerScr.HP);
        }

        meshRenderer.material.color = Color.magenta; //palette.ReturnCurrentColor();
        //state = State.DIE;
    }

    protected virtual IEnumerator CheckEnemyState()
    {
        while (!isDie)
        {
            yield return new WaitForSeconds(0.3f);

            float distance = Vector3.Distance(playerTr.transform.position, monsterTr.transform.position);
            
            
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
