using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    int currentLevel = 0;

    public GameObject[] levelUIObject;

    private void Start()
    {
        IncreaseCurrentLevel(1);
    }

    /// <summary>
    /// 현재 레벨을 리턴한다. 
    /// </summary>
    /// <returns></returns>
    public int ReturnCurrentLevel()
    {
        return currentLevel;
    }

    /// <summary>
    /// 현재 레벨을 증가시킨다.
    /// </summary>
    /// <param name="value">증가시킬 값</param>
    public void IncreaseCurrentLevel(int value)
    {
        currentLevel += value;

        for (int i=0; i< levelUIObject.Length; i++)
        {
            levelUIObject[i].GetComponent<LevelUIManager>().SetLevel_Txt(currentLevel);
        }
    }
}
