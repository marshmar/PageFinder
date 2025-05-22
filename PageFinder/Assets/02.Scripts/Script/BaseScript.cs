using UnityEngine;

public abstract class ScriptContext { }
public abstract class BaseScript
{
    // ������
    public BaseScript()
    {
        scriptData = ScriptableObject.CreateInstance<NewScriptData>();
    }
    protected bool[] upgraded = new bool[4] { true, false, false, false};
    // ��ũ��Ʈ ������ Ŭ����
    protected NewScriptData scriptData;
    // ��ũ��Ʈ �ൿ
    protected IScriptBehaviour scriptBehaviour;
    // ��ũ��Ʈ ��ƼĿ ����
    
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
}
