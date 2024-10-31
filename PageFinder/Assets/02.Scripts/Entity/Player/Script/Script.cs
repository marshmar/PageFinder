using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Script : MonoBehaviour
{
    public ScriptData scriptData;
    public int level;

    Image[] images;
    TMP_Text[] texts;


    private void Awake()
    {
        images = GetComponentsInChildren<Image>();
        images[0].sprite = scriptData.scriptBG;
        images[1].sprite = scriptData.scriptIcon;

        texts = GetComponentsInChildren<TMP_Text>();
        texts[0].text = scriptData.scriptName;
        texts[1].text = scriptData.scriptTypeDesc;
        texts[2].text = scriptData.scriptDesc;
    }

}
