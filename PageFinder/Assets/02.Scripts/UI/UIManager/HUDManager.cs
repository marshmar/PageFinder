using UnityEngine;

public class HUDManager : MonoBehaviour, IUIPanel
{
    public PanelType PanelType => PanelType.HUD;

    private PlayerUI playerUI;
    //private BossUI bossUI;
    private PageIndicatorUI mapIndicatorUI;

    public void Close()
    {
        playerUI.Close();
        mapIndicatorUI.Close();
        this.gameObject.SetActive(false);
    }

    public void Open()
    {
        this.gameObject.SetActive(true);

        playerUI.Open();
        mapIndicatorUI.Open();

        playerUI.Refresh();
        mapIndicatorUI.Refresh();
    }
}
