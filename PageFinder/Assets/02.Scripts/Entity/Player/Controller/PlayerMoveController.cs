using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 플레이어의 기본적인 이동을 담당하는 클래스
/// SRP(Single Responsibility Principle)에 따라 기존의 대쉬와 이동을 담당하는 클래스를
/// 서로 분리함. 애니메이션도 마찬가지.
/// </summary>
public class PlayerMoveController: MonoBehaviour, IListener
{
    #region Variables
    private Vector3 moveDir;
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
    #endregion

    
    public void Awake()
    {
        playerAttackControllerScr = DebugUtils.GetComponentWithErrorLogging<PlayerAttackController>(this.gameObject, "PlayerAttackController");
        playerSkillControllerScr = DebugUtils.GetComponentWithErrorLogging<PlayerSkillController>(this.gameObject, "PlayerSkillController");
        playerAnim = DebugUtils.GetComponentWithErrorLogging<PlayerAnim>(this.gameObject, "PlayerAnim");
        playerUtils = DebugUtils.GetComponentWithErrorLogging<PlayerUtils>(this.gameObject, "PlayerUtils");
        playerState = DebugUtils.GetComponentWithErrorLogging<PlayerState>(this.gameObject, "PlayerState");

        playerDashControllerScr = DebugUtils.GetComponentWithErrorLogging<PlayerDashController>(this.gameObject, "PlayerDashController");


    }

    public void Start()
    {
        EventManager.Instance.AddListener(EVENT_TYPE.UI_Changed, this);
    }
    // Update is called once per frame
    void Update()
    {
        if (!playerDashControllerScr.IsDashing && !playerSkillControllerScr.IsUsingSkill && !playerAttackControllerScr.IsAttacking /*&& !playerInkMagicControllerScr.IsUsingInkMagic*/ 
            && playUiOp.enabled && canMove)
        {
            // 키보드 이동
            KeyboardControl();
            // 조이스틱 이동
            JoystickControl();

            playerAnim.SetAnimationFloat("Movement", moveDir.magnitude);
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
        playerUtils.Tr.Translate(playerUtils.ModelTr.forward * playerState.CurMoveSpeed * Time.deltaTime);
        playerUtils.TurnToDirection(moveDir);
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


