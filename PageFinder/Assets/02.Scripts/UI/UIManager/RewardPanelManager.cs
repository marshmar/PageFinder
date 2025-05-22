using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class RewardPanelManager : MonoBehaviour, IUIPanel
{
    public PanelType PanelType => PanelType.Reward;

    private bool canDrawReward = true;

    private ScriptData selectData;
    private NewScriptData selectDataNew;
    private PlayerScriptController playerScriptController;
    private ScriptInventory scriptInventory;

    [Header("Rewards")]
    [SerializeField] private Script[] rewards;

    [Header("Button")]
    [SerializeField] private Button diaryButton;
    [SerializeField] private Button selectButton;

    public ScriptData SelectData { get => selectData; set => selectData = value; }
    public NewScriptData SelectDataNew { get => selectDataNew; set => selectDataNew = value; }
    public bool CanDrawReward { get => canDrawReward; set => canDrawReward = value; }

    private void Awake()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("PLAYER");
        playerScriptController = playerObj.GetComponent<PlayerScriptController>();
        scriptInventory = playerObj.GetComponent<ScriptInventory>();

        diaryButton.onClick.AddListener(() => EventManager.Instance.PostNotification(EVENT_TYPE.Open_Panel_Stacked, this, PanelType.Diary));
        //selectButton.onClick.AddListener(() => ApplyScriptData());
        selectButton.onClick.AddListener(() => ApplyScriptDataNew());
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
            //SetDistinctReward();
            SetDistinctRewardNew();
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
            //rewards[i].level = distinctScriptDatas[i].level;
            rewards[i].SetScriptUI();
        }
    }

    private void SetDistinctRewardNew()
    {
        var distinctScriptDatas = ScriptSystemManager.Instance.GetDistinctRandomScriptsNew(3);
        if (distinctScriptDatas == null)
        {
            Debug.LogError("Failed to create distinctScripts");
            return;
        }

        for (int i = 0; i < rewards.Length; i++)
        {
            rewards[i].NewScriptData = distinctScriptDatas[i];
            //rewards[i].level = distinctScriptDatas[i].level;
            rewards[i].SetScriptUINew();
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

    public void ApplyScriptDataNew()
    {
        /*        NewScriptData scriptData = ScriptableObject.CreateInstance<NewScriptData>();
                scriptData.CopyData(selectDataNew);*/

        Debug.Log("============Selected script info============");
        Debug.Log($"scriptID: {selectDataNew.scriptID}");
        Debug.Log($"scriptName: {selectDataNew.scriptName}");
        Debug.Log($"scriptRarity: {selectDataNew.rarity}");
        Debug.Log($"scriptMaxRarity: {selectDataNew.maxRarity}");
        Debug.Log($"scriptType: {selectDataNew.scriptType}");
        Debug.Log($"scriptInkType: {selectDataNew.inkType}");
        Debug.Log("============================================");

        scriptInventory.AddScript(selectDataNew);
    }
}
