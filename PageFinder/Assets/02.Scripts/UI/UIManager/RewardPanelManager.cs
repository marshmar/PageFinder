using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class RewardPanelManager : MonoBehaviour, IUIPanel
{
    public PanelType PanelType => PanelType.Reward;

    private bool canDrawReward = true;

    private ScriptData selectData;
    private PlayerScriptController playerScriptController;

    [Header("Rewards")]
    [SerializeField] private Script[] rewards;

    [Header("Button")]
    [SerializeField] private Button diaryButton;
    [SerializeField] private Button selectButton;

    public ScriptData SelectData { get => selectData; set => selectData = value; }
    public bool CanDrawReward { get => canDrawReward; set => canDrawReward = value; }

    private void Awake()
    {

        GameObject playerObj = GameObject.FindGameObjectWithTag("PLAYER");
        playerScriptController = playerObj.GetComponent<PlayerScriptController>();

        diaryButton.onClick.AddListener(() => EventManager.Instance.PostNotification(EVENT_TYPE.Open_Panel_Stacked, this, PanelType.Diary));
        selectButton.onClick.AddListener(() => ApplyScriptData());
        selectButton.onClick.AddListener(() => EventManager.Instance.PostNotification(EVENT_TYPE.Open_Panel_Exclusive, this, PanelType.HUD));
    }

    private void OnDestroy()
    {
        diaryButton.onClick.RemoveAllListeners();
        selectButton.onClick.RemoveAllListeners();
    }

    public void Open()
    {
        this.gameObject.SetActive(true);

        if (canDrawReward)
        {
            canDrawReward = false;
            SetDistinctReward();
        }
    }

    public void Close()
    {
        this.gameObject.SetActive(false);
    }

    private void SetDistinctReward()
    {
        var distinctScriptDatas = ScriptSystemManager.Instance.GetDistinctRandomScripts(3);
        if(distinctScriptDatas == null)
        {
            Debug.LogError("Failed to create distinctScripts");
            return;
        }  
        
        for(int i = 0; i < rewards.Length; i++)
        {
            rewards[i].ScriptData = distinctScriptDatas[i];
            rewards[i].level = distinctScriptDatas[i].level;
            rewards[i].SetScriptUI();
        }
    }

    public void ApplyScriptData()
    {
        ScriptData scriptData = ScriptableObject.CreateInstance<ScriptData>();
        scriptData.CopyData(selectData);
        playerScriptController.ScriptData = scriptData;
        //if (selectData.level != -1) selectData.level += 1;
        Debug.Log("id: " + selectData.scriptId + "\nName: " + selectData.scriptName + "\nLevel: " + selectData.level + "\nType: " + selectData.scriptType);
    }
}
