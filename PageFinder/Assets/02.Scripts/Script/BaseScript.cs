using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public abstract class ScriptContext { }
public abstract class BaseScript
{
    // ������
    public BaseScript()
    {
        scriptData = ScriptableObject.CreateInstance<NewScriptData>();
        dedicatedStickers = new Sticker[4] { null, null, null, null};
    }

    protected bool[] upgraded = new bool[4] { true, false, false, false};
    // ��ũ��Ʈ ������ Ŭ����
    protected NewScriptData scriptData;
    // ��ũ��Ʈ �ൿ
    protected IScriptBehaviour scriptBehaviour;

    // ��ũ��Ʈ ��ƼĿ ����
    protected Sticker generalSticker;
    protected Sticker[] dedicatedStickers;
    
    public event Action AfterEffect
    {
        add
        {
            scriptBehaviour.AfterEffect += value;
        }

        remove
        {
            scriptBehaviour.AfterEffect -= value;
        }
    }

    // ���׷��̵�
    public virtual void UpgradeScript(int upgradedRarity)
    {
        if (upgradedRarity <= 0)
        {
            Debug.Log("Cannot upgrade below rarity 0");
            return;
        }
        scriptData.rarity = upgradedRarity;

        for (int i = 0; i <= upgradedRarity; i++)
        {
            if (upgraded[i]) continue;

            upgraded[i] = true;
            Upgrade(i);

        }
    }

    public abstract void Upgrade(int rarity);

    public virtual void ExcuteBehaviour()
    {
        scriptBehaviour.ExcuteBehaviour();
    }

    public virtual bool CanExcuteBehaviour()
    {
        return scriptBehaviour.CanExcuteBehaviour();
    }



    #region Utils
    // ���̵�
    public int GetID()
    {
        return scriptData.scriptID;
    }

    // ����
    public int GetCurrRarity()
    {
        return scriptData.rarity;
    }

    public int GetMaxRarity()
    {
        return scriptData.maxRarity;
    }

    public string GetScriptName()
    {
        return scriptData.scriptName;
    }
    // ��ũ��Ʈ �ڽ�Ʈ
    public float GetInkCost()
    {
        return scriptData.inkCost;
    }

    // ��ũ��Ʈ Ÿ��
    public NewScriptData.ScriptType GetScriptType()
    {
        return scriptData.scriptType;   
    }

    public InkType GetInkType()
    {
        return scriptData.inkType;
    }

    public bool IsMaxRarity()
    {
        return scriptData.rarity == scriptData.maxRarity;
    }

    public NewScriptData GetCopiedData()
    {
        NewScriptData copiedData = ScriptableObject.CreateInstance<NewScriptData>();
        copiedData.CopyData(scriptData);

        return copiedData;
    }

    // ��ũ��Ʈ ������ ����
    public void CopyData(NewScriptData copyData)
    {
        scriptData.CopyData(copyData);
        scriptBehaviour.SetScriptData(scriptData);
    }

    public virtual void SetContext(ScriptContext context)
    {
        scriptBehaviour.SetContext(context);
    }
    #endregion

    #region Debug
    public void PrintScriptInfo()
    {
        Debug.Log($"scriptID: {GetID()}");
        Debug.Log($"scriptName: {GetScriptName()}");
        Debug.Log($"scriptRarity: {GetCurrRarity()}");
        Debug.Log($"scriptMaxRarity: {GetMaxRarity()}");
        Debug.Log($"scriptType: {GetScriptType()}");
        Debug.Log($"scriptInkType: {GetInkType()}");
    }
    #endregion

    #region Stickers
    public bool AttachGeneralSticker(Sticker sticker)
    {
        if (sticker.GetStickerType() != StickerType.General)
            return false;

        if(generalSticker != null)
        {
            generalSticker.Detach();
            generalSticker = null;
        }

        if (sticker.IsAttached())
            sticker.Detach();

        generalSticker = sticker;
        generalSticker.Attach(this);


        return true;
    }

    public bool AttachDedicatedSticker(Sticker sticker, int index)
    {
        if (sticker.GetStickerType() != StickerType.Dedicated)
            return false;

        if (sticker.GetDedicatedScriptTarget() != GetScriptType())
            return false;

        if (GetCurrRarity() < index)
            return false;

        if (dedicatedStickers.Any(s => s != null && s.GetID() == sticker.GetID()))
            return false;

        if (sticker.IsAttached())
            sticker.Detach();

        if (dedicatedStickers[index] != null)
        {
            dedicatedStickers[index].Detach();
            dedicatedStickers[index] = null;
        }

        dedicatedStickers[index] = sticker;
        dedicatedStickers[index].Attach(this);
        return true;
    }

    public Sticker GetGeneralSticker()
    { 
        return generalSticker; 
    }

    public Sticker GetDedicatedSticker(int index)
    {
        return dedicatedStickers[index];
    }

    public void DetachSticker(Sticker sticker)
    {
        if(generalSticker == sticker)
        {
            generalSticker = null;
        }

        for(int i = 0; i < dedicatedStickers.Length; i++)
        {
            if (dedicatedStickers[i] == sticker)
            {
                dedicatedStickers[i] = null;
            }
        }
    }

    public void DetachAllStickers()
    {
        if (generalSticker != null)
        {
            generalSticker.Detach();
            generalSticker = null;
        }

        for (int i = 0; i < dedicatedStickers.Length; i++)
        {
            if (dedicatedStickers[i] != null)
            {
                dedicatedStickers[i].Detach();
                dedicatedStickers[i] = null;
            }
        }
    }
    #endregion
}
