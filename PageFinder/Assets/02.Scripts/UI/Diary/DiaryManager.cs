using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DiaryManager : MonoBehaviour
{
    [SerializeField]
    private Canvas diaryCanvas;
    [SerializeField]
    private Button exitButton;
    private UIManager UIManagerScr;

    private PlayerScriptController playerScriptControllerScr;
    [SerializeField]
    private DiaryElement basickAttackScriptElement;
    [SerializeField]
    private DiaryElement dashScriptElement;
    [SerializeField]
    private DiaryElement skillScriptElement;
    [SerializeField]
    private DiaryElement magicScriptElement;
    [SerializeField]
    private List<DiaryElement> passiveScriptElements;

    

    private void Awake()
    {
        playerScriptControllerScr = DebugUtils.GetComponentWithErrorLogging<PlayerScriptController>(
            GameObject.FindGameObjectWithTag("PLAYER"), "PlayerScriptController");
        UIManagerScr = DebugUtils.GetComponentWithErrorLogging<UIManager>(this.gameObject, "UIManager");

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetDiaryUICanvasState(bool value, string nextState = "")
    {
        diaryCanvas.gameObject.SetActive(value);
        if (!value) return;

        SetDiaryScripts();
        SetExitEvent(nextState);
        
    }

    private void SetExitEvent(string nextState)
    {
        switch (nextState)
        {
            case "Battle":
                exitButton.onClick.AddListener(() => UIManagerScr.SetUIActiveState("Battle"));
                break;
            case "Reward":
                exitButton.onClick.AddListener(() => UIManagerScr.SetUIActiveState("BackDiaryFromReward"));
                break;
            case "Shop":
                exitButton.onClick.AddListener(() => UIManagerScr.SetUIActiveState("BackDiaryFromShop"));
                break;
            default:
                break;
        }
    }

    public void SetDiaryScripts()
    {
        int index = 0;
        foreach(ScriptData s in playerScriptControllerScr.PlayerScriptDictionary.Values)
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
                case ScriptData.ScriptType.MAGIC:
                    magicScriptElement.ScriptData = s;
                    break;
                
            }
        }
    }
}
