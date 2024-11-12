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

    string stateEffectName;
    string subjectName;
    float targetCircleSize;
    float speed;
    float abnormalTime;
    float damage;
    float moveDist;


    [SerializeField]
    private Transform subjectPos;

    Player playerScr;

    private void Start()
    {
        playerScr = GameObject.FindWithTag("PLAYER").GetComponent<Player>();

        circleToGrowInSize = transform.GetChild(0).gameObject;
        targetCircle = transform.GetChild(1).gameObject;
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 범위 체크를 시작한다.
    /// </summary>
    /// <param name="stateEffect"></param>
    /// <param name="subjectName"></param>
    /// <param name="targetCircleSize"></param>
    /// <param name="speed"></param>
    /// <param name="defaultCircleSize"></param>
    public void StartRangeCheck(string stateEffectName, string subjectName, float targetCircleSize, float speed, float abnormalTime, float damage, float moveDist = 0)
    {
        this.stateEffectName = stateEffectName;
        this.subjectName = subjectName;
        this.targetCircleSize = targetCircleSize;
        this.speed = speed;
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
            circleToGrowInSize.transform.localScale = Vector3.MoveTowards(circleToGrowInSize.transform.localScale, Vector3.one * targetCircleSize, Time.deltaTime * speed);
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

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
            if (hits[i].name.Equals(subjectName))
                continue;

            Debug.Log("적 원 범우 안에 들어온 적 : "+hits[i].name);

            // 적
            if (hits[i].CompareTag("ENEMY"))
            {
                //Debug.Log(hits[i].name + stateEffectName);
                // 상태 효과 

                switch (stateEffectName)
                {
                    case "KnockBack":
                        hits[i].GetComponent<Enemy>().SetStateEffect(stateEffectName, abnormalTime, hits[i].transform.position + (hits[i].transform.position - subjectPos.position).normalized * moveDist);
                        break;
                    case "Air":
                        hits[i].GetComponent<Enemy>().SetStateEffect(stateEffectName, abnormalTime, hits[i].transform.position + Vector3.up * moveDist);
                        break;
                    default:
                        hits[i].GetComponent<Enemy>().SetStateEffect(stateEffectName, abnormalTime, Vector3.zero);
                        break;
                }

                hits[i].GetComponent<Enemy>().HP -= damage;
                continue;
            }

            //Debug.Log("플레이어가 공격 범위 안에 들어와있습니다." + Vector3.Distance(subjectPos.position, playerScr.transform.position));
            //Debug.Log(hits[i].name);

            // 플레이어
            playerScr.HP -= damage;
            // 플레이어 효과 적용 함수도 나중에 호출하기

        }
    }

}
