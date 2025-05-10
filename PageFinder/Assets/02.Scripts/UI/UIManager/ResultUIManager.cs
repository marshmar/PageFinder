using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum ResultType
{
    NONE = 0,
    WIN = 1, // 보스 처치
    DEFAT = 2, // 플레이어 사망
    GOAL_FAIL = 3 // 퀘스트 페이지에서 목표 실패
}

public class ResultUIManager : MonoBehaviour
{
    [SerializeField]
    private Sprite[] resultSprites;

    [SerializeField]
    private Image resultImage;

    [SerializeField]
    private TMP_Text resultTxt;

    private ResultType resultType;
    private float resultDuration;

    private void OnEnable()
    {
        InitResult();
        StartCoroutine(CloseResultScreen());
    }

    private void InitResult()
    {
        resultImage.sprite = resultSprites[(int)resultType - 1];
        switch (resultType)
        {
            case ResultType.WIN:
            case ResultType.DEFAT:
                resultTxt.enabled = true;
                break;

            case ResultType.GOAL_FAIL:
                resultTxt.enabled = false;
                break;
        }
    }


    private void SetResultTxt()
    {
        switch (resultType)
        {
            case ResultType.WIN:
                resultTxt.text = $"게임에서 최종 승리하였습니다!\n{(int)resultDuration}초 후 시작화면으로 넘어갑니다.";
                break;

            case ResultType.DEFAT:
                resultTxt.text = $"스텔라의 체력을 모두 소진하였습니다...\n{(int)resultDuration}초 후 시작화면으로 넘어갑니다.";
                break;
        }
    }

    private IEnumerator CloseResultScreen()
    {
        while (resultDuration >= 0)
        {
            resultDuration -= Time.deltaTime;
            SetResultTxt();
            yield return null;
        }
        resultDuration = 0;
        SetResultTxt();

        // 다음 이동할 화면 정하기
        switch (resultType)
        {
            case ResultType.WIN: // 보스 처치시
            case ResultType.DEFAT: // 플레이어 사망시
                EventManager.Instance.PostNotification(EVENT_TYPE.GAME_END, this);
                break;

            // 수수께끼 목표 실패시
            case ResultType.GOAL_FAIL:
                // ToDo: UI Changed;
                //EventManager.Instance.PostNotification(EVENT_TYPE.UI_Changed, this, UIType.PageMap);
                break;
        }
    }

    public void SetResultData(ResultType resultType, float resultDuration)
    {
        this.resultType = resultType;
        this.resultDuration = resultDuration;
    }
}
