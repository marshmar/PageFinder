using UnityEngine;

public static class BaseScriptExtensions
{
    public static bool IsNull(this BaseScript baseScript)
    {
        if (baseScript == null)
        {
            Debug.LogError($"{baseScript.GetType().Name} wasn't assigned.");
            return true;
        }

        return false;
    }
}
