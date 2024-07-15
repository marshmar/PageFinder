using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stingray : LongRangeAttackEnemy
{
    // Start is called before the first frame update
    public override void Start()
    {
        // 초기 세팅해야할 값
        moveSpeed = 1;
        maxHP = 40;
        currHP = 40;
        atk = 10;
        def = 10;

        originalPos = transform.position;
        posType = PosType.SKY;
        moveType = MoveType.PATH;
        attackType = AttackType.PREEMPTIVE;

        traceDist = 15;
        attackDist = 10;
        cognitiveDist = 20;

        base.Start();
    }
}
