using UnityEngine;

public interface IScriptBehaviour
{
    public bool CanExcuteBehaviour();
    public void ExcuteBehaviour();
    public void GenerateInkMark();

    public void SetContext();
}
