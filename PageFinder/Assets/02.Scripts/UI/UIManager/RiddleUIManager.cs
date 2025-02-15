using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class RiddleUIManager : MonoBehaviour
{
    // Content
    [SerializeField]
    private TMP_Text contentTxt;

    // Problem
    [SerializeField]
    private GameObject problemSet;
    [SerializeField]
    private TMP_Text problemTxt;
    [SerializeField]
    private TMP_Text[] optionsTxt;

    [SerializeField]
    private GameObject nextPageBtn;

    private int currPageNum;
    private int problemPageNum;

    private int answerNum;

    private RiddleData currRiddleData;

    private void OnEnable()
    {
        Init();
    }

    private void Init()
    {
        currRiddleData = RiddleCSVReader.Instance.GetRiddleData(GameData.Instance.CurrStageNum);
        if (!currRiddleData)
            return;

        currPageNum = 0;
        answerNum = -1;

        // 대화 페이지 + 문제 페이지 + 응답 페이지 : +1인 이유는 0부터 시작이기 때문
        problemPageNum = currRiddleData.conversations.Count;

        SetContentTxt();
        SetAnswer();

        problemSet.SetActive(false);
        nextPageBtn.SetActive(true);
    }

    public void SetRiddleUICanvasState(bool value)
    {
        gameObject.SetActive(value);

        if (!value)
            return;

        Init();
    }

    private void SetContentTxt()
    {
        switch (answerNum)
        {
            case 0:
                contentTxt.text = currRiddleData.positiveConversation;
                contentTxt.enabled = true;
                problemSet.SetActive(false);
                nextPageBtn.SetActive(true);
                break;

            case 1:
                contentTxt.text = currRiddleData.positiveConversation;
                contentTxt.enabled = true;
                problemSet.SetActive(false);
                nextPageBtn.SetActive(true);
                break;

            // 스킵
            case 2:
                contentTxt.text = currRiddleData.neagativeConversation;
                contentTxt.enabled = true;
                problemSet.SetActive(false);
                nextPageBtn.SetActive(true);
                break;

            // 컨텐츠 페이지인 경우
            default:
                contentTxt.text = currRiddleData.conversations[currPageNum];
                break;
        }
    }

    private void SetAnswer()
    {
        for (int i = 0; i < currRiddleData.options.Length; i++)
            optionsTxt[i].text = currRiddleData.options[i];
    }

    public void MoveNextPage()
    {
        currPageNum++;

        // Content인 경우
        if (currPageNum < problemPageNum)
            SetContentTxt();
        // Problem인 경우
        else if (currPageNum == problemPageNum)
        {
            problemSet.SetActive(true);
            nextPageBtn.SetActive(false);
            contentTxt.enabled = false;
        }
        // Answer인 경우
        else if(currPageNum == problemPageNum+1)
            SetContentTxt();
        // 수수께끼 종료시
        else
        {
            List<CanvasType> canvasTypes = new List<CanvasType> { CanvasType.BATTLE, CanvasType.PLAYERUIINFO, CanvasType.PLAYERUIOP };

            switch (answerNum)
            {
                // 수수께끼 플레이 하는 경우
                case 0:
                case 1:
                    canvasTypes.Add(CanvasType.TIMER);
                    EventManager.Instance.PostNotification(EVENT_TYPE.UI_Changed, this, UIType.Battle);
                    break;

                // 수수께끼 생략하는 경우
                case 2:
                    EventManager.Instance.PostNotification(EVENT_TYPE.UI_Changed, this, UIType.Battle);
                    break;
            }
        }
    }

    public void ClickOption()
    {
        string cilcekdAnswerName = EventSystem.current.currentSelectedGameObject.name;

        for (int i = 0; i < problemSet.transform.childCount; i++)
        {
            if (cilcekdAnswerName.Contains((i + 1).ToString()))
            {
                answerNum = i;
                MoveNextPage();
                break;
            }
        }
    }

}
