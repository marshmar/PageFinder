using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ShieldTest : MonoBehaviour
{
    [SerializeField] private Slider hpSlider;
    [SerializeField] private Slider shieldSlider;

    [SerializeField] private float maxHP = 500;
    [SerializeField] private float curHP;
    [SerializeField] private float maxShield;
    [SerializeField] private float curShield;
    [SerializeField] private float maxShieldPercentage = 0.3f;

    [SerializeField] private float hpMangagementValue = 10.0f;
    [SerializeField] private float shieldGenerationValue = 50.0f;
    private void Start()
    {
        curHP = maxHP;
        maxShield = maxHP * maxShieldPercentage;
        curShield = 0;

        // 슬라이더 세팅
        hpSlider.maxValue = maxHP;
        hpSlider.value = curHP;
        shieldSlider.maxValue = maxHP;
    }

    public void DecreaseHP()
    {
        curHP = Mathf.Max(curHP - hpMangagementValue, 0f);
        //UpdateHealthBar();
        hpSlider.value = curHP;
        SetBarUI(maxHP, curHP, curShield);
    }

    public void IncreaseHP()
    {
        curHP = Mathf.Min(curHP + hpMangagementValue, maxHP);
        //UpdateHealthBar();
        hpSlider.value = curHP;
        SetBarUI(maxHP, curHP, curShield);
    }

    public void ActionShield()
    {
        StartCoroutine(GenerateShield());
    }

    public IEnumerator GenerateShield()
    {
        if (curShield + shieldGenerationValue > maxShield) yield break;
        curShield = Mathf.Min(curShield + shieldGenerationValue, maxShield);
        SetBarUI(maxHP, curHP, curShield);
        yield return new WaitForSeconds(2.0f);

        curShield -= shieldGenerationValue;
        SetBarUI(maxHP, curHP, curShield);
        //UpdateHealthBar();
    }

    private void SetBarUI(float maxHP, float curHP, float curShield)
    {
        if(curHP + curShield >= maxHP)
        {
            float hpRatio = curHP * (curHP / (curHP + curShield));
            hpSlider.value = hpRatio;
            shieldSlider.value = maxHP;
            
        }
        else
        {
            shieldSlider.value = curHP + curShield;
            hpSlider.value = curHP;
        }

        if(curShield == 0)
        {
            shieldSlider.value = curHP;
        }
    }
}
