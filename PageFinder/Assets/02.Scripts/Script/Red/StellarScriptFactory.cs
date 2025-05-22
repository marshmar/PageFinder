using UnityEngine;

public class StellarScriptFactory : IScriptFactory
{
    public BaseScript CreateScriptByID(int scriptID)
    {
        BaseScript newScript = null;

        switch (scriptID)
        {
            case 1:
                newScript = new NewFlameStrike();
                break;
            case 2:
                newScript = new FlameDash();
                break;
            case 3:
                newScript = new FireWork();
                break;
            case 4:
                newScript = new VineStrike();
                break;
            case 5:
                newScript = new VineDash();
                break;
            case 6:
                newScript = new CottonSpores();
                break;
            case 7:
                newScript = new BubbleStrike();
                break;
            case 8:
                newScript = new BubbleDash();
                break;
            case 9:
                newScript = new WaterSplash();
                break;
            default:
                Debug.LogError("No matching ID found.");
                break;
        }

        return newScript;
    }
}
