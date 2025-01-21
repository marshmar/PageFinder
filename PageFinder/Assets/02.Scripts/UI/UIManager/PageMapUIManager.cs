using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VectorGraphics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PageMapUIManager : MonoBehaviour
{
    public enum STATE
    {
        MOVE,
        READ
    }

    STATE state = STATE.MOVE;

    [SerializeField]
    private Canvas PageMapUICanvas;

    // PageMap
    [SerializeField]
    private Sprite[] pagesMoveType_Spr; // 0:clear    1:이동불가  3:이동가능 4: 윤곽선 두꺼운 버전
    [SerializeField]
    private RectTransform[] stagePage;
    [SerializeField]
    private Image playerIcon_Img;
    [SerializeField]
    private GameObject LinkedInk_Prefab;
    [SerializeField]
    private Transform linkedInks;
    string[] whitePageNames = { "", "", "" };

    // 아이콘 설명창
    [SerializeField]
    private Sprite[] pageIcons_Spr;
    [SerializeField]
    private GameObject iconDiscriptionObj;
    [SerializeField]
    private Image iconImg;
    [SerializeField]
    private TMP_Text iconTitleTxt;
    [SerializeField]
    private TMP_Text iconContentTxt;


    // 외곽선
    [SerializeField]
    private RectTransform clckedPageOutLine;

    // 코인
    [SerializeField]
    private TMP_Text coinTxt;

    // NextPage
    [SerializeField]
    private Button nextPageBtn;
    [SerializeField]
    private Sprite[] nextPageBtn_Sprites;

    GameObject currSelectedObj = null;

    int[] clearPageNum;

    [SerializeField]
    PageMap pageMap;
    // 강해담 수정
    [SerializeField]
    PlayerState playerState;


    public void SetPageMapUICanvasState(bool value, string prvUIName)
    {
        PageMapUICanvas.gameObject.SetActive(value);

        if (!value)
            return;

        /* [NextPage]
            * - 이동하기 관련 투명화되도록 설정 ok
            * 
            * [Discription]
            * - 플레이어 hp, Coin 값 최신화
            * - Discription before로 설정 ok
            * 
            * [페이지 맵]
            * - 이동한 지역, 이동가능한 지역, 이동불가능 지역 구분 ok
            * - 플레이어 위치 최신화 ok
            */
        currSelectedObj = null;
        clckedPageOutLine.gameObject.SetActive(false);
        setState(prvUIName);
        SetNextPageBtn(state == STATE.READ ? true : false);
        MovePagePosOfplayerIcon_Img();
        SetMoveTypesOfCurrPage();
        LinkClearPage();
        SetIconDiscription();
        SetCoinTxt();
    }

    private void setState(string prvUIName)
    {
        switch(prvUIName)
        {
            case "Setting":
                state = STATE.READ;
                break;

            default:
                state = STATE.MOVE;
                break;
        }
    }

    private void SetNextPageBtn(bool isActive)
    {
        if(pageMap.CurrPageNum == -1)
        {
            nextPageBtn.interactable = true;
            nextPageBtn.image.sprite = nextPageBtn_Sprites[0];
            clckedPageOutLine.gameObject.SetActive(true);
            clckedPageOutLine.position = GameObject.Find("0_0").GetComponent<RectTransform>().position;
            currSelectedObj = GameObject.Find("0_0");
            return;
        }

        if (state == STATE.MOVE)
            nextPageBtn.image.sprite = nextPageBtn_Sprites[0];
        else if (state == STATE.READ)
            nextPageBtn.image.sprite = nextPageBtn_Sprites[1];

        nextPageBtn.interactable = isActive;
    }

    /// <summary>
    /// 다음 페이지로 이동한다.
    /// </summary>
    public void MoveNextPage()
    {
        if (state == STATE.READ)
        {
            UIManager.Instance.SetUIActiveState("Battle");
            return;
        }

        string[] moveData = currSelectedObj.name.Split('_');
        Page pageToMove = pageMap.GetPageData(int.Parse(moveData[0]),  int.Parse(moveData[1]));

        switch(pageToMove.pageType)
        {
            case Page.PageType.TRANSACTION:
                break;

            case Page.PageType.RIDDLE:
                break;

            default:
                EnemyManager.Instance.SetEnemyAboutCurrPageMap(int.Parse(moveData[0]), pageToMove);
                break;
        }

        playerState.transform.position = pageToMove.GetSpawnPos();
        pageMap.CurrPageNum = int.Parse(moveData[1]);
        UIManager.Instance.SetUIActiveState(pageToMove.getPageTypeString());
    }

    /// <summary>
    /// 페이지들의 이동 타입을 설정한다.
    /// </summary>
    private void SetMoveTypesOfCurrPage()
    {
        int currStageNum = pageMap.CurrStageNum; // 범위 : [1~n - 1~n

        int[] whitePageNums = { -2, -2, -2 };
        int maxWhitePageNum = -1;
        SetwhitePageNames();

        for (int i = 0; i < whitePageNames.Length; i++)
        {
            if (whitePageNames[i].Equals(""))
                continue;
            whitePageNums[i] = int.Parse(whitePageNames[i].Split('_')[1]);
            if (whitePageNums[i] > maxWhitePageNum)
                maxWhitePageNum = whitePageNums[i];
        }

        int bluePageMaxNum = pageMap.CheckIfItIsSameColPageAbout1Stage(maxWhitePageNum);
    
        // 이동 여부에 따른 페이지 스프라이트 교체
        for (int pageNum = 0; pageNum < stagePage[currStageNum].transform.childCount; pageNum++)
        {
            stagePage[currStageNum].GetChild(pageNum).transform.localScale = Vector3.one;

            // 현재 페이지를 포함하여 이동가능한 페이지인 경우(하얀색)
            if (pageNum == whitePageNums[0] || pageNum == whitePageNums[1] || pageNum == whitePageNums[2])
                stagePage[currStageNum].GetChild(pageNum).GetComponent<Image>().sprite = pagesMoveType_Spr[0];
            // 이제는 갈 수 없는 페이지인 경우(파랑색)
            else if (pageNum <= bluePageMaxNum)
                stagePage[currStageNum].GetChild(pageNum).GetComponent<Image>().sprite = pagesMoveType_Spr[1];
            // 현재 말고 미래에 이동가능하지만 현재 이동 불가능한 페이지인 경우(빨간색)
            else
                stagePage[currStageNum].GetChild(pageNum).GetComponent<Image>().sprite = pagesMoveType_Spr[2];
        }

        // 플레이어 제일 처음 입장일 경우 0-0 칸 하얀색으로 변경
        if (whitePageNums[0] == -1)
            stagePage[currStageNum].GetChild(0).GetComponent<Image>().sprite = pagesMoveType_Spr[0];

    }

    private void LinkClearPage()
    {
        int currStageNum = pageMap.CurrStageNum; // 범위 : [1~n]

        clearPageNum = pageMap.GetClearPagesAboutStage1();

        for (int i=0; i<clearPageNum.Length; i++)
        {
            if (i == clearPageNum.Length-1)
                break;

            if (clearPageNum[i] == -1 || clearPageNum[i+1] == -1)
                break;

            Vector3 pos = (stagePage[currStageNum].GetChild(clearPageNum[i]).transform.position + stagePage[currStageNum].GetChild(clearPageNum[i + 1]).transform.position) / 2;
            Vector3 dir = (stagePage[currStageNum].GetChild(clearPageNum[i + 1]).transform.position - stagePage[currStageNum].GetChild(clearPageNum[i]).transform.position) / 100;
            Quaternion quaternion = Quaternion.Euler(0,0,-45); // 기본 남동쪽

            // 북동
            if (dir.y > 0)
                quaternion = Quaternion.Euler(0, 0, 45);

            GameObject obj = Instantiate(LinkedInk_Prefab, pos, quaternion, linkedInks);
            obj.GetComponent<Image>().raycastTarget = false;
        }
    }

    /// <summary>
    /// 클릭한 페이지들의 이동 타입을 설정한다.
    /// </summary>
    public void SetMoveTypesOfClickedPage()
    {
        currSelectedObj = EventSystem.current.currentSelectedGameObject;

        if (!clckedPageOutLine.gameObject.activeSelf)
            clckedPageOutLine.gameObject.SetActive(true);

        clckedPageOutLine.position = currSelectedObj.GetComponent<RectTransform>().position;
        SetIconDiscription();

        if (state == STATE.READ)
            return;

       
        bool canMove = false;


        if (pageMap.CurrPageNum == -1 && currSelectedObj.name.Equals("0_0"))
        {
            canMove = true;
        }
        else
        {
            for (int i = 1; i < whitePageNames.Length; i++)
            {
                // 현재 페이지 기준 북동, 남동쪽 페이지를 클릭한 경우
                if (currSelectedObj.name.Equals(whitePageNames[i]))
                {
                    canMove = true;
                    break;
                }
            }
        }

        if (!canMove)
        {
            SetNextPageBtn(false);
            return;
        }
        

        // 현재 페이지를 기준으로 북동, 남동쪽에 플레이어 아이콘이 위치한 경우에만 다음 페이지로 이동할 수 있다. 
        // 이동하기 버튼 투명화 해제
        SetNextPageBtn(true);
    }

    private void MovePagePosOfplayerIcon_Img()
    {
        int currStageNum = pageMap.CurrStageNum; // 범위 : [1~n]
        int currpageNum = pageMap.CurrPageNum;

        // 스테이지의 맨 처음
        if (currpageNum == -1)
            playerIcon_Img.enabled = true;
        else
        {
            playerIcon_Img.enabled = true;
            playerIcon_Img.transform.position = stagePage[currStageNum].GetChild(currpageNum).transform.position;
        }
    }

    private void SetwhitePageNames()
    {
        int currStageNum = pageMap.CurrStageNum;  // 범위 : [1~n]
        int currpageNum = pageMap.CurrPageNum;

        for (int i = 0; i < whitePageNames.Length; i++)
            whitePageNames[i] = "";

        // 맨 처음 시작
        if (currpageNum == -1)
        {
            whitePageNames[0] = "0_-1";
            return;
        }

        whitePageNames[0] = currStageNum.ToString() + "_" + currpageNum.ToString();

        switch (currpageNum)
        {
            case 0:
                whitePageNames[1] = currStageNum + "_1";
                whitePageNames[2] = currStageNum + "_2";
                break;

            case 1:
                whitePageNames[1] = currStageNum + "_3";
                whitePageNames[2] = currStageNum + "_4";
                break;

            case 2:
                whitePageNames[1] = currStageNum + "_4";
                whitePageNames[2] = currStageNum + "_5";
                break;

            case 3:
                whitePageNames[1] = currStageNum + "_6";
                break;

            case 4:
                whitePageNames[1] = currStageNum + "_6";
                whitePageNames[2] = currStageNum + "_7";
                break;

            case 5:
                whitePageNames[1] = currStageNum + "_7";
                break;

            case 6:
                whitePageNames[1] = currStageNum + "_8";
                whitePageNames[2] = currStageNum + "_9";
                break;

            case 7:
                whitePageNames[1] = currStageNum + "_9";
                break;

            case 8:
                whitePageNames[1] = currStageNum + "_10";
                break;

            case 9:
                whitePageNames[1] = currStageNum + "_10";
                break;
        }


        //RectTransform currPageObj = stagePage[currStageNum].GetChild(currpageNum).GetComponent<RectTransform>();
        //RaycastHit hit;

        //// 현재 플레이어가 위치해있는 페이지
        //whitePageNames[0] = currStageNum.ToString() + "_" + currpageNum.ToString();

        //// 북동쪽 체크
        //if (Physics.Raycast(currPageObj.position, new Vector3(1, 1, 0), out hit, pageImgDist))
        //    whitePageNames[1] = hit.transform.name;

        //// 남동쪽 체크
        //if (Physics.Raycast(currPageObj.position, new Vector3(1, -1, 0), out hit, pageImgDist))
        //    whitePageNames[2] = hit.transform.name;

        //Debug.Log(whitePageNames[1] + " " + whitePageNames[2]);
    }

    private void SetIconDiscription()
    {
        if (currSelectedObj == null)
        {
            iconDiscriptionObj.SetActive(false);
            return;
        }

        Sprite sprite = null;
        string iconName = "";
        string iconDiscription = "";

        string[] pageMapData = currSelectedObj.name.Split('_');
        int stageNum = int.Parse(pageMapData[0]);
        int pageNum = int.Parse(pageMapData[1]);
        Page page;

        iconDiscriptionObj.SetActive(true);
        page = pageMap.GetPageData(stageNum, pageNum);


        switch (page.pageType.ToString())
        {
            case "BATTLE":
                sprite = pageIcons_Spr[0];
                iconName = "전투";
                iconDiscription = "적에게 승리하면 스텔라를\n강화하는 스크립트를\n획득할 수 있습니다.";
                break;

            case "TRANSACTION":
                sprite = pageIcons_Spr[1];
                iconName = "거래";
                iconDiscription = "골드를 소모하여 스텔라를 강화하는 스크립트를 구매하거나, 기존 스크립트를 새로운 스크립트로 교환할 수 있습니다.";
                break;

            case "RIDDLE":
                sprite = pageIcons_Spr[2];
                iconName = "수수께끼";
                iconDiscription = "새로운 이야기를 발견하고\n선택에 따라 다른 결과를\n확인합니다.";
                break;

            case "MIDDLEBOSS":
                sprite = pageIcons_Spr[3];
                iconName = "중간보스";
                iconDiscription = "막강한 보스가 당신을 기다리고 있습니다. 이 페이지에서 승리하면 이번 챕터를 클리어할 수 있습니다.";
                break;

            default:
                Debug.LogWarning(currSelectedObj.name);
                break;

        }

        iconImg.sprite = sprite;
        iconImg.SetNativeSize();
        iconTitleTxt.text = iconName;
        iconContentTxt.text = iconDiscription;
    }

    private void SetCoinTxt()
    {
        // 강해담 수정 : player -> playerState
        coinTxt.text = playerState.Coin.ToString();
    }
}
