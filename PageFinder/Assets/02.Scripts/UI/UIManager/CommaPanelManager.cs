using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CommaPanelManager : MonoBehaviour, IUIPanel
{
    [SerializeField] private bool isFixedMap = false;
    [SerializeField] private ProceduralMapGenerator proceduralMapGenerator;
    [SerializeField] private FixedMap fixedMap;
    public PanelType PanelType => PanelType.Comma;

    [SerializeField] private ScriptSystemManager scriptSystemManager;
    [SerializeField] private Button synthesisButton;
    [SerializeField] private Button overwriteButton;
    [SerializeField] private GameObject synthesisPanel;
    [SerializeField] private GameObject overwritePanel;
    [SerializeField] private Button synthesizeButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Script commaScript;
    [SerializeField] private List<DiaryElement> passiveScriptElements;

    private PlayerScriptController playerScriptController;
    private List<ScriptData> scriptDatas = new();

    void Start()
    {
        playerScriptController = DebugUtils.GetComponentWithErrorLogging<PlayerScriptController>(GameObject.FindGameObjectWithTag("PLAYER"), "PlayerScriptController");
        synthesisButton.onClick.AddListener(OnSynthesisClickHandler);
        overwriteButton.onClick.AddListener(OnOverwriteClickHandler);
        overwriteButton.onClick.AddListener(() => EventManager.Instance.PostNotification(EVENT_TYPE.Open_Panel_Exclusive, this, PanelType.HUD));

        if(isFixedMap) overwriteButton.onClick.AddListener(() => fixedMap.playerNode.portal.gameObject.SetActive(true));
        else overwriteButton.onClick.AddListener(() => proceduralMapGenerator.playerNode.portal.gameObject.SetActive(true));

        synthesizeButton.onClick.AddListener(SynthesizeScript);
        exitButton.onClick.AddListener(() => EventManager.Instance.PostNotification(EVENT_TYPE.Open_Panel_Exclusive, this, PanelType.HUD));
        if(isFixedMap) exitButton.onClick.AddListener(() => fixedMap.playerNode.portal.gameObject.SetActive(true));
        else exitButton.onClick.AddListener(() => proceduralMapGenerator.playerNode.portal.gameObject.SetActive(true));
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
        foreach (ScriptData s in playerScriptController.PlayerScriptDictionary.Values)
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
            if ((scriptDatas[0].level == scriptDatas[1].level) && (scriptDatas[0].level < 2))
            {
                if (scriptDatas[1].level == scriptDatas[2].level)
                {
                    //Todo: level
                    //commaScript.level = scriptDatas[0].level + 2;
                    commaScript.ScriptData = CSVReader.Instance.GetRandomScriptByType(ScriptData.ScriptType.PASSIVE);
                    //Todo: level
                    //Debug.Log("level: " + commaScript.level);
                    ApplyScriptData();

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

    public void Close()
    {
        this.gameObject.SetActive(false);
    }

    public void Open()
    {
        this.gameObject.SetActive(true);
        if(isFixedMap) fixedMap.playerNode.portal.gameObject.SetActive(true);
        else proceduralMapGenerator.playerNode.portal.gameObject.SetActive(true);
    }

    public void ApplyScriptData()
    {
        ScriptData scriptData = ScriptableObject.CreateInstance<ScriptData>();
        scriptData.CopyData(commaScript.ScriptData);
        playerScriptController.ScriptData = scriptData;
        //if (selectData.level != -1) selectData.level += 1;
        Debug.Log("id: " + commaScript.ScriptData.scriptId + "\nName: " + commaScript.ScriptData.scriptName + "\nLevel: " + commaScript.ScriptData.level + "\nType: " + commaScript.ScriptData.scriptType);
    }
}