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
    [SerializeField]
    private Canvas PageMapUICanvas;

    // PageMap
    [SerializeField]
    private Sprite[] pagesMoveType_Spr; // 0:clear    1:이동불가  3:이동가능 4: 윤곽선 두꺼운 버전
    [SerializeField]
    private Transform[] stagePage;
    [SerializeField]
    private Image playerIcon_Img;
    [SerializeField]
    private GameObject LinkedInk_Prefab;
    [SerializeField]
    private Transform linkedInks;
    string[] whitePageNames = { "", "", "" };

    // Discription
    [SerializeField]
    private GameObject additionalDiscriptions;

    // Player Discription
    [SerializeField]
    private SliderBar playerHpBar;
    [SerializeField]
    private TMP_Text coinValue_Txt;


    // NextPage
    [SerializeField]
    private GameObject[] nextPageObjects;

    // Additional Discription Btn -2
    bool isClick = false;
    float clickTime = 0;

    // Additional Discription Btn -1
    [SerializeField]
    private GameObject additionalDiscritionsObj; 
    [SerializeField]
    private SVGImage additionalDiscritionIcon_SvgImg;
    [SerializeField]
    private TMP_Text additionalDiscritionIconName_Txt;
    [SerializeField]
    private TMP_Text additionalDiscritionIconDiscrition_Txt;
    [SerializeField]
    private Sprite[] additionalDiscritionIcons_Spr = new Sprite[4];
    string clickedPageIconName = "";


    GameObject currSelectedObj = null;

    int[] clearPageNum;

    PageMap pageMap;
    Player player;
    private void Start()
    {
        pageMap = GameObject.Find("Maps").GetComponent<PageMap>();
        player = GameObject.FindWithTag("PLAYER").GetComponent<Player>();

    }

    private void Update()
    {
        //SetAddtionalDiscription2();
        SetAddtionalDiscriptionClickTime();
    }

    public void SetPageMapUICanvasState(bool value)
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
        SetNextPageBtnTransparentState(true);
        MovePagePosOfplayerIcon_Img();
        SetMoveTypesOfCurrPage();
        //additionalDiscriptions.SetActive(false);
        additionalDiscritionsObj.SetActive(false);
        SetPlayerDiscription();
        LinkClearPage();
    }

    /// <summary>
    /// 다음 페이지 이동 관련 투명화 설정을 한다.
    /// </summary>
    public void SetNextPageBtnTransparentState(bool value)
    {
        Color currImgColor = nextPageObjects[0].GetComponent<Image>().color;

        if (value)
            currImgColor = new Color(currImgColor.r, currImgColor.g, currImgColor.b, 0.5f);
        else
            currImgColor = new Color(currImgColor.r, currImgColor.g, currImgColor.b, 1f);

        nextPageObjects[0].GetComponent<Image>().color = currImgColor;
        nextPageObjects[1].GetComponent<Button>().interactable = !value;
    }

    /// <summary>
    /// 다음 페이지로 이동한다.
    /// </summary>
    public void MoveNextPage()
    {
        string[] moveData = currSelectedObj.name.Split('-');
        Page pageToMove = pageMap.GetPageData(int.Parse(moveData[0]),  int.Parse(moveData[1]) - 1);
        EnemyManager.Instance.SetEnemyAboutCurrPageMap(int.Parse(moveData[0]), pageToMove);
        player.transform.position = pageToMove.GetSpawnPos();

        pageMap.CurrPageNum = int.Parse(moveData[1]);

        SetPageMapUICanvasState(false);
    }

    /// <summary>
    /// 페이지들의 이동 타입을 설정한다.
    /// </summary>
    private void SetMoveTypesOfCurrPage()
    {
        int currStageNum = pageMap.CurrStageNum - 1; // 범위 : [1~n - 1~n
        int currPageNum = pageMap.CurrPageNum - 1;
        int[] whitePageNums = { -1, -1, -1 };
        int bluePageMaxNum = pageMap.CheckIfItIsSameColPageAbout1Stage(currPageNum);
        

        SetwhitePageNames();

        for (int i = 0; i < whitePageNames.Length; i++)
        {
            if (whitePageNames[i].Equals(""))
                continue;
            whitePageNums[i] = int.Parse(whitePageNames[i].Split('-')[1]) - 1;
        }

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
    }

    private void LinkClearPage()
    {
        int currStageNum = pageMap.CurrStageNum - 1; // 범위 : [1~n]

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
        bool canMove = false;
        string[] tmpPageData;


        if (!pageMap.isClearPageAboutStage1(0) && currSelectedObj.name.Equals(whitePageNames[0]))
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
            return;

        // 클릭한 페이지가 이동가능한 페이지인 경우
        currSelectedObj.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);

        if (currSelectedObj.name.Equals(whitePageNames[1]))
        {
            tmpPageData = whitePageNames[2].Split('-');
            if (tmpPageData.Length == 2)
                stagePage[int.Parse(tmpPageData[0]) - 1].GetChild(int.Parse(tmpPageData[1]) - 1).transform.localScale = Vector3.one;
        }
        else
        {
            tmpPageData = whitePageNames[1].Split('-');
            if (tmpPageData.Length == 2)
                stagePage[int.Parse(tmpPageData[0]) - 1].GetChild(int.Parse(tmpPageData[1]) - 1).transform.localScale = Vector3.one;
        }

        // 현재 페이지를 기준으로 북동, 남동쪽에 플레이어 아이콘이 위치한 경우에만 다음 페이지로 이동할 수 있다. 
        // 이동하기 버튼 투명화 해제
        SetNextPageBtnTransparentState(false);

    }

    private void MovePagePosOfplayerIcon_Img()
    {
        int currStageNum = pageMap.CurrStageNum - 1; // 범위 : [1~n]
        int currpageNum = pageMap.CurrPageNum - 1;

        playerIcon_Img.transform.position = stagePage[currStageNum].GetChild(currpageNum).transform.position;
    }

    private void SetwhitePageNames()
    {
        int currStageNum = pageMap.CurrStageNum - 1;  // 범위 : [1~n]
        int currpageNum = pageMap.CurrPageNum - 1;
        Transform currPageObj = stagePage[currStageNum].GetChild(currpageNum);
        RaycastHit hit;

        for (int i = 0; i < whitePageNames.Length; i++)
            whitePageNames[i] = "";

     
        // 현재 플레이어가 위치해있는 페이지
        whitePageNames[0] = (currStageNum + 1).ToString() + "-" + (currpageNum + 1).ToString();
      
        // 1-1을 못깼을 때
        if (!pageMap.isClearPageAboutStage1(0))
            return;

        // 북동쪽 체크
        if (Physics.Raycast(currPageObj.position, new Vector3(1, 1, 0), out hit, 150))
            whitePageNames[1] = hit.transform.name;

        // 남동쪽 체크
        if (Physics.Raycast(currPageObj.position, new Vector3(1, -1, 0), out hit, 150))
            whitePageNames[2] = hit.transform.name;
    }

    public void AdditionalDiscriptionBtnDown()
    {
        isClick = true;
        clickedPageIconName = EventSystem.current.currentSelectedGameObject.transform.GetChild(0).GetComponent<SVGImage>().sprite.name.Split("IconSprite")[0];
    }

    public void AdditionalDiscriptionBtnUp()
    {
        clickTime = 0;
        isClick = false;
        //additionalDiscriptions.SetActive(false);
    }

    private void SetAddtionalDiscriptionClickTime()
    {
        if (!isClick)
            return;

        clickTime += Time.deltaTime;
        if (clickTime > 1f)
        {
            isClick = false; // 아래 함수 한번만 동작
            additionalDiscritionsObj.SetActive(true);
            SetAddtionalDiscription1();
            Debug.Log("롱 클릭");
            clickTime = 0;
        }
    }


    private void SetAddtionalDiscription1()
    {
        Sprite sprite = null;
        string iconName = "";
        string iconDiscription = "";

        switch (clickedPageIconName)
        {
            case "Battle":
                sprite = additionalDiscritionIcons_Spr[0];
                iconName = "전투";
                iconDiscription = "적에게 승리하면 스텔라를 강화하는 스크립트를 획득할 수 있습니다.";
                break;

            case "Transaction":
                sprite = additionalDiscritionIcons_Spr[1];
                iconName = "거래";
                iconDiscription = "골드를 소모하여 스텔라를 강화하는 스크립트를 구매하거나, 기존 스크립트를 새로운 스크립트로 교환할 수 있습니다.";
                break;

            case "Riddle":
                sprite = additionalDiscritionIcons_Spr[2];
                iconName = "수수께끼";
                iconDiscription = "ㄹ새로운 이야기를 발견하고 선택에 따라 다른 결과를 확인합니다.";
                break;

            case "MiddleBoss":
                sprite = additionalDiscritionIcons_Spr[3];
                iconName = "중간보스";
                iconDiscription = "막강한 보스가 당신을 기다리고 있습니다. 이 페이지에서 승리하면 이번 챕터를 클리어할 수 있습니다.";
                break;

            default:
                Debug.LogWarning(clickedPageIconName);
                break;

        }
        additionalDiscritionIcon_SvgImg.sprite = sprite;
        additionalDiscritionIconName_Txt.text = iconName;
        additionalDiscritionIconDiscrition_Txt.text = iconDiscription;
    }

    public void CloseAdditionalDiscrition()
    {
        additionalDiscritionsObj.SetActive(false);
    }

    private void SetPlayerDiscription()
    {
        // Hp Bar
        playerHpBar.SetMaxValueUI(player.MAXHP);
        playerHpBar.SetCurrValueUI(player.HP);

        // Coin
        coinValue_Txt.text = "100";
    }

}
