using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActiveDiaryElement : DiaryElement
{
    [SerializeField]
    private Sprite[] activeBackgroundImages;
    [SerializeField]
    private Image activeBackgroundImage;
    [SerializeField]
    private TMP_Text activeNameText;
    // Start is called before the first frame update
    public override void Awake()
    {
        base.Awake();
        activeNameText.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        
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
        activeNameText.text = scriptData.scriptName;
    }
}
