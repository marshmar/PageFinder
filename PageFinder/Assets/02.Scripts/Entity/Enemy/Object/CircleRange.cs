using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VectorGraphics;
using Unity.VisualScripting;
using UnityEngine;

public class CircleRange : MonoBehaviour
{
    GameObject circleToGrowInSize;
    GameObject targetCircle;

    Enemy.DebuffState debuffState;
    float targetDiamter;
    float debuffTime;
    float damage;
    float moveDist;
    int skillIndex;

    bool isBoss;

    [SerializeField]
    private Transform subjectPos;

    // player�� ���ҿ� ���� �з��Կ� ���� player->PlayerSate�� ����
    PlayerState playerState;
    MediumBossEnemy mediumBossEnemy; // ���� ������ �߰����� �̻󿡼��� ���� ��ų�� ����ϱ� ������ MediumBossEnemy�� ���� 
    EnemyAction enemyAction;

    private void Awake()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("PLAYER");
        playerState = DebugUtils.GetComponentWithErrorLogging<PlayerState>(playerObj, "PlayerState");
    }

    private void Start()
    {
        circleToGrowInSize = transform.GetChild(0).gameObject;
        targetCircle = transform.GetChild(1).gameObject;
        gameObject.SetActive(false);
    }

   /// <summary>
   /// ���� üũ ����
   /// </summary>
   /// <param name="skillIndex">��ų �ε���</param>
   /// <param name="stateEffectName">������ ���� ȿ��</param>
   /// <param name="subjectName">ȣ���� ��ü �̸�</param>
   /// <param name="targetDiamter">��ǥ ����</param>
   /// <param name="speed">���� ä������ �ӵ�</param>
   /// <param name="debuffTime">����ȿ�� ����ð�</param>
   /// <param name="damage">���� ������</param>
   /// <param name="moveDist">Ž�� �Ÿ�</param>
    public void StartRangeCheck(int skillIndex, Enemy.DebuffState debuffState, float targetCircleRadius, float debuffTime, float damage, float moveDist = 0, bool isBoss = false)
    {        
        this.skillIndex = skillIndex;
        this.debuffState = debuffState;
        this.targetDiamter = targetCircleRadius * 2;
        this.debuffTime = debuffTime;
        this.damage = damage;
        this.moveDist = moveDist;
        this.isBoss = isBoss;

        targetCircle.transform.localScale = Vector3.one * this.targetDiamter; 
        circleToGrowInSize.transform.localScale = Vector3.one;

        gameObject.SetActive(true);

        if(isBoss)
            mediumBossEnemy = DebugUtils.GetComponentWithErrorLogging<MediumBossEnemy>(transform.parent.gameObject, "MediumBossEnemy");
        else
            enemyAction = DebugUtils.GetComponentWithErrorLogging<EnemyAction>(transform.parent.gameObject, "EnemyAction");

        StartCoroutine(GrowSizeOfCircle());
    }

    /// <summary>
    /// ��ǥ�� ũ����� ���� ũ�⸦ ������Ų��.
    /// </summary>
    /// <returns></returns>
    private IEnumerator GrowSizeOfCircle()
    {
        while (circleToGrowInSize.transform.localScale.x < targetDiamter)
        {
            // Time.deltaTime * targetDiamter/2 : 1�ʴ� �� ���� ���� Ŀ������ ��
            // ���Ѵٸ� speed �߰��ؼ� ������ ���� ����
            circleToGrowInSize.transform.localScale = Vector3.MoveTowards(circleToGrowInSize.transform.localScale, Vector3.one * targetDiamter, Time.deltaTime);
            yield return null;
        }

        yield return new WaitForSeconds(0.3f);

        CheckObjectsInRange();
        
        gameObject.SetActive(false);
    }


    /// <summary>
    /// ���� �ȿ� �ִ� ������Ʈ�� üũ�Ѵ�.
    /// </summary>
    void CheckObjectsInRange()
    {
        Collider[] hits = Physics.OverlapSphere(subjectPos.position, this.targetDiamter/2, LayerMask.GetMask("ENEMY", "PLAYER"));

        for (int i = 0; i < hits.Length; i++)
        {
            // ���� ������ ������ �ڽ��� ���� ����
            if (hits[i].name.Equals(transform.parent.name))
                continue;

            // ��
            if (hits[i].CompareTag("ENEMY"))
            {
                EnemyAction hitEnemyScr = DebugUtils.GetComponentWithErrorLogging<EnemyAction>(hits[i].gameObject, "Enemy");

                if (debuffState == Enemy.DebuffState.KNOCKBACK)
                {
                    Vector3 dir = (hits[i].transform.position - subjectPos.position).normalized;
                    dir.y = 0;
                    hitEnemyScr.Hit(InkType.RED, damage, debuffState, debuffTime, dir);
                }
                else
                continue;
            }

            // �÷��̾�
            playerState.CurHp -= damage;
            Debug.Log($"�����ȿ� �÷��̾ ���Խ��ϴ�. {playerState.CurHp}");
            // �÷��̾� ȿ�� ���� �Լ��� ���߿� ȣ���ϱ�
        }

        if (isBoss)
            mediumBossEnemy.SkillEnd();
        else
            enemyAction.HP -= enemyAction.HP;
    }
}
