using UnityEngine;

public class HudPanelManager : MonoBehaviour, IUIPanel
{
    public PanelType PanelType => PanelType.HUD;

    private PlayerUI playerUI;
    //private BossUI bossUI;
    //private PageIndicatorUI mapIndicatorUI;

    private void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("PLAYER");
        if(playerObject != null)
        {
            playerUI = playerObject.GetComponent<PlayerUI>();
        }
    }
    public void Close()
    {
        playerUI.Close();
        //mapIndicatorUI.Close();
        this.gameObject.SetActive(false);
    }

    public void Open()
    {
        this.gameObject.SetActive(true);

        playerUI.Open();
        //mapIndicatorUI.Open();

        playerUI.Refresh();
        //mapIndicatorUI.Refresh();
    }
}
