using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommaUIManager : MonoBehaviour
{
    [SerializeField] private Canvas commaUICanvas;
    [SerializeField] private Button synthesisButton;
    [SerializeField] private Button overwriteButton;
    [SerializeField] private GameObject synthesisPanel;
    [SerializeField] private GameObject overwritePanel;
    [SerializeField] private List<DiaryElement> passiveScriptElements;

    private PlayerScriptController playerScriptController;

    void Start()
    {
        playerScriptController = DebugUtils.GetComponentWithErrorLogging<PlayerScriptController>(GameObject.FindGameObjectWithTag("PLAYER"), "PlayerScriptController");
        synthesisButton.onClick.AddListener(OnSynthesisClickHandler);
        overwriteButton.onClick.AddListener(OnOverwriteClickHandler);
        overwriteButton.onClick.AddListener(() => EventManager.Instance.PostNotification(EVENT_TYPE.UI_Changed, this, UIType.PageMap));
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
}