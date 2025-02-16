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

    // ��ȭ ����
    [SerializeField]
    private GameObject bulletPrefab;

    //�������̽�Ʈ ��ų ����� ����
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
    /// ��ȭ ���� �ִϸ��̼� ���� �� ��ȭ ���� ���۽� ȣ���ϴ� �Լ�
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

    #region ��ų üũ

    private void CheckSkill0Condition()
    {
        if (currHP < maxHP * 0.75f)
            skillConditions[0] = true;
    }

    //private void CheckSkill1Condition()
    //{
    //    float distance = Vector3.Distance(new Vector3(enemyTr.position.x, playerObj.transform.position.y, enemyTr.position.z), playerObj.transform.position);

    //    // ���� �Ÿ��� �������� �� + ��ų ������ Ȱ��ȭ���� �ʾ��� ��
    //    if (distance <= teleportFugitiveDist && !skillConditions[0])
    //    {
    //        //Debug.Log($"�ڷ���Ʈ ���� ���� : {distance}     {teleportFugitiveDist}");
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

    #region ��ų

    // ����
    private void Skill0()
    {
        EventManager.Instance.PostNotification(EVENT_TYPE.Generate_Shield_Enemy, this, 
            new System.Tuple<System.Tuple<float, float>, GameObject>
            (new System.Tuple<float, float>(maxHP * 0.05f, 10f), this.gameObject));
        StartCoroutine(ChangeInkTypeResistance(10, 30));
    }


    // ����� ��ȯ
    private void Skill1()
    {
        List<EnemyData> jiruruData = new List<EnemyData>();
        float dist = 3;
        // Witched �������� �߰��� ����� 4���� ��ȯ
        // �»�
        Vector3 pos = patrolDestinations[0] + new Vector3(-1, 0, 1) * dist;
        pos.y = 2;
        jiruruData.Add(EnemySetter.Instance.SetEnemyData(EnemyType.Chaser_Jiruru, new List<Vector3> { pos }));

        // ���
        pos = patrolDestinations[0] + new Vector3(1, 0, 1) * dist;
        pos.y = 2;
        jiruruData.Add(EnemySetter.Instance.SetEnemyData(EnemyType.Chaser_Jiruru, new List<Vector3> { pos }));

        // ����
        pos = patrolDestinations[0] + new Vector3(-1, 0, -1) * dist;
        pos.y = 2;
        jiruruData.Add(EnemySetter.Instance.SetEnemyData(EnemyType.Chaser_Jiruru, new List<Vector3> { pos }));

        //����
        pos = patrolDestinations[0] + new Vector3(1, 0, -1) * dist;
        pos.y = 2;
        jiruruData.Add(EnemySetter.Instance.SetEnemyData(EnemyType.Chaser_Jiruru, new List<Vector3> { pos }));

        EnemySetter.Instance.SpawnEnemys(jiruruData);
    }

    /// <summary>
    /// Skill Teleport �ִϸ��̼ǽ� ���� ���� ��ġ�� ȣ�� 
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

    #region ��ų ����Ʈ
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
            // �ڷ���Ʈ
            case 0:
                break;

            // ���� ���̽�Ʈ
            case 1:
                // ���� ������ ��ȭ ������ �ǵ��� �Ѵ�.
                currBasicAtkCnt = reinforcementAtkCnt;
                isSkill1InUse = false;
                break;

            // ���� ����
            case 2:
                //GameData.Instance.CurrWaveNum += 1;
                // 2���̺�� �Ѿ�鼭 ����� 4���� ��ȯ
                break;

            default:
                break;
        }

        base.SkillEnd();
    }
}
