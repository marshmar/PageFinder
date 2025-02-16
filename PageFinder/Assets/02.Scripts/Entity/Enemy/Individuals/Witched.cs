using Google.GData.AccessControl;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Witched : MediumBossEnemy
{
    [Header("Skill - Teleport")]
    [SerializeField]
    private float teleportRunDist;
    [SerializeField]
    private float teleportFugitiveDist;

    [Header("Circle Range")]
    [SerializeField]
    private CircleRange CircleRangeScr;

    private bool firstRunAboutSkill1;

    [SerializeField]
    private GameObject teleportEffectObj;

    // 강화 공격
    [SerializeField]
    private GameObject bulletPrefab;

    //폴더가이스트 스킬 사용중 여부
    bool isSkill1InUse;

    public override void InitStatValue()
    {
        base.InitStatValue();

        firstRunAboutSkill1 = false;

        teleportEffectObj.SetActive(false);
        isSkill1InUse = false;
    }

    protected override void BasicAttack()
    {
        SetBullet(bulletPrefab, 0, atk, 7);
    }

    /// <summary>
    /// 강화 공격 애니메이션 동작 중 강화 공격 시작시 호출하는 함수
    /// </summary>
    protected override void ReinforcementAttack()
    {
        int[] angles = { -60, 0, 60 };

        foreach (int angle in angles)
            SetBullet(bulletPrefab, angle, atk * 2, 10);
    }

    protected override void CheckSkillsCondition()
    {
        CheckSkill0Condition();
        CheckSill1Condition();
    }

    #region 스킬 체크

    private void CheckSkill0Condition()
    {
        if (currHP < maxHP * 0.75f)
            skillConditions[0] = true;
    }

    //private void CheckSkill1Condition()
    //{
    //    float distance = Vector3.Distance(new Vector3(enemyTr.position.x, playerObj.transform.position.y, enemyTr.position.z), playerObj.transform.position);

    //    // 도망 거리에 도달했을 때 + 스킬 조건이 활성화되지 않았을 때
    //    if (distance <= teleportFugitiveDist && !skillConditions[0])
    //    {
    //        //Debug.Log($"텔레포트 조건 만족 : {distance}     {teleportFugitiveDist}");
    //        skillConditions[0] = true;
    //    }
    //}

    private void CheckSill1Condition()
    {
        if (firstRunAboutSkill1)
            return;

        if (currHP <= maxHP * 0.2f && !skillConditions[1])
        {
            firstRunAboutSkill1 = true;
            skillConditions[1] = true;
        }
    }

    #endregion

    #region 스킬

    // 쉴드
    private void Skill0()
    {
        EventManager.Instance.PostNotification(EVENT_TYPE.Generate_Shield_Enemy, this, 
            new System.Tuple<System.Tuple<float, float>, GameObject>
            (new System.Tuple<float, float>(maxHP * 0.05f, 10f), this.gameObject));
        StartCoroutine(ChangeInkTypeResistance(10, 30));
    }


    // 지루루 소환
    private void Skill1()
    {
        List<EnemyData> jiruruData = new List<EnemyData>();
        float dist = 3;
        // Witched 기준으로 추격형 지루루 4마리 소환
        // 좌상
        Vector3 pos = patrolDestinations[0] + new Vector3(-1, 0, 1) * dist;
        pos.y = 2;
        jiruruData.Add(EnemySetter.Instance.SetEnemyData(EnemyType.Chaser_Jiruru, new List<Vector3> { pos }));

        // 우상
        pos = patrolDestinations[0] + new Vector3(1, 0, 1) * dist;
        pos.y = 2;
        jiruruData.Add(EnemySetter.Instance.SetEnemyData(EnemyType.Chaser_Jiruru, new List<Vector3> { pos }));

        // 좌하
        pos = patrolDestinations[0] + new Vector3(-1, 0, -1) * dist;
        pos.y = 2;
        jiruruData.Add(EnemySetter.Instance.SetEnemyData(EnemyType.Chaser_Jiruru, new List<Vector3> { pos }));

        //우하
        pos = patrolDestinations[0] + new Vector3(1, 0, -1) * dist;
        pos.y = 2;
        jiruruData.Add(EnemySetter.Instance.SetEnemyData(EnemyType.Chaser_Jiruru, new List<Vector3> { pos }));

        EnemySetter.Instance.SpawnEnemys(jiruruData);
    }

    /// <summary>
    /// Skill Teleport 애니메이션시 시전 동작 마치고 호출 
    /// </summary>
    private void Teleport()
    {
        Vector3 teleportPos = transform.position + (enemyTr.position - playerObj.transform.position).normalized * teleportRunDist;
        teleportPos.y = transform.position.y;

        enemyTr.position = teleportPos;
    }
    private void FolderGeist()
    {
        if (isSkill1InUse)
            return;

        isSkill1InUse = true;
        float damage = atk * 2; //atk * (450 / defaultAtkPercent)

        CircleRangeScr.StartRangeCheck(1, Enemy.DebuffState.STUN, 5, 2, damage, 1, true);
    }

    #endregion

    #region 스킬 이펙트
    private void TeleportEffect()
    {
        StartCoroutine(StartTeleportEffect());
    }

    IEnumerator StartTeleportEffect()
    {
        teleportEffectObj.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        teleportEffectObj.SetActive(false);
    }

    #endregion




    public override void SkillEnd()
    {
        switch (currSkillNum)
        {
            // 텔레포트
            case 0:
                break;

            // 폴더 가이스트
            case 1:
                // 다음 공격이 강화 공격이 되도록 한다.
                currBasicAtkCnt = reinforcementAtkCnt;
                isSkill1InUse = false;
                break;

            // 차원 연결
            case 2:
                //GameData.Instance.CurrWaveNum += 1;
                // 2웨이브로 넘어가면서 지루루 4마리 소환
                break;

            default:
                break;
        }

        base.SkillEnd();
    }
}
