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

    [SerializeField]
    private Sprite[] purchaseBtnSprites;

    [SerializeField]
    Player player;

    Image[] images;
    TMP_Text[] texts;
    string tempText;

    [SerializeField]
    ShopUIManager shopUIManager;

    private void Awake()
    {
        toggle = DebugUtils.GetComponentWithErrorLogging<Toggle>(transform, "Toggle");
        shopScriptManager = GameObject.Find("UIManager").GetComponent<ShopUIManager>();
    }
    private void OnEnable()
    {
        if (toggle != null)
        {
            toggle.onValueChanged.AddListener(OnToggleValueChanged);
        }
    }

    private void OnDisable()
    {
        if (toggle != null)
        {
            toggle.onValueChanged.RemoveListener(OnToggleValueChanged);
        }
    }

    private void OnToggleValueChanged(bool isOn)
    {
        if (isOn)
        {
            images[2].color = new Color(images[2].color.r, images[2].color.b, images[2].color.r, 1.0f);
            for (int i = 0; i < texts.Length; i++)
            {
                texts[i].color = new Color(texts[i].color.r, texts[i].color.g, texts[i].color.b, 1.0f);
            }
            
            // 구매가능한 경우
            if (scriptData.price < player.Coin)
            {
                selectButton.interactable = false;
                selectButton.GetComponent<Image>().sprite = purchaseBtnSprites[1];
                shopUIManager.coinToMinus = scriptData.price;
            }
            else
            {
                selectButton.interactable = true;
                selectButton.GetComponent<Image>().sprite = purchaseBtnSprites[0];
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
        images = GetComponentsInChildren<Image>();
        images[0].sprite = ScriptData.scriptBG;
        images[1].sprite = ScriptData.scriptBG;
        images[2].sprite = ScriptData.scriptIcon;

        texts = GetComponentsInChildren<TMP_Text>();
        texts[0].text = ScriptData.scriptName;
        switch (ScriptData.scriptType)
        {
            case ScriptData.ScriptType.BASICATTACK:
                tempText = "기본공격";
                break;
            case ScriptData.ScriptType.DASH:
                tempText = "잉크대시";
                break;
            case ScriptData.ScriptType.SKILL:
                tempText = "잉크스킬";
                break;
            case ScriptData.ScriptType.COMMON:
                tempText = "공용";
                break;
        }
        texts[1].text = tempText;
        tempText = ScriptData.scriptDesc.Replace("LevelData%", $"<color=red>{ScriptData.percentages[1] * 100}%</color>");
        texts[2].text = tempText;
    }
}



