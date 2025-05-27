using System;
using UnityEngine;
using System.Collections.Generic;

public class NewPlayerDashController : MonoBehaviour
{
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
        SetDashAction();
    }

    private void SetDashAction()
    {
        if (player.InputAction == null)
        {
            Debug.LogError("PlayerInput 컴포넌트가 존재하지 않습니다.");
            return;
        }

        if (player.InputAction.DashAction == null)
        {
            Debug.LogError("DashAction이 존재하지 않습니다.");
            return;
        }

        player.InputAction.DashAction.started += context =>
        {

        };

        player.InputAction.DashAction.performed += context =>
        {
            chargingDash = true;
        };

        player.InputAction.DashAction.canceled += context =>
        {
            NewDashCommand dashCommand = new NewDashCommand(this, Time.time);
            player.InputInvoker.AddInputCommand(dashCommand);
        };

        if (player.InputAction.CancelAction is null)
        {
            Debug.LogError("CancelAction이 존재하지 않습니다.");
            return;
        }

        player.InputAction.CancelAction.started += context =>
        {
            chargingDash = false;
            //dashCanceld = true;
            player.Target.OffAllTargetObjects();
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
