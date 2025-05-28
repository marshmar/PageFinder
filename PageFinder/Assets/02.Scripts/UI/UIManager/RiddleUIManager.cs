using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RiddleUIManager : MonoBehaviour, IUIPanel
{
    [SerializeField] private bool isFixedMap = false;
    [SerializeField]private GameObject problemSet;
    [SerializeField] private ProceduralMapGenerator proceduralMapGenerator;
    [SerializeField] private FixedMap fixedMap;
    [SerializeField] private Player player;

    private int currPageNum = 1;
    private int selectNum;

    [Header("Text")]
    [SerializeField] private TMP_Text problemText1;
    [SerializeField] private TMP_Text problemText2;
    [SerializeField] private TMP_Text coinText;

    [Header("Button")]
    [SerializeField] private Button nextPageBtn;
    [SerializeField] private Button option1Btn;
    [SerializeField] private Button option2Btn;
    [SerializeField] private Button option3Btn;

    [Header("Panel")]
    [SerializeField] private GameObject selectionPanel;

    [Header("Object")]
    [SerializeField] private GameObject titleObj;
    [SerializeField] private GameObject timerObj; 
    public PanelType PanelType => PanelType.Quest;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("PLAYER").GetComponent<Player>();
    }
    private void Start()
    {
        nextPageBtn.onClick.AddListener(() => MoveNextPage());
        option1Btn.onClick.AddListener(() => ClickOption(1));
        option2Btn.onClick.AddListener(() => ClickOption(2));
        option3Btn.onClick.AddListener(() => ClickOption(3));
        option1Btn.onClick.AddListener(() => MoveNextPage());
        option2Btn.onClick.AddListener(() => MoveNextPage());
        //option3Btn.onClick.AddListener(() => MoveNextPage());

    }

    private void ClickOption(int selectNum)
    {
        switch (selectNum)
        {
            case 1:
            case 2:
                this.selectNum = selectNum;
                break;
            case 3:
                selectionPanel.SetActive(false);
                EventManager.Instance.PostNotification(EVENT_TYPE.Open_Panel_Exclusive, this, PanelType.HUD);
                if (isFixedMap) fixedMap.playerNode.portal.gameObject.SetActive(true);
                else proceduralMapGenerator.playerNode.portal.gameObject.SetActive(true);
                currPageNum = 0;
                break;
        }
    }

    private void OnDestroy()
    {
/*        nextPageBtn.onClick.RemoveAllListeners();
        option1Btn.onClick.RemoveAllListeners();
        option2Btn.onClick.RemoveAllListeners();
        option3Btn.onClick.RemoveAllListeners();*/
    }

    private void Init()
    {
        problemText1.gameObject.SetActive(true);
        problemText2.gameObject.SetActive(false);
        selectionPanel.gameObject.SetActive(false);
        coinText.text = player.State.Coin.ToString();
        titleObj.gameObject.SetActive(true);
        nextPageBtn.gameObject.SetActive(true);
        /*        SetContentTxt();
                SetAnswer();

                problemSet.SetActive(false);
                nextPageBtn.gameObject.SetActive(true);*/
    }

    public void MoveNextPage()
    {
        currPageNum++;

        switch (currPageNum)
        {
            case 2:
                titleObj.gameObject.SetActive(false);
                problemText1.gameObject.SetActive(false);
                selectionPanel.SetActive(true);
                nextPageBtn.gameObject.SetActive(false);
                break;
            case 3:
                problemText1.gameObject.SetActive(false);
                problemText2.gameObject.SetActive(true);
                selectionPanel.SetActive(false);
                nextPageBtn.gameObject.SetActive(true);
                break;
            case 4:
                currPageNum = 0;
                EventManager.Instance.PostNotification(EVENT_TYPE.Open_Panel_Exclusive, this, PanelType.HUD);
                // 타이머도 활성화 해야 함
                GameData.Instance.SpawnEnemies();
                timerObj.gameObject.SetActive(true);
                TimerUIManager timerUIManager  = timerObj.GetComponent<TimerUIManager>();
                timerUIManager.InitTime();
                break;
        }
    }
   /* private void SetContentTxt()
    {
        switch (answerNum)
        {
            case 0:
                contentTxt.text = currRiddleData.positiveConversation;
                contentTxt.enabled = true;
                problemSet.SetActive(false);
                nextPageBtn.gameObject.SetActive(true);
                break;

            case 1:
                contentTxt.text = currRiddleData.positiveConversation;
                contentTxt.enabled = true;
                problemSet.SetActive(false);
                nextPageBtn.gameObject.SetActive(true);
                break;

            // 스킵
            case 2:
                contentTxt.text = currRiddleData.neagativeConversation;
                contentTxt.enabled = true;
                problemSet.SetActive(false);
                nextPageBtn.gameObject.SetActive(true);
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
        if (currPageNum < problemPageNum) SetContentTxt();
        // Problem인 경우
        else if (currPageNum == problemPageNum)
        {
            problemSet.SetActive(true);
            nextPageBtn.gameObject.SetActive(false);
            contentTxt.enabled = false;
        }
        // Answer인 경우
        else if(currPageNum == problemPageNum+1) SetContentTxt();
        // 수수께끼 종료시
        else
        {
            switch (answerNum)
            {
                // 수수께끼 플레이 하는 경우
                case 0:
                case 1:
                    // ToDo: UI Changed;
                    EventManager.Instance.PostNotification(EVENT_TYPE.Open_Panel_Exclusive, this, PanelType.HUD);
                    // 타이머도 활성화 해야 함
                    GameData.Instance.SpawnEnemies();
                    break;

                // 수수께끼 생략하는 경우
                case 2:
                    // ToDo: UI Changed;
                    EventManager.Instance.PostNotification(EVENT_TYPE.Open_Panel_Exclusive, this, PanelType.HUD);
                    if(isFixedMap) fixedMap.playerNode.portal.gameObject.SetActive(true);
                    else proceduralMapGenerator.playerNode.portal.gameObject.SetActive(true);
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
*/
    public void Open()
    {
        this.gameObject.SetActive(true);
        Init();
    }

    public void Close()
    {
        this.gameObject.SetActive(false);
    }
}