using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ScriptDataRepository : MonoBehaviour
{
    private List<ScriptData> scriptDatas;
    private PlayerScriptController controller;

    public List<ScriptData> ScriptDatas { get => scriptDatas;}

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
}
