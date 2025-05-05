using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DiaryElement : MonoBehaviour
{
    protected ScriptData scriptData;
    [SerializeField] protected GameObject scriptDescriptionObject;
    [SerializeField] protected Image backgroundImage;

    protected Image[] scriptDescriptionImages;
    protected TMP_Text[] scriptDescriptionTexts;
    protected Toggle toggle;
    [SerializeField] protected Image icon;
    [SerializeField] protected Sprite[] backGroundImages;

    public virtual ScriptData ScriptData { 
        get => scriptData; 
        set{
            scriptData = value;
            if(value == null) toggle.interactable = false;
            else
            {
                toggle.interactable = true;
                SetScriptPanels();
            }
        }  
    }

    public virtual void Awake()
    {
        toggle = DebugUtils.GetComponentWithErrorLogging<Toggle>(this.gameObject, "Toggle");
    }

    protected void OnEnable()
    {
        if (toggle != null)
        {
            toggle.isOn = false;
            toggle.onValueChanged.AddListener(OnToggleValueChanged);
            if (scriptData == null) toggle.interactable = false;
            else toggle.interactable = true;
        }
    }

    protected void OnDisable()
    {
        if (toggle != null)
        {
            toggle.onValueChanged.RemoveListener(OnToggleValueChanged);
            // 좌측 스크립트 상세 설명 오브젝트 비활성화
            scriptDescriptionObject.SetActive(false);
            // 전체 배경을 선택 안된 배경으로 변경
            backgroundImage.sprite = backGroundImages[0];
        }
    }

    public virtual void OnToggleValueChanged(bool isOn)
    {
        if (isOn)
        {
            if (scriptData == null) return;
            scriptDescriptionObject.SetActive(true);
            backgroundImage.sprite = backGroundImages[1];
            SetScriptDescription();
        }
        else
        {
            if (scriptData == null) return;

            backgroundImage.sprite = backGroundImages[0];
            scriptDescriptionObject.SetActive(false);
        }
    }

    public virtual void SetScriptPanels()
    {
        icon.sprite = scriptData.scriptIcon;
    }

    public virtual void SetScriptDescription()
    {
        if(!DebugUtils.CheckIsNullWithErrorLogging<ScriptData>(scriptData))
        {
            scriptDescriptionImages = scriptDescriptionObject.GetComponentsInChildren<Image>();
            scriptDescriptionImages[0].sprite = scriptData.scriptBG;
            scriptDescriptionImages[1].sprite = scriptData.scriptIcon;

            scriptDescriptionTexts = scriptDescriptionObject.GetComponentsInChildren<TMP_Text>();

            string tempText = null;
            switch (scriptData.scriptType)
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
            scriptDescriptionTexts[1].text = tempText;
            if(scriptData.level <= 0)
            {               
                tempText = scriptData.scriptDesc.Replace("LevelData%", $"<color=red>{scriptData.percentages[0] * 100}%</color>");
                scriptDescriptionTexts[0].text = scriptData.scriptName;
            }
            else
            {
                tempText = scriptData.scriptDesc.Replace("LevelData%", $"<color=red>{scriptData.percentages[scriptData.level] * 100}%</color>");
                scriptDescriptionTexts[0].text = scriptData.scriptName + $" +{scriptData.level}";
            }

            scriptDescriptionTexts[2].text = tempText;
        }
    }
}