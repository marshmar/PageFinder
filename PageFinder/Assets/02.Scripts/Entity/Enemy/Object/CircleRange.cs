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

    Enemy.AbnomralState abnormalState;
    float targetCircleSize;
    float abnormalTime;
    float damage;
    float moveDist;
    int skillIndex;

    [SerializeField]
    private Transform subjectPos;

    Player playerScr;
    MediumBossEnemy mediumBossEnemy; // 적의 종류가 중간보스 이상에서만 광역 스킬을 사용하기 때문에 MediumBossEnemy로 세팅 

    private void Awake()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("PLAYER");
        playerScr = DebugUtils.GetComponentWithErrorLogging<Player>(playerObj, "Player");
        mediumBossEnemy = DebugUtils.GetComponentWithErrorLogging<MediumBossEnemy>(transform.parent.gameObject, "MediumBossEnemy");
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
   /// <param name="targetCircleSize">목표 범위</param>
   /// <param name="speed">원이 채워지는 속도</param>
   /// <param name="abnormalTime">상태효과 적용시간</param>
   /// <param name="damage">가할 데미지</param>
   /// <param name="moveDist">탐색 거리</param>
    public void StartRangeCheck(int skillIndex, Enemy.AbnomralState abnormalState, float targetCircleSize, float abnormalTime, float damage, float moveDist = 0)
    {        
        this.skillIndex = skillIndex;
        this.abnormalState = abnormalState;
        this.targetCircleSize = targetCircleSize * 2;
        this.abnormalTime = abnormalTime;
        this.damage = damage;
        this.moveDist = moveDist;

        targetCircle.transform.localScale = Vector3.one * this.targetCircleSize; 
        circleToGrowInSize.transform.localScale = Vector3.one;

        gameObject.SetActive(true);

        StartCoroutine(GrowSizeOfCircle());
    }

    /// <summary>
    /// 목표한 크기까지 원의 크기를 증가시킨다.
    /// </summary>
    /// <returns></returns>
    private IEnumerator GrowSizeOfCircle()
    {
        while (circleToGrowInSize.transform.localScale.x < targetCircleSize)
        {
            // Time.deltaTime * targetCircleSize/2 : 1초당 한 블럭씩 원이 커지도록 함
            // 원한다면 speed 추가해서 빠르게 증가 가능
            circleToGrowInSize.transform.localScale = Vector3.MoveTowards(circleToGrowInSize.transform.localScale, Vector3.one * targetCircleSize, Time.deltaTime * targetCircleSize/2);
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
        Collider[] hits = Physics.OverlapSphere(subjectPos.position, 9, LayerMask.GetMask("ENEMY", "PLAYER"));

        for (int i = 0; i < hits.Length; i++)
        {
            // 감지 범위를 시전한 자신인 경우는 제외
            if (hits[i].name.Equals(transform.parent.name))
                continue;

            // 적
            if (hits[i].CompareTag("ENEMY"))
            {
                //Debug.Log(hits[i].name + stateEffectName);
                Enemy hitEnemyScr = DebugUtils.GetComponentWithErrorLogging<Enemy>(hits[i].gameObject, "Enemy");

                // 상태 효과 
                switch (abnormalState)
                {
                    case Enemy.AbnomralState.STUN:
                        hitEnemyScr.SetStateEffect(abnormalState, abnormalTime, Vector3.zero);
                        break;

                    case Enemy.AbnomralState.KNOCKBACK:
                        hitEnemyScr.SetStateEffect(abnormalState, abnormalTime, hits[i].transform.position + (hits[i].transform.position - subjectPos.position).normalized * moveDist);
                        break;

                    case Enemy.AbnomralState.BINDING:
                        hitEnemyScr.SetStateEffect(abnormalState, abnormalTime, Vector3.zero);
                        break;

                    case Enemy.AbnomralState.AIR:
                        hitEnemyScr.SetStateEffect(abnormalState, abnormalTime, hits[i].transform.position + Vector3.up * moveDist);
                        break;

                    default:
                        //hitEnemyScr.SetStateEffect(Enemy.AbnomralState.NONE, abnormalTime, Vector3.zero);
                        break;
                }

                hitEnemyScr.HP -= damage;
                continue;
            }

            // 플레이어
            playerScr.HP -= damage;
            // 플레이어 효과 적용 함수도 나중에 호출하기
        }

        InitSkillAni();
    }

    /// <summary>
    /// 스킬 애니메이션을 초기화한다.
    /// </summary>
    void InitSkillAni()
    {
        // 광역 공격이 끝나고 애니메이션 세팅 초기화
        switch (skillIndex)
        {
            case 0:
                mediumBossEnemy.Skill0AniEnd();
                break;

            case 1:
                mediumBossEnemy.Skill1AniEnd();
                break;

            case 2:
                mediumBossEnemy.Skill2AniEnd();
                break;
        }
    }
}
