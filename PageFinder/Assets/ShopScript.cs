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

    [SerializeField]
    // 강해담 수정: player -> playerState
    //Player player;
    PlayerState playerState;

    Image[] images;
    TMP_Text[] texts;
    string tempText;

    [SerializeField]
    ShopUIManager shopUIManager;

    private void Awake()
    {
        toggleGroup = GetComponentInParent<ToggleGroup>();
        images = GetComponentsInChildren<Image>();
        texts = GetComponentsInChildren<TMP_Text>();
        toggle = DebugUtils.GetComponentWithErrorLogging<Toggle>(transform, "Toggle");
        shopScriptManager = GameObject.Find("UIManager").GetComponent<ShopUIManager>();

        GameObject playerObj = GameObject.FindWithTag("PLAYER");
        playerState = DebugUtils.GetComponentWithErrorLogging<PlayerState>(playerObj, "PlayerState");
    }
    private void OnEnable()
    {
        if (toggle != null)
        {
            Debug.Log("toggle 비활성화");
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

          
            //Debug.Log($"선택한 상품 가격 : {scriptData.price}  플레이어 코인 : {player.Coin}");
            
            // 구매 가능한 경우
            if (scriptData.price <= playerState.Coin)
            {
                selectButton.interactable = true;
                selectButton.GetComponent<Image>().sprite = purchaseBtnSprites[0];
                shopUIManager.coinToMinus = scriptData.price;
                //Debug.Log("구매가능 스프라이트로 변경");
            }
            // 구매 불가능한 경우
            else
            {
                selectButton.interactable = false;
                selectButton.GetComponent<Image>().sprite = purchaseBtnSprites[1];
                //Debug.Log("구매불가능 스프라이트로 변경");
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
                tempText = "기본공격";
                break;
            case ScriptData.ScriptType.DASH:
                tempText = "잉크대시";
                break;
            case ScriptData.ScriptType.SKILL:
                tempText = "잉크스킬";
                break;
            // 강해담 추가
            // ------------------------------------
            case ScriptData.ScriptType.PASSIVE:
                tempText = "패시브";
                break;
            case ScriptData.ScriptType.MAGIC:
                tempText = "잉크매직";
                break;
            // --------------------------------
        }
        texts[1].text = tempText;
        tempText = ScriptData.scriptDesc.Replace("LevelData%", $"<color=red>{ScriptData.percentages[1] * 100}%</color>");
        texts[2].text = tempText;
        texts[3].text = ScriptData.price.ToString();
    }
}



