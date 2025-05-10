using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RiddleUIManager : MonoBehaviour, IUIPanel
{
    [SerializeField]private GameObject problemSet;
    [SerializeField] private ProceduralMapGenerator proceduralMapGenerator;
    [SerializeField] RiddleCSVReader riddleCSVReader;
    private int currPageNum;
    private int problemPageNum;

    private int answerNum;

    private RiddleData currRiddleData;

    [Header("Text")]
    [SerializeField] private TMP_Text contentTxt;
    [SerializeField] private TMP_Text problemTxt;
    [SerializeField] private TMP_Text[] optionsTxt;

    [Header("Button")]
    [SerializeField] private Button nextPageBtn;
    [SerializeField] private Button option1Btn;
    [SerializeField] private Button option2Btn;
    [SerializeField] private Button option3Btn;


    public PanelType PanelType => PanelType.Quest;

    private void Start()
    {
        nextPageBtn.onClick.AddListener(() => MoveNextPage());
        option1Btn.onClick.AddListener(() => ClickOption());
        option2Btn.onClick.AddListener(() => ClickOption());
        option3Btn.onClick.AddListener(() => ClickOption());
    }

    private void OnDestroy()
    {
        nextPageBtn.onClick.RemoveAllListeners();
        option1Btn.onClick.RemoveAllListeners();
        option2Btn.onClick.RemoveAllListeners();
        option3Btn.onClick.RemoveAllListeners();
    }

    private void Init()
    {
        currRiddleData = riddleCSVReader.GetRiddleData(1);
        if (!currRiddleData) return;

        currPageNum = 0;
        answerNum = -1;

        // 대화 페이지 + 문제 페이지 + 응답 페이지 : +1인 이유는 0부터 시작이기 때문
        problemPageNum = currRiddleData.conversations.Count;

        SetContentTxt();
        SetAnswer();

        problemSet.SetActive(false);
        nextPageBtn.gameObject.SetActive(true);
    }

    private void SetContentTxt()
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
                    proceduralMapGenerator.playerNode.portal.gameObject.SetActive(true);
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