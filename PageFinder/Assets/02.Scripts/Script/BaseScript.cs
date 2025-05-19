using UnityEngine;

public abstract class BaseScript
{
    // ��ũ��Ʈ ������ Ŭ����
    protected NewScriptData scriptData;
    // ��ũ��Ʈ �ൿ
    protected IScriptBehaviour scriptBehaviour;
    // ��ũ��Ʈ ��ƼĿ ����

    // ���׷��̵�
    public abstract void UpgrageScript(int rarity);

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
    }

    public virtual void SetContext()
    {
        scriptBehaviour.SetContext();
    }

    #region �ʿ��� ��ũ��Ʈ ������ ������ �Լ���
    // ���̵�
    public int GetID()
    {
        return scriptData.scriptID;
    }

    // ����
    public int GetRarity()
    {
        return scriptData.rarity;
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
    #endregion
}
