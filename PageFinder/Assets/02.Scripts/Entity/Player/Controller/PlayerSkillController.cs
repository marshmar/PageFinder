using System;
using UnityEngine;

public class PlayerSkillController : MonoBehaviour, IListener
{
    #region Variables
    private bool _canUseSkill               = true;
    private bool _isChargingSkill           = false;
    private bool _skillCanceled             = false;
    private bool _isUsingSkill              = false;
    private const float SkillAnimThreshild  = 0.8f;

    // Hashing
    private Player _player;
    private BaseScript _script;
    #endregion

    #region Properties

    public bool IsChargingSkill { get => _isChargingSkill; set => _isChargingSkill = value; }
    public bool IsUsingSkill { get => _isUsingSkill; set => _isUsingSkill = value; }

    public float SkillCost
    {
        get
        {
            if (_script.IsNull()) return float.NaN;
            if (!(_script is SkillScript skillScript)) return float.NaN;

            return skillScript.SkillCost;
        }
    }

    public float SkillCoolTime
    {
        get
        {
            if (_script.IsNull()) return float.NaN;
            if (!(_script is SkillScript skillScript)) return float.NaN;

             return skillScript.SkillCoolTime;
        }
    }

    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        _player = DebugUtils.GetComponentWithErrorLogging<Player>(this.gameObject, "Player");
    }

    private void Start()
    {
        AddListener();

        InitializeSkillAction();
        InitializeCancelAction();
    }

    private void Update()
    {
        if (_isChargingSkill && _script is IChargableScript chargableSkillScript)
        {
            chargableSkillScript.ChargeBehaviour();
        }

        if (_isUsingSkill)
        {
            _isUsingSkill = _player.Anim.HasAnimPassedTime("Player_Skill_Turning", SkillAnimThreshild);
            if (!_isUsingSkill)
            {
                _player.MoveController.CanMove = true;
                _player.MoveController.CanTurn = true;
            }
        }
    }
    private void OnDestroy()
    {
        RemoveListener();
    }
    #endregion

    #region Initialization
    private void InitializeSkillAction()
    {
        var skillAction = _player.InputAction.GetInputAction(PlayerInputActionType.Skill);
        if (skillAction == null)
        {
            Debug.LogError("Skill Action is null");
            return;
        }


        skillAction.performed += context =>
        {
            if (_script.CanExcuteBehaviour())
                _isChargingSkill = true;
        };

        skillAction.canceled += context =>
        {
            SkillCommand skillCommand = new SkillCommand(this, Time.time);
            _player.InputInvoker.AddInputCommand(skillCommand);
        };
    }

    private void InitializeCancelAction()
    {
        var cancelAction = _player.InputAction.GetInputAction(PlayerInputActionType.Cancel);
        if (cancelAction == null)
        {
            Debug.LogError("Cancel Action is null.");
            return;
        }

        cancelAction.started += context =>
        {
            _player.TargetingVisualizer.OffAllTargetObjects();
            _isChargingSkill = false;
            _skillCanceled = true;
        };
    }
    #endregion

    #region Actions
    public bool CanExcuteBehaviour()
    {
        if (_script.IsNull()) return false;

        return _script.CanExcuteBehaviour() && _canUseSkill;
    }

    public void ExcuteBehaviour()
    {
        if(_script.IsNull()) return;

        _script.ExcuteBehaviour();
    }

    public void CreateContext(BaseScript script)
    {
        _script = script;

        SkillContext skillContext = new SkillContext()
        {
            Player = _player,
        };

        script.SetContext(skillContext);
    }

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
        EventManager.Instance.AddListener(EVENT_TYPE.Open_Panel_Exclusive, this);
        EventManager.Instance.AddListener(EVENT_TYPE.Open_Panel_Stacked, this);
        EventManager.Instance.AddListener(EVENT_TYPE.InkDashWating, this);
        EventManager.Instance.AddListener(EVENT_TYPE.InkDashTutorialCleared, this);
    }

    public void RemoveListener()
    {
        EventManager.Instance.RemoveListener(EVENT_TYPE.Open_Panel_Exclusive, this);
        EventManager.Instance.RemoveListener(EVENT_TYPE.Open_Panel_Stacked, this);
        EventManager.Instance.RemoveListener(EVENT_TYPE.InkDashWating, this);
        EventManager.Instance.RemoveListener(EVENT_TYPE.InkDashTutorialCleared, this);
    }

    public void OnEvent(EVENT_TYPE eventType, Component Sender, object Param = null)
    {
        switch (eventType)
        {
            case EVENT_TYPE.Open_Panel_Exclusive:
            case EVENT_TYPE.Open_Panel_Stacked:
                _isUsingSkill = false;
                break;
            case EVENT_TYPE.InkDashWating:
                _canUseSkill = false;
                break;
            case EVENT_TYPE.InkDashTutorialCleared:
                _canUseSkill = true;
                break;
        }
    }
    #endregion













}
