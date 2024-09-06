using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Witched : MediumBossEnemy
{
    [SerializeField]
    private GameObject[] ReinforcenmentAttackTarget = new GameObject[3];
    private GameObject[] ReinforcementAttackObjects = new GameObject[3];
    [SerializeField]
    private Transform reinforcementAttackPos;

    [Header("Skill - Teleport")]
    [SerializeField]
    private float teleportRunDist;
    [SerializeField]
    private float teleportFugitiveDist;

    public override void Start()
    {
        // base.Start에서 해당 코루틴들을 미리 돌리지 않도록 설정.
        isUpdaterCoroutineWorking = true;
        isAnimationCoroutineWorking = true;

        base.Start();

        //Vector3 posTomove = Vector3.zero;
        // 강화 공격 투사체
        for (int i = 0; i < ReinforcementAttackObjects.Length; i++)
        {
            ReinforcementAttackObjects[i] = Instantiate(ReinforcementAttack_Prefab, new Vector3(enemyTr.position.x, -10, enemyTr.position.z), Quaternion.identity, GameObject.Find("Projectiles").transform);
            ReinforcementAttackObjects[i].GetComponent<Projectile>().Init(gameObject.name, "- ReinforcementAttackAction" + i, 10, reinforcementAttackPos, ReinforcenmentAttackTarget[i]); // 60도 3갈래
        }

        StartCoroutine(Updater());
        StartCoroutine(Animation());
    }

    /// <summary>
    /// 강화 공격 애니메이션 동작 중 강화 공격 시작시 호출하는 함수
    /// </summary>
    protected override void ReinforcementAttack()
    {
        // 강화 기본 공격은 기본 공격 쿨타임이 돌았을 때 + 기본 공격 횟수가 4번째일 때 실행한다.

        // 강화 기본 공격 활성화
        for (int i = 0; i < ReinforcementAttackObjects.Length; i++)
        {
            ReinforcementAttackObjects[i].SetActive(true);
            ReinforcementAttackObjects[i].GetComponent<Projectile>().SetDirToMove();
        }
    }

    protected override void CheckSkillsCondition()
    {
        CheckTeleportCondition();
    }

    private void CheckTeleportCondition()
    {
        float distance = Vector3.Distance(enemyTr.position, playerObj.transform.position);

        // 도망 거리에 도달했을 때 + 스킬 조건이 활성화되지 않았을 때
        if (distance <= teleportFugitiveDist && !skillCondition[0])
            skillCondition[0] = true;
    }

    /// <summary>
    /// Skill Teleport 애니메이션시 시전 동작 마치고 호출 
    /// </summary>
    private void Teleport()
    {
        Vector3 teleportPos = playerObj.transform.position + (enemyTr.position - playerObj.transform.position).normalized * teleportRunDist;

        enemyTr.position = teleportPos;
        Debug.Log("TelePort");
    }

    private void Skill0AniEnd()
    {
        SkillAniEnd();

        // 다음 공격이 강화 공격이 되도록 한다.
        currDefaultAtkCnt = maxDefaultAtkCnt;
    }
}
