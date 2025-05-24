using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommaUIManager : MonoBehaviour
{
    [SerializeField] private ScriptManager scriptManager;
    [SerializeField] private Canvas commaUICanvas;
    [SerializeField] private Button synthesisButton;
    [SerializeField] private Button overwriteButton;
    [SerializeField] private GameObject synthesisPanel;
    [SerializeField] private GameObject overwritePanel;
    [SerializeField] private Button synthesizeButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Script commaScript;
    [SerializeField] private List<DiaryElement> passiveScriptElements;
    [SerializeField] PlayerState playerState;

    private int overwriteCount = 0;
    private PlayerScriptController playerScriptController;
    private List<ScriptData> scriptDatas = new();

    void Start()
    {
        playerScriptController = DebugUtils.GetComponentWithErrorLogging<PlayerScriptController>(GameObject.FindGameObjectWithTag("PLAYER"), "PlayerScriptController");
        synthesisButton.onClick.AddListener(OnSynthesisClickHandler);
        overwriteButton.onClick.AddListener(OnOverwriteClickHandler);
        // ToDo: UI Changed;
        //overwriteButton.onClick.AddListener(() => EventManager.Instance.PostNotification(EVENT_TYPE.UI_Changed, this, UIType.PageMap));
        synthesizeButton.onClick.AddListener(SynthesizeScript);
        //exitButton.onClick.AddListener(() => EventManager.Instance.PostNotification(EVENT_TYPE.UI_Changed, this, UIType.PageMap));
    }

    private void OnSynthesisClickHandler()
    {
        synthesisPanel.SetActive(!synthesisPanel.activeInHierarchy);
        SetDiaryScripts();
    }

    private void OnOverwriteClickHandler()
    {
        overwritePanel.SetActive(!overwritePanel.activeInHierarchy);
    }

    private void OnDestroy()
    {
        synthesisButton.onClick.RemoveAllListeners();
        overwriteButton.onClick.RemoveAllListeners();
        synthesizeButton.onClick.RemoveAllListeners();
    }

    private void SetDiaryScripts()
    {
        int index = 0;
        foreach (ScriptData s in playerScriptController.PlayerScriptDictionary.Values)
        {
            switch (s.scriptType)
            {
                case ScriptData.ScriptType.PASSIVE:
                    passiveScriptElements[index].ScriptData = s;
                    index++;
                    break;
            }
        }
    }

    private void InitializeDiaryScripts()
    {
        int index = 0;
        foreach(ScriptData s in playerScriptController.PlayerScriptDictionary.Values)
        {
            if (s.scriptType == ScriptData.ScriptType.PASSIVE)
            {
                passiveScriptElements[index].ScriptData = null;
                index++;
            }
        }
    }

    public void AddScriptData(ScriptData scriptData)
    {
        scriptDatas.Add(scriptData);
    }

    public void RemoveScriptData(ScriptData scriptData)
    {
        scriptDatas.Remove(scriptData);
    }

    public int GetScriptCount()
    { 
        return scriptDatas.Count;
    }

    public void SynthesizeScript()
    {
        if (scriptDatas.Count == 3)
        {
            Debug.Log("1: " + scriptDatas[0].level + ", 2: " + scriptDatas[1].level + ", 3: " + scriptDatas[2].level);
            if((scriptDatas[0].level == scriptDatas[1].level) && (scriptDatas[0].level < 2))
            {
                if (scriptDatas[1].level == scriptDatas[2].level)
                {
                    //Todo: level
                    //commaScript.level = scriptDatas[0].level + 2;
                    commaScript.ScriptData = CSVReader.Instance.GetRandomScriptByType(ScriptData.ScriptType.PASSIVE);
                    //Todo: level
                    //Debug.Log("level: " + commaScript.level);
                    scriptManager.SelectData = commaScript.ScriptData;
                    scriptManager.ApplyScriptData();

                    if (playerScriptController.RemoveScriptData(scriptDatas[0].scriptId) &&
                        playerScriptController.RemoveScriptData(scriptDatas[1].scriptId) &&
                        playerScriptController.RemoveScriptData(scriptDatas[2].scriptId))
                    {
                        InitializeDiaryScripts();
                        
                        commaScript.gameObject.SetActive(true);
                        SetDiaryScripts();
                    }
                }
            }
        }
    }

    public void OverwriteScript(ScriptData selectedScriptData)
    {
        commaScript.ScriptData = ScriptSystemManager.Instance.GetRandomScriptExcludingType(ScriptData.ScriptType.PASSIVE, selectedScriptData.level, selectedScriptData.inkType);
        scriptManager.SelectData = commaScript.ScriptData;
        scriptManager.ApplyScriptData();
        playerScriptController.RemoveScriptData(selectedScriptData.scriptId);
        playerState.Coin -= (int)(100 * (1 + 0.5f * (overwriteCount - 1)));
    }
}