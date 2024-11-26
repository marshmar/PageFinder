using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class PageMap : MonoBehaviour
{
    [SerializeField]
    private BattlePage[] battlePage1;
    [SerializeField]
    public RiddlePage[] riddlePage1;
    [SerializeField]
    private BattlePage mediumBossPage;
    [SerializeField]
    private Page[] shopPage1;

    private Page[] pages1 = new Page[12];


    private int currStageNum;  // 0~2
    private int currPageNum; // -1 : 맨 처음 플레이어아이콘 존재 x   0-0 : 0 페이지     0-10: 보스 페이지

    BattleUIManager battleUIManager;

    List<List<int>> pageColData = new List<List<int>>()
        {
            new List<int> { 0 },
            new List<int> { 1,2 },
            new List<int> { 3,4,5 },
            new List<int> { 6,7 },
            new List<int> { 8,9 },
            new List<int> { 10 }
        };

    public int CurrStageNum
    {
        get
        {
            return currStageNum;
        }
        set
        {
            currStageNum = value;
        }
    }

    public int CurrPageNum
    {
        get
        {
            return currPageNum;
        }
        set
        {
            if (currPageNum >= pages1.Length)
            {
                CurrStageNum += 1;
                currPageNum = 1;
            }
            
            currPageNum = value;
        }
    }

    private void Awake()
    {
        battleUIManager = GameObject.Find("UIManager").GetComponent<BattleUIManager>();
        SetPage();
    }

    public void SetPageClearData(bool value = true)
    {
        pages1[currPageNum].IsClear = value;

        switch (pages1[currPageNum].pageType)
        {
            case Page.PageType.BATTLE:
                battleUIManager.StartCoroutine(battleUIManager.SetClearDataUI(value));
                break;

            case Page.PageType.RIDDLE:
                battleUIManager.StartCoroutine(battleUIManager.SetClearDataUI(value));
                break;

            case Page.PageType.TRANSACTION:
                UIManager.Instance.SetUIActiveState("PageMap");
                break;

            case Page.PageType.MIDDLEBOSS:
                UIManager.Instance.SetUIActiveState("Success");
                break;
        }
    }

    public Page GetPageData(int stageNum, int pageNum)
    {  
        switch(stageNum)
        {
            case 0:
                return pages1[pageNum];

            //case 2:
            //    return pages2[pageNum];

            //case 3:
            //    return pages3[pageNum];

            default:
                Debug.LogWarning(stageNum);
                return pages1[0];
        }
    }

    /// <summary>
    /// 같은 열의 페이지인지 검사한다.
    /// </summary>
    /// <param name="stageNum"></param>
    /// <param name="pageNum"></param>
    /// <returns></returns>
    public int CheckIfItIsSameColPageAbout1Stage(int pageNum)
    {
        for (int i=0; i< pageColData.Count; i++)
        {
            if (pageColData[i].IndexOf(pageNum) != -1)
                return pageColData[i].Last(); // 해당 열의 가장 마지막 값
        }

        //Debug.LogWarning(currPageNum);
        return -1;
    }

    public int[] GetClearPagesAboutStage1()
    {
        int[] clearPageNums = { -1, -1, -1, -1, -1, -1, -1 };
        int index = 0;

        for (int col = 0; col < pageColData.Count; col++)
        {
            for (int pageNum = 0; pageNum < pageColData[col].Count; pageNum++)
            {
                index = pageColData[col][pageNum];
                if (pages1[index].IsClear)
                {
                    clearPageNums[col] = index;
                    break;
                }
            }
        }

        return clearPageNums;
    }

    public bool isClearPageAboutStage1(int num)
    { 
        return pages1[num].IsClear;
    }

    private void SetPage()
    {
        int pages1Index = 0;
        for (int i=0; i< battlePage1.Length; i++) // 6개
            pages1[pages1Index++] = battlePage1[i];
        for(int i=0; i<riddlePage1.Length; i++) // 2개
            pages1[pages1Index++] = riddlePage1[i];
        for (int i = 0; i < shopPage1.Length; i++)
            pages1[pages1Index++] = shopPage1[i];

        pages1[pages1Index] = mediumBossPage;
        currStageNum = 0;
        currPageNum = -1;
    }

    public bool CheckIfCurrStageIsPageToWant(Page.PageType pageType)
    {
        return pages1[currPageNum].pageType == pageType ? true : false;
    }

    public Page GetCurrPage()
    {
        return pages1[currPageNum];
    }
}
