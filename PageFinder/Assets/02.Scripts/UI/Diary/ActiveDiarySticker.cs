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
                icon.sprite = defaultIcon;
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
        base.Awake();

        droppableUI = GetComponent<DroppableUI>();

        if (droppableUI != null)
        {
            droppableUI.dropEvent += (Sticker s, DropResult dr) => TryAttachSticker(s, dr);
        }

        defaultIcon = GetComponent<Image>().sprite;
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
            Debug.Log("½ºÆ¼Ä¿ ÀåÂø ½ÇÆÐ");
            return;
        }

        switch (sticker.GetStickerType())
        {
            case StickerType.General:
                dr.Success = script.AttachGeneralSticker(sticker);
                if (dr.Success)
                {
                    Debug.Log("°ø¿ë ½ºÆ¼Ä¿ ÀåÂø");
                    this.Sticker = sticker;
                }
                else
                    Debug.Log("½ºÆ¼Ä¿ ÀåÂø ½ÇÆÐ");
                break;
            case StickerType.Dedicated:
                dr.Success = script.AttachDedicatedSticker(sticker, index);
                if (dr.Success)
                {
                    Debug.Log("Å¸°Ù ½ºÆ¼Ä¿ ÀåÂø");
                    this.Sticker = sticker;
                }
                else
                    Debug.Log("½ºÆ¼Ä¿ ÀåÂø ½ÇÆÐ");
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

        this.script = null;
        this.sticker = null;
    }
}
