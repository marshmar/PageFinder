using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bansha : HighEnemy
{
    [Header("SoundWave")]
    [SerializeField]
    private GameObject SoundWaveObject_Prefab;
    [SerializeField]
    private Transform soundWavePos;

    GameObject[] SoundWaves = new GameObject[3];

    [SerializeField]
    private int maxSoundWaveFireCnt;

    public override void Start()
    {
        // base.Start에서 해당 코루틴들을 미리 돌리지 않도록 설정.
        isUpdaterCoroutineWorking = true;
        isAnimationCoroutineWorking = true;

        base.Start();

        // 투사체 관련
        for (int i = 0; i < SoundWaves.Length; i++)
        {
            SoundWaves[i] = Instantiate(SoundWaveObject_Prefab, new Vector3(enemyTr.position.x, -10, enemyTr.position.z), Quaternion.identity, GameObject.Find("Projectiles").transform); //GameObject.Find("Bullet").transform
            SoundWaves[i].GetComponent<Projectile>().Init(gameObject.name, gameObject.name + " - SoundWaves" + i, 10, soundWavePos, playerObj);
        }

        // 상급 몬스터이기 때문에 스킬 조건은 true로 설정하여 쿨타임에만 의존하게 한다.
        skillCondition[0] = true;

        StartCoroutine(Updater());
        StartCoroutine(Animation());
    }

    private void Skill0()
    {
        StartCoroutine(SoundWave());
    }

    private IEnumerator SoundWave()
    {
        int soundWavesIndex;

        for (int i = 0; i < maxSoundWaveFireCnt; i++)
        {
            soundWavesIndex = FindSoundWaveThatCanBeUsed();

            if (soundWavesIndex == -1) // 사용할 수 있는 투사체가 없을 경우 
            {
                Debug.LogWarning("사용할 수 있는 SoundWave 부족함");
                continue;
            }

            SoundWaves[soundWavesIndex].SetActive(true);
            SoundWaves[soundWavesIndex].GetComponent<Projectile>().SetDirToMove();

            yield return new WaitForSeconds(0.5f);
        }
    }

    int FindSoundWaveThatCanBeUsed()
    {
        for (int i = 0; i < SoundWaves.Length; i++)
        {
            if (SoundWaves[i].activeSelf) // 사용중인 총알 
                continue;
            return i;
        }
        return -1;
    }
}
