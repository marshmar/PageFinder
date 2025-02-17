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

    // player를 역할에 따라서 분류함에 따라 player->PlayerSate로 변경
    PlayerState playerState;
    MediumBossEnemy mediumBossEnemy; // 적의 종류가 중간보스 이상에서만 광역 스킬을 사용하기 때문에 MediumBossEnemy로 세팅 
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
   /// 범위 체크 시작
   /// </summary>
   /// <param name="skillIndex">스킬 인덱스</param>
   /// <param name="stateEffectName">적용할 상태 효과</param>
   /// <param name="subjectName">호출한 객체 이름</param>
   /// <param name="targetDiamter">목표 범위</param>
   /// <param name="speed">원이 채워지는 속도</param>
   /// <param name="debuffTime">상태효과 적용시간</param>
   /// <param name="damage">가할 데미지</param>
   /// <param name="moveDist">탐색 거리</param>
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
    /// 목표한 크기까지 원의 크기를 증가시킨다.
    /// </summary>
    /// <returns></returns>
    private IEnumerator GrowSizeOfCircle()
    {
        while (circleToGrowInSize.transform.localScale.x < targetDiamter)
        {
            // Time.deltaTime * targetDiamter/2 : 1초당 한 블럭씩 원이 커지도록 함
            // 원한다면 speed 추가해서 빠르게 증가 가능
            circleToGrowInSize.transform.localScale = Vector3.MoveTowards(circleToGrowInSize.transform.localScale, Vector3.one * targetDiamter, Time.deltaTime);
            yield return null;
        }

        yield return new WaitForSeconds(0.3f);

        CheckObjectsInRange();
        
        gameObject.SetActive(false);
    }


    /// <summary>
    /// 범위 안에 있는 오브젝트를 체크한다.
    /// </summary>
    void CheckObjectsInRange()
    {
        Collider[] hits = Physics.OverlapSphere(subjectPos.position, this.targetDiamter/2, LayerMask.GetMask("ENEMY", "PLAYER"));

        for (int i = 0; i < hits.Length; i++)
        {
            // 감지 범위를 시전한 자신인 경우는 제외
            if (hits[i].name.Equals(transform.parent.name))
                continue;

            // 적
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

            // 플레이어
            playerState.CurHp -= damage;
            Debug.Log($"범위안에 플레이어가 들어왔습니다. {playerState.CurHp}");
            // 플레이어 효과 적용 함수도 나중에 호출하기
        }

        if (isBoss)
            mediumBossEnemy.SkillEnd();
        else
            enemyAction.HP -= enemyAction.HP;
    }
}
