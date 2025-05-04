using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Script : MonoBehaviour
{
    [SerializeField] private bool toggleMode;
    [SerializeField] private Button selectButton;
    private string tempText;
    private Toggle toggle;
    private Image[] images;
    private TMP_Text[] texts;
    private ToggleGroup toggleGroup;
    private ScriptData scriptData;
    private ScriptManager scriptManagerScr;

    public int level;

    private void Awake()
    {
        if (toggleMode)
        {
            toggleGroup = GetComponentInParent<ToggleGroup>();
            toggle = DebugUtils.GetComponentWithErrorLogging<Toggle>(transform, "Toggle");
        }
        images = GetComponentsInChildren<Image>();
        texts = GetComponentsInChildren<TMP_Text>();
        scriptManagerScr = GameObject.Find("UIManager").GetComponent<ScriptManager>();
    }

    private void OnEnable()
    {
        if (toggle != null)
        {
            toggle.isOn = false;
            toggle.onValueChanged.AddListener(OnToggleValueChanged);
        }
    }

    private void OnDisable()
    {
        if (toggle != null)
        {
            toggle.onValueChanged.RemoveListener(OnToggleValueChanged);
            if(toggleGroup!= null)
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
            if(toggleGroup != null) toggleGroup.allowSwitchOff = false;

            images[2].color = new Color(images[2].color.r, images[2].color.b, images[2].color.r, 1.0f);
            for (int i = 0; i < texts.Length; i++)
            {
                texts[i].color = new Color(texts[i].color.r, texts[i].color.g, texts[i].color.b, 1.0f);
            }
            selectButton.interactable = true;
            scriptManagerScr.SelectData = scriptData;
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
    
    public ScriptData ScriptData { get => scriptData; set { 
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

        texts = GetComponentsInChildren<TMP_Text>();
        //texts[0].text = ScriptData.scriptName;
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
        //tempText = ScriptData.scriptDesc.Replace("LevelData%", $"<color=red>{ScriptData.percentages[0] * 100}%</color>");
        texts[2].text = tempText;
    }
}