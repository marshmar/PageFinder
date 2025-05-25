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

    private StickerDataParser stickerDataParser;
    private StickerDataRepository stickerDataRepository;
    private StickerFactory stickerFactory;

    private Player player;
    private PlayerScriptController playerScriptController;

    //private ScriptInventory scriptInventory;
    //private StickerInventory stickerInventory;

    public override void Awake()
    {
        base.Awake();

        scriptDataParser = GetComponent<ScriptDataParser>();
        scriptUIMapper = GetComponent<ScriptUIMapper>();
        scriptDataRepository = GetComponent<ScriptDataRepository>();
        scriptFactory = GetComponent<ScriptFactory>();

        stickerDataParser = GetComponent<StickerDataParser>();
        stickerDataRepository = GetComponent<StickerDataRepository>();
        stickerFactory = GetComponent<StickerFactory>();

        GameObject playerObject = GameObject.FindGameObjectWithTag("PLAYER");

        playerScriptController = playerObject.GetComponent<PlayerScriptController>(); 
        player = playerObject.GetComponent<Player>();

        Init();
    }

    public void Init()
    {
        var scriptDataList = scriptDataParser.Parse(scriptUIMapper);
        var newScriptDataList = scriptDataParser.ParseScript();
    
        scriptDataRepository.SaveScriptDatas(scriptDataList);
        scriptDataRepository.SaveScriptDatasNew(newScriptDataList);

        var stickerDataList = stickerDataParser.ParseSticker();
        stickerDataRepository.SaveStickerDatas(stickerDataList);
    }
    
    public List<ScriptSystemData> MakeDistinctRewards(int count)
    {
        var rewards = new List<ScriptSystemData>();

        // count is exclusive, so we add 1 to include it.
        int scriptDataCounts = 2;
        //int scriptDataCounts = (count > 1) ? UnityEngine.Random.Range(1, count + 1) : 1;
        var scriptDatas = GetDistinctRandomScriptsNew(scriptDataCounts);

        foreach( var scriptData in scriptDatas)
        {
            rewards.Add(scriptData);
        }

        int stickerDataCounts = count - scriptDataCounts;
        var stickerDatas = GetRandomStickers(stickerDataCounts);

        foreach (var stickerData in stickerDatas)
        {
            rewards.Add(stickerData);
        }

        return rewards;
    }

    public List<StickerData> GetDistinctRandomStickers(int stickerDataCounts)
    {
        if (player.StickerInventory == null)
        {
            Debug.LogError("Failed To assign ScriptInventory");
            return null;
        }

        return stickerDataRepository.GetDistinctRandomStickers(player.StickerInventory, stickerDataCounts);
    }

    public List<StickerData> GetRandomStickers(int stickerDataCounts)
    {
        if (player.StickerInventory == null)
        {
            Debug.LogError("Failed To assign ScriptInventory");
            return null;
        }

        return stickerDataRepository.GetRandomStickers(player.StickerInventory, stickerDataCounts);
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
        if(player.ScriptInventory == null)
        {
            Debug.LogError("Failed To assign ScriptInventory");
            return null;
        }

        return scriptDataRepository.GetDistinctRandomScripts(player.ScriptInventory, count);
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

    public NewScriptData GetRandomScriptByType(NewScriptData.ScriptType targetType)
    {
        var filteredScripts = scriptDataRepository.NewScriptDatas
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
    public NewScriptData GetRandomScriptExcludingType(NewScriptData.ScriptType targetType)
    {
        var filteredScripts = scriptDataRepository.NewScriptDatas
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

    public BaseScript CreateScriptByID(int scriptID, CharacterType characterType = CharacterType.Stellar)
    {
        if(scriptFactory == null)
        {
            Debug.LogError("Failed To assign ScriptFactory");
            return null;
        }

        return scriptFactory.CreateScriptByID(characterType, scriptID);
    }

    public Sticker CreateStickerByID(int stickerID, CharacterType characterType = CharacterType.Stellar)
    {
        if (stickerFactory == null)
        {
            Debug.LogError("Failed To assign ScriptFactory");
            return null;
        }

        return stickerFactory.CreateStickerByID(characterType, stickerID);
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

    public Sprite GetStickerIconByID(int stickerID)
    {
        return scriptUIMapper.GetStickerIconByID(stickerID);
    }

    #endregion
}
