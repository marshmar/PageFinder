using UnityEngine;

public interface IUIPanel
{
    public PanelType PanelType { get; }
    public void Open();
    public void Close();
}
