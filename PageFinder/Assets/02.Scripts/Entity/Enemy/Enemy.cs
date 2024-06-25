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

    protected int posType = -1; // 포지션(육상, 비행)
    protected int moveType = -1; // 행동 패턴(경로이동, 랜덤이동)
    protected int attackType = -1; // 공격 성향(선공, 지속 선공)

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

        // 이전에 하위 클래스에서 값을 할당했다면 여기서 다시 초기화하지 않고,
        // 할당하지 않았다면 0으로 초기화
        if (posType == -1)
            posType = 0;

        if (moveType == -1)
            moveType = 0;

        if (attackType == -1)
            attackType = 0;


        meshRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();
    }

}
