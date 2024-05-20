using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExpUIManager : MonoBehaviour
{
    public Canvas Exp_Canvas;
    public Slider Exp_Slid;

    static bool tmp = true; 

    private void Start()
    {
        if(tmp)
        {
            // 맨 처음에만 경험치 리셋 + 다음 씬으로 넘어가도 경험치가 리셋되지 않게함
            tmp = false;
            ResetExpBar();
        }
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
    public IEnumerator ChangeExpBarValue(float currentExp, float totalExp)
    {
        float barSpeed = 1.5f;
        float goalValue = currentExp / totalExp;

        while(Exp_Slid.value < goalValue)
        {
            Exp_Slid.value += Time.deltaTime * barSpeed;
            yield return null;
        }
        if(currentExp == totalExp)
        {
            yield return new WaitForSeconds(0.2f);
            ResetExpBar();
        }   
    }
}
