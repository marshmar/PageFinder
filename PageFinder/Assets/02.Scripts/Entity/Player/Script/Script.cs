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
    private PlayerState playerState;
    private ShopUIManager shopUIManager;
    private ScriptManager scriptManager;

    public int level;

    public ScriptData ScriptData { get => scriptData; set { scriptData = value; SetScript(); } }

    private void Awake()
    {
        toggleGroup = GetComponentInParent<ToggleGroup>();
        toggle = GetComponent<Toggle>();
        if (toggle != null && toggleGroup != null) toggleMode = true;
        images = GetComponentsInChildren<Image>();
        texts = GetComponentsInChildren<TMP_Text>();
        scriptManager = GameObject.Find("UIManager").GetComponent<ScriptManager>();
        playerState = DebugUtils.GetComponentWithErrorLogging<PlayerState>(GameObject.FindWithTag("PLAYER"), "PlayerState");
        shopUIManager = GameObject.Find("UIManager").GetComponent<ShopUIManager>();
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
        if (isOn)
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
                scriptManager.SelectData = scriptData;
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

    private void SetScript()
    {
        if (toggleMode)
        {
            toggle.isOn = false;
            toggleGroup.allowSwitchOff = true;
        }

        images = GetComponentsInChildren<Image>();
        images[0].sprite = ScriptData.scriptBG;
        images[1].sprite = ScriptData.scriptBG;
        images[2].sprite = ScriptData.scriptIcon;

        texts = GetComponentsInChildren<TMP_Text>();
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
            case ScriptData.ScriptType.PASSIVE:
                tempText = "패시브";
                break;
            case ScriptData.ScriptType.MAGIC:
                tempText = "잉크매직";
                break;
        }
        texts[1].text = tempText;
        if (level <= 0)
        {
            texts[0].text = ScriptData.scriptName;
            tempText = ScriptData.scriptDesc.Replace("LevelData%", $"<color=red>{ScriptData.percentages[0] * 100}%</color>");
        }
        else
        {
            texts[0].text =  ScriptData.scriptName  + $" +{level}";
            tempText = ScriptData.scriptDesc.Replace("LevelData%", $"<color=red>{ScriptData.percentages[level] * 100}%</color>");
        }
        texts[2].text = tempText;
        if(isShopScript) texts[3].text = ScriptData.price.ToString();
    }
}