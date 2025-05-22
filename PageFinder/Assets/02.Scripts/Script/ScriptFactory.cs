using UnityEngine;
using System.Collections.Generic;

public enum CharacterType
{
    Stellar
}
public class ScriptFactory : MonoBehaviour
{
    private Dictionary<CharacterType, IScriptFactory> scriptFactories = new Dictionary<CharacterType, IScriptFactory>();

    private void Start()
    {
        scriptFactories.Add(CharacterType.Stellar, new StellarScriptFactory());
    }

    public BaseScript CreateScriptByID(CharacterType charType, int scriptID)
    {
        if (!scriptFactories.TryGetValue(charType, out IScriptFactory scriptFactory))
        {
            Debug.LogError($"Not surpported character Type : {charType}");
            return null;
        }
        BaseScript newScript = scriptFactory.CreateScriptByID(scriptID);
        return newScript;
    }
}
