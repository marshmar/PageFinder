using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.Rendering.DebugUI;

public class Stingray : EnemyController
{
    public GameObject Bullet_Prefab;

    GameObject[] Bullet = new GameObject[3];

    int maxReloadTime = 3;
    float currentReloadTime = 0;

    private void Update()
    {
        SetReloadTime();
    }

    // Start is called before the first frame update
    public override void Start()
    {
        // 기본 능력치 세팅
        // 이속, 최대 체력, 현재 체력, 공격력, 방어력
        moveSpeed = 0.8f;
        maxHP = 40.0f;
        atk = 10.0f;

        // 타입 세팅
        posType = PosType.SKY; // 공중
        moveType = MoveType.RANDOM; // 랜덤 이동
        attackType = AttackType.PREEMPTIVE;

        traceDist = 10;
        attackDist = 7;
        //cognitiveDist = 5f;

        base.Start();

        // 총알 관련
        for(int i=0; i< Bullet.Length; i++)
        {
            Bullet[i] = Instantiate(Bullet_Prefab, new Vector3(monsterTr.position.x, -10, monsterTr.position.z), Quaternion.identity, GameObject.Find("Projectile").transform); //GameObject.Find("Bullet").transform
            Bullet[i].name = gameObject.name + " - Projectile" + i;
            Bullet[i].GetComponent<StingrayBullet>().ParentName = gameObject.name; 
            Bullet[i].SetActive(false);
        }
    }

    /// <summary>
    /// 랜덤 이동
    /// </summary>
    public override void MoveRandom()
    {
        
        float distance = Vector3.Distance(posToMove[currentPosIndexToMove], monsterTr.transform.position);

        state = State.MOVE;

        if (distance <= 1.5f)
        {
            SetCurrentPosIndexToMove();

            // 앞으로 이동할 좌표를 랜덤하게 지정
            while (distance < cognitiveDist || agent.pathPending) // 이전 좌표와 인지 범위 내에서 새로 생성한 좌표의 거리가 최소 3이상 될 수 있게 설정
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
    }

    protected override IEnumerator EnemyAction()
    {
        while (!isDie)
        {
            switch (state)
            {
                case State.IDLE:
                    //Debug.Log("Idle");
                    break;
                case State.MOVE:
                    agent.SetDestination(posToMove[currentPosIndexToMove]);
                    agent.stoppingDistance = 0;
                    agent.isStopped = false;
                    break;
                case State.TRACE:
                    //Debug.Log("Trace");
                    agent.SetDestination(playerTr.position);
                    agent.stoppingDistance = 0;
                    agent.isStopped = false;
                    break;
                case State.ATTACK:
                    //Debug.Log("Attack");
                    agent.SetDestination(playerTr.position);
                    agent.stoppingDistance = 5;
                    agent.isStopped = false;
                    FireBullet();
                    break;
                case State.DIE:
                    Die();
                    break;
            }
            yield return new WaitForSeconds(0.3f);
        }
    }

    void FireBullet()
    {
        if (currentReloadTime < maxReloadTime)
            return;

        int bulletIndex = FindBulletThatCanBeUsed();
        if (bulletIndex == -1) // 사용할 수 있는 총알이 없을 경우 
            return;

        Bullet[bulletIndex].SetActive(true);
        Bullet[bulletIndex].GetComponent<StingrayBullet>().Init();
        currentReloadTime = 0;
    }

    /// <summary>
    /// 사용할 수 있는 총알을 찾는다.
    /// </summary>
    /// <returns>-1 : 사용할 수 있는 총알 없음 / 0~Bullet.Length-1 : 사용할 수 있는 총알 인덱스</returns>
    int FindBulletThatCanBeUsed()
    {
        for(int i=0; i<Bullet.Length; i++)
        {
            if (Bullet[i].activeSelf) // 사용중인 총알 
                continue;
            return i;
        }
        return -1;
    }


    void SetReloadTime()
    {
        if (currentReloadTime >= maxReloadTime)
            return;

        currentReloadTime += Time.deltaTime;
    }

    // 투사체 발사 로직 

    /* <가오리 공격 루틴>
     * 1. 공격 사정거리 이내
     * 2. 투사체 1회 발사 (일직선) 
     * 3. 1초 후 2번으로 
     * 
     */
}
