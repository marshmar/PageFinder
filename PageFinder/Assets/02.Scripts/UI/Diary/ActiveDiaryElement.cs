using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActiveDiaryElement : DiaryElement
{
    [SerializeField] private Sprite[] activeBackgroundImages;
    [SerializeField] private Image activeBackgroundImage;
    [SerializeField] private TMP_Text activeNameText;

    public override void Awake()
    {
        base.Awake();
        activeNameText.text = "";
    }

    public override void SetScriptPanels()
    {
        icon.sprite = scriptData.scriptIcon;
        switch (scriptData.inkType)
        {
            case InkType.RED:
                activeBackgroundImage.sprite = activeBackgroundImages[0];
                break;
            case InkType.GREEN:
                activeBackgroundImage.sprite = activeBackgroundImages[1];
                break;
            case InkType.BLUE:
                activeBackgroundImage.sprite = activeBackgroundImages[2];
                break;
        }

        if (scriptData.level <= 0) activeNameText.text = scriptData.scriptName;
        else activeNameText.text = scriptData.scriptName + $" +{scriptData.level}";
    }
}