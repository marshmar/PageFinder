using System;
using UnityEngine;
using System.Collections;

public class PlayerAttackController : MonoBehaviour, IListener
{
    #region Variables
    private int _comboCount = 0;
    private bool _isAttacking = false;
    private bool _isNextAttackBuffered = false;
    private bool _isAbleAttack = true;

    // Hashing
    private BaseScript _script;
    private Player _player;
    private WaitForSeconds _attackDealy;

    [Header("Effects")]
    [SerializeField] private GameObject[] baEffectRed;
    [SerializeField] private GameObject[] baEffectGreen;
    [SerializeField] private GameObject[] baEffectBlue;
    #endregion

    #region Properties

    public bool IsAttacking { get => _isAttacking; set => _isAttacking = value; }
    public int ComboCount { get => _comboCount; set => _comboCount = value; }
    public bool IsNextAttackBuffered { get => _isNextAttackBuffered; set => _isNextAttackBuffered = value; }
    public bool IsAbleAttack { get => _isAbleAttack;}
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        _player = this.GetComponentSafe<Player>();
        _attackDealy = new WaitForSeconds(0.5f);

        AddListener();
    }

    private void Start()
    {
        InitializeAttackAction();
    }

    private void OnDestroy()
    {
        RemoveListener();
    }
    #endregion

    #region Initialization

    private void InitializeAttackAction()
    {
        var attackAction = _player.InputAction.GetInputAction(PlayerInputActionType.Attack);
        if (attackAction == null)
        {
            Debug.LogError("Attack Action is null");
            return;
        }

        attackAction.canceled += context =>
        {
            BasicAttackCommand basicAttackCommand = new BasicAttackCommand(this, Time.time);
            _player.InputInvoker.AddInputCommand(basicAttackCommand);
        };
    }
    #endregion

    #region Actions
    public void CreateContext(BaseScript script)
    {
        _script = script;

        const int effectArrayLengthTreshold = 3;
        if (baEffectRed == null || baEffectRed.Length < effectArrayLengthTreshold) return;
        if (baEffectGreen == null || baEffectGreen.Length < effectArrayLengthTreshold) return;
        if (baEffectBlue == null || baEffectBlue.Length < effectArrayLengthTreshold) return;

        BasicAttackContext baContext = new BasicAttackContext()
        {
            Player = _player,
            BaEffectRed = this.baEffectRed,
            BaEffectGreen = this.baEffectGreen,
            BaEffectBlue = this.baEffectBlue,
        };

        _script.SetContext(baContext);
    }

    public void ExcuteBehaviour()
    {
        if (_script.IsNull()) return;

        _script.ExcuteBehaviour();
    }

    public void ExcuteAnim()
    {
        if (_script.IsNull()) return;

        if (_script is IAnimatedBasedScript animatedBasedScript)
        {
            animatedBasedScript.ExcuteAnim();
        }
    }

    public IEnumerator DelayedSetAttack()
    {
        yield return _attackDealy;
        _isAbleAttack = true;
    }
    #endregion

    #region Getter
    #endregion

    #region Setter
    #endregion

    #region Utilities
    public bool IsAnimatedBasedAttack()
    {
        if (_script.IsNull()) return false;

        return _script is IAnimatedBasedScript;
    }

    public bool CanExcuteBehaviour()
    {
        if (_script.IsNull()) return false;

        return _script.CanExcuteBehaviour();
    }
    #endregion

    #region Events
    public void AddListener()
    {
        EventManager.Instance.AddListener(EVENT_TYPE.Open_Panel_Exclusive, this);
        EventManager.Instance.AddListener(EVENT_TYPE.Open_Panel_Stacked, this);
        EventManager.Instance.AddListener(EVENT_TYPE.Stage_Clear, this);
        EventManager.Instance.AddListener(EVENT_TYPE.Stage_Start, this);
        EventManager.Instance.AddListener(EVENT_TYPE.InkDashWating, this);
        EventManager.Instance.AddListener(EVENT_TYPE.InkDashTutorialCleared, this);
        EventManager.Instance.AddListener(EVENT_TYPE.InkSkillWaiting, this);
        EventManager.Instance.AddListener(EVENT_TYPE.InkSkillTutorialCleared, this);
    }

    public void RemoveListener()
    {
        EventManager.Instance.RemoveListener(EVENT_TYPE.Open_Panel_Exclusive, this);
        EventManager.Instance.RemoveListener(EVENT_TYPE.Open_Panel_Stacked, this);
        EventManager.Instance.RemoveListener(EVENT_TYPE.Stage_Clear, this);
        EventManager.Instance.RemoveListener(EVENT_TYPE.Stage_Start, this);
        EventManager.Instance.RemoveListener(EVENT_TYPE.InkDashWating, this);
        EventManager.Instance.RemoveListener(EVENT_TYPE.InkDashTutorialCleared, this);
        EventManager.Instance.RemoveListener(EVENT_TYPE.InkSkillWaiting, this);
        EventManager.Instance.RemoveListener(EVENT_TYPE.InkSkillTutorialCleared, this);
    }

    public void OnEvent(EVENT_TYPE eventType, Component Sender, object Param = null)
    {
        switch (eventType)
        {
            case EVENT_TYPE.Open_Panel_Exclusive:
            case EVENT_TYPE.Open_Panel_Stacked:
                PanelType nextPanel = (PanelType)Param;
                if (nextPanel == PanelType.HUD)
                    StartCoroutine(DelayedSetAttack());
                else
                    _isAbleAttack = false;
                break;

            case EVENT_TYPE.Stage_Clear:
                _isAbleAttack = false;
                break;
            case EVENT_TYPE.Stage_Start:
                NodeType nodeType = ((Node)Param).type;
                switch (nodeType)
                {
                    case NodeType.Treasure:
                    case NodeType.Comma:
                    case NodeType.Market:
                    case NodeType.Quest:
                        _isAbleAttack = false;
                        break;
                    default:
                        StartCoroutine(DelayedSetAttack());
                        break;
                }
                break;

            case EVENT_TYPE.InkDashWating:
            case EVENT_TYPE.InkSkillWaiting:
                _isAbleAttack = false;
                break;
            case EVENT_TYPE.InkDashTutorialCleared:
            case EVENT_TYPE.InkSkillTutorialCleared:
                _isAbleAttack = true;
                break;
        }
    }
    #endregion








   

    
}
