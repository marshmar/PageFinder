using UnityEngine;

public interface IScriptFactory
{
    public BaseScript CreateScriptByID(int scriptID);
}
