using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    [Header("Circle Range")]
    [SerializeField]
    private CircleRange CircleRangeScr;

    private bool firstRunAboutSkill2;

    string[] jiruruNames = new string[4];

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

        skillCondition[1] = true;
        firstRunAboutSkill2 = false;

        jiruruNames[0] = EnemyManager.Instance.CreateEnemy(0, "Jiruru", transform.position + new Vector3(3, 0, 3), Vector3.zero);
        jiruruNames[1] = EnemyManager.Instance.CreateEnemy(0, "Jiruru", transform.position + new Vector3(3, 0, -3), Vector3.zero);
        jiruruNames[2] = EnemyManager.Instance.CreateEnemy(0, "Jiruru", transform.position + new Vector3(-3 , 0,  3), Vector3.zero);
        jiruruNames[3] = EnemyManager.Instance.CreateEnemy(0, "Jiruru", transform.position + new Vector3(-3, 0, -3), Vector3.zero);

        for (int i = 0; i < jiruruNames.Count(); i++)
            EnemyManager.Instance.DeactivateEnemy(jiruruNames[i]);

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
        CheckDimensionalConnection();

        if (stateEffect == StateEffect.BINDING || stateEffect == StateEffect.AIR)
        {
            for (int i = 0; i < skillCondition.Count; i++)
            {
                // 움직임 스킬인 경우
                if (moveSkillTypeData[i])
                {
                    skillCondition[i] = false;
                    break;
                }
            }
        }
    }

    private void CheckTeleportCondition()
    {
        float distance = Vector3.Distance(enemyTr.position, playerObj.transform.position);

        // 도망 거리에 도달했을 때 + 스킬 조건이 활성화되지 않았을 때
        if (distance <= teleportFugitiveDist && !skillCondition[0])
            skillCondition[0] = true;
    }

    private void CheckDimensionalConnection()
    {
        if (firstRunAboutSkill2)
            return;

        if (currHP < maxHP * 0.4 && !skillCondition[2])
        {
            firstRunAboutSkill2 = true;
            skillCondition[2] = true;
            Debug.Log("Hp 40% 미만");
        }
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

    private void FolderGeist()
    {
        float damage = 1; //atk * (450 / defaultAtkPercent)
        CircleRangeScr.StartRangeCheck("KnockBack", "Witched", 10, 5, 1, damage, 1);
    }

    private void DimensionalConnection()
    {
        MaxShield = maxHP * 0.2f;
    }


    private void Skill0AniEnd()
    {
        SkillAniEnd();

        // 다음 공격이 강화 공격이 되도록 한다.
        currDefaultAtkCnt = maxDefaultAtkCnt;
    }

    private void Skill2AniEnd()
    {
        // 의식 실패
        if (currShield <= 0)
        {
            SetStateEffect("Stun", 3, Vector3.zero);
            Debug.Log("의식 실패");
        }
            
        else // 의식 성공
        {
            Debug.Log("의식 성공");
            for(int i=0; i< jiruruNames.Count(); i++)
            {
                EnemyManager.Instance.ActivateEnemy("Jiruru");
            }
        }   
    }
}
