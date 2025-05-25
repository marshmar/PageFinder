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

    //private PlayerScriptController playerScriptController;
    private Player player;
    private List<ScriptData> scriptDatas = new();

    private List<Sticker> stickers = new();
    void Start()
    {
        //playerScriptController = DebugUtils.GetComponentWithErrorLogging<PlayerScriptController>(GameObject.FindGameObjectWithTag("PLAYER"), "PlayerScriptController");
        player = DebugUtils.GetComponentWithErrorLogging<Player>(GameObject.FindGameObjectWithTag("PLAYER"), "Player");
        synthesisButton.onClick.AddListener(OnSynthesisClickHandler);
        overwriteButton.onClick.AddListener(OnOverwriteClickHandler);
        overwriteButton.onClick.AddListener(() => EventManager.Instance.PostNotification(EVENT_TYPE.Open_Panel_Exclusive, this, PanelType.HUD));

        if(isFixedMap) overwriteButton.onClick.AddListener(() => fixedMap.playerNode.portal.gameObject.SetActive(true));
        else overwriteButton.onClick.AddListener(() => proceduralMapGenerator.playerNode.portal.gameObject.SetActive(true));

        synthesizeButton.onClick.AddListener(SynthesizeSticker);
        exitButton.onClick.AddListener(() => EventManager.Instance.PostNotification(EVENT_TYPE.Open_Panel_Exclusive, this, PanelType.HUD));
        if(isFixedMap) exitButton.onClick.AddListener(() => fixedMap.playerNode.portal.gameObject.SetActive(true));
        else exitButton.onClick.AddListener(() => proceduralMapGenerator.playerNode.portal.gameObject.SetActive(true));
    }

    private void OnSynthesisClickHandler()
    {
        synthesisPanel.SetActive(!synthesisPanel.activeInHierarchy);
        SetDiaryStickers();
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

/*    private void SetDiaryScripts()
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
    }*/

    private void SetDiaryStickers()
    {
        int index = 0;
        var stickerList = player.StickerInventory.GetPlayerStickerList();
        foreach(var sticker in stickerList)
        {
            passiveScriptElements[index].Sticker = sticker;
            index++;
        }
    }

/*    private void InitializeDiaryScripts()
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
    }*/

    private void InitializeDiaryScripts()
    {
        int index = 0;
        var stickerList = player.StickerInventory.GetPlayerStickerList();
        foreach (var sticker in stickerList)
        {
            passiveScriptElements[index].Sticker = null;
            index++;
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


    public void AddSticker(Sticker sticker)
    {
        stickers.Add(sticker);
    }

    public void RemoveSticker(Sticker sticker)
    {
        stickers.Remove(sticker);
    }

    public int GetScriptCount()
    {
        return scriptDatas.Count;
    }

    public int GetStickerCount()
    {
        return stickers.Count;
    }

/*    public void SynthesizeScript()
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
*/
    public void SynthesizeSticker()
    {
        if (stickers.Count == 3)
        {
            Debug.Log("1: " + stickers[0].GetCurrRarity() + ", 2: " + stickers[1].GetCurrRarity() + ", 3: " + stickers[2].GetCurrRarity());
            if ((scriptDatas[0].level == scriptDatas[1].level) && (scriptDatas[0].level < 2))
            {
                if (scriptDatas[1].level == scriptDatas[2].level)
                {
                    //Todo: level
                    //commaScript.level = scriptDatas[0].level + 2;
                    var randomSticker = ScriptSystemManager.Instance.GetRandomStickers(1)[0];
                    StickerData upgaradedStickerData = ScriptableObject.CreateInstance<StickerData>();
                    upgaradedStickerData.CopyData(randomSticker);
                    upgaradedStickerData.rarity += 1;
                    commaScript.ScriptSystemData = upgaradedStickerData;
                    //Todo: level
                    //Debug.Log("level: " + commaScript.level);
                    //ApplyScriptData();
                    ApplySticker();

                    if (player.StickerInventory.RemoveStikcer(stickers[0])&&
                        player.StickerInventory.RemoveStikcer(stickers[1]) &&
                        player.StickerInventory.RemoveStikcer(stickers[2]))
                    {
                        InitializeDiaryScripts();

                        commaScript.gameObject.SetActive(true);
                        SetDiaryStickers();
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
        if(isFixedMap) proceduralMapGenerator.playerNode.portal.gameObject.SetActive(true);
        else fixedMap.playerNode.portal.gameObject.SetActive(true);
    }

/*    public void ApplyScriptData()
    {
        ScriptData scriptData = ScriptableObject.CreateInstance<ScriptData>();
        scriptData.CopyData(commaScript.ScriptData);
        playerScriptController.ScriptData = scriptData;
        //if (selectData.level != -1) selectData.level += 1;
        Debug.Log("id: " + commaScript.ScriptData.scriptId + "\nName: " + commaScript.ScriptData.scriptName + "\nLevel: " + commaScript.ScriptData.level + "\nType: " + commaScript.ScriptData.scriptType);
    }*/

    public void ApplySticker()
    {
        if(commaScript.ScriptSystemData is StickerData stickerData)
        {
            player.StickerInventory.AddSticker(stickerData);
        }
    }
}
