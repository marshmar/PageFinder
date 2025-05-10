using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleUIManager : MonoBehaviour
{
    [SerializeField]
    private Canvas BattleUICanvas;

    [SerializeField]
    private TMP_Text pageTypeTxt;

    [SerializeField]
    private GameObject goalContentImg;
    [SerializeField]
    private TMP_Text goalContentTxt;

    [SerializeField]
    private GameObject goalDetailContentImg;
    [SerializeField]
    private TMP_Text goalDetailContentTxt;

    bool isBattlePage;
    NodeType nodeType;

    // ���� Ȱ��ȭ ����
    public void SetBattleUICanvasState(bool value, bool isSetting)
    {
        //BattleUICanvas.gameObject.SetActive(value);

        //if (!value)
        //    return;

        //// �������� �� ���� ���� ���� ȭ���� Battle�� ��ȯ�� ���� �̹� BattleUI�� ���� �����Ƿ� ���� �Լ� �������� �ʰ� ����
        //if (isSetting)
        //    return;

        //bool isBattle = GameData.Instance.GetCurrPageType() == PageType.BATTLE;

        //// �»�� Ÿ��Ʋ �̸� ����
        //SetPageTypeTxt(isBattle ? "��Ʋ ������" : "�������� ������");

        //// �߾�, ���� ��ǥ ����
        //StartCoroutine(SetGoalData());
    }

    private void OnEnable()
    {
        (isBattlePage, nodeType) = GameData.Instance.isBattlePage();

        SetPageTypeTxt(isBattlePage ? "��Ʋ ������" : "�������� ������");
        StartCoroutine(SetGoalData());
    }

    private void SetPageTypeTxt(string value)
    {
        pageTypeTxt.text = value;
    }

    IEnumerator SetGoalData()
    {
        SetGoalContent(true);
        SetGoalDetailContent(false);

        float currentTime = 0.0f;
        float goalContentFadeTime = 2f;

        while (currentTime / goalContentFadeTime < 1)
        {
            currentTime += Time.deltaTime;

            goalContentImg.GetComponent<Image>().color = new Color(1, 1, 1, Mathf.Lerp(1, 0, currentTime / goalContentFadeTime));
            goalContentTxt.color = new Color(169/255.0f, 109/255.0f, 79/255.0f, Mathf.Lerp(1, 0, currentTime / goalContentFadeTime));
            yield return null;
        }

        SetGoalContent(false);
        SetGoalDetailContent(true);
    }

    /// <summary>
    /// ��Ʋ������ ���۽� ȭ�� �߾ӿ� �ߴ� ��ǥ ���� ����
    /// </summary>
    /// <param name="value"></param>
    void SetGoalContent(bool value)
    {
        goalContentImg.SetActive(value);

        if (!value) return;
       
        goalContentImg.GetComponent<Image>().color = Color.white;
        goalContentTxt.color = new Color(169 / 255.0f, 109 / 255.0f, 79 / 255.0f, 1);

        if (isBattlePage)
        {
            if(nodeType == NodeType.Boss) goalContentTxt.text = $"������ óġ�ϼ���!";
            else goalContentTxt.text = $"��� ���� óġ�ϼ���!";
        }
        else goalContentTxt.text = $"���� �ٸ� ����縦 óġ�ϼ���!";
    }

    /// <summary>
    /// ��Ʋ������ ������ ȭ�� ������ �ߴ� ���� ��ǥ ���� ����
    /// </summary>
    /// <param name="value"></param>
    void SetGoalDetailContent(bool value)
    {
        goalDetailContentImg.SetActive(value);
        goalDetailContentTxt.fontStyle = FontStyles.Normal;
        if (!value) return;

        if (isBattlePage)
        {
            if (nodeType == NodeType.Boss) goalDetailContentTxt.text = $"������ óġ�ϱ�!";
            else goalDetailContentTxt.text = $"��� ���� óġ�ϱ�!";
        }
        else goalDetailContentTxt.text = $"���� �ٸ�\n����� óġ�ϱ�!";
    }

    /// <summary>
    /// ���� ��ư ���� �� ����
    /// </summary>
    public void ActivateSettingUI()
    {
        // ToDo: UI Changed;
        //EventManager.Instance.PostNotification(EVENT_TYPE.UI_Changed, this, UIType.Setting);
    }
}