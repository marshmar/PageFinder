using System;
using UnityEngine;
using System.Collections.Generic;

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
}
