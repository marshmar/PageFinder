using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private Slider hpBar;
    protected float maxHP;
    protected float currHP;
    protected float atk;

    protected MeshRenderer meshRenderer;
    // 에너미의 사망 여부
    protected bool isDie = false;
    public float ATK
    {
        get { return atk; }
        set
        {
            atk += value;
        }
    }

    public float HP
    {
        get { return currHP; }
        set
        {
            currHP = value;
            hpBar.value = currHP;
            //meshRenderer.material.color = Color.red;
            if (currHP <= 0)
            {
                Die();
            }
        }
    }
    public virtual void Start()
    {
        maxHP = 40.0f;
        currHP = maxHP;
        hpBar.maxValue = maxHP;
        hpBar.value = maxHP;

        atk = 10.0f;
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void Die()
    {
        Destroy(this.gameObject);
    }
}
