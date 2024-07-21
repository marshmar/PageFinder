using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.Rendering.DebugUI;

public class LongRangeAttackEnemy : EnemyController
{
    public GameObject Projectile_Prefab;

    //List 로 변경하여 개수 능동적으로 변경할 수 있게 해보기
    GameObject[] projectile = new GameObject[3];

    int projectileCnt = 5;
    int maxReloadTime = 3;
    float currentReloadTime = 0;

    private void Update()
    {
        SetReloadTime();
    }

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        // 투사체 관련
        for (int i = 0; i < projectile.Length; i++)
        {
            projectile[i] = Instantiate(Projectile_Prefab, new Vector3(monsterTr.position.x, -10, monsterTr.position.z), Quaternion.identity, GameObject.Find("Projectile").transform); //GameObject.Find("Bullet").transform
            projectile[i].name = gameObject.name + " - Projectile" + i;
            projectile[i].GetComponent<StingrayBullet>().ParentName = gameObject.name;
            projectile[i].SetActive(false);
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
                    FireProjectileObject();
                    break;
                case State.DIE:
                    Die();
                    break;
            }
            yield return new WaitForSeconds(0.3f);
        }
    }

    void FireProjectileObject()
    {
        if (currentReloadTime < maxReloadTime)
            return;

        int projectileIndex = FindBulletThatCanBeUsed();
        if (projectileIndex == -1) // 사용할 수 있는 총알이 없을 경우 
            return;

        projectile[projectileIndex].SetActive(true);
        projectile[projectileIndex].GetComponent<StingrayBullet>().Init();
        currentReloadTime = 0;
    }

    /// <summary>
    /// 사용할 수 있는 투사체를 찾는다.
    /// </summary>
    /// <returns>-1 : 사용할 수 있는 투사체 없음 / 0~Bullet.Length-1 : 사용할 수 있는 투사체 인덱스</returns>
    int FindBulletThatCanBeUsed()
    {
        for (int i = 0; i < projectile.Length; i++)
        {
            if (projectile[i].activeSelf) // 사용중인 총알 
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
