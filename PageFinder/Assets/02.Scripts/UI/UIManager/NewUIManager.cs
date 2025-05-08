using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum PanelType
{
    Shop,
    HUD,
    Setting,
    Script,
    Result,
    Treasure
}

public class NewUIManager : Singleton<NewUIManager>
{
    private Dictionary<PanelType, IUIPanel> panels;

    private void Start()
    {
        AddPanels();
    }

    private void AddPanels()
    {
        panels = new Dictionary<PanelType, IUIPanel>();

        IUIPanel[] foundPanels = GetComponentsInChildren<IUIPanel>();
        foreach(var panel in foundPanels)
        {
            if (!panels.ContainsKey(panel.PanelType))
            {
                panels.Add(panel.PanelType, panel);
            }
        }
    }

    public void SwitchPanel(PanelType openPanel)
    {
        foreach(var panel in panels)
        {
            if (panel.Key == openPanel) panel.Value.Open();
            else panel.Value.Close();
        }
    }
}
