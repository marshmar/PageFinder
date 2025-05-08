using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShieldManager : Subject
{
    [SerializeField] private List<Shield> temporaryShields = new List<Shield>(8);
    [SerializeField] private List<Shield> permanentShields = new List<Shield>(8);
    [SerializeField] private List<Shield> toRemoveShields = new List<Shield>(8);
    private float curShield;
    private Stat maxShield;
    public float CurShield { get => curShield; set => curShield = value; }
    public Stat MaxShield { get => maxShield;}

    private WaitForSeconds shieldDelayDuration = new WaitForSeconds(1.0f);
    private bool isAbleCreateShield = true;

    private readonly object shieldLock = new object();

    [System.Serializable]
    public class Shield
    {
        public float maxValue;
        public float curValue;
        public float duration;
        public float timeRemaning;
        public Shield(float maxValue, float duration)
        {
            this.maxValue = maxValue;
            this.curValue = maxValue;
            this.duration = duration;
            this.timeRemaning = duration;
        }

        public void UpdateShield(float deltaTime)
        {
            // 지속시간이 없는 실드일경우 지속시간 계산 x
            if (duration == -1) return;

            timeRemaning -= deltaTime;
            if (timeRemaning <= 0)
            {
                curValue = 0;
            }
        }
    }

    void Update()
    {
        // 지속시간이 존재하는 실드가 생성되었을 경우 실드 지속시간 계산
        if (temporaryShields.Count >= 1)
        {
            UpdateShieldRemaningTime();
        }

        // 지워야할 실드가 존재할 경우 실드 지우기
        if (toRemoveShields.Count >= 1)
        {
            // 실드 제거시에는 lock을 걸어 CalculateDamageWithDecreasingShield 함수가 대기하도록 설정
            lock (shieldLock)
            {
                foreach (var shield in toRemoveShields)
                {
                    if (shield.duration >= 0)
                    {
                        temporaryShields.Remove(shield);
                    }
                    else
                    {
                        permanentShields.Remove(shield);
                    }
                }
                toRemoveShields.Clear();
            }

        }
    }


    private void UpdateShieldRemaningTime()
    {
        bool updated = false;
        for (int i = temporaryShields.Count - 1; i >= 0; i--)
        {
            temporaryShields[i].UpdateShield(Time.deltaTime);
            if (temporaryShields[i].timeRemaning <= 0)
            {
                temporaryShields[i].curValue = 0;
                toRemoveShields.Add(temporaryShields[i]);
                //temporaryShields.Remove(temporaryShields[i]);

                updated = true;
            }
        }
        if (updated)
        {
            UpdateShieldValues();
            NotifyObservers();
        }
    }

    public void Init(float maxValue)
    {
        maxShield = new Stat(maxValue);
    }

    public void GenerateShield(float value, float duration)
    {
        if (!isAbleCreateShield || curShield >= maxShield.Value) return; // 실드생성 쿨타임이거나 현재 실드가 maxShield보다 크거나 같을 때 실드 생성 제한

        Shield shield = new Shield(value, duration);

        // 실드 추가시에 maxShield보다 커지지 않도록
        if (curShield + shield.curValue > maxShield.Value) shield.curValue = maxShield.Value - curShield;

        if (duration == -1) permanentShields.Insert(0, shield);
        else temporaryShields.Insert(0, shield);
        UpdateShieldValues();

        StartCoroutine(ShieldDelay());
    }

    private void UpdateShieldValues()
    {
        curShield = 0f;
        foreach (var shield in temporaryShields)
        {
            curShield += shield.curValue;
        }
        foreach (var shield in permanentShields)
        {
            curShield += shield.curValue;
        }

        curShield = Mathf.Min(curShield, maxShield.Value);
    }

    public float CalculateDamageWithDecreasingShield(float damage)
    {
        // 데미지 계산시에 lock을 걸어 계산 도중에 shield가 제거되는 일이 없도록 설정
        lock (shieldLock)
        {
            //  먼저 지속시간이 존재하는 실드에서 데미지 차감(오래된 실드 먼저 차감)
            damage = DecreaseShield(temporaryShields, damage);

            // 데미지가 남아있는 경우 지속시간이 존재하지 않는 실드에서 데미지 차감(오래된 실드 먼저 차감)
            if (damage > 0)
            {
                damage = DecreaseShield(permanentShields, damage);
            }

            UpdateShieldValues();
            NotifyObservers();
        }


        return damage;
    }

    private float DecreaseShield(List<Shield> shields, float damage)
    {
        for (int i = shields.Count - 1; i >= 0; i--)
        {
            if (shields[i].curValue > 0)
            {
                float absorbed = Mathf.Min(damage, shields[i].curValue);
                shields[i].curValue -= absorbed;
                damage -= absorbed;

                // 실드가 전부 단 경우 지워야할 실드 목록에 추가
                if (shields[i].curValue <= 0)
                {
                    toRemoveShields.Add(shields[i]);
                }

                // 모든 데미지가 차감되었을 경우 return
                if (damage <= 0)
                {
                    return 0;
                };
            }
        }
        return damage;
    }

    private IEnumerator ShieldDelay()
    {
        isAbleCreateShield = false;

        yield return shieldDelayDuration;

        isAbleCreateShield = true;
    }
}