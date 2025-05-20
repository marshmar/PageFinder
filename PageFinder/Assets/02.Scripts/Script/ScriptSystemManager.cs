using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ScriptSystemManager : Singleton<ScriptSystemManager>
{
    private ScriptDataParser scriptDataParser;
    private ScriptUIMapper scriptUIMapper;
    private ScriptDataRepository scriptDataRepository;

    private PlayerScriptController playerScriptController;

    public override void Awake()
    {
        base.Awake();

        scriptDataParser = GetComponent<ScriptDataParser>();
        scriptUIMapper = GetComponent<ScriptUIMapper>();
        scriptDataRepository = GetComponent<ScriptDataRepository>();

        playerScriptController = GameObject.FindGameObjectWithTag("PLAYER").GetComponent<PlayerScriptController>(); 

        Init();
    }

    public void Init()
    {
        var scriptDataList = scriptDataParser.Parse(scriptUIMapper);
        scriptDataRepository.SaveScriptDatas(scriptDataList);
    }


    public List<ScriptData> GetDistinctRandomScripts(int count)
    {
        if(playerScriptController == null)
        {
            Debug.LogError("Faield To assign PlayerScriptController");
            return null;
        }

        return scriptDataRepository.GetDistinctRandomScripts(playerScriptController, count);
    }

    public ScriptData GetRandomScriptByType(ScriptData.ScriptType targetType)
    {
        var filteredScripts = scriptDataRepository.ScriptDatas
            .Where(script => script.scriptType == targetType)
            .ToList();

        if (filteredScripts.Count == 0)
        {
            Debug.LogWarning($"No scripts of type {targetType} found.");
            return null;
        }

        int randomIndex = UnityEngine.Random.Range(0, filteredScripts.Count);
        return filteredScripts[randomIndex];
    }

    public ScriptData GetRandomScriptExcludingType(ScriptData.ScriptType targetType)
    {
        var filteredScripts = scriptDataRepository.ScriptDatas
            .Where(script => script.scriptType != targetType)
            .ToList();

        if (filteredScripts.Count == 0)
        {
            Debug.LogWarning($"No scripts of type {targetType} found.");
            return null;
        }

        int randomIndex = UnityEngine.Random.Range(0, filteredScripts.Count);
        return filteredScripts[randomIndex];
    }

    public ScriptData GetScriptByID(int scriptID)
    {
        return scriptDataRepository.GetScriptByID(scriptID);
    }
}
