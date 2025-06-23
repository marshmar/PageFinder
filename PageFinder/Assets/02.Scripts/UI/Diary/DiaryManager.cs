using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiaryManager : MonoBehaviour, IUIPanel
{
    [SerializeField] private Button exitButton;
    [SerializeField] private DiaryElement basickAttackScriptElement;
    [SerializeField] private DiaryElement dashScriptElement;
    [SerializeField] private DiaryElement skillScriptElement;
    [SerializeField] private GameObject stickerElementsPanel;
    [SerializeField] private List<DiaryElement> stickerElements;
    
    private PlayerScriptController playerScriptController;
    private Player player;

    public PanelType PanelType => PanelType.Diary;

    private void Awake()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("PLAYER");
        playerScriptController = DebugUtils.GetComponentWithErrorLogging<PlayerScriptController>(playerObj, "PlayerScriptController");
        player = playerObj.GetComponent<Player>();

        exitButton.onClick.AddListener(() => EventManager.Instance.PostNotification(EVENT_TYPE.Close_Top_Panel, this));

        stickerElements = new List<DiaryElement>();
        var stickerArray = stickerElementsPanel.GetComponentsInChildren<DiaryElement>();
        foreach(var sticker in stickerArray)
        {
            stickerElements.Add(sticker);
        }
    }

    private void OnDestroy()
    {
        exitButton.onClick.RemoveAllListeners();
    }

    /*    public void SetDiaryUICanvasState(bool value, string nextState = "")
        {
            diaryCanvas.gameObject.SetActive(value);
            if (!value) return;

            SetDiaryScripts();
            SetExitEvent(nextState);
        }*/

    /*private void SetExitEvent(string nextState)
    {
        switch (nextState)
        {
            
            // ToDo: UI Changed;
            *//*            case "Battle":
                            exitButton.onClick.AddListener(() => EventManager.Instance.PostNotification(EVENT_TYPE.UI_Changed, this, UIType.Battle));
                            break;
                        case "Reward":
                            exitButton.onClick.AddListener(() => EventManager.Instance.PostNotification(EVENT_TYPE.UI_Changed, this, UIType.BackDiaryFromReward));
                            break;
                        case "Shop":
                            exitButton.onClick.AddListener(() => EventManager.Instance.PostNotification(EVENT_TYPE.UI_Changed, this, UIType.BackDiaryFromShop));
                            break;
                        default:
                            break;*//*
        }
    }*/

    public void SetDiaryScripts()
    {
        int index = 0;
        foreach(ScriptData s in playerScriptController.PlayerScriptDictionary.Values)
        {
            switch (s.scriptType)
            {
                case ScriptData.ScriptType.BasicAttack:
                    basickAttackScriptElement.ScriptData = s;
                    break;
                case ScriptData.ScriptType.Dash:
                    dashScriptElement.ScriptData = s;
                    break;
                case ScriptData.ScriptType.Skill:
                    skillScriptElement.ScriptData = s;
                    break;
                case ScriptData.ScriptType.Passive:
                    //passiveScriptElements[index].ScriptData = s;
                    index++;
                    break;
            }
        }
    }

    public void SetDiaryScriptsNew()
    {
        basickAttackScriptElement.elementType = DiaryElementType.Script;
        dashScriptElement.elementType = DiaryElementType.Script;
        skillScriptElement.elementType = DiaryElementType.Script;

        basickAttackScriptElement.Script = player.ScriptInventory.GetPlayerScriptByScriptType(NewScriptData.ScriptType.BasicAttack);
        dashScriptElement.Script = player.ScriptInventory.GetPlayerScriptByScriptType(NewScriptData.ScriptType.Dash);
        skillScriptElement.Script = player.ScriptInventory.GetPlayerScriptByScriptType(NewScriptData.ScriptType.Skill);
    }

    public void Open()
    {
        this.gameObject.SetActive(true);
        //SetDiaryScripts();
        SetDiaryScriptsNew();
        SetDiaryStickers();
    }

    public void SetDiaryStickers()
    {
        //var stickerList = player.StickerInventory.GetPlayerStickerList();
        var stickerList = player.StickerInventory.GetUnEquipedStickerList();
        Debug.Log($"장착되지 않은 스티커 카운트: {stickerList.Count}");
        for(int i = 0; i < stickerElements.Count; i++)
        {
            stickerElements[i].elementType = DiaryElementType.Sticker;
            if (i < stickerList.Count)
            {
                stickerElements[i].Sticker = stickerList[i];
            }
            else
            {
                stickerElements[i].Sticker = null;
            }

        }

    }

    public void Close()
    {
        this.gameObject.SetActive(false);
    }
}