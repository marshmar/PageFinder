using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using UnityEngine;

public class PlayerState : MonoBehaviour, IListener, IObserver, IEntityState
{
    #region defaultValue
    private const float defaultMaxHp = 500f;
    private const float defaultMaxInk = 100f;
    private const float defaultInkGain = 0.2f;
    private const float defaultAttackSpeed = 1f;
    private const float defaultAttackRange = 3f;
    private const float defaultAtk = 50f;
    private const float defaultMoveSpeed = 7f;
    private const float defaultCriticalChance = 0.15f;
    private const float defaultCriticalDmg = 1.5f;
    #endregion

    #region currValue
    private float maxHp;
    private float curHp;
    private float maxInk;
    private float curInk;
    private float curInkGain;
    private float curAttackSpeed;
    private float curAttackRange;
    private float curAtk;
    private float curMoveSpeed;
    private float curCriticalChance;
    private float curCriticalDmg;
    private float maxShield;
    private float maxShieldPercentage = 0.3f;
    private float curShield;
    private int coin;
    private float dmgBonus;
    private float dmgResist;
    #endregion

    #region Default Value Properties
    public float DefaultMaxHp { get => defaultMaxHp; }
    public float DefaultMaxInk { get => defaultMaxInk; }
    public float DefaultAttackSpeed { get => defaultAttackSpeed; }
    public float DefaultAttackRange { get => defaultAttackRange; }
    public float DefaultAtk { get => defaultAtk; }
    public float DefaultMoveSpeed { get => defaultMoveSpeed; }
    public float DefaultCritical { get => defaultCriticalChance; }
    public float DefaultInkGain { get => defaultInkGain; }
    public float DefaultCriticalDmg { get => defaultCriticalDmg; }

    #endregion

    #region Multiplier
    private float maxHpMultiplier = 1;
    private float curHpMultiplier = 1;
    private float curAtkMultiplier = 1;
    private float curMoveSpeedMultiplier = 1;
    private float curAttackSpeedMultiplier = 1;
    #endregion
    #region Cur value Properties
    public float MaxHp { 
        get => maxHp;
        set 
        {
            // maxHP�� �þ ���, �þ ��ŭ�� ü���� ���� hp���� �����ֱ�
            float hpInterval = value - maxHp;

            maxHp = value;
            playerUI.SetMaxHPBarUI(maxHp);

            CurHp += hpInterval;
        }
    }
    public float CurHp
    {
        get => curHp;
        set { 
            // ������ ��� ���� ���� �ʿ�
            float inputDamage = curHp - value;

            if(inputDamage < 0)
            {
                curHp = curHp + -inputDamage;
                if (curHp > maxHp) curHp = maxHp;
            }
            else
            {
                // ������ ����
                float ReducedDamage = inputDamage * (1 - dmgResist * 0.01f);
                float finalDamage = shieldManager.CalculateDamageWithDecreasingShield(ReducedDamage);
                if (finalDamage <= 0) 
                {
                    playerUI.SetStateBarUIForCurValue(maxHp, curHp, CurShield);
                    return;
                }

                playerUI.StartDamageFlash(curHp, finalDamage, maxHp);
                curHp -= finalDamage;
                playerUI.ShowDamageIndicator(); // ToDo: �� �κе� Event��� ���α׷������� ���� �� ���� ������?
                //playerUI.SetCurrHPBarUI(curHp);

                
               /* // ���������� ���� ���常ŭ ����
                inputDamage -= CurShield;
                // ���� ���尡 ���������� ���� ��� ���常 ����
                if(inputDamage <= 0)
                {
                    CurShield = -inputDamage;
                    return;
                }

                // ���� ���尡 ���������� ������� �ʰ��� ��ŭ hp ����
                if(inputDamage > 0)
                {
                    curHp = Mathf.Max(0, curHp - inputDamage);
                    CurShield = 0;
                    playerUI.ShowDamageIndicator();
                }*/
            }

            playerUI.SetStateBarUIForCurValue(maxHp, curHp, CurShield);

            if (curHp <= 0)
            {
                curHp = 0f;
                //UIManager.Instance.SetUIActiveState("Defeat");

                // �ֽ�ǥ ����
                // �ٷ� Ÿ��Ʋ �̵� ���� �й� UI ǥ���� ResultUIManager���� GameEnd�� �̵�
                EventManager.Instance.PostNotification(EVENT_TYPE.UI_Changed, this, UIType.Defeat);
                // EventManager.Instance.PostNotification(EVENT_TYPE.GAME_END, this);
            }

        }
    }

    public void ExtraInkGain()
    {
        CurInk += MaxInk * 0.05f;
    }

    public float MaxInk { get => maxInk; set { maxInk = value; playerUI.SetMaxInkUI(maxInk); } }
    public float CurInk { 
        get => curInk;
        set 
        {
            curInk = Mathf.Clamp(value, 0, maxInk);
            playerUI.SetCurrInkBarUI(curInk);

            if(curInk < maxInk)
            {
                RecoverInk();
            }
        }
    }
    public float CurInkGain
    {
        get => curInkGain; set 
        {
            curInkGain = Mathf.Clamp(value, 0.1f, 0.5f);
            //curInkGain = value; 
        }
    }
    public float CurAttackSpeed
    {
        get => curAttackSpeed;
        set { 
            curAttackSpeed = value;
            playerAttackController.SetAttckSpeed(curAttackSpeed);
        }
    }
    public float CurAttackRange { get => curAttackRange; set => curAttackRange = value; }
    public float CurAtk { get => curAtk; set => curAtk = value; }
    public float CurMoveSpeed { get => curMoveSpeed; set => curMoveSpeed = value; }
    public float CurCriticalChance { get => curCriticalChance; set => curCriticalChance = value; }

    public float CurShield {
        get => shieldManager.CurShield;
        set
        {
            curShield = Mathf.Max(0, value);
            playerUI.SetStateBarUIForCurValue(MaxHp, curHp, value);
        }
    }
    public float MaxShield { get => shieldManager.MaxShield;
        set
        {
            shieldManager.MaxShield = value;
            //playerUI.SetMaxShieldUI(MaxHp, CurHp, maxShield);
        }
    }

    public int Coin { get => coin; set => coin = value; }
    public float CurCriticalDmg { get => curCriticalDmg; set {
            curCriticalDmg = Mathf.Clamp(value, 1.0f, 3.0f);
            //curCriticalDmg = value; 
        } 
    }
    public float DmgBonus { get => dmgBonus; set => dmgBonus = value; }
    public float DmgResist { get => dmgResist;
        set
        {
            dmgResist = value;
        }
    }

    #endregion

    #region Multiplier Properties
    private Dictionary<EntityState, float> mutipliers = new Dictionary<EntityState, float>();
    public Dictionary<EntityState, float> Multipliers { get;}
    #endregion

    #region Hashing
    private ShieldManager shieldManager;
    private PlayerUI playerUI;
    private WaitForSeconds inkRecoveryDelay;
    private IEnumerator inkRecoveryCoroutine;
    private PlayerBuff playerBuff;
    private PlayerAttackController playerAttackController;
    #endregion

    #region Buff
    public bool thickVine = false;
    #endregion

    [SerializeField] private GameObject shieldEffect;
    private void Awake()
    {
        // Hashing
        playerUI = DebugUtils.GetComponentWithErrorLogging<PlayerUI>(this.gameObject, "PlayerUI");
        playerAttackController = DebugUtils.GetComponentWithErrorLogging<PlayerAttackController>(this.gameObject, "PlayerAttackController");
        shieldManager = DebugUtils.GetComponentWithErrorLogging<ShieldManager>(this.gameObject, "ShieldManager");
        shieldManager.Attach(this);
    }

    private void Start()
    {
        SetBasicState();

        inkRecoveryDelay = new WaitForSeconds(0.5f);

        EventManager.Instance.AddListener(EVENT_TYPE.Generate_Shield_Player, this);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            EventManager.Instance.PostNotification(EVENT_TYPE.Generate_Shield_Player, this, new System.Tuple<float, float>(50f, 3f));
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            CurHp -= 50.0f;
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            CurHp += 50.0f;
        }
    }
    private void SetBasicState()
    {
        // �⺻�� ����
        MaxHp = defaultMaxHp;
        CurHp = MaxHp;
        MaxInk = defaultMaxInk;
        CurInk = MaxInk;
        curInkGain = defaultInkGain;
        CurAttackSpeed = defaultAttackSpeed;
        CurAttackRange = defaultAttackRange;
        CurAtk = defaultAtk;
        CurMoveSpeed = defaultMoveSpeed;
        CurCriticalChance = defaultCriticalChance;
        CurCriticalDmg = defaultCriticalDmg;
        MaxShield = curHp * maxShieldPercentage;
        dmgBonus = 0f;
        dmgResist = 0f;
    }

    // UniTask ����ϸ� ����
    public void RecoverInk()
    {
        // is not null
        if(inkRecoveryCoroutine is not null)
        {
            StopCoroutine(inkRecoveryCoroutine);
        }
        inkRecoveryCoroutine = RecoverInkCoroutine();
        StartCoroutine(inkRecoveryCoroutine);
    }

    private void OnDestroy()
    {
        inkRecoveryCoroutine = null;
    }

    private IEnumerator RecoverInkCoroutine()
    {
        yield return inkRecoveryDelay;

        while(CurInk < MaxInk)
        {
            curInk += MaxInk * CurInkGain * Time.deltaTime;
            curInk = Mathf.Clamp(curInk, 0, maxInk);
            playerUI.SetCurrInkBarUI(curInk);

            // ��ũ ������ ���� ȸ���� ������ �̺�Ʈ ���ֱ�
            EventManager.Instance.PostNotification(EVENT_TYPE.InkGage_Changed, this, curInk);
            yield return null;
        }
    }

    private float FinalStatCalculator(float baseStat, float permanentBuff, float permanentMultiplier, float permanentDebuff, float temporaryBuff, float temporaryDebuff)
    {
        return (baseStat + permanentBuff) * (permanentMultiplier) * (1 - permanentDebuff) * (1 + temporaryBuff - temporaryDebuff);
    }

    public void OnEvent(EVENT_TYPE eventType, Component Sender, object param)
    {
        switch (eventType)
        {
            case EVENT_TYPE.Generate_Shield_Player:
                float shieldAmount = ((System.Tuple<float, float>)param).Item1;
                float shieldDuration = ((System.Tuple<float, float>)param).Item2;

                shieldManager.GenerateShield(shieldAmount, shieldDuration);
                playerUI.SetStateBarUIForCurValue(maxHp, curHp, CurShield);
                shieldEffect.SetActive(true);
                break;
        }
    }

    public void Notify(Subject subject)
    {
        playerUI.SetStateBarUIForCurValue(maxHp, curHp, CurShield);
        if(CurShield <= 0)
        {
            shieldEffect.SetActive(false);
            if (thickVine) DmgResist -= 10.0f;
        }
        else
        {
            if(thickVine) DmgResist += 10.0f;

        }
    }

    /// <summary>
    /// ���� �Է� ������ ���, damageMultiplier�� ���� ���
    /// </summary>
    /// <param name="damageMultiplier"></param>
    /// <returns></returns>
    public float CalculateDamageAmount(float damageMultiplier)
    {
        if (CheckCritical())
            return curAtk * damageMultiplier * curCriticalDmg;
        else
            return curAtk * damageMultiplier;
    }

    // ũ��Ƽ�� Ȯ�� ���
    private bool CheckCritical()
    {
        return Random.Range(0f, 100f) <= curCriticalChance;
    }
}
