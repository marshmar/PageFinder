using Google.GData.AccessControl;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;


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

    // ��ǥ�� ������� ������ ��츦 ������ �ڷ�ƾ ȣ���ؾ��ϴµ� ���� �ȵ�.


    private void OnEnable()
    {
        bool isBattle = false;
        NodeType currNodeType = GameData.Instance.GetCurrPageType();
        if(currNodeType == NodeType.Start || currNodeType == NodeType.Battle_Elite
            || currNodeType == NodeType.Battle_Normal || currNodeType == NodeType.Boss)
        {
            isBattle = true;
        }

        SetPageTypeTxt(isBattle ? "��Ʋ ������" : "�������� ������");
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

        if (!value)
            return;
       
        goalContentImg.GetComponent<Image>().color = Color.white;
        goalContentTxt.color = new Color(169 / 255.0f, 109 / 255.0f, 79 / 255.0f, 1);

        if(GameData.Instance.isBattlePage())
            goalContentTxt.text = $"��� ���� óġ�ϼ���!";
        else
            goalContentTxt.text = $"���� �ٸ� ����縦 óġ�ϼ���!";
    }

    /// <summary>
    /// ��Ʋ������ ������ ȭ�� ������ �ߴ� ���� ��ǥ ���� ����
    /// </summary>
    /// <param name="value"></param>
    void SetGoalDetailContent(bool value)
    {
        goalDetailContentImg.SetActive(value);
        goalDetailContentTxt.fontStyle = FontStyles.Normal;
        if (!value)
            return;

        if (GameData.Instance.isBattlePage())
            goalDetailContentTxt.text = $"��� ���� óġ�ϱ�"; // ex ) ��θӸ� ����縦 óġ�ϱ�
        else
            goalDetailContentTxt.text = $"���� �ٸ�\n����� óġ�ϱ�"; // ex ) ��θӸ� ����縦 óġ�ϱ�
    }

    /// <summary>
    /// ���� ��ư ���� �� ����
    /// </summary>
    public void ActivateSettingUI()
    {
        EventManager.Instance.PostNotification(EVENT_TYPE.UI_Changed, this, UIType.Setting);
    }
}
