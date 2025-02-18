using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopScript : MonoBehaviour
{
    [SerializeField]
    public Button selectButton;
    private Toggle toggle;
    private ScriptData scriptData;
    private ShopUIManager shopScriptManager;
    public int level;

    private ToggleGroup toggleGroup;

    [SerializeField]
    private Sprite[] purchaseBtnSprites;

    PlayerState playerState;

    Image[] images;
    TMP_Text[] texts;
    string tempText;

    private void Awake()
    {
        toggleGroup = GetComponentInParent<ToggleGroup>();
        images = GetComponentsInChildren<Image>();
        texts = GetComponentsInChildren<TMP_Text>();
        toggle = DebugUtils.GetComponentWithErrorLogging<Toggle>(transform, "Toggle");
        shopScriptManager = GameObject.Find("UIManager").GetComponent<ShopUIManager>();

        playerState = DebugUtils.GetComponentWithErrorLogging<PlayerState>(GameObject.FindWithTag("PLAYER"), "PlayerState");
    }
    private void OnEnable()
    {
        if (toggle != null)
        {
            Debug.Log("toggle ��Ȱ��ȭ");
            toggle.isOn = false;
            toggle.onValueChanged.AddListener(OnToggleValueChanged);
        }
    }

    private void OnDisable()
    {
        if (toggle != null)
        {
            toggle.onValueChanged.RemoveListener(OnToggleValueChanged);
            if (toggleGroup != null)
            {
                toggleGroup.allowSwitchOff = true;
                toggle.isOn = false;
                selectButton.interactable = false;
            }

        }
    }

    private void OnToggleValueChanged(bool isOn)
    {
        if (isOn)
        {
            if (scriptData == null) return;

            if (toggleGroup != null)
            {
                toggleGroup.allowSwitchOff = false;
            }

            images[2].color = new Color(images[2].color.r, images[2].color.b, images[2].color.r, 1.0f);
            for (int i = 0; i < texts.Length; i++)
            {
                texts[i].color = new Color(texts[i].color.r, texts[i].color.g, texts[i].color.b, 1.0f);
            }

          
            //Debug.Log($"������ ��ǰ ���� : {scriptData.price}  �÷��̾� ���� : {player.Coin}");
            
            // ���� ������ ���
            if (scriptData.price <= playerState.Coin)
            {
                selectButton.interactable = true;
                selectButton.GetComponent<Image>().sprite = purchaseBtnSprites[0];
                shopScriptManager.coinToMinus = scriptData.price;
                //Debug.Log("���Ű��� ��������Ʈ�� ����");
            }
            // ���� �Ұ����� ���
            else
            {
                selectButton.interactable = false;
                selectButton.GetComponent<Image>().sprite = purchaseBtnSprites[1];
                //Debug.Log("���źҰ��� ��������Ʈ�� ����");
            }

            shopScriptManager.SelectData = scriptData;
        }
        else
        {
            images[2].color = new Color(images[2].color.r, images[2].color.b, images[2].color.r, 183f / 255f);
            for (int i = 0; i < texts.Length; i++)
            {
                texts[i].color = new Color(texts[i].color.r, texts[i].color.g, texts[i].color.b, 183f / 255f);
            }
        }
    }

    public ScriptData ScriptData
    {
        get => scriptData; set
        {
            scriptData = value;
            SetScript();
        }
    }

    private void SetScript()
    {
        toggle.isOn = false;
        toggleGroup.allowSwitchOff = true;

        images = GetComponentsInChildren<Image>();
        images[0].sprite = ScriptData.scriptBG;
        images[1].sprite = ScriptData.scriptBG;
        images[2].sprite = ScriptData.scriptIcon;
        toggle.isOn = false;
        toggleGroup.allowSwitchOff = true;

        texts = GetComponentsInChildren<TMP_Text>();
        texts[0].text = ScriptData.scriptName;
        switch (ScriptData.scriptType)
        {
            case ScriptData.ScriptType.BASICATTACK:
                tempText = "�⺻����";
                break;
            case ScriptData.ScriptType.DASH:
                tempText = "��ũ���";
                break;
            case ScriptData.ScriptType.SKILL:
                tempText = "��ũ��ų";
                break;
            // ���ش� �߰�
            // ------------------------------------
            case ScriptData.ScriptType.PASSIVE:
                tempText = "�нú�";
                break;
            case ScriptData.ScriptType.MAGIC:
                tempText = "��ũ����";
                break;
            // --------------------------------
        }
        texts[1].text = tempText;
        tempText = ScriptData.scriptDesc.Replace("LevelData%", $"<color=red>{ScriptData.percentages[1] * 100}%</color>");
        texts[2].text = tempText;
        texts[3].text = ScriptData.price.ToString();
    }
}



