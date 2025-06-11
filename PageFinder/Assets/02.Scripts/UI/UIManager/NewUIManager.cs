using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum PanelType
{
    Market,
    HUD,
    Setting,
    Reward,
    Result,
    Treasure,
    PageIndicator,
    Quest,
    Comma,
    Diary,
    PageMap,
    None
}

public class NewUIManager : Singleton<NewUIManager>, IListener
{
    private Dictionary<PanelType, IUIPanel> panels;
    private Stack<PanelType> panelHistory = new Stack<PanelType>();
    private PanelType currentPanel = PanelType.None;
    private PanelType previousPanel = PanelType.None;
    private int count;

    [SerializeField] private RewardPanelManager rewardPanelManager;
    [SerializeField] private ShopUIManager shopUIManager;
    [SerializeField] private PageIndicatorUI pageIndicatorUI;
    [SerializeField] private ResultUIManager resultUIManager;

    private void Start()
    {
        AddPanels();
        AddListener();
    }

    private void OnDestroy()
    {
        RemoveListener();
    }


    private void AddPanels()
    {
        panels = new Dictionary<PanelType, IUIPanel>();

        IUIPanel[] foundPanels = GetComponentsInChildren<IUIPanel>(true);
        foreach(var panel in foundPanels)
        {
            if (!panels.ContainsKey(panel.PanelType))
            {
                panels.Add(panel.PanelType, panel);
            }
        }
    }

    public void OpenPanelStacked(PanelType openPanel)
    {
        if(currentPanel != PanelType.None)
        {
            previousPanel = currentPanel;
            panelHistory.Push(currentPanel);
        }

        OpenPanel(openPanel);
    }

    public void CloseCurrentAndBack()
    {
        if (panelHistory.Count > 0)
        {
            PanelType previousPanel = panelHistory.Pop();
            OpenPanel(previousPanel);
        }
        else
        {
            Debug.LogWarning("No panel to go back to.");
        }
    }


    public bool OpenPanel(PanelType openPanel)
    {
        foreach (var panel in panels)
        {
            if (panel.Key == openPanel) panel.Value.Open();
            else panel.Value.Close();
        }

        currentPanel = openPanel;
        return true;
    }

    public void OpenPanelExclusive(PanelType openPanel)
    {
        if(OpenPanel(openPanel))
            panelHistory.Clear();
    }

    private void SwitchPanelByNodeType(NodeType nodeType)
    {
        PanelType nextUiPanel = PanelType.None;
        switch (nodeType)
        {
            case NodeType.Start:
            case NodeType.Battle_Normal:
            case NodeType.Battle_Elite:
            case NodeType.Battle_Elite1:
            case NodeType.Battle_Elite2:
            case NodeType.Boss:
            case NodeType.Unknown:
                nextUiPanel = PanelType.HUD;
                break;
            case NodeType.Quest:
                nextUiPanel = PanelType.Quest;
                break;
            case NodeType.Market:
                nextUiPanel = PanelType.Market;
                break;
            case NodeType.Treasure:
                nextUiPanel = PanelType.Treasure;
                break;
            case NodeType.Comma:
                nextUiPanel = PanelType.Comma;
                break;
        }

        if (nextUiPanel == PanelType.None)
        {
            Debug.LogError("Next Panel is None");
            return;
        }

        OpenPanel(nextUiPanel);
    }

    public void AddListener()
    {
        EventManager.Instance.AddListener(EVENT_TYPE.Stage_Start, this);
        EventManager.Instance.AddListener(EVENT_TYPE.Stage_Clear, this);
        EventManager.Instance.AddListener(EVENT_TYPE.Stage_Failed, this);
        EventManager.Instance.AddListener(EVENT_TYPE.Open_Panel_Stacked, this);
        EventManager.Instance.AddListener(EVENT_TYPE.Open_Panel_Exclusive, this);
        EventManager.Instance.AddListener(EVENT_TYPE.Close_Top_Panel, this);
    }

    public void RemoveListener()
    {
        EventManager.Instance.RemoveListener(EVENT_TYPE.Stage_Start, this);
        EventManager.Instance.RemoveListener(EVENT_TYPE.Stage_Clear, this);
        EventManager.Instance.RemoveListener(EVENT_TYPE.Stage_Failed, this);
        EventManager.Instance.RemoveListener(EVENT_TYPE.Open_Panel_Stacked, this);
        EventManager.Instance.RemoveListener(EVENT_TYPE.Open_Panel_Exclusive, this);
        EventManager.Instance.RemoveListener(EVENT_TYPE.Close_Top_Panel, this);
    }

    public void OnEvent(EVENT_TYPE eventType, Component Sender, object Param = null)
    {
        switch (eventType)
        {
            case EVENT_TYPE.Stage_Clear:
                shopUIManager.CanDrawScripts = true;
                resultUIManager.SetResultData(ResultType.StageClear, 3f);
                OpenPanelExclusive(PanelType.Result);
                break;
            case EVENT_TYPE.Stage_Start:
                Node node = (Node)Param;
                rewardPanelManager.CanDrawReward = true;
                shopUIManager.CanDrawScripts = true;
                SwitchPanelByNodeType(node.type);
                pageIndicatorUI.Open(node.type);
                break;
            case EVENT_TYPE.Stage_Failed:
                break;
            case EVENT_TYPE.Open_Panel_Stacked:
                var panelTypeStacked = (PanelType)Param;
                OpenPanelStacked(panelTypeStacked);
                break;
            case EVENT_TYPE.Open_Panel_Exclusive:
                var panelTypeExclusive = (PanelType)Param;
                if (panelTypeExclusive == PanelType.Setting && !CanOpenSettingPanel()) return;
                OpenPanelExclusive(panelTypeExclusive);
                break;
            case EVENT_TYPE.Close_Top_Panel:
                CloseCurrentAndBack();
                break;
        }
    }

    private bool CanOpenSettingPanel()
    {
        if (currentPanel == PanelType.HUD) return true;

        return false;
    }
}
