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

    [SerializeField]
    private GameObject nextPageBtn;

    private int currPageNum = 1;
    private int pageToSelectNum = 3;
    private int lastPageNum = 5;

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
        SetNextPageBtnState(true);
    }

    private void SetBookImg()
    {
        if(currPageNum == lastPageNum-1)
        {
            if (selectedContentNum == 0 || selectedContentNum == 1)
                bookImg.sprite = bookSprites[3];
            else
                bookImg.sprite = bookSprites[4];
        }
        else
            bookImg.sprite = bookSprites[currPageNum - 1];
    }

    private void SetNextPageBtnState(bool value)
    {
        nextPageBtn.SetActive(value);
    }


    private void SetAnswerSetState(bool value)
    {
        answerSet.SetActive(value);
    }

    public void MoveNextPage()
    {
        currPageNum++;
        SetBookImg();


        // 3페이지일 때
        if(currPageNum == pageToSelectNum)
        {
            SetAnswerSetState(true);
            SetNextPageBtnState(false);
        }
        else
        {
            SetAnswerSetState(false);
            SetNextPageBtnState(true);
        }


        // 마지막 페이지
        if (currPageNum == lastPageNum)
        {
            int index = 0;
            Page pageToMove = pageMap.GetPageData(pageMap.CurrStageNum, pageMap.CurrPageNum);

            if (pageMap.CurrPageNum == 6)
                index = 0;
            else
                index = 1;

            switch (selectedContentNum)
            {
                // 보스 몬스터 이동 속도 증가
                case 0:
                    pageMap.riddlePage1[index].moveSpeed[0] = 1.5f;
                    EnemyManager.Instance.SetEnemyAboutCurrPageMap(pageMap.CurrStageNum, pageToMove);
                    UIManager.Instance.SetUIActiveState("Battle");
                    break;

                // 보스 몬스터 Hp 증가
                case 1:
                    pageMap.riddlePage1[index].maxHp[0] = 550;
                    EnemyManager.Instance.SetEnemyAboutCurrPageMap(pageMap.CurrStageNum, pageToMove);
                    UIManager.Instance.SetUIActiveState("Battle");
                    break;

                // 종료
                case 2:
                    UIManager.Instance.SetUIActiveState("PageMap");
                    break;

                default:
                    Debug.LogWarning(selectedContentNum);
                    break;
            }
        }
    }

    public void ClickAnswer()
    {
        string cilcekdAnswerName = EventSystem.current.currentSelectedGameObject.name;

        for(int i=0; i< answerSet.transform.childCount; i++)
        {
            if(cilcekdAnswerName.Contains((i+1).ToString()))
            {
                selectedContentNum = i;
                MoveNextPage();
                break;
            }
        }
    }

}
