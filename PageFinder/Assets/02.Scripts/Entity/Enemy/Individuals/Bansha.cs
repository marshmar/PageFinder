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

    protected override void InitStat()
    {
        base.InitStat();

        // 상급 몬스터이기 때문에 스킬 조건은 true로 설정하여 쿨타임에만 의존하게 한다.
        //skillConditions[0] = true;
    }

    protected override void BasicAttack()
    {
        SetBullet(SoundWaveObject_Prefab, 0, atk);
    }

    // Sound Wave
    private void Skill0()
    {
        int[] angles = { -30, 0, 30 };

        for(int i=0; i< angles.Length; i++)
            SetBullet(SoundWaveObject_Prefab, angles[i], atk);
    }
}
