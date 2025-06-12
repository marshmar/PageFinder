using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using UnityEngine;

public class PlayerState : MonoBehaviour, IListener, IObserver, IEntityState
{
    #region Variables

    #region defaultValue
    private const float defaultMaxHp                = 500f;
    private const float defaultMaxInk               = 100f;
    private const float defaultInkGain              = 0.2f;
    private const float defaultAttackSpeed          = 0f;
    private const float defaultAttackRange          = 3f;
    private const float defaultAtk                  = 50f;
    private const float defaultMoveSpeed            = 7f;
    private const float defaultCriticalChance       = 0.15f;
    private const float defaultCriticalDmg          = 1.5f;
    private const float defaultMaxShieldPercentage  = 0.3f;
    private const float defaultDmgBouns             = 0f;
    private const float defaultDmgResist            = 0f;
    private const float inkRecoverDelayTime         = 0.5f;
    #endregion

    #region MaxValue
    private const float maxHPLimit      = 1500f;
    private const float maxInkLimit     = 300f;
    private const float maxInkGain      = 0.5f;
    private const float maxAttackSpeed  = 1f;
    private const float maxMoveSpeed    = 30f;
    private const float maxCriticalDmg  = 3f;
    #endregion

    #region MinValue
    private const float minHPLimit      = 1f;
    private const float minInkLimit     = 100f;
    private const float minInkGain      = 0.1f;
    private const float minAttackSpeed  = 0f;
    private const float minMoveSpeed    = 1f;
    private const float minCriticalDmg  = 1f;
    #endregion

    #region currValue
    private ClampedStat maxHp;
    private float curHp;
    private ClampedStat maxInk;
    private float curInk;
    private ClampedStat curInkGain;
    private ClampedStat curAttackSpeed;
    private Stat curAttackRange;
    private Stat curAtk;
    private ClampedStat curMoveSpeed;
    private Stat curCriticalChance;
    private ClampedStat curCriticalDmg;
    private Stat maxShieldPercentage;
    private float curShield;
    private int coin;
    private Stat dmgBonus;
    private Stat dmgResist;
    #endregion

    private Player player;
    [SerializeField] private GameObject shieldEffect;
    [SerializeField] private ResultUIManager resultUIManager;

    #region Hashing
    private ShieldManager shieldManager;
    private WaitForSeconds inkRecoveryDelay;
    private IEnumerator inkRecoveryCoroutine;
    #endregion

    #endregion

    #region Properties
    #region Default Value Properties
    public float DefaultMaxHp 
    { 
        get => defaultMaxHp; 
    }

    public float DefaultMaxInk 
    { 
        get => defaultMaxInk; 
    }

    public float DefaultAttackSpeed 
    { 
        get => defaultAttackSpeed; 
    }

    public float DefaultAttackRange 
    { 
        get => defaultAttackRange; 
    }

    public float DefaultAtk 
    { 
        get => defaultAtk; 
    }

    public float DefaultMoveSpeed 
    { 
        get => defaultMoveSpeed; 
    }

    public float DefaultCritical 
    { 
        get => defaultCriticalChance; 
    }

    public float DefaultInkGain 
    { 
        get => defaultInkGain; 
    }

    public float DefaultCriticalDmg 
    { 
        get => defaultCriticalDmg; 
    }

    #endregion

    #region Cur value Properties
    public Stat MaxHp
    {
        get => maxHp;
    }

    public float CurHp
    {
        get => curHp;
        set
        {
            float inputDamage = curHp - value;

            if (inputDamage < 0)
            {
                curHp = curHp + -inputDamage;
                if (curHp > maxHp.Value) curHp = maxHp.Value;
            }
            else
            {
                float ReducedDamage = inputDamage * (1 - DmgResist.Value * 0.01f);
                float finalDamage = shieldManager.CalculateDamageWithDecreasingShield(ReducedDamage);
                if (finalDamage <= 0)
                {
                    player.UI.SetStateBarUIForCurValue(maxHp.Value, curHp, CurShield);
                    return;
                }

                player.UI.StartDamageFlash(curHp, finalDamage, maxHp.Value);
                curHp -= finalDamage;
                player.UI.ShowDamageIndicator(); // ToDo: 이 부분도 Event기반 프로그래밍으로 만들 수 있지 않을까?
                                                
            }

            player.UI.SetStateBarUIForCurValue(maxHp.Value, curHp, CurShield);

            if (curHp <= 0)
            {
                curHp = 0f;

                // Display defeat UI
                AudioManager.Instance.Play(Sound.dead, AudioClipType.SequenceSfx);
                resultUIManager.SetResultData(ResultType.DEFEAT, 3f);
                EventManager.Instance.PostNotification(EVENT_TYPE.Player_Dead, this);
                EventManager.Instance.PostNotification(EVENT_TYPE.Open_Panel_Exclusive, this, PanelType.Result);
            }

        }
    }

    public Stat MaxInk 
    { 
        get => maxInk; 
    }

    public float CurInk
    {
        get => curInk;
        set
        {
            curInk = Mathf.Clamp(value, 0, maxInk.Value);
            player.UI.SetCurrInkBarUI(curInk);

            if (curInk < maxInk.Value)
            {
                RecoverInk();
            }
        }
    }

    public Stat CurInkGain
    {
        get => curInkGain;
    }

    public Stat CurAttackSpeed
    {
        get => curAttackSpeed;
    }

    public Stat CurAttackRange 
    { 
        get => curAttackRange; 
    }

    public Stat CurAtk 
    { 
        get => curAtk; 
    }

    public Stat CurMoveSpeed 
    {
        get => curMoveSpeed; 
    }

    public Stat CurCriticalChance 
    { 
        get => curCriticalChance; 
    }

    public float CurShield
    {
        get => shieldManager.CurShield;
        set
        {
            curShield = Mathf.Max(0, value);
            player.UI.SetStateBarUIForCurValue(MaxHp.Value, curHp, value);
        }
    }

    public Stat MaxShield 
    { 
        get => shieldManager.MaxShield; 
    }

    public int Coin 
    { 
        get => coin; 
        set => coin = value; 
    }

    public Stat CurCriticalDmg 
    { 
        get => curCriticalDmg; 
    }

    public Stat DmgBonus 
    { 
        get => dmgBonus; 
    }

    public Stat DmgResist 
    { 
        get => dmgResist; 
    }
    #endregion

    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        // Hashing
        player = this.GetComponentSafe<Player>();
        shieldManager = this.GetComponentSafe<ShieldManager>();
        shieldManager.Attach(this);
        inkRecoveryDelay = new WaitForSeconds(inkRecoverDelayTime);
    }

    private void Start()
    {
        Initialize();

        // Event
        AddListener();

    }

    private void OnDestroy()
    {
        inkRecoveryCoroutine = null;

        RemoveListener();
    }
    #endregion

    #region Initialization
    private void Initialize()
    {
        maxHp = new ClampedStat(defaultMaxHp, minHPLimit, maxHPLimit);
        maxInk = new ClampedStat(defaultMaxInk, minInkLimit, maxInkLimit);
        curInkGain = new ClampedStat(defaultInkGain, minInkGain, maxInkGain);
        curAttackSpeed = new ClampedStat(defaultAttackSpeed, minAttackSpeed, maxAttackSpeed);
        curAttackRange = new Stat(defaultAttackRange);
        curAtk = new Stat(defaultAtk);
        curMoveSpeed = new ClampedStat(defaultMoveSpeed, minMoveSpeed, maxMoveSpeed);
        curCriticalChance = new Stat(defaultCriticalChance);
        curCriticalDmg = new ClampedStat(defaultCriticalDmg, minCriticalDmg, maxCriticalDmg);
        maxShieldPercentage = new Stat(defaultMaxShieldPercentage);
        dmgBonus = new Stat(defaultDmgBouns);
        dmgResist = new Stat(defaultDmgResist);

        // connet UI;
        player.UI.SetMaxHPBarUI();
        player.UI.SetMaxInkUI();

        // properties
        CurHp = maxHp.Value;
        CurInk = MaxInk.Value;

        // shield
        shieldManager.Init(maxHp.Value * maxShieldPercentage.Value);

        //maxHp.OnModified += SyncCurHpWithMax;
        //curAttackSpeed.OnModified += SetAttackSpeed;
    }
    #endregion

    #region Actions
    private void RecoverInk()
    {
        // is not null
        if (inkRecoveryCoroutine is not null)
        {
            StopCoroutine(inkRecoveryCoroutine);
        }
        inkRecoveryCoroutine = RecoverInkCoroutine();
        StartCoroutine(inkRecoveryCoroutine);
    }

    private IEnumerator RecoverInkCoroutine()
    {
        yield return inkRecoveryDelay;

        while (curInk < MaxInk.Value)
        {
            curInk += MaxInk.Value * CurInkGain.Value * Time.deltaTime;
            curInk = Mathf.Clamp(curInk, 0, maxInk.Value);
            player.UI.SetCurrInkBarUI(curInk);

            // 잉크 게이지 값이 회복될 때마다 이벤트 쏴주기
            EventManager.Instance.PostNotification(EVENT_TYPE.InkGage_Changed, this, curInk);
            yield return null;
        }
    }
    #endregion

    #region Getter
    #endregion

    #region Setter
    #endregion

    #region Utilities
    /// <summary>
    /// Final damage calculation including critical hits.
    /// </summary>
    /// <param name="damageMultiplier"></param>
    /// <returns></returns>
    public float CalculateDamageAmount(float damageMultiplier)
    {
        if (CheckCritical())
            return curAtk.Value * damageMultiplier * curCriticalDmg.Value * (1 + DmgBonus.Value);
        else
            return curAtk.Value * damageMultiplier * (1 + DmgBonus.Value);
    }

    private bool CheckCritical()
    {
        return Random.Range(0f, 100f) <= curCriticalChance.Value;
    }
    #endregion

    #region Events
    public void AddListener()
    {
        EventManager.Instance.AddListener(EVENT_TYPE.Generate_Shield_Player, this);
    }

    public void RemoveListener()
    {
        EventManager.Instance.RemoveListener(EVENT_TYPE.Generate_Shield_Player, this);
    }

    public void OnEvent(EVENT_TYPE eventType, Component Sender, object param)
    {
        switch (eventType)
        {
            case EVENT_TYPE.Generate_Shield_Player:
                var shieldData = (System.Tuple<float, float, InkType>)param;
                float shieldAmount = shieldData.Item1;
                float shieldDuration = shieldData.Item2;
                InkType shieldInkType = shieldData.Item3;

                shieldManager.GenerateShield(shieldAmount, shieldDuration);
                player.UI.SetStateBarUIForCurValue(maxHp.Value, curHp, CurShield);
                shieldEffect.SetActive(true);
                break;
        }
    }

    public void Notify(Subject subject)
    {
        player.UI.SetStateBarUIForCurValue(maxHp.Value, curHp, CurShield);

        if (CurShield <= 0)
        {
            shieldEffect.SetActive(false);
        }
    }
    #endregion


    /*    private void SyncCurHpWithMax()
        {
            // maxHP가 늘어날 경우, 늘어난 만큼의 체력을 현재 hp에서 더해주기
            float hpInterval = value - maxHp.Value;

            maxHp.RawValue = value;
            playerUI.SetMaxHPBarUI(maxHp.Value);

            CurHp += hpInterval;
        }*/
}
