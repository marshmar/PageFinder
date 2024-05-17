using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    int currentLevel = 0;

    // 스크립트 관련
    LevelUIManager levelUIManager;
    private void Awake()
    {
        levelUIManager = GameObject.Find("UIManager").GetComponent<LevelUIManager>();
        IncreaseCurrentLevel(0);
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
        levelUIManager.SetLevel_Txt(currentLevel);
    }


}
