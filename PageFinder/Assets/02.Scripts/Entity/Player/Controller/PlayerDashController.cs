using System;
using UnityEngine;
using System.Collections.Generic;

public class PlayerDashController : MonoBehaviour
{
    #region Variables
    private bool _canDash       = true;
    private bool _chargingDash  = false;
    private bool _isDashing = false;

    // Hashing
    private Player _player;
    private BaseScript _script;

    public Action FixedUpdateDashAction { get; set; }
    #endregion

    #region Properties
    public bool IsDashing { get => _isDashing; set => _isDashing = value; }

    public float DashCost
    {
        get
        {
            if (_script.IsNull()) return float.NaN;
            if (!(_script is DashScript dashScript)) return float.NaN;

                
            return dashScript.DashCost.Value;
        }
    }

    public float DashCoolTime
    {
        get
        {
            if (_script.IsNull()) return float.NaN;
            if (!(_script is DashScript dashScript)) return float.NaN;

             return dashScript.DashCoolTime;
        }
    }
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        _player = this.GetComponentSafe<Player>();
    }

    private void Start()
    {
        InitializeDashAction();
        InitializeCancelAction();
    }

    private void FixedUpdate()
    {
        FixedUpdateDashAction?.Invoke();
    }

    private void Update()
    {
        if (_script.IsNull()) return;

        if (_chargingDash && _script is IChargableScript chargableDashScript)
        {
            chargableDashScript.ChargeBehaviour();
        }
    }
    #endregion

    #region Initialization
    private void InitializeDashAction()
    {
        var dashAction = _player.InputAction.GetInputAction(PlayerInputActionType.Dash);
        if (dashAction == null)
        {
            Debug.LogError("Dash Action is null");
            return;
        }

        dashAction.performed += context =>
        {
            _chargingDash = true;
        };

        dashAction.canceled += context =>
        {
            DashCommand dashCommand = new DashCommand(this, Time.time);
            _player.InputInvoker.AddInputCommand(dashCommand);
        };
    }

    private void InitializeCancelAction()
    {
        var cancelAction = _player.InputAction.GetInputAction(PlayerInputActionType.Cancel);
        if (cancelAction == null)
        {
            Debug.LogError("Cancel Action is null");
            return;
        }

        cancelAction.started += context =>
        {
            _chargingDash = false;
            //dashCanceld = true;
            _player.TargetingVisualizer.OffAllTargetObjects();
        };
    }
    #endregion

    #region Actions
    public bool CanExcuteBehaviour()
    {
        if (_script.IsNull()) return false;

        return _script.CanExcuteBehaviour();
    }

    public void ExcuteBehaviour()
    {
        if(_script.IsNull()) return;

        _script.ExcuteBehaviour();
        _chargingDash = false;
    }

    public void CreateContext(BaseScript script)
    {
        _script = script;

        DashContext baContext = new DashContext()
        {
            Player = _player,
        };

        script.SetContext(baContext);
    }
    #endregion

    #region Getter
    #endregion

    #region Setter
    #endregion

    #region Utilities
    #endregion

    #region Events
    #endregion
}
