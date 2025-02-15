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

        // ��ȭ ������ + ���� ������ + ���� ������ : +1�� ������ 0���� �����̱� ����
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

            // ��ŵ
            case 2:
                contentTxt.text = currRiddleData.neagativeConversation;
                contentTxt.enabled = true;
                problemSet.SetActive(false);
                nextPageBtn.SetActive(true);
                break;

            // ������ �������� ���
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

        // Content�� ���
        if (currPageNum < problemPageNum)
            SetContentTxt();
        // Problem�� ���
        else if (currPageNum == problemPageNum)
        {
            problemSet.SetActive(true);
            nextPageBtn.SetActive(false);
            contentTxt.enabled = false;
        }
        // Answer�� ���
        else if(currPageNum == problemPageNum+1)
            SetContentTxt();
        // �������� �����
        else
        {
            List<CanvasType> canvasTypes = new List<CanvasType> { CanvasType.BATTLE, CanvasType.PLAYERUIINFO, CanvasType.PLAYERUIOP };

            switch (answerNum)
            {
                // �������� �÷��� �ϴ� ���
                case 0:
                case 1:
                    canvasTypes.Add(CanvasType.TIMER);
                    EventManager.Instance.PostNotification(EVENT_TYPE.UI_Changed, this, UIType.Battle);
                    break;

                // �������� �����ϴ� ���
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
