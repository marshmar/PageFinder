using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController: MonoBehaviour
{
    #region Variables
    // Move
    private Vector3 moveDir;
    [SerializeField]
    private VirtualJoystick moveJoystick;


    private PlayerInk playerInkScr;

    private PlayerAttackController playerAttackControllerScr;
    private PlayerSkillController playerSkillControllerScr;
    private Player playerScr;
    private IDash dash;

    private float dashPower;
    private float dashDuration;
    private float dashWidth;
    private float dashCooltime;
    private float dashCost;

    #endregion

    public float DashPower { 
        get => dashPower; 
        set {
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
        set {
            dashWidth = value;
            dash.DashWidth = dashWidth; 
        }
    }
    public float DashCooltime
    {
        get => dashCooltime;
        set {
            dashCooltime = value;
            dash.DashCooltime = dashCooltime; }
    }
    public float DashCost
    {
        get => dashCost;
        set {
            dashCost = value;
            dash.DashCost = value; 
        }
    }
    public bool IsDashing { get => dash.IsDashing; set => dash.IsDashing = value; }

    public void Awake()
    {
        dashCooltime = 0.5f;
        dashPower = 4.0f;
        dashDuration = 0.2f;
        dashWidth = 2.0f;
        dashCost = 30.0f;
        ResetDecorator();
    }

    // 데커레이터를 Base Decorator로 초기화
    public void ResetDecorator()
    {
        dash = new Dash();
        dash.DashCooltime = dashCooltime;
        dash.DashPower = dashPower;
        dash.DashDuration = dashDuration;
        dash.DashWidth = dashWidth;
        dash.IsDashing = false;
        dash.DashCost = dashCost;
    }

    // 대쉬 데커레이터 세팅
    public void SetDecorator(InkType dashInkType)
    {
        switch (dashInkType)
        {
            case InkType.RED:
                dash = new DashDecoratorRed();
                break;
            case InkType.GREEN:
                dash = new DashDecoratorGreen();
                break;
            case InkType.BLUE:
                dash = new DashDecoratorBlue();
                break;
        }

        dash.DashCooltime = dashCooltime;
        dash.DashPower = dashPower;
        dash.DashDuration = dashDuration;
        dash.DashWidth = dashWidth;
        dash.IsDashing = false;
        dash.DashCost = dashCost;
    }
    public void Start()
    {
        playerAttackControllerScr = DebugUtils.GetComponentWithErrorLogging<PlayerAttackController>(this.gameObject, "PlayerAttackController");
        playerInkScr = DebugUtils.GetComponentWithErrorLogging<PlayerInk>(this.gameObject, "PlayerInk");
        playerSkillControllerScr = DebugUtils.GetComponentWithErrorLogging<PlayerSkillController>(this.gameObject, "PlayerSkillController");
        playerScr = DebugUtils.GetComponentWithErrorLogging<Player>(this.gameObject, "Player");
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!dash.IsDashing && !playerSkillControllerScr.IsUsingSkill && !playerAttackControllerScr.IsAttacking)
        {
            // 키보드 이동
            KeyboardControl();
            // 조이스틱 이동
            JoystickControl();

            playerScr.Anim.SetFloat("Movement", moveDir.magnitude);
        }
    }

    void FixedUpdate()
    {
        if (dash.IsDashing)
        {
            dash.GenerateInkMark(playerInkScr, playerScr);
            dash.DashMovement(playerScr);
        }
        else
        {
            dash.EndDash(playerScr);
        }
    }

    private void KeyboardControl()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        moveDir = new Vector3(h, 0, v).normalized;
        if (h != 0 || v != 0)
        {
           Move(moveDir);
        }
    }

    private void JoystickControl()
    {
        // 이동 조이스틱의 x, y 값 읽어오기
        float x = moveJoystick.Horizontal();
        float y = moveJoystick.Vertical();

        if(x!= 0 || y != 0)
        {
            moveDir = new Vector3(x, 0, y).normalized;
            Move(moveDir);
        }
    }

    private void Move(Vector3 moveDir)
    {
        playerScr.Tr.Translate(playerScr.ModelTr.forward * playerScr.MoveSpeed * Time.deltaTime);
        playerScr.TurnToDirection(moveDir);
    }

    public void Dash(Vector3? dir = null)
    {
        if(playerScr.CurrInk >= DashCost)
            StartCoroutine(dash.DashCoroutine(dir, playerAttackControllerScr, this, playerScr));
    }
}


