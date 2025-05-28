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
    private string textControl = "WASD�� ����������!";
    private string textBasicAttack1 = "���� ����ü�� �� ���� �׸��� �Ÿ��� �־�..";
    private string textBasicAttack2 = "���� ���콺 Ŭ������ ��������";
    private string textBasicAttack3 = "���߾�, �ٷ� �װž�!";
    private string textInkDash1 = "�̹��� space�ٸ� ���� ����ũ ��á��� ����غ���";
    private string textInkDash2 = "��~ ������ ������? ��ũ ��ô� ������ �ڸ��� ��� ��ũ�� ����";
    private string textReward1 = "�� ���� ���� �������� �Ѿ �ð��̾�";
    private string textReward2 = "�����ر��� ����, ��ũ��Ʈ�� ��ȭ �� �뺻�� �������̾�";
    private string textReward3 = "�� �� �ϳ��� �����ؼ� ��������! ���� ���� ���ϰ� ������ٰž�";
    private string textPageSelection = "���� ��θ� ��������. ��Ŭ������ �������� ������ ��";

    // B Text
    private string textInkSkill1 = "E Ű�� ��ų�� ����� �� �־�! EŰ�� ��� ���� ��� ���� ������� ���� ��������. �����Ѹ�ŭ ���� ���� ��ũ�� ����ϴϱ� ������ ����� �ʿ���";
    private string textInkSkill2 = "E Ű�� ������ ��ų�� ����غ�!";
    private string textInkSkill3 = "���Ҿ�! ��ũ ��ų�� ���� ������ ��ũ�� �����ϴµ� ������! �������� ������ ū ������ �ɰž�.";
    private string textInkFusion1 = "�ٸ� ���� ��ũ�� ���� ���� �̻����� ����ġ�� �ó����� �߻���. �����÷��� ��ũ���� ������ �¼� �ð��̾�";
    private string textInkFusion2 = "���ҹٴ١��� ���� ��ũ�� �ʷ� ��ũ�� �������� �߻���. �ҹٴ� ������ ������ ���� ���ظ� �ް� ����.";
    private string textInkFusion3 = "���������� �ʷ� ��ũ�� �Ķ� ��ũ�� �������� �߻���. ���� ���� ������ ���� ü���� ȸ���� �� �־�.";
    private string textInkFusion4 = "���������� �Ȱ���. �Ȱ��� �Ķ����� ������ ��ũ�� �������� �߻���. �Ȱ��� ������ ���������� ���� �Ⱥ��� �츮�� ���� ���ϰ� �� �� ���� �������� ����";

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