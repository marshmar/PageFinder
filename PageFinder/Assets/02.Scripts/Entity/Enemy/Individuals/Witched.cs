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

    private bool firstRunAboutSkill2;

    [SerializeField]
    private GameObject teleportEffectObj;

    // 강화 공격
    [SerializeField]
    private GameObject bulletPrefab;

    //폴더가이스트 스킬 사용중 여부
    bool isSkill1InUse;

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

            if (currHP <= 0)
            {
                if (gameObject.name.Contains("Jiruru"))
                    playerState.Coin += 50;
                else if (gameObject.name.Contains("Bansha"))
                    playerState.Coin += 100;
                else
                    playerState.Coin += 250;

                // <해야할 처리>
                EnemyPooler.Instance.ReleaseEnemy(enemyType, gameObject);
                //Debug.Log("적 비활성화");
                // 플레이어 경험치 획득
                // 토큰 생성 
                //Die();
            }

        }
    }

    protected override void InitStat()
    {
        base.InitStat();

        skillConditions[1] = true;
        firstRunAboutSkill2 = false;

        teleportEffectObj.SetActive(false);
        isSkill1InUse = false;
    }

    protected override void BasicAttack()
    {
        SetBullet(bulletPrefab, 0, atk);
    }

    /// <summary>
    /// 강화 공격 애니메이션 동작 중 강화 공격 시작시 호출하는 함수
    /// </summary>
    protected override void ReinforcementAttack()
    {
        int[] angles = { -60, 0, 60 };

        foreach (int angle in angles)
            SetBullet(bulletPrefab, angle, atk);
    }

    protected override void CheckSkillsCondition()
    {
        CheckTeleportCondition();
        CheckDimensionalConnection();
    }

    private void CheckTeleportCondition()
    {
        float distance = Vector3.Distance(new Vector3(enemyTr.position.x, playerObj.transform.position.y, enemyTr.position.z), playerObj.transform.position);

        // 도망 거리에 도달했을 때 + 스킬 조건이 활성화되지 않았을 때
        if (distance <= teleportFugitiveDist && !skillConditions[0])
        {
            //Debug.Log($"텔레포트 조건 만족 : {distance}     {teleportFugitiveDist}");
            skillConditions[0] = true;
        }
    }

    private void CheckDimensionalConnection()
    {
        if (firstRunAboutSkill2)
            return;

        if (currHP < maxHP * 0.4 && !skillConditions[2])
        {
            firstRunAboutSkill2 = true;
            skillConditions[2] = true;
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
    }

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


    private void FolderGeist()
    {
        if (isSkill1InUse)
            return;

        isSkill1InUse = true;
        float damage = atk * 2; //atk * (450 / defaultAtkPercent)
        Debug.Log(damage);
        CircleRangeScr.StartRangeCheck(1, Enemy.DebuffState.STUN, 5, 2, damage, 1);
    }

    public void DimensionalConnection()
    {
        //MaxShield = maxHP * 0.2f;
        //Debug.Log("DimensionalConnection");
    }

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
                GameData.Instance.CurrWaveNum += 1;
                // 2웨이브로 넘어가면서 지루루 4마리 소환
                break;

            default:
                break;
        }

        base.SkillEnd();
    }
}
