using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Script : MonoBehaviour
{
    [SerializeField] private Button selectButton;
    [SerializeField] private Sprite[] purchaseBtnSprites;
    [SerializeField] private bool isShopScript;
    
    private bool toggleMode = false;
    private string tempText;
    private Toggle toggle;
    private Image[] images;
    private TMP_Text[] texts;
    private ToggleGroup toggleGroup;
    private ScriptData scriptData;
    private NewScriptData newScriptData;
    private ScriptSystemData scriptSystemData;
    private PlayerState playerState;
    [SerializeField] private ShopUIManager shopUIManager;
    [SerializeField] private RewardPanelManager rewardPanelManager;
    //[SerializeField] private ScriptManager scriptManager;

    public ScriptData ScriptData { get => scriptData; set { scriptData = value;/* SetScript();*/ } }
    public NewScriptData NewScriptData { get => newScriptData; set { newScriptData = value; } }

    public ScriptSystemData ScriptSystemData { get => scriptSystemData; set => scriptSystemData = value; }

    private void Awake()
    {
        toggleGroup = GetComponentInParent<ToggleGroup>();
        toggle = GetComponent<Toggle>();
        if (toggle != null && toggleGroup != null) toggleMode = true;
        images = GetComponentsInChildren<Image>();
        texts = GetComponentsInChildren<TMP_Text>();
        playerState = DebugUtils.GetComponentWithErrorLogging<PlayerState>(GameObject.FindWithTag("PLAYER"), "PlayerState");
        //shopUIManager = GameObject.Find("UIManager").GetComponent<ShopUIManager>();
    }

    private void OnEnable()
    {
        if (toggleMode)
        {
            toggle.isOn = false;
            toggle.onValueChanged.AddListener(OnToggleValueChanged);
        }
    }

    private void OnDisable()
    {
        if (toggleMode)
        {
            toggle.onValueChanged.RemoveListener(OnToggleValueChanged);
            toggleGroup.allowSwitchOff = true;
            toggle.isOn = false;
            selectButton.interactable = false;
        }
    }

    private void OnToggleValueChanged(bool isOn)
    {
        /*        if (isOn)
                {
                    if (scriptData == null) return;
                    if(toggleMode) toggleGroup.allowSwitchOff = false;

                    images[2].color = new Color(images[2].color.r, images[2].color.b, images[2].color.r, 1.0f);
                    for (int i = 0; i < texts.Length; i++)
                    {
                        texts[i].color = new Color(texts[i].color.r, texts[i].color.g, texts[i].color.b, 1.0f);
                    }

                    if (isShopScript)
                    {
                        // When purchase is possible
                        if (scriptData.price <= playerState.Coin)
                        {
                            selectButton.interactable = true;
                            selectButton.GetComponent<Image>().sprite = purchaseBtnSprites[0];
                            shopUIManager.coinToMinus = scriptData.price;
                            //Debug.Log("Change to a purchasable sprite");
                        }
                        // When purchase is not possible
                        else
                        {
                            selectButton.interactable = false;
                            selectButton.GetComponent<Image>().sprite = purchaseBtnSprites[1];
                            //Debug.Log("Change to unpurchasable sprite");
                        }
                        shopUIManager.SelectData = scriptData;
                    }
                    else
                    {
                        selectButton.interactable = true;
                        rewardPanelManager.SelectData = scriptData;
                    }
                }
                else
                {
                    images[2].color = new Color(images[2].color.r, images[2].color.b, images[2].color.r, 183f / 255f);
                    for (int i = 0; i < texts.Length; i++)
                    {
                        texts[i].color = new Color(texts[i].color.r, texts[i].color.g, texts[i].color.b, 183f / 255f);
                    }
                }*/

        if (isOn)
        {
            if (scriptSystemData == null) return;
            if (toggleMode) toggleGroup.allowSwitchOff = false;

            images[2].color = new Color(images[2].color.r, images[2].color.b, images[2].color.r, 1.0f);
            for (int i = 0; i < texts.Length; i++)
            {
                texts[i].color = new Color(texts[i].color.r, texts[i].color.g, texts[i].color.b, 1.0f);
            }

            if (isShopScript)
            {
                // When purchase is possible
                if(scriptSystemData is NewScriptData scriptData)
                {
                    if (scriptData.price[scriptData.rarity] <= playerState.Coin)
                    {
                        selectButton.interactable = true;
                        selectButton.GetComponent<Image>().sprite = purchaseBtnSprites[0];
                        shopUIManager.coinToMinus = scriptData.price[scriptData.rarity];
                        //Debug.Log("Change to a purchasable sprite");
                    }
                    // When purchase is not possible
                    else
                    {
                        selectButton.interactable = false;
                        selectButton.GetComponent<Image>().sprite = purchaseBtnSprites[1];
                        //Debug.Log("Change to unpurchasable sprite");
                    }
                }
                else if(scriptSystemData is StickerData stickerData)
                {
                    if (stickerData.price[stickerData.rarity] <= playerState.Coin)
                    {
                        selectButton.interactable = true;
                        selectButton.GetComponent<Image>().sprite = purchaseBtnSprites[0];
                        shopUIManager.coinToMinus = stickerData.price[stickerData.rarity];
                        //Debug.Log("Change to a purchasable sprite");
                    }
                    // When purchase is not possible
                    else
                    {
                        selectButton.interactable = false;
                        selectButton.GetComponent<Image>().sprite = purchaseBtnSprites[1];
                        //Debug.Log("Change to unpurchasable sprite");
                    }
                }
                shopUIManager.SelectedData = scriptSystemData;
            }
            else
            {
                selectButton.interactable = true;
                rewardPanelManager.SelectedData = scriptSystemData;
            }
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

    public void SetScriptUI()
    {
        if (toggleMode)
        {
            toggle.isOn = false;
            toggleGroup.allowSwitchOff = true;
        }

        images = GetComponentsInChildren<Image>();
        images[0].sprite = scriptData.scriptBG;
        images[1].sprite = scriptData.scriptBG;
        images[2].sprite = scriptData.scriptIcon;

        texts = GetComponentsInChildren<TMP_Text>();
        switch (scriptData.scriptType)
        {
            case ScriptData.ScriptType.BasicAttack:
                tempText = "기본공격";
                break;
            case ScriptData.ScriptType.Dash:
                tempText = "잉크대시";
                break;
            case ScriptData.ScriptType.Skill:
                tempText = "잉크스킬";
                break;
            case ScriptData.ScriptType.Passive:
                tempText = "패시브";
                break;
        }
        texts[1].text = tempText;
        if (scriptData.level <= 0)
        {
            texts[0].text = scriptData.scriptName;
            tempText = scriptData.scriptDesc.Replace("LevelData%", $"<color=red>{scriptData.percentages[0] * 100}%</color>");
        }
        else
        {
            texts[0].text = scriptData.scriptName  + $" +{scriptData.level}";
            tempText = scriptData.scriptDesc.Replace("LevelData%", $"<color=red>{scriptData.percentages[scriptData.level] * 100}%</color>");
        }
        texts[2].text = tempText;
        if(isShopScript) texts[3].text = scriptData.price.ToString();
    }

    public void SetScriptUINew()
    {
        if (toggleMode)
        {
            toggle.isOn = false;
            toggleGroup.allowSwitchOff = true;
        }

        images = GetComponentsInChildren<Image>();

        if(scriptSystemData is NewScriptData scriptData)
        {
            SetUIForScript(scriptData);
        }
        else if(scriptSystemData is StickerData stickerData)
        {
            SetUIForSticker(stickerData);
        }

    }

    private void SetUIForScript(NewScriptData scriptData)
    {
        images[0].sprite = ScriptSystemManager.Instance.GetScriptBackground(scriptData.inkType);
        images[1].sprite = ScriptSystemManager.Instance.GetScriptBackground(scriptData.inkType);
        images[2].sprite = ScriptSystemManager.Instance.GetScriptIconByScriptTypeAndInkType(scriptData.scriptType, scriptData.inkType);

        texts = GetComponentsInChildren<TMP_Text>();
        switch (scriptData.scriptType)
        {
            case NewScriptData.ScriptType.BasicAttack:
                tempText = "기본공격";
                break;
            case NewScriptData.ScriptType.Dash:
                tempText = "잉크대시";
                break;
            case NewScriptData.ScriptType.Skill:
                tempText = "잉크스킬";
                break;
        }
        texts[1].text = tempText;
        texts[0].text = scriptData.scriptName + $" +{scriptData.rarity}";

        tempText = scriptData.scriptDesc[scriptData.rarity];
        if (scriptData.rarity == 0)
        {
            tempText = tempText.Replace("%RED%", $"<color=red>빨강</color>");
            tempText = tempText.Replace("%GREEN%", $"<color=green>초록</color>");
            tempText = tempText.Replace("%BLUE%", $"<color=blue>파랑</color>");
        }
        texts[2].text = tempText;
        Color textColor;
        if (ColorUtility.TryParseHtmlString("#3F3A36", out textColor))
        {
            texts[2].color = textColor;
        }
        if (isShopScript) texts[3].text = scriptData.price[scriptData.rarity].ToString();
    }

    private void SetUIForSticker(StickerData stickerData)
    {
        images[0].sprite = ScriptSystemManager.Instance.GetStickerBackground();
        images[1].sprite = ScriptSystemManager.Instance.GetStickerBackground();
        images[2].sprite = ScriptSystemManager.Instance.GetStickerIconByID(stickerData.stickerID);

        texts = GetComponentsInChildren<TMP_Text>();
        if(stickerData.stickerType == StickerType.General)
        {
            tempText = "공용";
        }
        else
        {
            switch (stickerData.dedicatedScriptTarget)
            {
                case NewScriptData.ScriptType.BasicAttack:
                    tempText = "기본공격";
                    break;
                case NewScriptData.ScriptType.Dash:
                    tempText = "잉크대시";
                    break;
                case NewScriptData.ScriptType.Skill:
                    tempText = "잉크스킬";
                    break;
            }
        }
        
        texts[1].text = tempText;
        texts[0].text = stickerData.stickerName + $" +{stickerData.rarity}";

        tempText = stickerData.stickerDesc.Replace("%LevelData%", $"<color=red>{stickerData.levelData[stickerData.rarity] * 100}%</color>");
        texts[2].text = tempText;
        Color textColor;
        if (ColorUtility.TryParseHtmlString("#FFFFFF", out textColor))
        {
            texts[2].color = textColor;
        }


        if (isShopScript) texts[3].text = stickerData.price[stickerData.rarity].ToString();
    }
}