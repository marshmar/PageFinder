using UnityEngine;

public abstract class BaseScript
{
    // 스크립트 데이터 클래스
    protected NewScriptData scriptData;
    // 스크립트 행동
    protected IScriptBehaviour scriptBehaviour;
    // 스크립트 스티커 슬롯

    // 업그레이드
    public abstract void UpgrageScript(int rarity);

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
    }

    public virtual void SetContext()
    {
        scriptBehaviour.SetContext();
    }

    #region 필요한 스크립트 데이터 얻어오는 함수들
    // 아이디
    public int GetID()
    {
        return scriptData.scriptID;
    }

    // 성급
    public int GetRarity()
    {
        return scriptData.rarity;
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
    #endregion
}
