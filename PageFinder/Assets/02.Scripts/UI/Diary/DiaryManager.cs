using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiaryManager : MonoBehaviour, IUIPanel
{
    [SerializeField] private Button exitButton;
    [SerializeField] private DiaryElement basickAttackScriptElement;
    [SerializeField] private DiaryElement dashScriptElement;
    [SerializeField] private DiaryElement skillScriptElement;
    [SerializeField] private List<DiaryElement> passiveScriptElements;
    
    private PlayerScriptController playerScriptController;

    public PanelType PanelType => PanelType.Diary;

    private void Awake()
    {
        playerScriptController = DebugUtils.GetComponentWithErrorLogging<PlayerScriptController>(GameObject.FindGameObjectWithTag("PLAYER"), "PlayerScriptController");
        exitButton.onClick.AddListener(() => EventManager.Instance.PostNotification(EVENT_TYPE.Close_Top_Panel, this));
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
                case ScriptData.ScriptType.BASICATTACK:
                    basickAttackScriptElement.ScriptData = s;
                    break;
                case ScriptData.ScriptType.DASH:
                    dashScriptElement.ScriptData = s;
                    break;
                case ScriptData.ScriptType.SKILL:
                    skillScriptElement.ScriptData = s;
                    break;
                case ScriptData.ScriptType.PASSIVE:
                    passiveScriptElements[index].ScriptData = s;
                    index++;
                    break;
            }
        }
    }

    public void Open()
    {
        this.gameObject.SetActive(true);
        SetDiaryScripts();
    }

    public void Close()
    {
        this.gameObject.SetActive(false);
    }
}