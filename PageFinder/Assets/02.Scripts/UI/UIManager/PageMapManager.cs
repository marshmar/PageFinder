using UnityEngine;

public class PageMapManager : MonoBehaviour, IUIPanel
{
    public PanelType PanelType => PanelType.PageMap;

    public void Close()
    {
        this.gameObject.SetActive(false);
    }

    public void Open()
    {
        this.gameObject.SetActive(true);
    }

}
