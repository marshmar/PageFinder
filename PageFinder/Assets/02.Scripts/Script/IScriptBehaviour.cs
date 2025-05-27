using UnityEngine;
using System;

public interface IScriptBehaviour
{
    public event Action AfterEffect;
    public bool CanExcuteBehaviour();
    public void ExcuteBehaviour();
    public void ExcuteAnim();
    public void GenerateInkMark(Vector3 position);

    public void SetContext(ScriptContext context);

    public void SetScriptData(NewScriptData scriptData);
}

public interface ISkillBehaviour : IScriptBehaviour
{

}

public interface IChargeBehaviour : IScriptBehaviour
{
    public void ChargingBehaviour();
}