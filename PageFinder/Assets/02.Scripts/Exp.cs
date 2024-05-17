using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exp : MonoBehaviour
{

    // 스크립트 관련
    ExpUIManager expUIManager;
    ReinforceUIManager reinforceUIManager;
    Level level;

    float currentExp = 0;
    float totalExp = 100;

    private void Awake()
    {
        GameObject uiManager = GameObject.Find("UIManager");
        expUIManager = uiManager.GetComponent<ExpUIManager>();
        reinforceUIManager = uiManager.GetComponent<ReinforceUIManager>();
        level = GetComponent<Level>();
    }

   
    /// <summary>
    /// 경험치를 증가시킨다.
    /// </summary>
    /// <param name="value">경험치를 증가시킨다.</param>
    public void IncreaseExp(float value)
    {
        currentExp += value;

        expUIManager.ChangeExpBarValue(currentExp, totalExp);
        if(CheckIfTotalExpAndCurrentExpAreSame()) // 현재 경험치가 총 경험치를 전부 채웠을 경우(= 레벨업할 경우)
        {
            Debug.Log("LevelUp");
            
            Debug.Log("Exp Reset : " + currentExp);
            reinforceUIManager.StartCoroutine(reinforceUIManager.ActivateReinforceUI()); // 기억 시스템 UI 동작
        }

    }

    /// <summary>
    /// 경험치의 값을 0으로 리셋한다. 
    /// </summary>
    public void ResetExp()
    {
        currentExp = 0;
    }

    /// <summary>
    /// 현재 경험치를 리턴한다.
    /// </summary>
    /// <returns>현재 경험치</returns>
    public float ReturnCurrentExp()
    {
        return currentExp;
    }

    /// <summary>
    /// 총 경험치를 변경한다.
    /// </summary>
    /// <param name="value">변경할 총 경험치 값</param>
    public void ChangeTotalExp(float value)
    {
        totalExp = value;
    }

    /// <summary>
    /// 총 경험치와 현재 경험치가 같은지 체크한다.
    /// </summary>
    /// <returns>같을 경우 true</returns>
    bool CheckIfTotalExpAndCurrentExpAreSame() // 레벨업할 경우
    {
        if (currentExp == totalExp)
            return true;
        return false;
    }
}
