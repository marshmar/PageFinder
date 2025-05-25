using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ScriptDataRepository : MonoBehaviour
{
    private List<ScriptData> scriptDatas;
    private List<NewScriptData> newScriptDatas;


    public List<ScriptData> ScriptDatas { get => scriptDatas;}
    public List<NewScriptData> NewScriptDatas { get => newScriptDatas; set => newScriptDatas = value; }

    public void SaveScriptDatas(List<ScriptData> csvScriptDatas)
    {
        scriptDatas = new List<ScriptData>();

        foreach(var scriptData in csvScriptDatas)
        {
            ScriptData copyData = ScriptableObject.CreateInstance<ScriptData>();
            copyData.CopyData(scriptData);

            scriptDatas.Add(copyData);
        }
    }

    public List<ScriptData> GetDistinctRandomScripts(PlayerScriptController playerScriptController, int count)
    {
        var result = new List<ScriptData>();

        while (result.Count < count)
        {
            int index = Random.Range(0, scriptDatas.Count);

            if (result.Any(s => s.scriptId == scriptDatas[index].scriptId))
                continue;

            ScriptData playerScript = playerScriptController.CheckScriptDataAndReturnIndex(scriptDatas[index].scriptId);
            if(playerScript != null)
            {
                if (playerScript.level == -1 || playerScript.level >= 2) continue;
                ScriptData upgradedScriptData = ScriptableObject.CreateInstance<ScriptData>();
                upgradedScriptData.CopyData(playerScript);
                upgradedScriptData.level = playerScript.level + 1;
                result.Add(upgradedScriptData);
                continue;
            }

            result.Add(scriptDatas[index]);
        }


        return result;
    }

    public ScriptData GetScriptByID(int scriptID)
    {
        return scriptDatas.Find(s => s.scriptId == scriptID);
    }

    public void SaveScriptDatasNew(List<NewScriptData> csvNewScriptDatas)
    {
        newScriptDatas = new List<NewScriptData>();

        foreach (var scriptData in csvNewScriptDatas)
        {
            NewScriptData copyData = ScriptableObject.CreateInstance<NewScriptData>();
            copyData.CopyData(scriptData);

            newScriptDatas.Add(copyData);
        }

        Debug.Log(newScriptDatas.Count);
    }

    public List<NewScriptData> GetDistinctRandomScripts(ScriptInventory scriptInventory, int count)
    {
        var result = new List<NewScriptData>();

        while (result.Count < count)
        {
            int index = Random.Range(0, newScriptDatas.Count);

            if (result.Any(s => s.scriptID == newScriptDatas[index].scriptID))
                continue;

            BaseScript playerScript = scriptInventory.FindPlayerScriptByID(newScriptDatas[index].scriptID);
            if (playerScript != null)
            {
                if (playerScript.IsMaxRarity()) continue;
                NewScriptData upgradedScriptData = ScriptableObject.CreateInstance<NewScriptData>();
                upgradedScriptData.CopyData(playerScript.GetCopiedData());
                upgradedScriptData.rarity = playerScript.GetCurrRarity() + 1;
                result.Add(upgradedScriptData);
                continue;
            }

            result.Add(newScriptDatas[index]);
        }

        /*Debug.Log("============Each script info============");
        foreach (var data in result)
        {
            Debug.Log($"scriptID: {data.scriptID}");
            Debug.Log($"scriptName: {data.scriptName}");
            Debug.Log($"scriptRarity: {data.rarity}");
            Debug.Log($"scriptMaxRarity: {data.maxRarity}");
            Debug.Log($"scriptType: {data.scriptType}");
            Debug.Log($"scriptInkType: {data.inkType}");
            Debug.Log("============================================");
        }*/

        return result;
    }

    public NewScriptData GetScriptByIDNew(int scriptID)
    {
        return newScriptDatas.Find(s => s.scriptID == scriptID);
    }
}
