using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpBar : SliderBar
{
    [SerializeField]
    protected Slider shieldBar;

    // Hp Bar 동작 방식

    // 
    /* 전체 Bar = 현재 Hp + 쉴드량 
     * 
     * Max Hp는 그대로 유지, 현재 Hp로부터 추가되는 실드량 만큼 오른쪽부터 채우기
     *
     * currHp + maxShieldValue > Max Hp : 오른쪽 끝부터 왼쪽 방향으로 쉴드 채우기
     * currHp + maxShieldValue <= Max Hp : curr Hp 오른쪽 끝부터 오른쪽 방향으로 쉴드 채우기
     * 
     * entity 클래스에서 쉴드량을 정의해야 함
     * maxShieldValue
     * currShieldValue
     * 
     * bar.maxValue와 bar.currValue 값을 이용하여 shieldBar 크기와 위치를 조절하여 쉴드를 만들어야함 
     */

    /// <summary>
    /// SliderBar의 최대 값을 조정하는 함수
    /// </summary>
    /// <param name="maxValue"></param>
    public void SetMaxValueUI(float maxHp, float currHp, float maxShieldValue, float currShieldValue)
    {
        //SetShieldBarData(maxHp, currHp, maxShieldValue, currShieldValue);
        bar.maxValue = maxHp;
    }

    /// <summary>
    /// SliderBar의 현재 값을 조정하는 함수
    /// </summary>
    /// <param name="currValue"></param>
    public void SetCurrValueUI(float maxHp, float currHp, float maxShield, float currShield)
    {
        SetShieldBarData(maxHp, currHp, maxShield, currShield);
        bar.value = currHp;
    }


    private void SetShieldBarData(float maxHp, float currHp, float maxShield, float currShield)
    {
        // 바 채워지는 방향 정하기 

        if(maxShield + currHp > maxHp) // Hp 제일 끝부분에서부터 채워야하는 경우
        {
            shieldBar.direction = Slider.Direction.LeftToRight;
            shieldBar.transform.position = new Vector3(1, shieldBar.transform.position.y, 0);
            shieldBar.GetComponent<RectTransform>().pivot = new Vector2(1, 0.5f);

            Debug.Log("currHp + 쉴드 값이 최대값 보다 큼");
        }
        else
        {
            shieldBar.direction = Slider.Direction.RightToLeft;
            shieldBar.transform.position = new Vector3(-1f + 2 * currHp/maxHp, transform.position.y, transform.position.z);
            shieldBar.GetComponent<RectTransform>().pivot = new Vector2(0, 0.5f);
            Debug.Log("currHp + 쉴드 값이 최대값 보다 작음");
        }


        // shield Bar는 무조건 최대 값 1을 가지고 꽉 채워서 시작한다. 

        shieldBar.maxValue = maxShield;
        shieldBar.value = currShield;
        shieldBar.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 100 * maxShield / maxHp);
    }
}
