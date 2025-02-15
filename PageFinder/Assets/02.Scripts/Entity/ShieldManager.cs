using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShieldManager : Subject
{
    [SerializeField] private List<Shield> temporaryShields = new List<Shield>(8);
    [SerializeField] private List<Shield> permanentShields = new List<Shield>(8);
    [SerializeField] private List<Shield> toRemoveShields = new List<Shield>(8);
    private float curShield;
    private float maxShield;
    public float CurShield { get => curShield; set => curShield = value; }
    public float MaxShield { get => maxShield; set => maxShield = value; }

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
            // ���ӽð��� ���� �ǵ��ϰ�� ���ӽð� ��� x
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
        // ���ӽð��� �����ϴ� �ǵ尡 �����Ǿ��� ��� �ǵ� ���ӽð� ���
        if (temporaryShields.Count >= 1)
        {
            UpdateShieldRemaningTime();
        }

        // �������� �ǵ尡 ������ ��� �ǵ� �����
        if (toRemoveShields.Count >= 1)
        {
            // �ǵ� ���Žÿ��� lock�� �ɾ� CalculateDamageWithDecreasingShield �Լ��� ����ϵ��� ����
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

    public void GenerateShield(float value, float duration)
    {
        if (!isAbleCreateShield || curShield >= maxShield) return; // �ǵ���� ��Ÿ���̰ų� ���� �ǵ尡 maxShield���� ũ�ų� ���� �� �ǵ� ���� ����

        Shield shield = new Shield(value, duration);

        // �ǵ� �߰��ÿ� maxShield���� Ŀ���� �ʵ���
        if (curShield + shield.curValue > maxShield) shield.curValue = maxShield - curShield;

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

        curShield = Mathf.Min(curShield, maxShield);
    }

    public float CalculateDamageWithDecreasingShield(float damage)
    {
        // ������ ���ÿ� lock�� �ɾ� ��� ���߿� shield�� ���ŵǴ� ���� ������ ����
        lock (shieldLock)
        {
            //  ���� ���ӽð��� �����ϴ� �ǵ忡�� ������ ����(������ �ǵ� ���� ����)
            damage = DecreaseShield(temporaryShields, damage);

            // �������� �����ִ� ��� ���ӽð��� �������� �ʴ� �ǵ忡�� ������ ����(������ �ǵ� ���� ����)
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

                // �ǵ尡 ���� �� ��� �������� �ǵ� ��Ͽ� �߰�
                if (shields[i].curValue <= 0)
                {
                    toRemoveShields.Add(shields[i]);
                }

                // ��� �������� �����Ǿ��� ��� return
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