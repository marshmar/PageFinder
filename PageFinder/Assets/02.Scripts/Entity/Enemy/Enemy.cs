using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : Entity
{
    [SerializeField]
    private Slider hpBar;

    // 이동할 좌표
    public Vector3 originalPos;

    protected int posType; // 포지션(육상, 비행)
    protected int moveType; // 행동 패턴(경로이동, 랜덤이동)
    protected int attackType; // 공격 성향(선공, 지속 선공)

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

        currHP = maxHP;
        hpBar.maxValue = maxHP;
        hpBar.value = maxHP;

        meshRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();
    }
}
