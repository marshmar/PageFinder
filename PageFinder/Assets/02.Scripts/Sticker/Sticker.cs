using UnityEngine;

public abstract class Sticker
{
    protected StickerData stickerData;
    protected BaseScript target;
    protected bool isAttached;
    public Sticker()
    {
        stickerData = ScriptableObject.CreateInstance<StickerData>();
    }

    public void Attach(BaseScript target)
    {
        this.target = target;
        isAttached = true;
        // 로직 활성화
    }

    public void Detach()
    {
        if (target != null)
        {
            target.DetachSticker(this);
            target = null;
        }
        isAttached = false;

        // 로직 비활성화
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

    public abstract void StickerLogic();
    #endregion
}
