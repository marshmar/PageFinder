using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

using TMPro;

public class ActiveDiaryElement : DiaryElement
{
    [SerializeField] private Sprite[] activeBackgroundImages;
    [SerializeField] private Image activeBackgroundImage;
    [SerializeField] private TMP_Text activeNameText;

    [SerializeField] private ActiveDiarySticker generalSticker;
    [SerializeField] private List<ActiveDiarySticker> dedicatedStickers;

    public override BaseScript Script
    {
        get => script;
        set
        {
            script = value;
            if (value == null)
                toggle.interactable = false;
            else
            {
                toggle.interactable = true;
                SetScriptPanelsNew();
                SetAttachedStickers();
            }
        }
    }
    public override void Awake()
    {
        base.Awake();
        activeNameText.text = "";

        for(int i = 0; i < dedicatedStickers.Count; i++)
        {
            dedicatedStickers[i].Index = i;
        }

    }


    public override void SetScriptPanels()
    {
        icon.sprite = scriptData.scriptIcon;
        switch (scriptData.inkType)
        {
            case InkType.RED:
                activeBackgroundImage.sprite = activeBackgroundImages[0];
                break;
            case InkType.GREEN:
                activeBackgroundImage.sprite = activeBackgroundImages[1];
                break;
            case InkType.BLUE:
                activeBackgroundImage.sprite = activeBackgroundImages[2];
                break;
        }

        if (scriptData.level <= 0) activeNameText.text = scriptData.scriptName;
        else activeNameText.text = scriptData.scriptName + $" +{scriptData.level}";
    }

    public override void SetScriptPanelsNew()
    {
        if(elementType == DiaryElementType.Script && script != null)
        {
            NewScriptData newScriptData = script.GetCopiedData();
            icon.sprite = ScriptSystemManager.Instance.GetScriptIconByScriptTypeAndInkType(newScriptData.scriptType, newScriptData.inkType);
            switch (newScriptData.inkType)
            {
                case InkType.RED:
                    activeBackgroundImage.sprite = activeBackgroundImages[0];
                    break;
                case InkType.GREEN:
                    activeBackgroundImage.sprite = activeBackgroundImages[1];
                    break;
                case InkType.BLUE:
                    activeBackgroundImage.sprite = activeBackgroundImages[2];
                    break;
            }
        }
    }

    public void SetAttachedStickers()
    {
        generalSticker.elementType = DiaryElementType.Sticker;
        generalSticker.stickerSlotType = StickerType.General;
        generalSticker.Script = this.script;
        generalSticker.Sticker = script.GetGeneralSticker();
        
        generalSticker.SetDroppable(true);

        for(int i = 0; i < 4; i++)
        {
            dedicatedStickers[i].elementType = DiaryElementType.Sticker;
            dedicatedStickers[i].stickerSlotType = StickerType.Dedicated;
            dedicatedStickers[i].Sticker = script.GetDedicatedSticker(i);

            if( i <= script.GetCurrRarity())
            {
                dedicatedStickers[i].Script = this.script;
                dedicatedStickers[i].SetDroppable(true);
            }
            else
            {
                dedicatedStickers[i].SetDroppable(false);
            }
        }    
    }
}