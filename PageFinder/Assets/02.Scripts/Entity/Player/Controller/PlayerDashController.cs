using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// �÷��̾��� �뽬�� ���� ���� ó���ϴ� Ŭ����
/// </summary>
public class PlayerDashController : MonoBehaviour, IListener
{
    #region Variables
    private DashDecorator dash;
    private Coroutine extraEffectCoroutine;
    private float dashPower;
    private float dashDuration;
    private float dashWidth;
    private float dashCoolTime;
    private float dashCost;
    private bool isDashing;

    private PlayerState playerState;
    private PlayerUtils playerUtils;
    private PlayerAnim playerAnim;
    private PlayerInkType playerInkType;
    private PlayerAttackController playerAttackControllerScr;
    private PlayerSkillController playerSkillController;
    private PlayerTargetingVisualizer playerTarget;
    private PlayerInputInvoker playerInputInvoker;

    private PlayerInputAction input;
    private Vector3 dashDir;
    private bool chargingDash = false;
    private bool dashCanceld = false;
    #endregion

    #region Properties
    public float DashPower
    {
        get => dashPower;
        set
        {
            dashPower = value;
            dash.DashPower = dashPower;
        }
    }
    public float DashDuration
    {
        get => dashDuration;
        set
        {
            dashDuration = value;
            dash.DashDuration = dashDuration;

        }
    }
    public float DashWidth
    {
        get => dashWidth;
        set
        {
            dashWidth = value;
            dash.DashWidth = dashWidth;
        }
    }
    public float DashCooltime
    {
        get => dashCoolTime;
        set
        {
            dashCoolTime = value;
            dash.DashCooltime = dashCoolTime;
        }
    }
    public float DashCost
    {
        get => dashCost;
        set
        {
            dashCost = value;
            dash.DashCost = value;
        }
    }
    public bool IsDashing { get => isDashing; set => isDashing = value; }
    public bool ChargingDash { get => chargingDash; set => chargingDash = value; }

    #endregion


    private void Awake()
    {
        dashCoolTime = 0.3f;
        dashPower = 4.0f;
        dashDuration = 0.2f;
        dashWidth = 2.0f;
        dashCost = 30.0f;
        isDashing = false;

        playerAttackControllerScr = DebugUtils.GetComponentWithErrorLogging<PlayerAttackController>(this.gameObject, "PlayerAttackController");
        playerSkillController = DebugUtils.GetComponentWithErrorLogging<PlayerSkillController>(this.gameObject, "PlayerSkillController");
        playerState = DebugUtils.GetComponentWithErrorLogging<PlayerState>(this.gameObject, "PlayerState");
        playerUtils = DebugUtils.GetComponentWithErrorLogging<PlayerUtils>(this.gameObject, "PlayerUtils");
        playerAnim = DebugUtils.GetComponentWithErrorLogging<PlayerAnim>(this.gameObject, "PlayerAnim");
        playerInkType = DebugUtils.GetComponentWithErrorLogging<PlayerInkType>(this.gameObject, "PlayerInkType");
        playerTarget = DebugUtils.GetComponentWithErrorLogging<PlayerTargetingVisualizer>(this.gameObject, "PlayerTarget");
        input = DebugUtils.GetComponentWithErrorLogging<PlayerInputAction>(this.gameObject, "PlayerInputAction");
        playerInputInvoker = DebugUtils.GetComponentWithErrorLogging<PlayerInputInvoker>(this.gameObject, "PlayerInputInvoker");

        dash = new Dash(this);         // �⺻ �뽬�� ���ڷ����� ����
    }

    private void Start()
    {
        // PlayerInputAction���� Awake���� action�� �������ֱ⿡ Start���� �����ؾ� ��.
        SetDashAction();

        AddListener();
    }

    private void Update()
    {
        if (chargingDash && !playerSkillController.IsChargingSkill)
        {
            SetDashDirection();
            playerTarget.FixedLineTargeting(dashDir, dashPower, dashWidth);
        }
    }

    private void OnDestroy()
    {
        RemoveListener();
    }

    private void SetDashAction()
    {
        if (input is null)
        {
            Debug.LogError("PlayerInput ������Ʈ�� �������� �ʽ��ϴ�.");
            return;
        }

        if (input.DashAction is null)
        {
            Debug.LogError("DashAction�� �������� �ʽ��ϴ�.");
            return;
        }

        input.DashAction.started += context =>
        {
            dashDir = Vector3.zero;
        };

        input.DashAction.performed += context =>
        {
            chargingDash = true;
        };

        input.DashAction.canceled += context =>
        {
            //DashCommand dashCommand = new DashCommand(this, Time.time);
            //playerInputInvoker.AddInputCommand(dashCommand);
        };

        if (input.CancelAction is null)
        {
            Debug.LogError("CancelAction�� �������� �ʽ��ϴ�.");
            return;
        }

        input.CancelAction.started += context =>
        {
            chargingDash = false;
            dashCanceld = true;
            playerTarget.OffAllTargetObjects();
        };
    }


    private void SetDashDirection()
    {
        Vector2 screenDirection = Input.mousePosition - Camera.main.WorldToScreenPoint(playerUtils.Tr.position);
        dashDir = new Vector3(screenDirection.x, 0f, screenDirection.y).normalized;
    }

    public void SetDecoratorByInkType(InkType dashInkType, float scriptValue)
    {
        switch (dashInkType)
        {
            case InkType.RED:
                dash = new DashDecoratorRed(this, scriptValue);
                break;
            case InkType.GREEN:
                dash = new DashDecoratorGreen(this, scriptValue);
                break;
            case InkType.BLUE:
                dash = new DashDecoratorBlue(this);
                break;
        }
    }

    private void FixedUpdate()
    {
        if (isDashing && !playerSkillController.IsChargingSkill)
        {
            dash.GenerateInkMark(playerInkType, playerUtils);
            dash.DashMovement(playerUtils);
        }
    }

    public bool CheckDashExcutable()
    {
        return playerState.CurInk >= DashCost /*&& !playerAttackControllerScr.IsAttacking*/
    && !playerSkillController.IsUsingSkill && !isDashing && !playerSkillController.IsChargingSkill;
    }

    public void ExcuteDash()
    {
        if (!dashCanceld)
        {
            if (!chargingDash) Dash(); // �뽬�� ��¡���� �ʾ��� ��� ª�� �뽬
            else Dash(dashDir); // �뽬�� ��¡���� ��� ���� ������ ������ �뽬 ����
        }

        playerTarget.OffAllTargetObjects();
        chargingDash = false;
        dashCanceld = false;
    }

    public void Dash(Vector3? dir = null)
    {
        if(CheckDashExcutable())
        {
            StartCoroutine(dash.DashCoroutine(dir, playerUtils, playerAnim, playerState));
            if(extraEffectCoroutine is not null) StopCoroutine(extraEffectCoroutine);

            extraEffectCoroutine = StartCoroutine(dash.ExtraEffectCoroutine(playerState));

            // �뽬 ȿ���� ���
            AudioManager.Instance.Play(Sound.dashVfx1, AudioClipType.DashSfx);
        }
    }

    public void AddListener()
    {
        EventManager.Instance.AddListener(EVENT_TYPE.Joystick_Short_Released, this);
        EventManager.Instance.AddListener(EVENT_TYPE.Joystick_Long_Released, this);
    }

    public void RemoveListener()
    {
        EventManager.Instance.RemoveListener(EVENT_TYPE.Joystick_Short_Released, this);
        EventManager.Instance.RemoveListener(EVENT_TYPE.Joystick_Long_Released, this);
    }

    public void OnEvent(EVENT_TYPE eventType, Component sender, object param)
    {
        switch (eventType)
        {
            case EVENT_TYPE.Joystick_Short_Released:
                if(sender.name.Equals(PlayerUI.playerDashJoystickName)) Dash();
                break;
            case EVENT_TYPE.Joystick_Long_Released:
                if (sender.name.Equals(PlayerUI.playerDashJoystickName))
                {
                    Vector3 dir = (Vector3)param;
                    if (dir == Vector3.zero) Dash();
                    else Dash(dir);
                }
                break;
        }
    }
}