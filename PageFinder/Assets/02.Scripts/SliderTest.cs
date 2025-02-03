using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SliderTest : MonoBehaviour
{
    [Range(1f, 500f)]
    public float maxHP = 500;
    [SerializeField] private float curHP;
    [SerializeField] private float maxShield;
    [SerializeField] private float curShield = 0;
    [SerializeField] private float shieldPercentage = 0.3f;

    public Slider sliderBar;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        curHP = maxHP;
        maxShield = maxHP * shieldPercentage;
        sliderBar.maxValue = maxHP;
        sliderBar.value = curHP;
    }

    public void DecreaseHP()
    {
        curHP -= 10.0f;
        if (curHP <= 0) curHP = 0f;
        sliderBar.value = curHP;
    }

    public void IncreaseHP()
    {
        curHP += 10.0f;
        if (curHP > maxHP) curHP = maxHP;
        sliderBar.value = curHP;
    }

    public void AddShield()
    {
        StartCoroutine(GenerateShield());
    }

    public IEnumerator GenerateShield()
    {
        curShield += 10.0f;
        if (curShield > maxShield) curShield = maxShield;

        yield return new WaitForSeconds(0.3f);

        curShield -= 10.0f;
    }
}
