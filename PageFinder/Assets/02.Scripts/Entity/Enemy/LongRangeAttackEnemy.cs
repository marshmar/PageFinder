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

    int maxReloadTime = 3;
    float currentReloadTime = 0;

    public virtual void Update()
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
            projectile[i] = Instantiate(Projectile_Prefab, new Vector3(monsterTr.position.x, -10, monsterTr.position.z), Quaternion.identity, GameObject.Find("Projectiles").transform); //GameObject.Find("Bullet").transform
            projectile[i].name = gameObject.name + " - Projectile" + i;
            projectile[i].GetComponent<Projectile>().ParentName = gameObject.name;
            projectile[i].SetActive(false);
        }
    }

    protected override IEnumerator EnemyAction()
    {
        while (!isDie)
        {
            SetCurrentSkillCoolTime();
            ChangeCurrentStateToSkillState();

            switch (state)
            {
                case State.IDLE:
                    ani.SetBool("isIdle", true);
                    ani.SetBool("isMove", false);
                    ani.SetBool("isAttack", false);
                    ani.SetBool("isStun", false);
                    break;
                case State.MOVE:
                    ani.SetBool("isIdle", false);
                    ani.SetBool("isMove", true);
                    ani.SetBool("isAttack", false);
                    ani.SetBool("isStun", false);

                    agent.SetDestination(posToMove[currentPosIndexToMove]);
                    agent.stoppingDistance = 0;
                    agent.isStopped = false;
                    break;
                case State.TRACE:
                    ani.SetBool("isIdle", false);
                    ani.SetBool("isMove", true);
                    ani.SetBool("isAttack", false);
                    ani.SetBool("isStun", false);

                    agent.SetDestination(playerTr.position);
                    agent.stoppingDistance = attackDist;
                    agent.isStopped = false;
                    break;
                case State.ATTACK:
                    ani.SetBool("isIdle", false);
                    ani.SetBool("isMove", false);
                    ani.SetBool("isAttack", true);
                    ani.SetBool("isStun", false);

                    agent.SetDestination(playerTr.position);
                    agent.stoppingDistance = attackDist;
                    agent.isStopped = true;
                    FireProjectileObject();
                    break;
                case State.STUN:
                    ani.SetFloat("stunTime", stunTime);
                    ani.SetBool("isIdle", false);
                    ani.SetBool("isMove", false);
                    ani.SetBool("isAttack", false);
                    ani.SetBool("isStun", true);

                    agent.isStopped = true;
                    break;
                case State.SKILL:
                    // 해당 적 클래스에서 재정의하여 원하는 스킬을 호출한다. 
                    Debug.Log("Skill 사용");
                    break;
                case State.DIE:
                    Die();
                    break;
            }
            yield return null;
        }
    }

    protected void FireProjectileObject()
    {
        if (currentReloadTime < maxReloadTime)
            return;

        int projectileIndex = FindBulletThatCanBeUsed();
        if (projectileIndex == -1) // 사용할 수 있는 총알이 없을 경우 
            return;
        //Debug.Log("총알 발사");
        projectile[projectileIndex].SetActive(true);
        projectile[projectileIndex].GetComponent<Projectile>().Init();
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

    protected void SetReloadTime()
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
