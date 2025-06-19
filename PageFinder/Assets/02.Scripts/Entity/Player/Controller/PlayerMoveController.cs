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
    private Vector3 curMoveDir, beforeMoveDir;
    [SerializeField] private VirtualJoystick moveJoystick;
    [SerializeField] private Canvas playUiOp;

    private PlayerAttackController playerAttackControllerScr;
    private PlayerSkillController playerSkillControllerScr;
    //private PlayerInkMagicController playerInkMagicControllerScr;
    private PlayerDashController playerDashControllerScr;
    private PlayerDashController newPlayerDashController;
    private PlayerSkillController newPlayerSkillController;
    //private Player playerScr;
    private PlayerAnim playerAnim;
    private PlayerUtils playerUtils;
    private PlayerState playerState;
    private Player player;

    private bool canMove= true;
    private bool isMoving = false;
    private bool moveTurn = true;
    private PlayerInputAction input;

    public bool IsMoving { get => isMoving; set => isMoving = value; }
    public bool MoveTurn { get => moveTurn; set => moveTurn = value; }
    public bool CanMove { get => canMove; set => canMove = value; }
    #endregion


    public void Awake()
    {
        player = this.GetComponentSafe<Player>();
        playerAttackControllerScr = DebugUtils.GetComponentWithErrorLogging<PlayerAttackController>(this.gameObject, "PlayerAttackController");
        playerSkillControllerScr = DebugUtils.GetComponentWithErrorLogging<PlayerSkillController>(this.gameObject, "PlayerSkillController");
        playerAnim = DebugUtils.GetComponentWithErrorLogging<PlayerAnim>(this.gameObject, "PlayerAnim");
        playerUtils = DebugUtils.GetComponentWithErrorLogging<PlayerUtils>(this.gameObject, "PlayerUtils");
        playerState = DebugUtils.GetComponentWithErrorLogging<PlayerState>(this.gameObject, "PlayerState");
        newPlayerDashController = DebugUtils.GetComponentWithErrorLogging<PlayerDashController>(this.gameObject, "NewPlayerDashCotroller");
        newPlayerSkillController = DebugUtils.GetComponentWithErrorLogging<PlayerSkillController>(this.gameObject, "NewPlayerSkillController");
        playerDashControllerScr = DebugUtils.GetComponentWithErrorLogging<PlayerDashController>(this.gameObject, "PlayerDashController");
        input = DebugUtils.GetComponentWithErrorLogging<PlayerInputAction>(this.gameObject, "PlayerInputAction");

        EventManager.Instance.AddListener(EVENT_TYPE.Open_Panel_Exclusive, this);
        EventManager.Instance.AddListener(EVENT_TYPE.Open_Panel_Stacked, this);
        EventManager.Instance.AddListener(EVENT_TYPE.Stage_Clear, this);
        EventManager.Instance.AddListener(EVENT_TYPE.Stage_Start, this);
    }
    private void Start()
    {
        SetMoveAction(); // PlayerInputAction에서 Awake에서 action을 설정해주기에 Start에서 설정해야 함.
        // ToDo: UI Changed;
        //EventManager.Instance.AddListener(EVENT_TYPE.UI_Changed, this);
    }

    // Update is called once per frame
    void Update()
    {
        if (CheckCanMove())
        {
            SetMoveAnimation();

            Move(curMoveDir);

            // 조이스틱 이동
            //JoystickControl();

        }      

    }

    private void OnDestroy()
    {
        RemoveListener();
    }

    public void SetMoveAnimation()
    {
/*        if (playerAttackControllerScr.IsAttacking && isMoving)
        {
            playerAnim.SetLayerWeight(1, 1f);
        }
        else
        {
            playerAnim.SetLayerWeight(1, 0f);
        }*/
        playerAnim.SetAnimationFloat("Movement", curMoveDir.magnitude);
        //playerUtils.SetSpineRotation(false, curMoveDir);
    }

    // MoveAction 세팅
    private void SetMoveAction()
    {
        var moveAction = player.InputAction.GetInputAction(PlayerInputActionType.Move);
        if (moveAction == null)
        {
            Debug.LogError("Move Action is null");
            return;
        }

        moveAction.performed += context =>
        {
            SetMoveVector(context);
        };

        moveAction.canceled += context =>
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

    private bool CheckCanMove()
    {
        if (newPlayerDashController.IsDashing || newPlayerSkillController.IsUsingSkill) return false;

        return !playerDashControllerScr.IsDashing && !playerSkillControllerScr.IsUsingSkill /*&& !playerAttackControllerScr.IsAttacking *//*&& playUiOp.enabled*/ && canMove
            && !newPlayerDashController.IsDashing;
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
            // 이동 조이스틱의 x, y 값 읽어오기
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
        if (moveDir == Vector3.zero) 
        {
            isMoving = false;
            return; 
        }

        if (!Physics.Raycast(playerUtils.Tr.position + new Vector3(0f, 0.5f, 0f), moveDir, 0.4f, 1 << 7))
        {
            isMoving = true;
/*            if (playerAttackControllerScr.IsAttacking)
                playerUtils.Tr.Translate(playerUtils.ModelTr.forward * playerState.CurMoveSpeed * 0.8f * Time.deltaTime);
            else*/
            playerUtils.Tr.Translate(playerUtils.ModelTr.forward * playerState.CurMoveSpeed.Value * Time.deltaTime);
        }

        if (moveTurn)
            playerUtils.TurnToDirection(moveDir);
    }

    public void AddListener()
    {

    }

    public void RemoveListener()
    {

    }

    public void OnEvent(EVENT_TYPE eventType, Component Sender, object Param = null)
    {
        switch (eventType)
        {
            case EVENT_TYPE.Open_Panel_Exclusive:
            case EVENT_TYPE.Open_Panel_Stacked:
                PanelType nextPanel = (PanelType)Param;
                if (nextPanel == PanelType.HUD)
                    canMove = true;
                else
                    canMove = false;
                break;
            case EVENT_TYPE.Stage_Clear:
                canMove = false;
                break;
            case EVENT_TYPE.Stage_Start:
                NodeType nodeType = ((Node)Param).type;
                switch (nodeType)
                {
                    case NodeType.Treasure:
                    case NodeType.Comma:
                    case NodeType.Market:
                    case NodeType.Quest:
                        canMove = false;
                        break;
                    default:
                        canMove = true;
                        break;
                }
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


