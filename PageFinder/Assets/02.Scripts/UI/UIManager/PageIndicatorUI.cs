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
                tempText = "배틀 페이지";
                break;
            case NodeType.Boss:
                tempText = "보스 페이지";
                break;
            case NodeType.Comma:
                tempText = "콤마 페이지";
                break;
            case NodeType.Market:
                tempText = "상점 페이지";
                break;
            case NodeType.Treasure:
                tempText = "트레저 페이지";
                break;
            case NodeType.Quest:
                tempText = "퀘스트 페이지";
                break;
            default:
                break;
        }

        pageIndciateText.text = tempText;
    }
}
