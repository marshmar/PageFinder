using System;
using UnityEngine;
using System.Collections.Generic;

public class NewPlayerDashController : MonoBehaviour
{
    private bool canDash = true;
    private Player player;
    private bool chargingDash = false;
    private BaseScript script;
    private bool isDashing = false;
    public Action FixedUpdateDashAction { get; set; }
    public bool IsDashing { get => isDashing; set => isDashing = value; }

    public float DashCost
    {
        get
        {
            if(script != null && script is DashScript dashScript)
            {
                return dashScript.DashCost.Value;
            }

            return -1f;
        }
    }

    public float DashCoolTime
    {
        get
        {
            if (script != null && script is DashScript dashScript)
            {
                return dashScript.DashCoolTime;
            }

            return -1f;
        }
    }
    private void Awake()
    {
        player = DebugUtils.GetComponentWithErrorLogging<Player>(this.gameObject, "Player");

    }

    private void Start()
    {
        InitializeDashAction();
        InitializeCancelAction();
    }



    private void InitializeDashAction()
    {
        var dashAction = player.InputAction.GetInputAction(PlayerInputActionType.Dash);
        if (dashAction == null)
        {
            Debug.LogError("Dash Action is null");
            return;
        }

        dashAction.started += context =>
        {

        };

        dashAction.performed += context =>
        {
            chargingDash = true;
        };

        dashAction.canceled += context =>
        {
            DashCommand dashCommand = new DashCommand(this, Time.time);
            player.InputInvoker.AddInputCommand(dashCommand);
        };
    }

    private void InitializeCancelAction()
    {
        var cancelAction = player.InputAction.GetInputAction(PlayerInputActionType.Cancel);
        if (cancelAction == null)
        {
            Debug.LogError("Cancel Action is null");
            return;
        }

        cancelAction.started += context =>
        {
            chargingDash = false;
            //dashCanceld = true;
            player.TargetingVisualizer.OffAllTargetObjects();
        };
    }

    public bool CanExcuteBehaviour()
    {
        if (script == null)
        {
            Debug.LogError("Dash script is not Assigned");
            return false;
        }

        return script.CanExcuteBehaviour();
    }

    public void ExcuteBehaviour()
    {
        if (script == null)
        {
            Debug.LogError("Dash script is not Assigned");
            return;
        }
        script.ExcuteBehaviour();
        chargingDash = false;
    }

    private void FixedUpdate()
    {
        FixedUpdateDashAction?.Invoke();
    }
    private void Update()
    {
        if (chargingDash)
        {
            if(script != null && script is IChargableScript chargableDashScript)
            {
                chargableDashScript.ChargeBehaviour();
            }
        }
    }

    public void SetScript(BaseScript script)
    {
        this.script = script;

        DashContext baContext = new DashContext()
        {
            player = this.player,
        };

        script.SetContext(baContext);
    }
}
