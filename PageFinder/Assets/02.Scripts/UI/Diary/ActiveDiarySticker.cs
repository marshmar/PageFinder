using UnityEngine;
using UnityEngine.UI;

public class ActiveDiarySticker : DiaryElement
{
    private int index = 0;
    private DroppableUI droppableUI;
    private DiaryManager diaryManager;
    public StickerType stickerSlotType;
    private Sprite defaultIcon;

    public int Index { get => index; set => index = value; }

    public override BaseScript Script
    {
        get => script;
        set {
            script = value;
        }
    }

    public override void Awake()
    {
        base.Awake();

        droppableUI = GetComponent<DroppableUI>();
        diaryManager = GetComponentInParent<DiaryManager>();

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
        
        if(dr.Success)
            diaryManager.SetDiaryStickers();

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
