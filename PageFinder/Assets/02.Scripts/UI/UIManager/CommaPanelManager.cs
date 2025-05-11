using UnityEngine;

public class CommaPanelManager : MonoBehaviour, IUIPanel
{
    [SerializeField] private ProceduralMapGenerator proceduralMapGenerator;
    public PanelType PanelType => PanelType.Comma;

    public void Close()
    {
        this.gameObject.SetActive(false);
    }

    public void Open()
    {
        this.gameObject.SetActive(true);
        proceduralMapGenerator.playerNode.portal.gameObject.SetActive(true);
    }

}
