using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class TutorialManager : MonoBehaviour, IListener
{
    [SerializeField] private GameObject rewardPanel;
    [SerializeField] private PlayerAttackController attackController;
    [SerializeField] private GameObject tutorialA;
    [SerializeField] private GameObject tutorialB;
    [SerializeField] private GameObject targetPanel;

    // A Text
    private string textControl = "WASD로 움직여보자!";
    private string textBasicAttack1 = "낯선 생명체가 날 향해 그르렁 거리고 있어..";
    private string textBasicAttack2 = "왼쪽 마우스 클릭으로 공격하자";
    private string textBasicAttack3 = "잘했어, 바로 그거야!";
    private string textInkDash1 = "이번엔 space바를 눌러 ‘잉크 대시’를 사용해보자";
    private string textInkDash2 = "오~ 적응이 빠른데? 잉크 대시는 지나간 자리에 노멀 잉크를 남겨";
    private string textReward1 = "자 이제 다음 페이지로 넘어갈 시간이야";
    private string textReward2 = "설정붕괴의 잔해, 스크립트는 동화 속 대본의 조각들이야";
    private string textReward3 = "셋 중 하나를 선택해서 가져가자! 힘을 더욱 강하게 만들어줄거야";
    private string textPageSelection = "다음 경로를 선택하자. 좌클릭으로 아이콘을 누르면 돼";

    // B Text
    private string textInkSkill1 = "E 키로 스킬을 사용할 수 있어! E키를 길게 누른 상로 세부 방향까지 조절 가능하지. 강력한만큼 많은 양의 잉크를 사용하니까 전략적 사용이 필요해";
    private string textInkSkill2 = "E 키를 눌러서 스킬을 사용해봐!";
    private string textInkSkill3 = "좋았어! 잉크 스킬은 넓은 범위에 잉크를 전개하는데 유리해! 앞으로의 전투에 큰 도움이 될거야.";
    private string textInkFusion1 = "다른 색의 잉크가 일정 범위 이상으로 겹쳐치면 시너지가 발생해. ‘스플래시 잉크’로 강적에 맞설 시간이야";
    private string textInkFusion2 = "‘불바다’는 빨간 잉크와 초록 잉크가 겹쳐지면 발생해. 불바다 위에서 적들은 지속 피해를 받게 되지.";
    private string textInkFusion3 = "‘습지’는 초록 잉크와 파랑 잉크가 겹쳐지면 발생해. 나는 습지 위에서 잃은 체력을 회복할 수 있어.";
    private string textInkFusion4 = "마지막으로 안개야. 안개는 파랑색과 빨간색 잉크가 겹쳐지면 발생해. 안개의 적들을 순간적으로 앞이 안보여 우리를 공격 못하게 돼 자 이제 실전으로 들어가자";

    // B Image
    [SerializeField] private Sprite skillImg;
    [SerializeField] private List<Sprite> inkFusionImgs;


    //Tutorial
    private bool firstBasicAttack = false;
    private bool canBasicAttack = false;

    private bool firstInkDash = false;
    private bool canInkDash = false;

    private bool firstInkSkill = false;
    private bool canInkSkill = false;

    private bool firstStage = false;
    private bool canStage = false;


    private bool closed = false;

    private int textIndex;
    private int imgIndex;
    private int stageIndex = 0;
    
    private void Awake()
    {
        EventManager.Instance.AddListener(EVENT_TYPE.FirstBasicAttack, this);
        EventManager.Instance.AddListener(EVENT_TYPE.FirstInkDash, this);
        EventManager.Instance.AddListener(EVENT_TYPE.FirstInkSkill, this);
        EventManager.Instance.AddListener(EVENT_TYPE.Stage_Start, this);
    }

    public void SendA(string text, float duration)
    {
        var A = Instantiate(tutorialA, targetPanel.transform);
        A.GetComponentInChildren<TMP_Text>().text = text;
        Destroy(A, duration);
    }

    public GameObject SendANew(string text)
    {
        var A = Instantiate(tutorialA, targetPanel.transform);
        A.GetComponentInChildren<TMP_Text>().text = text;
        return A;
    }

    public void SendAToPageMap(float duration, GameObject pageMap)
    {
        var A = Instantiate(tutorialA, pageMap.transform);
        A.GetComponentInChildren<TMP_Text>().text = textPageSelection;
        Destroy(A, duration);
    }

    private void SendAToReward(string text, float duration, GameObject reward)
    {
        var A = Instantiate(tutorialA, reward.transform);
        A.GetComponentInChildren<TMP_Text>().text = text;
        Destroy(A, duration);
    }

    public void SendB(string text, Sprite img)
    {
        var B = Instantiate(tutorialB, targetPanel.transform);
        B.transform.GetChild(3).GetComponent<TMP_Text>().text = text;
        B.transform.GetChild(4).GetComponent<Image>().sprite = img;
        B.transform.GetChild(5).GetComponent<Button>().onClick.AddListener(() => Destroy(B));
        B.transform.GetChild(5).GetComponent<Button>().onClick.AddListener(() => closed = true);
    }

    public void SendBInk(List<string> text, List<Sprite> img)
    {
        textIndex = 0;
        imgIndex = 0;
        var B = Instantiate(tutorialB, targetPanel.transform);
        TMP_Text tempText = B.transform.GetChild(3).GetComponent<TMP_Text>();
        tempText.text = text[textIndex];

        B.transform.GetChild(4).GetComponent<Image>().sprite = img[imgIndex];

        B.transform.GetChild(4).GetChild(0).GetComponent<Button>().onClick.AddListener(() => ChangeImg(B, img[imgIndex > 0 ? --imgIndex : imgIndex]));
        B.transform.GetChild(4).GetChild(0).GetComponent<Button>().onClick.AddListener(() => ChangeText(tempText, text[textIndex > 0 ? --textIndex : textIndex]));
        B.transform.GetChild(4).GetChild(1).GetComponent<Button>().onClick.AddListener(() => ChangeImg(B, img[imgIndex >= img.Count-1 ? imgIndex : ++imgIndex]));
        B.transform.GetChild(4).GetChild(1).GetComponent<Button>().onClick.AddListener(() => ChangeText(tempText, text[textIndex >= text.Count - 1 ? textIndex : ++textIndex]));
        B.transform.GetChild(5).GetComponent<Button>().onClick.AddListener(() => Destroy(B));
        B.transform.GetChild(5).GetComponent<Button>().onClick.AddListener(() => Time.timeScale = 1f);
    }
    public void ChangeImg(GameObject B, Sprite img)
    {
        B.transform.GetChild(4).GetComponent<Image>().sprite = img;
    }

    public void ChangeText(TMP_Text t, string text)
    {
        t.text = text;
    }

    private void Start()
    {
        StartCoroutine(PlayTutorial());
    }

    IEnumerator PlayTutorial()
    {
        // Control
        SendA(textControl, 4f);
        
        // Basic Attack
        yield return new WaitForSeconds(5f);
        GameData.Instance.SpawnEnemies();
        SendA(textBasicAttack1, 2f);
        yield return new WaitForSeconds(2.5f);
        SendA(textBasicAttack2, 4f);

        canBasicAttack = true;
        yield return new WaitUntil(() => firstBasicAttack);
        SendA(textBasicAttack3, 2f);

        // Ink Dash
        yield return new WaitForSeconds(2.5f);
        GameObject tempobj = SendANew(textInkDash1);

        canInkDash = true;
        EventManager.Instance.PostNotification(EVENT_TYPE.InkDashWating, this);
        yield return new WaitUntil(() => firstInkDash);
        Destroy(tempobj);
        EventManager.Instance.PostNotification(EVENT_TYPE.InkDashTutorialCleared, this);
        SendA(textInkDash2, 3f);
        
        // Reward
        yield return new WaitForSeconds(4f);
        SendA(textReward1, 2f);
        yield return new WaitUntil(() => rewardPanel.activeInHierarchy);
        SendAToReward(textReward2, 1.5f, rewardPanel);
        yield return new WaitForSeconds(2f);
        SendAToReward(textReward3, 1f, rewardPanel);

        // Skill
        canStage = true;
        yield return new WaitUntil(() => firstStage);

        yield return new WaitForSeconds(1f);
        SendB(textInkSkill1, skillImg);
        Time.timeScale = 0f;

        yield return new WaitUntil(() => closed);
        closed = false;
        Time.timeScale = 1f;

        canInkSkill = true;
        EventManager.Instance.PostNotification(EVENT_TYPE.InkSkillWaiting, this);
        GameObject tempObj2 = SendANew(textInkSkill2);
        yield return new WaitUntil(() => firstInkSkill);
        EventManager.Instance.PostNotification(EVENT_TYPE.InkSkillTutorialCleared, this);
        Destroy(tempObj2);
        SendA(textInkSkill3, 3f);


        yield return new WaitUntil(() => stageIndex == 4);
        SendA(textInkFusion1, 3f);

        
        yield return new WaitForSeconds(2f);
        Time.timeScale = 0f;
        SendBInk(new List<string>() { textInkFusion2, textInkFusion3, textInkFusion4 }, inkFusionImgs);

    }

    public void OnEvent(EVENT_TYPE eventType, Component Sender, object Param = null)
    {
        switch (eventType)
        {
            case EVENT_TYPE.FirstBasicAttack:
                if (canBasicAttack)
                {
                    firstBasicAttack = true;
                    EventManager.Instance.RemoveListener(EVENT_TYPE.FirstBasicAttack, this);
                }
                break;
            case EVENT_TYPE.FirstInkDash:
                if (canInkDash)
                {
                    firstInkDash = true;
                    EventManager.Instance.RemoveListener(EVENT_TYPE.FirstInkDash, this);
                }
                break;
            case EVENT_TYPE.FirstInkSkill:
                if (canInkSkill)
                {
                    firstInkSkill = true;
                    EventManager.Instance.RemoveListener(EVENT_TYPE.FirstInkSkill, this);
                }
                break;
            case EVENT_TYPE.Stage_Start:
                if (canStage)
                    firstStage = true;
                stageIndex++;
                break;

        }
    }
}