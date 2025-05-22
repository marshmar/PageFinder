using UnityEngine;

public abstract class ScriptContext { }
public abstract class BaseScript
{
    // 생성자
    public BaseScript()
    {
        scriptData = ScriptableObject.CreateInstance<NewScriptData>();
    }
    protected bool[] upgraded = new bool[4] { true, false, false, false};
    // 스크립트 데이터 클래스
    protected NewScriptData scriptData;
    // 스크립트 행동
    protected IScriptBehaviour scriptBehaviour;
    // 스크립트 스티커 슬롯
    
    // 업그레이드
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

    // 스크립트 데이터 복사
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
    // 아이디
    public int GetID()
    {
        return scriptData.scriptID;
    }

    // 성급
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
    // 스크립트 코스트
    public float GetInkCost()
    {
        return scriptData.inkCost;
    }

    // 스크립트 타입
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
