using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : Entity
{
    [SerializeField]
    private Slider hpBar;

    protected MeshRenderer meshRenderer;
    // 에너미의 사망 여부
    protected bool isDie = false;

    public override float HP
    {
        get { return currHP; }
        set
        {
            currHP = value;
            hpBar.value = currHP;
            if (currHP <= 0)
            {
                Die();
            }
        }
    }
    public override void Start()
    {
        base.Start();

        maxHP = 40.0f;
        atk = 10.0f;
        currHP = maxHP;
        hpBar.maxValue = maxHP;
        hpBar.value = maxHP;

        meshRenderer = GetComponent<MeshRenderer>();
    }
}
