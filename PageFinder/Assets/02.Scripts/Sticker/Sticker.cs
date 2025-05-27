using UnityEngine;

public enum StickerLogicType
{
    InstantEffect,
    AfterEffect
}

public abstract class Sticker
{
    protected StickerData stickerData;
    protected BaseScript target;
    protected bool isAttached;
    private StickerLogicType logicType;

    public StickerLogicType LogicType { get => logicType; set => logicType = value; }

    public Sticker()
    {
        stickerData = ScriptableObject.CreateInstance<StickerData>();
    }

    public void Attach(BaseScript target)
    {
        this.target = target;
        isAttached = true;
        // 로직 활성화
        AttachStickerLogic();
    }

    public void Detach()
    {
        if (target != null)
        {
            // 로직 비활성화
            DetachStickerLogic();

            target.DetachSticker(this);
            target = null;
        }

        isAttached = false;
    }

    #region Utils
    public void CopyData(StickerData copyData)
    {
        stickerData.CopyData(copyData);
    }

    public int GetID()
    {
        return stickerData.stickerID;
    }

    public string GetName()
    {
        return stickerData.stickerName;
    }

    public int GetCurrRarity()
    {
        return stickerData.rarity;
    }

    public bool IsMaxRarity()
    {
        return stickerData.rarity == stickerData.maxRarity;
    }

    public bool IsAttached()
    {
        return isAttached;
    }

    public StickerData GetCopiedData()
    {
        StickerData newData = ScriptableObject.CreateInstance<StickerData>();
        newData.CopyData(stickerData);
        return newData;
    }

    public NewScriptData.ScriptType GetDedicatedScriptTarget()
    {
        return stickerData.dedicatedScriptTarget;
    }

    public StickerType GetStickerType()
    {
        return stickerData.stickerType;
    }

    public abstract void AttachStickerLogic();

    public abstract void DetachStickerLogic();
    #endregion
}
