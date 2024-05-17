using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelUIManager : MonoBehaviour
{
    public TMP_Text Level_Txt;

    /// <summary>
    /// 레벨 텍스트의 값을 설정한다. 
    /// </summary>
    /// <param name="value">현재 레벨</param>
    public void SetLevel_Txt(int currentLevel)
    {
        Level_Txt.text = currentLevel.ToString();
    }
}
