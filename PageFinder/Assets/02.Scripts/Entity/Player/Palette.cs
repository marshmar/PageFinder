using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UI;

public class Palette : MonoBehaviour
{
    Color currentColor = Color.green; //    0:빨     1:초     2:파

    List<Color> totalColors = new List<Color>() { Color.red, Color.blue, Color.green}; // 리스트 사용 이유 : 게임 시 계속 추가하거나 삭제될 수 있기에 

    // 스크립트 관련
    PaletteUIManager paletteManager;

    private void Start()
    {
        paletteManager = GameObject.Find("UIManager").GetComponent<PaletteUIManager>();
    }

    /// <summary>
    /// 현재 색깔을 변경한다.
    /// </summary>
    /// <param name="color"></param>
    public void ChangeCurrentColor(Color color)
    {
        if (color.Equals(Color.red))
            currentColor = Color.red;
        else if (color.Equals(Color.green))
            currentColor = Color.green;
        else if (color.Equals(Color.blue))
            currentColor = Color.blue;
        else if (color.Equals(Color.magenta))
            currentColor = Color.magenta;
        else if (color.Equals(Color.yellow))
            currentColor = Color.yellow;
        else if (color.Equals(Color.cyan))
            currentColor = Color.yellow;
        else
        {
            Debug.LogWarning(color);
            currentColor = Color.clear;
        }
    }

    /// <summary>
    /// 사용할 색깔을 얻는다. 
    /// </summary>
    /// <param name="colorIndex"></param>
    /// <returns></returns>
    public Color GetColorToUse(int colorIndex) // 위의 아이콘 색깔, 현재 아이콘 색깔, 아래 아이콘 색깔
    {
        if (colorIndex >= totalColors.Count || colorIndex <= -1) // totalColors의 인덱스에 최소, 최대를 넘어가는 경우
        {
            Debug.LogWarning("인덱스 초과");
            return Color.clear;
        }
       
        return totalColors[colorIndex];
    }

    public int GetTotalColorCount()
    {
        return totalColors.Count;
    }

    /// <summary>
    /// 현재 색깔을 얻는다. 
    /// </summary>
    /// <returns></returns>
    public Color GetCurrentColor()
    {
        return currentColor;
    }

    /// <summary>
    /// 새로운 색깔을 추가한다.
    /// </summary>
    /// <param name="color"></param>
    public void AddNewColor(Color color)
    {
        totalColors.Add(color);
        paletteManager.SetPaletteObjects();
    }
}
