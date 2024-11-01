using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RiddleUIManager : MonoBehaviour
{
    [SerializeField]
    private Canvas RiddleUICanavs;

    [SerializeField]
    private Image bookImg;
    [SerializeField]
    private Sprite[] bookSprites;

    [SerializeField]
    private TMP_Text problemTitleTxt;
    [SerializeField]
    private TMP_Text problemContentTxt;
    [SerializeField]
    private GameObject answerSet;

    private int currPageNum = 1;
    private int lastPageNum = 4;

    private int selectedContentNum = -1;

    PageMap pageMap;

    private void Start()
    {
        pageMap = GameObject.Find("Maps").GetComponent<PageMap>();
    }

    public void SetRiddleUICanvasState(bool value)
    {
        RiddleUICanavs.gameObject.SetActive(value);

        if (!value)
            return;

        selectedContentNum = -1;
        SetBookImg();
        SetAnswerSetState(false);
    }

    private void SetBookImg()
    {
        if(currPageNum == lastPageNum)
        {
            if (selectedContentNum == 0 || selectedContentNum == 1)
                bookImg.sprite = bookSprites[3];
            else
                bookImg.sprite = bookSprites[4];
        }
        else
            bookImg.sprite = bookSprites[currPageNum - 1];
    }


    private void SetAnswerSetState(bool value)
    {
        answerSet.SetActive(value);
    }

    public void MoveNextPage()
    {
        // 1,2 페이지
        if(currPageNum < lastPageNum - 1)
        {
            currPageNum++;
            SetBookImg();
            Debug.Log("1,2페이지");
            if(currPageNum == lastPageNum - 1)
                SetAnswerSetState(true);
        }
        // 3페이지일 때
        else if(currPageNum == lastPageNum -1)
        {
            Debug.Log("3페이지");
            if (selectedContentNum == -1)
                return;

            currPageNum++;
            SetBookImg();
            SetAnswerSetState(false);
        }
        // 마지막 페이지
        else
        {
            switch (selectedContentNum)
            {
                // 보스 몬스터 이동 속도 증가
                case 0:
                    pageMap.riddlePage1[pageMap.CurrPageNum - 1].target_moveSpeed = 1.5f;
                    UIManager.Instance.SetUIActiveState("RiddlePlay");
                    break;

                // 보스 몬스터 Hp 증가
                case 1:
                    pageMap.riddlePage1[pageMap.CurrPageNum - 1].target_hp = 300;
                    UIManager.Instance.SetUIActiveState("RiddlePlay");
                    break;

                // 종료
                case 2:
                    pageMap.SetPageClearData();
                    break;

                default:
                    Debug.LogWarning(selectedContentNum);
                    break;
            }
        }
    }

    public void ClickAnswer()
    {
        Debug.Log(EventSystem.current.currentSelectedGameObject);
        string cilcekdAnswerName = EventSystem.current.currentSelectedGameObject.name;

        for(int i=0; i< answerSet.transform.childCount; i++)
        {
            if(cilcekdAnswerName.Contains((i+1).ToString()))
            {
                selectedContentNum = i;
                break;
            }
        }
    }

}
