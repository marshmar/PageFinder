using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PageIndicatorUI : MonoBehaviour, IUIElement
{
    [SerializeField] private Image pageIndicateIcon;
    [SerializeField] private TMP_Text pageIndciateText;

    [SerializeField] private ProceduralMapGenerator proceduralMapGenerator;

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

    public void Open()
    {
        if (pageIndicateIcon != null)
            pageIndicateIcon.gameObject.SetActive(true);

        if (pageIndciateText != null)
            pageIndciateText.gameObject.SetActive(true);
    }

    public void Refresh()
    {
        SetPageIndicateTextAndIcons();
    }
    
    private void SetPageIndicateTextAndIcons()
    {
        if(proceduralMapGenerator == null)
        {
            Debug.LogError("Need To Get ProceduralMapGenerator");
            return;
        }

        Node currNode = proceduralMapGenerator.playerNode;
        string tempText = "";

        if (currNode != null)
        {
            switch (currNode.type)
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

                default:
                    break;
            }
        }

        pageIndciateText.text = tempText;
    }
}
