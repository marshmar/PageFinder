using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Benshee : LongRangeAttackEnemy
{

    public GameObject SoundWaveObject_Prefab;

    float maxSoundWaveReloadTime = 0.3f;
    float currentSoundWaveReloadTime = 0;

    int soundWavesIndex = 0;
    GameObject[] SoundWaves = new GameObject[3];


    public override void Start()
    {
        base.Start();

        // 투사체 관련
        for (int i = 0; i < SoundWaves.Length; i++)
        {
            SoundWaves[i] = Instantiate(SoundWaveObject_Prefab, new Vector3(monsterTr.position.x, -10, monsterTr.position.z), Quaternion.identity, GameObject.Find("Projectiles").transform); //GameObject.Find("Bullet").transform
            SoundWaves[i].name = gameObject.name + " - SoundWaves" + i;
            SoundWaves[i].GetComponent<Projectile>().ParentName = gameObject.name;
            SoundWaves[i].SetActive(false);
        }
    }

    public override void Update()
    {
        SetReloadTime();
        SetSoundWaveReloadTime();
    }

    protected override IEnumerator EnemyAction()
    {
        while (!isDie)
        {
            SetCurrentSkillCoolTime();
            ChangeCurrentStateToSkillState();

            switch (state)
            {
                case State.IDLE:
                    ani.SetBool("isIdle", true);
                    ani.SetBool("isMove", false);
                    ani.SetBool("isAttack", false);
                    ani.SetBool("isStun", false);
                    break;
                case State.MOVE:
                    ani.SetBool("isIdle", false);
                    ani.SetBool("isMove", true);
                    ani.SetBool("isAttack", false);
                    ani.SetBool("isStun", false);

                    agent.SetDestination(posToMove[currentPosIndexToMove]);
                    agent.stoppingDistance = 0;
                    agent.isStopped = false;
                    break;
                case State.TRACE:
                    ani.SetBool("isIdle", false);
                    ani.SetBool("isMove", true);
                    ani.SetBool("isAttack", false);
                    ani.SetBool("isStun", false);

                    agent.SetDestination(playerTr.position);
                    agent.stoppingDistance = attackDist;
                    agent.isStopped = false;
                    break;
                case State.ATTACK:
                    ani.SetBool("isIdle", false);
                    ani.SetBool("isMove", false);
                    ani.SetBool("isAttack", true);
                    ani.SetBool("isStun", false);

                    agent.SetDestination(playerTr.position);
                    agent.stoppingDistance = attackDist;
                    agent.isStopped = true;
                    FireProjectileObject();
                    break;
                case State.STUN:
                    ani.SetFloat("stunTime", stunTime);
                    ani.SetBool("isIdle", false);
                    ani.SetBool("isMove", false);
                    ani.SetBool("isAttack", false);
                    ani.SetBool("isStun", true);

                    agent.isStopped = true;
                    break;
                case State.SKILL:
                    // 해당 적 클래스에서 재정의하여 원하는 스킬을 호출한다. 
                    Debug.Log("Skill 사용");
                    agent.SetDestination(playerTr.position);
                    agent.isStopped = true;
                    SoundWave();
                    break;
                case State.DIE:
                    Die();
                    break;
            }
            yield return null;
        }
    }

    void SoundWave()
    {
        if (currentSoundWaveReloadTime < maxSoundWaveReloadTime)
            return;

        int soundWavesIndex = FindSoundWaveThatCanBeUsed();

        if (soundWavesIndex == -1) // 사용할 수 있는 총알이 없을 경우 
            return;
        else if(soundWavesIndex == SoundWaves.Length-1) // 마지막 음파를 날리고 난 뒤 스킬 사용할 수 있도록 초기화
        {
            skillUsageStatus[0] = false;
            state = State.MOVE;
        }
           

        //Debug.Log("총알 발사");
        SoundWaves[soundWavesIndex].SetActive(true);
        SoundWaves[soundWavesIndex].GetComponent<Projectile>().Init();
        currentSoundWaveReloadTime = 0;
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

    protected void SetSoundWaveReloadTime()
    {
        if (currentSoundWaveReloadTime >= maxSoundWaveReloadTime)
            return;

        currentSoundWaveReloadTime += Time.deltaTime;
    }
}
