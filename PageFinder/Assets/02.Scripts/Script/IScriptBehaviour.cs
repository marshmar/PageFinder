using UnityEngine;

public interface IScriptBehaviour
{
    public bool CanExcuteBehaviour();
    public void ExcuteBehaviour();
    public void ExcuteAnim();
    public void GenerateInkMark(Vector3 position);

    public void SetContext(ScriptContext context);

    public void SetScriptData(NewScriptData scriptData);
}

public interface ISkillBehaviour : IScriptBehaviour
{
    public bool ChangeSkill(string skillName);
}

public interface IChargeSkillBehaviour : ISkillBehaviour
{
    public void ChargingBehaviour();
}