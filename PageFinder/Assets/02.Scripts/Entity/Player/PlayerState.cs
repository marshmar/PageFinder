using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using UnityEngine;

public class PlayerState : MonoBehaviour, IListener, IObserver, IEntityState
{
    #region Variables

    #region defaultValue
    private const float defaultMaxHp            = 500f;
    private const float defaultMaxInk           = 100f;
    private const float defaultInkGain          = 0.2f;
    private const float defaultAttackSpeed      = 0f;
    private const float defaultAttackRange      = 3f;
    private const float defaultAtk              = 50f;
    private const float defaultMoveSpeed        = 7f;
    private const float defaultCriticalChance   = 0.15f;
    private const float defaultCriticalDmg      = 1.5f;
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
    }

    private void Start()
    {
        SetBasicState();
        AddListener();

        inkRecoveryDelay = new WaitForSeconds(0.5f);
        curAttackSpeed.OnModified += SetAttackSpeed;
    }
    #endregion

    #region Initialization
    #endregion

    #region Actions
    #endregion

    #region Getter
    #endregion

    #region Setter
    #endregion

    #region Utilities
    #endregion

    #region Events
    public void AddListener()
    {
        EventManager.Instance.AddListener(EVENT_TYPE.Generate_Shield_Player, this);
    }

    public void OnEvent(EVENT_TYPE eventType, Component Sender, object param)
    {
        switch (eventType)
        {
            case EVENT_TYPE.Generate_Shield_Player:
                float shieldAmount = ((System.Tuple<float, float>)param).Item1;
                float shieldDuration = ((System.Tuple<float, float>)param).Item2;

                shieldManager.GenerateShield(shieldAmount, shieldDuration);
                player.UI.SetStateBarUIForCurValue(maxHp.Value, curHp, CurShield);
                shieldEffect.SetActive(true);
                break;
        }
    }
    #endregion

    #region Buff
    private bool thickVine = false;
    public float thickVineValue;
    public bool ThickVine { get => thickVine; set => thickVine = value; }
    #endregion


    private void SetAttackSpeed()
    {
        // ToDo: Need To Change 
        //player.AttackController.SetAttckSpeed(curAttackSpeed.Value);
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
        // 기본값 설정
        maxHp = new ClampedStat(defaultMaxHp, 1, 1500);
        playerUI.SetMaxHPBarUI();
        CurHp = maxHp.Value;
        maxInk = new ClampedStat(defaultMaxInk, 100, 300);
        playerUI.SetMaxInkUI();
        CurInk = MaxInk.Value;
        curInkGain = new ClampedStat(defaultInkGain, 0.1f, 0.5f);
        curAttackSpeed = new ClampedStat(defaultAttackSpeed, 0f, 1f);
        curAttackRange = new Stat(defaultAttackRange);
        curAtk = new Stat(defaultAtk);
        curMoveSpeed = new ClampedStat(defaultMoveSpeed, 1f, 30f);
        curCriticalChance = new Stat(defaultCriticalChance);
        curCriticalDmg = new ClampedStat(defaultCriticalDmg, 1f, 3f);
        maxShieldPercentage = new Stat(0.3f);
        shieldManager.Init(maxHp.Value * maxShieldPercentage.Value);
        //MaxShield = new Stat(maxHp.Value * maxShieldPercentage.Value);
        dmgBonus = new Stat(0f);
        dmgResist = new Stat(0f);
        
        
/*        MaxHp = defaultMaxHp;
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
        dmgResist = 0f;*/

        //maxHp.OnModified += SyncCurHpWithMax;
    }

/*    private void SyncCurHpWithMax()
    {
        // maxHP가 늘어날 경우, 늘어난 만큼의 체력을 현재 hp에서 더해주기
        float hpInterval = value - maxHp.Value;

        maxHp.RawValue = value;
        playerUI.SetMaxHPBarUI(maxHp.Value);

        CurHp += hpInterval;
    }*/

    // UniTask 사용하면 좋다
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

        while(CurInk < MaxInk.Value)
        {
            curInk += MaxInk.Value * CurInkGain.Value * Time.deltaTime;
            curInk = Mathf.Clamp(curInk, 0, maxInk.Value);
            playerUI.SetCurrInkBarUI(curInk);

            // 잉크 게이지 값이 회복될 때마다 이벤트 쏴주기
            EventManager.Instance.PostNotification(EVENT_TYPE.InkGage_Changed, this, curInk);
            yield return null;
        }
    }

    private float FinalStatCalculator(float baseStat, float permanentBuff, float permanentMultiplier, float permanentDebuff, float temporaryBuff, float temporaryDebuff)
    {
        return (baseStat + permanentBuff) * (permanentMultiplier) * (1 - permanentDebuff) * (1 + temporaryBuff - temporaryDebuff);
    }



    public void Notify(Subject subject)
    {
        playerUI.SetStateBarUIForCurValue(maxHp.Value, curHp, CurShield);
        if(CurShield <= 0)
        {
            shieldEffect.SetActive(false);
            //if (thickVine) DmgResist = 0.0f;
        }
    }

    /// <summary>
    /// 최종 입력 데미지 계산, damageMultiplier는 피해 계수
    /// </summary>
    /// <param name="damageMultiplier"></param>
    /// <returns></returns>
    public float CalculateDamageAmount(float damageMultiplier)
    {
        if (curAtk == null) Debug.Log("CurAtk is null");
        if (DmgBonus == null) Debug.Log("DmgBonus is null");

        if (CheckCritical())
            return curAtk.Value * damageMultiplier * curCriticalDmg.Value * (1 + DmgBonus.Value);
        else
            return curAtk.Value * damageMultiplier * (1+DmgBonus.Value);
    }

    // 크리티컬 확률 계산
    private bool CheckCritical()
    {
        return Random.Range(0f, 100f) <= curCriticalChance.Value;
    }

    public void PerceivedTemperature(int count)
    {
        DmgBonus.RemoveAllFromSource(this);
        DmgBonus.AddModifier(new StatModifier(0.03f * count, StatModifierType.FlatPermanent, this));
    }

    public void EnergyOfVegetation(int count)
    {
        MaxHp.RemoveAllFromSource(this);
        MaxHp.AddModifier(new StatModifier((1 + 0.04f * count), StatModifierType.FlatPermanent, this));
    }
}
