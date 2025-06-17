using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using UnityEngine;

public class PlayerState : MonoBehaviour, IListener, IObserver, IEntityState
{
    #region Variables

    #region defaultValue
    private const float DefaultMaxHpValue                = 500f;
    private const float DefaultMaxInkValue               = 100f;
    private const float DefaultInkGainValue              = 0.2f;
    private const float DefaultAttackSpeedValue          = 0f;
    private const float DefaultAttackRangeValue          = 3f;
    private const float DefaultAtkValue                  = 50f;
    private const float DefaultMoveSpeedValue            = 7f;
    private const float DefaultCriticalChanceValue       = 0.15f;
    private const float DefaultCriticalDmgValue          = 1.5f;
    private const float DefaultMaxShieldPercentageValue  = 0.3f;
    private const float DefaultDmgBounsValue             = 0f;
    private const float DefaultDmgResistValue            = 0f;
    private const float InkRecoverDelayTime         = 0.5f;
    #endregion

    #region MaxValue
    private const float MaxHPLimit      = 1500f;
    private const float MaxInkLimit     = 300f;
    private const float MaxInkGain      = 0.5f;
    private const float MaxAttackSpeed  = 1f;
    private const float MaxMoveSpeed    = 30f;
    private const float MaxCriticalDmg  = 3f;
    #endregion

    #region MinValue
    private const float MinHPLimit      = 1f;
    private const float MinInkLimit     = 100f;
    private const float MinInkGain      = 0.1f;
    private const float MinAttackSpeed  = 0f;
    private const float MinMoveSpeed    = 1f;
    private const float MinCriticalDmg  = 1f;
    #endregion

    #region currValue
    private ClampedStat _maxHp;
    private float _curHp;
    private ClampedStat _maxInk;
    private float _curInk;
    private ClampedStat _curInkGain;
    private ClampedStat _curAttackSpeed;
    private Stat _curAttackRange;
    private Stat _curAtk;
    private ClampedStat _curMoveSpeed;
    private Stat _curCriticalChance;
    private ClampedStat _curCriticalDmg;
    private Stat _maxShieldPercentage;
    private float _curShield;
    private int _coin;
    private Stat _dmgBonus;
    private Stat _dmgResist;
    #endregion



    #region Hashing
    private Player _player;
    [SerializeField] private GameObject shieldEffect;
    [SerializeField] private ResultUIManager resultUIManager;
    private ShieldManager _shieldManager;
    private WaitForSeconds _inkRecoveryDelay;
    private IEnumerator _inkRecoveryCoroutine;
    #endregion

    #endregion

    #region Properties
    #region Default Value Properties
    public float DefaultMaxHp 
    { 
        get => DefaultMaxHpValue; 
    }

    public float DefaultMaxInk 
    { 
        get => DefaultMaxInkValue; 
    }

    public float DefaultAttackSpeed 
    { 
        get => DefaultAttackSpeedValue; 
    }

    public float DefaultAttackRange 
    { 
        get => DefaultAttackRangeValue; 
    }

    public float DefaultAtk 
    { 
        get => DefaultAtkValue; 
    }

    public float DefaultMoveSpeed 
    { 
        get => DefaultMoveSpeedValue; 
    }

    public float DefaultCritical 
    { 
        get => DefaultCriticalChanceValue; 
    }

    public float DefaultInkGain 
    { 
        get => DefaultInkGainValue; 
    }

    public float DefaultCriticalDmg 
    { 
        get => DefaultCriticalDmgValue; 
    }

    #endregion

    #region Cur value Properties
    public Stat MaxHp
    {
        get => _maxHp;
    }

    public float CurHp
    {
        get => _curHp;
        set
        {
            float inputDamage = _curHp - value;

            if (inputDamage < 0)
            {
                _curHp = _curHp + -inputDamage;
                if (_curHp > _maxHp.Value) _curHp = _maxHp.Value;
            }
            else
            {
                float ReducedDamage = inputDamage * (1 - DmgResist.Value * 0.01f);
                float finalDamage = _shieldManager.CalculateDamageWithDecreasingShield(ReducedDamage);
                if (finalDamage <= 0)
                {
                    _player.UI.SetStateBarUIForCurValue(_maxHp.Value, _curHp, CurShield);
                    return;
                }

                _player.UI.StartDamageFlash(_curHp, finalDamage, _maxHp.Value);
                _curHp -= finalDamage;
                _player.UI.ShowDamageIndicator(); // ToDo: 이 부분도 Event기반 프로그래밍으로 만들 수 있지 않을까?
                                                
            }

            _player.UI.SetStateBarUIForCurValue(_maxHp.Value, _curHp, CurShield);

            if (_curHp <= 0)
            {
                _curHp = 0f;

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
        get => _maxInk; 
    }

    public float CurInk
    {
        get => _curInk;
        set
        {
            _curInk = Mathf.Clamp(value, 0, _maxInk.Value);
            _player.UI.SetCurrInkBarUI(_curInk);

            if (_curInk < _maxInk.Value)
            {
                RecoverInk();
            }
        }
    }

    public Stat CurInkGain
    {
        get => _curInkGain;
    }

    public Stat CurAttackSpeed
    {
        get => _curAttackSpeed;
    }

    public Stat CurAttackRange 
    { 
        get => _curAttackRange; 
    }

    public Stat CurAtk 
    { 
        get => _curAtk; 
    }

    public Stat CurMoveSpeed 
    {
        get => _curMoveSpeed; 
    }

    public Stat CurCriticalChance 
    { 
        get => _curCriticalChance; 
    }

    public float CurShield
    {
        get => _shieldManager.CurShield;
        set
        {
            _curShield = Mathf.Max(0, value);
            _player.UI.SetStateBarUIForCurValue(MaxHp.Value, _curHp, value);
        }
    }

    public Stat MaxShield 
    { 
        get => _shieldManager.MaxShield; 
    }

    public int Coin 
    { 
        get => _coin; 
        set => _coin = value; 
    }

    public Stat CurCriticalDmg 
    { 
        get => _curCriticalDmg; 
    }

    public Stat DmgBonus 
    { 
        get => _dmgBonus; 
    }

    public Stat DmgResist 
    { 
        get => _dmgResist; 
    }
    #endregion

    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        // Hashing
        _player = this.GetComponentSafe<Player>();
        _shieldManager = this.GetComponentSafe<ShieldManager>();
        _shieldManager.Attach(this);
        _inkRecoveryDelay = new WaitForSeconds(InkRecoverDelayTime);
    }

    private void Start()
    {
        Initialize();

        // Event
        AddListener();

    }

    private void OnDestroy()
    {
        _inkRecoveryCoroutine = null;

        RemoveListener();
    }
    #endregion

    #region Initialization
    private void Initialize()
    {
        _maxHp = new ClampedStat(DefaultMaxHpValue, MinHPLimit, MaxHPLimit);
        _maxInk = new ClampedStat(DefaultMaxInkValue, MinInkLimit, MaxInkLimit);
        _curInkGain = new ClampedStat(DefaultInkGainValue, MinInkGain, MaxInkGain);
        _curAttackSpeed = new ClampedStat(DefaultAttackSpeedValue, MinAttackSpeed, MaxAttackSpeed);
        _curAttackRange = new Stat(DefaultAttackRangeValue);
        _curAtk = new Stat(DefaultAtkValue);
        _curMoveSpeed = new ClampedStat(DefaultMoveSpeedValue, MinMoveSpeed, MaxMoveSpeed);
        _curCriticalChance = new Stat(DefaultCriticalChanceValue);
        _curCriticalDmg = new ClampedStat(DefaultCriticalDmgValue, MinCriticalDmg, MaxCriticalDmg);
        _maxShieldPercentage = new Stat(DefaultMaxShieldPercentageValue);
        _dmgBonus = new Stat(DefaultDmgBounsValue);
        _dmgResist = new Stat(DefaultDmgResistValue);

        // connet UI;
        _player.UI.SetMaxHPBarUI();
        _player.UI.SetMaxInkUI();

        // properties
        CurHp = _maxHp.Value;
        CurInk = MaxInk.Value;

        // shield
        _shieldManager.Init(_maxHp.Value * _maxShieldPercentage.Value);

        //maxHp.OnModified += SyncCurHpWithMax;
        //curAttackSpeed.OnModified += SetAttackSpeed;
    }
    #endregion

    #region Actions
    private void RecoverInk()
    {
        // is not null
        if (_inkRecoveryCoroutine is not null)
        {
            StopCoroutine(_inkRecoveryCoroutine);
        }
        _inkRecoveryCoroutine = RecoverInkCoroutine();
        StartCoroutine(_inkRecoveryCoroutine);
    }

    private IEnumerator RecoverInkCoroutine()
    {
        yield return _inkRecoveryDelay;

        while (_curInk < MaxInk.Value)
        {
            _curInk += MaxInk.Value * CurInkGain.Value * Time.deltaTime;
            _curInk = Mathf.Clamp(_curInk, 0, _maxInk.Value);
            _player.UI.SetCurrInkBarUI(_curInk);

            // 잉크 게이지 값이 회복될 때마다 이벤트 쏴주기
            EventManager.Instance.PostNotification(EVENT_TYPE.InkGage_Changed, this, _curInk);
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
            return _curAtk.Value * damageMultiplier * _curCriticalDmg.Value * (1 + DmgBonus.Value);
        else
            return _curAtk.Value * damageMultiplier * (1 + DmgBonus.Value);
    }

    private bool CheckCritical()
    {
        return Random.Range(0f, 100f) <= _curCriticalChance.Value;
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

                _shieldManager.GenerateShield(shieldAmount, shieldDuration);
                _player.UI.SetStateBarUIForCurValue(_maxHp.Value, _curHp, CurShield);
                shieldEffect.SetActive(true);
                break;
        }
    }

    public void Notify(Subject subject)
    {
        _player.UI.SetStateBarUIForCurValue(_maxHp.Value, _curHp, CurShield);

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
