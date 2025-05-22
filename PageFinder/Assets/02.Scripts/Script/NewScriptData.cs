using UnityEngine;

[CreateAssetMenu(fileName = "NewScriptData", menuName = "ScriptSystem/NewScriptData")]
public class NewScriptData : ScriptableObject
{
    public enum ScriptType { BasicAttack, Dash, Skill, None}

    public int scriptID;
    public ScriptType scriptType;
    public float inkCost;
    public string scriptName;
    public string[] scriptDesc;
    public InkType inkType;
    public int rarity;
    public int maxRarity = 3;
    public int[] price;
    public float[] levelData;

    public void CopyData(NewScriptData copyData)
    {
        scriptID = copyData.scriptID;
        scriptType = copyData.scriptType;
        inkCost = copyData.inkCost;
        scriptName = copyData.scriptName;
        inkType = copyData.inkType;
        rarity = copyData.rarity;

        scriptDesc = new string[4] { copyData.scriptDesc[0], copyData.scriptDesc[1], copyData.scriptDesc[2], copyData.scriptDesc[3] };
        price = new int[4] { copyData.price[0], copyData.price[1], copyData.price[2], copyData.price[3] };
        levelData = new float[4] {copyData.levelData[0], copyData.levelData[1], copyData.levelData[2], copyData.levelData[3] };
    }
}
