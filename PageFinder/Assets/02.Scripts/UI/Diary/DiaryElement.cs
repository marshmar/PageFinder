using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public enum DiaryElementType
{
    Script,
    Sticker
}

public class DiaryElement : MonoBehaviour
{
    private bool synthesisMode = false;
    private Sprite originSprite;
    [SerializeField] private CommaPanelManager commaPanelManager;
    protected ScriptData scriptData;
    protected NewScriptData newScriptData;
    //protected ScriptSystemData scriptSystemData;

    protected BaseScript script;
    protected Sticker sticker;
    public DiaryElementType elementType;

    [SerializeField] protected GameObject scriptDescriptionObject;
    [SerializeField] protected Image backgroundImage;
    [SerializeField] protected Image iconBg;

    protected Image[] scriptDescriptionImages;
    protected TMP_Text[] scriptDescriptionTexts;
    protected Toggle toggle;
    [SerializeField] protected Image icon;
    [SerializeField] protected Sprite[] backGroundImages;
    [SerializeField] protected DiaryManager diaryManager;

    protected DraggableUI draggableUI;
    protected Sprite defaultIcon;

    public virtual ScriptData ScriptData {
        get => scriptData; 
        set{
            scriptData = value;
            if (value == null)
            {
                toggle.interactable = false;
            }
            else
            {
                toggle.interactable = true;
                SetScriptPanels();
            }
        }  
    }

    public virtual NewScriptData NewScriptData
    {
        get => newScriptData;
        set
        {
            newScriptData = value;
            if(value == null)
            {
                toggle.interactable = false;
                icon.sprite = null;
            }
            else
            {
                toggle.interactable = true;
                SetScriptPanelsNew();
            }
        }
    }

/*    public virtual ScriptSystemData ScriptSystemData
    {
        get => scriptSystemData;
        set
        {
            scriptSystemData = value;
            if (value == null)
            {
                toggle.interactable = false;
                if (iconBg != null)
                    iconBg.color = new Color(iconBg.color.r, iconBg.color.g, iconBg.color.b, 76f);
            }
            else
            {
                toggle.interactable = true;
                SetScriptPanelsNew();
                if (iconBg != null)
                    iconBg.color = new Color(iconBg.color.r, iconBg.color.g, iconBg.color.b, 200f);
            }
        }
    }*/

    public virtual BaseScript Script
    {
        get => script;
        set
        {
            script = value;
            if (value == null)
            {
                toggle.interactable = false;
            }

            else
            {
                toggle.interactable = true;
                SetScriptPanelsNew();
            }
        }
    }
    public virtual Sticker Sticker
    {
        get => sticker;
        set
        {
            sticker = value;
            if (value == null)
            {
                toggle.interactable = false;
                if(draggableUI != null)
                {
                    draggableUI.canDrag = false;
                }
                icon.color = new Color(icon.color.r, icon.color.g, icon.color.b, 0f);
                icon.sprite = null;
            }

            else
            {
                toggle.interactable = true;
                if (draggableUI != null)
                {
                    draggableUI.canDrag = true;
                }
                SetScriptPanelsNew();
            }
        }
    }

    public virtual void Awake()
    {
        toggle = DebugUtils.GetComponentWithErrorLogging<Toggle>(this.gameObject, "Toggle");
        draggableUI = GetComponent<DraggableUI>();
        diaryManager = GetComponentInParent<DiaryManager>();
        defaultIcon = GetComponent<Image>().sprite;

        if(draggableUI != null)
        {
            draggableUI.dropSuccessEvent += ResetElement;
            draggableUI.dropSuccessEvent += diaryManager.SetDiaryStickers;

        }
        if (scriptDescriptionObject == null) synthesisMode = true;
    }

    protected void OnEnable()
    {
        if (toggle != null)
        {
            toggle.isOn = false;
            toggle.onValueChanged.AddListener(OnToggleValueChanged);
/*            if (scriptSystemData == null)
            {
                toggle.interactable = false;

            }
            else { 
                toggle.interactable = true;
            }*/
        }
    }

    protected void OnDisable()
    {
        if (toggle != null)
        {
            toggle.onValueChanged.RemoveListener(OnToggleValueChanged);
            
            if (!synthesisMode)
            {
                // Disable left script detail object
                scriptDescriptionObject.SetActive(false);
                // Change entire background to unselected background
                //backgroundImage.sprite = backGroundImages[0];
            }

            if (draggableUI != null)
            {
                draggableUI.beginDragEvent -= () => SetIconObjectState(false);
                draggableUI.dropFailEvent -= () => SetIconObjectState(true);
            }
        }
    }

    public virtual void OnToggleValueChanged(bool isOn)
    {
        if (isOn)
        {
            if (commaPanelManager != null && commaPanelManager.GetScriptCount() >= 3) return; 
            if (synthesisMode) commaPanelManager.AddSticker(sticker);

            if (elementType == DiaryElementType.Script && script == null) return;
            if (elementType == DiaryElementType.Sticker && sticker == null) return;
            if (synthesisMode) return;

            //if (scriptSystemData == null || synthesisMode) return;
            scriptDescriptionObject.SetActive(true);
            //backgroundImage.sprite = backGroundImages[1];
            //SetScriptDescription();

            switch (elementType)
            {
                case DiaryElementType.Script:
                    SetScriptDescriptionNew(script.GetCopiedData());
                    break;
                case DiaryElementType.Sticker:
                    SetStickerDescription(sticker.GetCopiedData());
                    break;
            }
/*
            if (scriptSystemData is NewScriptData newScriptData)
                SetScriptDescriptionNew(newScriptData);
            else if (scriptSystemData is StickerData stickerData)
                SetStickerDescription(stickerData);*/
        }
        else
        {
            if (synthesisMode) commaPanelManager.RemoveSticker(sticker);
            if (elementType == DiaryElementType.Script && script == null) return;
            if (elementType == DiaryElementType.Sticker && sticker == null) return;
            if (synthesisMode) return;
            //if (newScriptData == null || synthesisMode) return;
            //backgroundImage.sprite = backGroundImages[0];
            scriptDescriptionObject.SetActive(false);
        }
    }

    public virtual void SetScriptPanels()
    {
        icon.sprite = scriptData.scriptIcon;
    }

    public virtual void SetScriptPanelsNew()
    {
        switch (elementType)
        {
            case DiaryElementType.Script:
                NewScriptData newScriptData = script.GetCopiedData();
                icon.sprite = ScriptSystemManager.Instance.GetScriptIconByScriptTypeAndInkType(newScriptData.scriptType, newScriptData.inkType);
                break;
            case DiaryElementType.Sticker:
                StickerData stickerData = sticker.GetCopiedData();
                icon.sprite = ScriptSystemManager.Instance.GetStickerIconByID(stickerData.stickerID);
                icon.color = new Color(icon.color.r, icon.color.g, icon.color.b, 200f);
                if (draggableUI != null)
                {
                    draggableUI.dragImg = icon.sprite;
                    draggableUI.beginDragEvent += () => SetIconObjectState(false);
                    draggableUI.dropFailEvent += () => SetIconObjectState(true);
                }
                break;
        }
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

    public void SetScriptDescriptionNew(NewScriptData newScriptData)
    {
        if (!DebugUtils.CheckIsNullWithErrorLogging<NewScriptData>(newScriptData))
        {
            scriptDescriptionImages = scriptDescriptionObject.GetComponentsInChildren<Image>();
            scriptDescriptionImages[0].sprite = ScriptSystemManager.Instance.GetScriptBackground(newScriptData.inkType);
            scriptDescriptionImages[1].sprite = ScriptSystemManager.Instance.GetScriptIconByScriptTypeAndInkType(newScriptData.scriptType, newScriptData.inkType);

            scriptDescriptionTexts = scriptDescriptionObject.GetComponentsInChildren<TMP_Text>();

            string tempText = null;
            switch (newScriptData.scriptType)
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

            scriptDescriptionTexts[1].text = tempText;

            scriptDescriptionTexts[0].text = newScriptData.scriptName + $" +{newScriptData.rarity}";

            tempText = newScriptData.scriptDesc[newScriptData.rarity];
            if (newScriptData.rarity == 0)
            {
                tempText = tempText.Replace("%RED%", $"<color=red>빨강</color>");
                tempText = tempText.Replace("%GREEN%", $"<color=green>초록</color>");
                tempText = tempText.Replace("%BLUE%", $"<color=blue>파랑</color>");
            }

            scriptDescriptionTexts[2].text = tempText;
        }
    }

    public void SetStickerDescription(StickerData stickerData)
    {
        if (!DebugUtils.CheckIsNullWithErrorLogging<ScriptSystemData>(stickerData))
        {
            scriptDescriptionImages = scriptDescriptionObject.GetComponentsInChildren<Image>();
            scriptDescriptionImages[0].sprite = ScriptSystemManager.Instance.GetScriptBackground(InkType.RED);
            scriptDescriptionImages[1].sprite = ScriptSystemManager.Instance.GetStickerIconByID(stickerData.stickerID);

            scriptDescriptionTexts = scriptDescriptionObject.GetComponentsInChildren<TMP_Text>();

            string tempText = null;
            if (stickerData.stickerType == StickerType.General)
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

            scriptDescriptionTexts[1].text = tempText;
            scriptDescriptionTexts[0].text = stickerData.stickerName + $" +{stickerData.rarity}";

            tempText = stickerData.stickerDesc.Replace("%LevelData%", $"<color=red>{stickerData.levelData[stickerData.rarity] * 100}%</color>");
            scriptDescriptionTexts[2].text = tempText;
        }
    }

    public void SetIconObjectState(bool state)
    {
        if(state)
            icon.color = new Color(icon.color.r, icon.color.g, icon.color.b, 200f);
        else
            icon.color = new Color(icon.color.r, icon.color.g, icon.color.b, 0f);
        toggle.interactable = state;
        toggle.isOn = state;
    }

    public virtual void ResetElement()
    {
        icon.color = new Color(icon.color.r, icon.color.g, icon.color.b, 0f);
        icon.sprite = null;
        toggle.interactable = false;
        toggle.isOn = false;

        this.Script = null;
        this.Sticker = null;
    }
}