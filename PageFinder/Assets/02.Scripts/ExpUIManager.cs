using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExpUIManager : MonoBehaviour
{
    public Canvas Exp_Canvas;
    public Slider Exp_Slid;

    private void Start()
    {
        ResetExpBar();
    }

    /// <summary>
    /// Exp Bar의 value 값을 변경한다.
    /// </summary>
    public void ResetExpBar()
    {
        Exp_Slid.value = 0;
    }

    /// <summary>
    /// Exp Bar의 값을 변경한다. 
    /// </summary>
    /// <param name="currentExp">현재 경험치</param>
    /// <param name="totalExp">총 경험치</param>
    public void ChangeExpBarValue(float currentExp, float totalExp)
    {
        Exp_Slid.value = currentExp / totalExp;
    }

    // 나중에 경험치가 일정하게 차는 애니메이션 구현하기 

}
