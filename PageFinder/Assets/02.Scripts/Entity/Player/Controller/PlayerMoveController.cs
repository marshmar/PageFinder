using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// �÷��̾��� �⺻���� �̵��� ����ϴ� Ŭ����
/// SRP(Single Responsibility Principle)�� ���� ������ �뽬�� �̵��� ����ϴ� Ŭ������
/// ���� �и���. �ִϸ��̼ǵ� ��������.
/// </summary>
public class PlayerMoveController: MonoBehaviour, IListener
{
    #region Variables
    private Vector3 curMoveDir, beforeMoveDir;
    [SerializeField] private VirtualJoystick moveJoystick;
    [SerializeField] private Canvas playUiOp;

    private PlayerAttackController playerAttackControllerScr;
    private PlayerSkillController playerSkillControllerScr;
    //private PlayerInkMagicController playerInkMagicControllerScr;
    private PlayerDashController playerDashControllerScr;
    //private Player playerScr;
    private PlayerAnim playerAnim;
    private PlayerUtils playerUtils;
    private PlayerState playerState;

    private bool canMove= true;

    private PlayerInputAction input;
    #endregion

    
    public void Awake()
    {
        playerAttackControllerScr = DebugUtils.GetComponentWithErrorLogging<PlayerAttackController>(this.gameObject, "PlayerAttackController");
        playerSkillControllerScr = DebugUtils.GetComponentWithErrorLogging<PlayerSkillController>(this.gameObject, "PlayerSkillController");
        playerAnim = DebugUtils.GetComponentWithErrorLogging<PlayerAnim>(this.gameObject, "PlayerAnim");
        playerUtils = DebugUtils.GetComponentWithErrorLogging<PlayerUtils>(this.gameObject, "PlayerUtils");
        playerState = DebugUtils.GetComponentWithErrorLogging<PlayerState>(this.gameObject, "PlayerState");

        playerDashControllerScr = DebugUtils.GetComponentWithErrorLogging<PlayerDashController>(this.gameObject, "PlayerDashController");
        input = DebugUtils.GetComponentWithErrorLogging<PlayerInputAction>(this.gameObject, "PlayerInputAction");

        SetMoveAction();
    }

    public void Start()
    {
        EventManager.Instance.AddListener(EVENT_TYPE.UI_Changed, this);
    }
    // Update is called once per frame
    void Update()
    {
        if (!CheckIsMoveable())
        {
            Move(curMoveDir);

            // ���̽�ƽ �̵�
            //JoystickControl();

            playerAnim.SetAnimationFloat("Movement", curMoveDir.magnitude);
        }

    }

    private void SetMoveAction()
    {
        if(input is null)
        {
            Debug.LogError("PlayerInput ������Ʈ�� �������� �ʽ��ϴ�.");
            return;
        }

        if(input.MoveAction is null)
        {
            Debug.LogError("Move Action�� �������� �ʽ��ϴ�.");
            return;
        }

        input.MoveAction.performed += context =>
        {
            SetMoveVector(context);
        };
        input.MoveAction.canceled += context =>
        {
            curMoveDir = Vector3.zero;
        };
    }
    public void SetMoveVector(InputAction.CallbackContext context)
    {
        Vector2 contextVec = context.ReadValue<Vector2>();
        beforeMoveDir = curMoveDir;
        curMoveDir = new Vector3(contextVec.x, 0, contextVec.y);
    }

    private bool CheckIsMoveable()
    {
        return playerDashControllerScr.IsDashing && !playerSkillControllerScr.IsUsingSkill && !playerAttackControllerScr.IsAttacking && playUiOp.enabled && canMove;
    }
/*    private void KeyboardControl()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        curMoveDir = new Vector3(h, 0, v).normalized;
        if (h != 0 || v != 0)
        {
           Move(curMoveDir);
        }
    }

    private void JoystickControl()
    {
        // �̵� ���̽�ƽ�� x, y �� �о����
        float x = moveJoystick.Horizontal();
        float y = moveJoystick.Vertical();

        if(x!= 0 || y != 0)
        {
            curMoveDir = new Vector3(x, 0, y).normalized;
            Move(curMoveDir);
        }
    }*/

    private void Move(Vector3 moveDir)
    {
        if (moveDir == Vector3.zero) return;

        playerUtils.Tr.Translate(playerUtils.ModelTr.forward * playerState.CurMoveSpeed * Time.deltaTime);
        playerUtils.TurnToDirection(curMoveDir, Vector3.Dot(curMoveDir, beforeMoveDir) > 0);
    }

    public void OnEvent(EVENT_TYPE eventType, Component sender, object param = null)
    {
        switch (eventType)
        {
            case EVENT_TYPE.UI_Changed:
                var uiChanged = (UIType)param;
                CheckMovable(uiChanged);
                break;
        }
    }

    private void CheckMovable(UIType uiType)
    {
        switch (uiType)
        {
            case UIType.Battle:
            case UIType.PageMap:
            case UIType.RiddlePlay:
                canMove = true;
                break;
            default:
                canMove = false;
                break;
        }
    }
}


