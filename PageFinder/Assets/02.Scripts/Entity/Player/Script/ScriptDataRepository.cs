using UnityEngine;
using System.Collections.Generic;

public class ScriptDataRepository : MonoBehaviour
{
    private List<ScriptData> scriptDatas;
    private PlayerScriptController controller;

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

            if (result.Contains(scriptDatas[index]))
            {
                continue;
            }

            ScriptData playerScript = playerScriptController.CheckScriptDataAndReturnIndex(index);
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
}
