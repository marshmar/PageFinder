using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ScriptSystemManager : Singleton<ScriptSystemManager>
{
    private ScriptDataParser scriptDataParser;
    private ScriptUIMapper scriptUIMapper;
    private ScriptDataRepository scriptDataRepository;
    private ScriptFactory scriptFactory;

    private PlayerScriptController playerScriptController;
    private ScriptInventory scriptInventory;

    public override void Awake()
    {
        base.Awake();

        scriptDataParser = GetComponent<ScriptDataParser>();
        scriptUIMapper = GetComponent<ScriptUIMapper>();
        scriptDataRepository = GetComponent<ScriptDataRepository>();
        scriptFactory = GetComponent<ScriptFactory>();

        GameObject playerObject = GameObject.FindGameObjectWithTag("PLAYER");

        playerScriptController = playerObject.GetComponent<PlayerScriptController>(); 
        scriptInventory = playerObject.GetComponent<ScriptInventory>();

        Init();
    }

    public void Init()
    {
        var scriptDataList = scriptDataParser.Parse(scriptUIMapper);
        var newScriptDataList = scriptDataParser.ParseNew();

        scriptDataRepository.SaveScriptDatas(scriptDataList);
        scriptDataRepository.SaveScriptDatasNew(newScriptDataList);
    }


    public List<ScriptData> GetDistinctRandomScripts(int count)
    {
        if(playerScriptController == null)
        {
            Debug.LogError("Failed To assign PlayerScriptController");
            return null;
        }

        return scriptDataRepository.GetDistinctRandomScripts(playerScriptController, count);
    }

    public List<NewScriptData> GetDistinctRandomScriptsNew(int count)
    {
        if(scriptInventory == null)
        {
            Debug.LogError("Failed To assign ScriptInventory");
            return null;
        }

        return scriptDataRepository.GetDistinctRandomScripts(scriptInventory, count);
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

    public NewScriptData GetScriptByIDNew(int scriptID)
    {
        return scriptDataRepository.GetScriptByIDNew(scriptID);
    }

    public BaseScript CreateScritByID(int scriptID, CharacterType characterType = CharacterType.Stellar)
    {
        if(scriptFactory == null)
        {
            Debug.LogError("Failed To assign ScriptFactory");
            return null;
        }

        return scriptFactory.CreateScriptByID(characterType, scriptID);
    }
    #region UI
    public Sprite GetScriptIconByID(int scriptID)
    {
        return scriptUIMapper.GetScriptIconByID(scriptID);
    }

    public Sprite GetScriptIconByScriptTypeAndInkType(NewScriptData.ScriptType scriptType, InkType inkType)
    {
        return scriptUIMapper.GetScriptIconByScriptTypeAndInkType(scriptType, inkType);
    }

    public Sprite GetScriptBackground(InkType inkType)
    {
        return scriptUIMapper.GetScriptBackground(inkType);
    }


    #endregion
}
