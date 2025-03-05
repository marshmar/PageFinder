using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum InkType
{
    RED,
    GREEN,
    BLUE,   
    FIRE,   // 불바다
    MIST,   // 안개
    SWAMP   // 습지
}
public class InkMark : MonoBehaviour
{
    #region Variables
    public float duration;
    public float fusionDuration;
    private float spawnTime;
    private bool isFusioned;
    private bool isPlayerInTrigger;

    private bool isAbleFusion;
    private bool isOtherMarkInTrigger;
    private InkType currType;
    private InkMarkType currInkMarkType;

    private SpriteRenderer spriterenderer;
    private PlayerState playerState;
    private bool decreasingTransparency;
    private Coroutine transparencyCoroutine;

    private float inkFusionCircleThreshold = 0.3f;
    #endregion

    #region Properties
    public InkType CurrType { get => currType; set => currType = value; }
    public float SpawnTime { get => spawnTime; set
        {
            spawnTime = value;
            spriterenderer.color = new Color(spriterenderer.color.r, spriterenderer.color.g, spriterenderer.color.b, 1.0f);
        }
    }
    public bool IsFusioned { get => isFusioned; set => isFusioned = value; }
    public bool IsPlayerInTrigger { get => isPlayerInTrigger; set => isPlayerInTrigger = value; }
    public InkMarkType CurrInkMarkType { get => currInkMarkType; set => currInkMarkType = value; }
    public bool IsAbleFusion { get => isAbleFusion; set => isAbleFusion = value; }
    #endregion


    private void Awake()
    {
        isFusioned = false;
        IsPlayerInTrigger = false;
        spawnTime = 0.0f;
        spriterenderer = GetComponentInChildren<SpriteRenderer>();


        playerState = DebugUtils.GetComponentWithErrorLogging<PlayerState>
            (GameObject.FindGameObjectWithTag("PLAYER"), "PlayerState"
            );
    }

    public void SetInkMarkData(InkMarkType inkMarkType, InkType inkType)
    {
        currInkMarkType = inkMarkType;
        currType = inkType;

        SetSprites();
    }

    private void OnDestroy()
    {
        //playerInkMagicControllerScr.InkMarks.Remove(this);
    }

    private void OnDisable()
    {
        ResetInkMark();
    }

    private void ResetInkMark()
    {
        spriterenderer.color = new Color(spriterenderer.color.r, spriterenderer.color.g, spriterenderer.color.b, 1f);
        spawnTime = 0f;
        duration = 0f;
        isFusioned = false;
        isPlayerInTrigger = false;
        decreasingTransparency = false;
    }

    private void Update()
    {
        spawnTime += Time.deltaTime;
        if (spawnTime >= duration - 1.0f && !decreasingTransparency)
        {
            isAbleFusion = false;
            decreasingTransparency = true;
            StartCoroutine(DecreaseTransparency());
        }

        if (spawnTime >= duration)
        {
            InkMarkPooler.Instance.Pool.Release(this);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("INKMARK"))
        {
            if(other.TryGetComponent<InkMark>(out InkMark otherMark))
            {
                switch (otherMark.CurrInkMarkType)
                {
                    case InkMarkType.DASH:
                        break;
                    default:
                        if (CheckIntersectCircle(other))
                        {
                            Debug.Log("잉크 합성 가능");
                        }
                        break;
                }
            }
        }
    }


    private void OnTriggerExit(Collider other)
    {

    }

    //https://yupdown.tistory.com/31
    private bool CheckIntersectCircle(Collider other)
    {
        float r1 = transform.localScale.x * 0.5f;
        float r2 = other.transform.localScale.x * 0.5f;
        float distance = Vector3.Distance(transform.position, other.transform.position);

        // 겹치는 부분의 중심각
        float thetaR1 = 2 * Mathf.Acos((distance * distance + r1 * r1 - r2 * r2) / (2 * distance * r1));
        float thetaR2 = 2 * Mathf.Acos((distance * distance + r2 * r2 - r1 * r1) / (2 * distance * r2));

        // 겹치는 부분의 삼각형 넓이
        float triangleAreaR1 = r1 * r1 * 0.5f * Mathf.Sin(thetaR1);
        float triangleAreaR2 = r2 * r2 * 0.5f * Mathf.Sin(thetaR2);

        // 겹치는 부분의 부채꼴의 넓이
        float fanAreaR1 = r1 * r1 * 0.5f * thetaR1;
        float fanAreaR2 = r2 * r2 * 0.5f * thetaR2;

        // 최종적으로 겹치는 부분의 넓이
        float intersectArea = fanAreaR1 - triangleAreaR1 + fanAreaR2 - triangleAreaR2;
        float circleArea = r1 * r1 * (float)Mathf.PI;

        Debug.Log($"intersectArea: {intersectArea}, CircleArea: {circleArea}");

        if (intersectArea / circleArea >= inkFusionCircleThreshold) return true;

        return false;
    }

    // https://ko.wikipedia.org/wiki/%EC%8B%A0%EB%B0%9C%EB%81%88_%EA%B3%B5%EC%8B%9D
    // http://www.gingaminga.com/Data/Note/oriented_bounding_boxes/#1._%EC%86%8C%EA%B0%9C%EA%B8%80
    // https://kwaksh2319.tistory.com/46

    public void SetSprites()
    {
        if(spriterenderer == null)
        {
            Debug.Log(gameObject.name + "'s spriterenderer is null");
        }

        InkMarkSetter.Instance.SetInkMarkScaleAndDuration(currInkMarkType, transform, ref duration);

        if(!InkMarkSetter.Instance.SetInkMarkSprite(currInkMarkType, currType, spriterenderer))
        {
            Debug.LogError("잉크마크 스프라이트 할당 실패");
        }
    }

    public IEnumerator DecreaseTransparency()
    {
        float time = 0.0f;

        while (time <= 1.0f)
        {
            time += Time.deltaTime;
            spriterenderer.color = new Color(spriterenderer.color.r, spriterenderer.color.g, spriterenderer.color.b, 1 - time);

            yield return null;

        }

        yield break;
    }

}
