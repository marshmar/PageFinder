using UnityEngine;
using UnityEngine.UI;

public class ActiveDiarySticker : DiaryElement
{
    private int index = 0;
    private DroppableUI droppableUI;
    public StickerType stickerSlotType;

    public int Index { get => index; set => index = value; }

    public override BaseScript Script
    {
        get => script;
        set {
            script = value;
        }
    }

    public override Sticker Sticker
    {
        get => sticker;
        set
        {
            sticker = value;
            if (value == null)
            {
                toggle.interactable = false;
                if (draggableUI != null)
                {
                    draggableUI.canDrag = false;
                }
                //icon.sprite = defaultIcon;
                icon.color = new Color(icon.color.r, icon.color.g, icon.color.b, 153f);
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

    public override void Awake()
    {
        toggle = DebugUtils.GetComponentWithErrorLogging<Toggle>(this.gameObject, "Toggle");
        draggableUI = GetComponent<DraggableUI>();
        diaryManager = GetComponentInParent<DiaryManager>();
        defaultIcon = GetComponent<Image>().sprite;

        if (draggableUI != null)
        {
            draggableUI.dropSuccessEvent += ResetElement;
            draggableUI.dropSuccessEvent += diaryManager.SetDiaryStickers;

        }

        droppableUI = GetComponent<DroppableUI>();

        if (droppableUI != null)
        {
            droppableUI.dropEvent += (Sticker s, DropResult dr) => TryAttachSticker(s, dr);
        }

    }

    private void OnDestroy()
    {
        if (droppableUI != null)
        {
            droppableUI.dropEvent -= (Sticker s, DropResult dr) => TryAttachSticker(s, dr);
        }
    }



    public void TryAttachSticker(Sticker sticker, DropResult dr)
    {
        this.elementType = DiaryElementType.Sticker;

        if(script == null)
        {
            Debug.LogError("Script is not Assigned");
            return;
        }

        dr.Success = stickerSlotType == sticker.GetStickerType();

        if (!dr.Success)
        {
            Debug.Log("스티커 장착 실패");
            return;
        }

        switch (sticker.GetStickerType())
        {
            case StickerType.General:
                dr.Success = script.AttachGeneralSticker(sticker);
                if (dr.Success)
                {
                    Debug.Log("공용 스티커 장착");
                    this.Sticker = sticker;
                }
                else
                    Debug.Log("스티커 장착 실패");
                break;
            case StickerType.Dedicated:
                dr.Success = script.AttachDedicatedSticker(sticker, index);
                if (dr.Success)
                {
                    Debug.Log("타겟 스티커 장착");
                    this.Sticker = sticker;
                }
                else
                    Debug.Log("스티커 장착 실패");
                break;
        }
    }

    public void SetDroppable(bool state)
    {
        droppableUI.canDroppbable = state;
    }

    public void SetToggleInteractable(bool state)
    {
        toggle.interactable = state;
    }

    public override void ResetElement()
    {
        icon.sprite = defaultIcon;
        toggle.interactable = false;
        toggle.isOn = false;
        icon.color = new Color(icon.color.r, icon.color.g, icon.color.b, 153f);
        /*        if(this.sticker != null)
                {
                    sticker.Detach();
                }*/
        this.sticker = null;
    }

    public override void SetScriptPanelsNew()
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

    public override void SetIconObjectState(bool state)
    {
        toggle.interactable = state;
        toggle.isOn = state;

        if (state)
        {
            StickerData stickerData = sticker.GetCopiedData();
            icon.sprite = ScriptSystemManager.Instance.GetStickerIconByID(stickerData.stickerID);
            icon.color = new Color(icon.color.r, icon.color.g, icon.color.b, 200f);
            Debug.Log("원래 아이콘으로 설정");
        }

        else
        {
            icon.sprite = defaultIcon;
            icon.color = new Color(icon.color.r, icon.color.g, icon.color.b, 153f);
        }


    }
}
