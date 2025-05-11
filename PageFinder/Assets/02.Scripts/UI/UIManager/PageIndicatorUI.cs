using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PageIndicatorUI : MonoBehaviour
{
    [SerializeField] private Image pageIndicateIcon;
    [SerializeField] private TMP_Text pageIndciateText;

    [SerializeField] private ProceduralMapGenerator proceduralMapGenerator;

    public PanelType PanelType => PanelType.PageIndicator;

    private void Awake()
    {
        proceduralMapGenerator = GameObject.Find("ProceduralMap").GetComponent<ProceduralMapGenerator>();
    }

    public void Close()
    { 
        if (pageIndicateIcon != null)
            pageIndicateIcon.gameObject.SetActive(false);

        if (pageIndciateText != null)
            pageIndciateText.gameObject.SetActive(false);
    }

    public void Open(NodeType type)
    {
        if (pageIndicateIcon != null)
            pageIndicateIcon.gameObject.SetActive(true);

        if (pageIndciateText != null)
            pageIndciateText.gameObject.SetActive(true);

        SetPageIndicateTextAndIcons(type);
    }

    public void Refresh(NodeType type)
    {
        SetPageIndicateTextAndIcons(type);
    }
    
    private void SetPageIndicateTextAndIcons(NodeType type)
    {
        string tempText = "";

        switch (type)
        {
            case NodeType.Start:
            case NodeType.Battle_Normal:
            case NodeType.Battle_Elite:
                tempText = "��Ʋ ������";
                break;
            case NodeType.Boss:
                tempText = "���� ������";
                break;
            case NodeType.Comma:
                tempText = "�޸� ������";
                break;
            case NodeType.Market:
                tempText = "���� ������";
                break;
            case NodeType.Treasure:
                tempText = "Ʈ���� ������";
                break;
            case NodeType.Quest:
                tempText = "����Ʈ ������";
                break;
            default:
                break;
        }

        pageIndciateText.text = tempText;
    }
}
