using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bansha : HighEnemy
{
    [SerializeField]
    private GameObject SoundWaveObject_Prefab;

    [SerializeField]
    private int maxSoundWaveFireCnt;

    protected override void BasicAttack()
    {
        SetBullet(SoundWaveObject_Prefab, 0, curAtk, 5);
    }

    // Sound Wave
    private void Skill0()
    {
        int[] angles = { -30, 0, 30 };

        for(int i=0; i< angles.Length; i++)
            SetBullet(SoundWaveObject_Prefab, angles[i], curAtk * 1.35f, 7);
    }
}
