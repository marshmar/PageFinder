using Google.GData.AccessControl;
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

    int jiruruCnt = 4;

    [SerializeField]
    private GameObject teleportEffectObj;

    bool controlEffect;

    public override float HP
    {
        get
        {
            return currHP; //+ currShield;  // 100 + 50   - 55
        }
        set
        {
            // 감소시켜도 쉴드가 남아있는 경우
            //if (value > currHP)
            //{
            //    CurrShield = value - currHP;
            //}
            //else // 감소시켜도 쉴드가 남아있지 않은 경우
            //{
            //    CurrShield = 0;
            //    currHP = value;
            //}

            currHP = value;

            Hit();
            hpBar.SetCurrValueUI(currHP);
            if (currHP <= 0)
            {
                if (gameObject.name.Contains("Jiruru"))
                    playerScr.Coin += 50;
                else if (gameObject.name.Contains("Bansha"))
                    playerScr.Coin += 100;
                else
                    playerScr.Coin += 250;
                isDie = true;
                // <해야할 처리>
                EnemyManager.Instance.DestroyEnemy("enemy", gameObject);
                //Debug.Log("적 비활성화");
                // 플레이어 경험치 획득
                // 토큰 생성 
                //Die();
            }

        }
    }

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

        teleportEffectObj.SetActive(false);
        controlEffect = false;

        StartCoroutine(Updater());
        StartCoroutine(Animation());
    }

    protected override void SkillAni()
    {
        if (currSkillName.Equals(""))
            return;

        for (int i = 0; i < skillNames.Length; i++)
        {
            if (!currSkillName.Equals(skillNames[i]))
                continue;

            if(i==0)
            {
                if (!controlEffect)
                {
                    controlEffect = true;
                    StartCoroutine(StartTeleportEffect(skillNames[i]));
                }
            }
            else
                 SetAniVariableValue(skillNames[i]); // 스킬 인덱스에 따라 string값 변경하도록 수정하기
            //Debug.Log(skillNames[i] + "애니메이션 활성화");
            break;
        }
    }

    IEnumerator StartTeleportEffect(string skillName)
    {
        teleportEffectObj.SetActive(true);
        yield return new WaitForSeconds(0.9f);
        teleportEffectObj.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        SetAniVariableValue(skillName);

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
        //Debug.Log("ReinforcementAttack");
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
        float distance = Vector3.Distance(new Vector3(enemyTr.position.x, playerObj.transform.position.y, enemyTr.position.z), playerObj.transform.position);

        // 도망 거리에 도달했을 때 + 스킬 조건이 활성화되지 않았을 때
        if (distance <= teleportFugitiveDist && !skillCondition[0])
        {
            //Debug.Log($"텔레포트 조건 만족 : {distance}     {teleportFugitiveDist}");
            skillCondition[0] = true;
        }
    }

    private void CheckDimensionalConnection()
    {
        if (firstRunAboutSkill2)
            return;

        if (currHP < maxHP * 0.4 && !skillCondition[2])
        {
            firstRunAboutSkill2 = true;
            skillCondition[2] = true;
            //Debug.Log("Hp 40% 미만");
        }
    }

    /// <summary>
    /// Skill Teleport 애니메이션시 시전 동작 마치고 호출 
    /// </summary>
    private void Teleport()
    {
        Vector3 teleportPos = transform.position + (enemyTr.position - playerObj.transform.position).normalized * teleportRunDist;
        teleportPos.y = transform.position.y;

        enemyTr.position = teleportPos;
        //Debug.Log("TelePort");
    }


    private void FolderGeist()
    {
        float damage = atk * (200 / defaultAtkPercent); //atk * (450 / defaultAtkPercent)
        
        CircleRangeScr.StartRangeCheck("KnockBack", gameObject.name, 5, 3, 1, damage, 1);
        //Debug.Log("FolderGeist");
    }

    private void DimensionalConnection()
    {
        //MaxShield = maxHP * 0.2f;
        //Debug.Log("DimensionalConnection");
    }


    private void Skill0AniEnd()
    {
        SkillAniEnd();

        // 다음 공격이 강화 공격이 되도록 한다.
        currDefaultAtkCnt = maxDefaultAtkCnt;
        controlEffect = false;
    }

    private void Skill2AniEnd()
    {
        for (int i = 0; i < jiruruCnt; i++)
            EnemyManager.Instance.ActivateEnemy("Jiruru");
        //SetStateEffect("Stun", 3, Vector3.zero);
        SkillAniEnd();
    }
}
