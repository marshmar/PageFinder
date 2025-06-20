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
    private Vector3 _curMoveDir, _beforeMoveDir;
    private bool _canMove    = true;
    private bool _isMoving   = false;
    private bool _canTurn    = true;

    // Hashing
    private Player _player;
    private Dictionary<EVENT_TYPE, Action<object>> _eventHandlers;
    [SerializeField] private VirtualJoystick moveJoystick;
    [SerializeField] private Canvas playUiOp;
    #endregion

    #region Properties

    public bool IsMoving { get => _isMoving; set => _isMoving = value; }
    public bool CanTurn { get => _canTurn; set => _canTurn = value; }
    public bool CanMove { get => _canMove; set => _canMove = value; }
    #endregion

    #region Unity Lifecycle

    public void Awake()
    {
        _player = this.GetComponentSafe<Player>();
    }

    private void Start()
    {
        AddListener();

        InitializeEventHandlers();
        InitializeMoveAction();
    }

    private void OnDestroy()
    {
        RemoveListener();
    }

    void Update()
    {
        if (CheckCanMove())
        {
            PlayMoveAnim();

            Move();

            // 조이스틱 이동
            //JoystickControl();
        }

    }
    #endregion

    #region Initialization
    private void InitializeMoveAction()
    {
        var moveAction = _player.InputAction.GetInputAction(PlayerInputActionType.Move);
        if (moveAction == null)
        {
            Debug.LogError("Move Action is null");
            return;
        }

        moveAction.performed += context =>
        {
            SetMoveDirection(context);
        };

        moveAction.canceled += context =>
        {
            _curMoveDir = Vector3.zero;
        };
    }

    private void InitializeEventHandlers()
    {
        _eventHandlers = new Dictionary<EVENT_TYPE, Action<object>>()
        {
            { EVENT_TYPE.Open_Panel_Exclusive,  OpenPanelHandlers},
            { EVENT_TYPE.Open_Panel_Stacked, OpenPanelHandlers},
            { EVENT_TYPE.Stage_Clear, param => _canMove = false},
            { EVENT_TYPE.Stage_Start, StageStartHandlers},
        };
    }
    #endregion

    #region Actions
    public void PlayMoveAnim()
    {
        _player.Anim.SetAnimationFloat("Movement", _curMoveDir.magnitude);

        // Upper and lower body split animation logic
        /*        if (playerAttackControllerScr.IsAttacking && isMoving)
        {
            playerAnim.SetLayerWeight(1, 1f);
        }
        else
        {
            playerAnim.SetLayerWeight(1, 0f);
        }*/
        //playerUtils.SetSpineRotation(false, curMoveDir);
    }

    public void SetMoveDirection(InputAction.CallbackContext context)
    {
        Vector2 contextVec = context.ReadValue<Vector2>();
        _beforeMoveDir = _curMoveDir;
        _curMoveDir = new Vector3(contextVec.x, 0, contextVec.y);
    }


    private void Move()
    {
        if (_curMoveDir == Vector3.zero)
        {
            _isMoving = false;
            return;
        }

        // Prevent movement if there is an obstacle in the direction of movement
        {
            Vector3 rayStartOffeset = new Vector3(0f, 0.5f, 0f);
            int mapLayer = LayerMask.GetMask("MAP");
            float rayDistance = 0.4f;
            if (!Physics.Raycast(_player.Utils.Tr.position + rayStartOffeset, _curMoveDir, rayDistance, mapLayer))
            {
                _isMoving = true;
                /*            if (playerAttackControllerScr.IsAttacking)
                                playerUtils.Tr.Translate(playerUtils.ModelTr.forward * playerState.CurMoveSpeed * 0.8f * Time.deltaTime);
                            else*/
                _player.Utils.Tr.Translate(_player.Utils.ModelTr.forward * _player.State.CurMoveSpeed.Value * Time.deltaTime);
            }
        }

        if (_canTurn)
            _player.Utils.TurnToDirection(_curMoveDir);
    }

    private void OpenPanelHandlers(object param)
    {
        PanelType nextPanel = (PanelType)param;
        _canMove = nextPanel == PanelType.HUD ? true : false;
    }

    private void StageStartHandlers(object param)
    {
        NodeType nodeType = ((Node)param).type;
        switch (nodeType)
        {
            case NodeType.Treasure:
            case NodeType.Comma:
            case NodeType.Market:
            case NodeType.Quest:
                _canMove = false;
                break;
            default:
                _canMove = true;
                break;
        }
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
    #endregion

    #region Getter
    #endregion

    #region Setter
    #endregion

    #region Utilities
    private bool CheckCanMove()
    {
        if (_player.DashController.IsDashing || _player.SkillController.IsUsingSkill) return false;

        if (!_canMove) return false;

        return true;
    }
    #endregion

    #region Events
    public void AddListener()
    {
        EventManager.Instance.AddListener(EVENT_TYPE.Open_Panel_Exclusive, this);
        EventManager.Instance.AddListener(EVENT_TYPE.Open_Panel_Stacked, this);
        EventManager.Instance.AddListener(EVENT_TYPE.Stage_Clear, this);
        EventManager.Instance.AddListener(EVENT_TYPE.Stage_Start, this);
    }

    public void RemoveListener()
    {

        EventManager.Instance.RemoveListener(EVENT_TYPE.Open_Panel_Exclusive, this);
        EventManager.Instance.RemoveListener(EVENT_TYPE.Open_Panel_Stacked, this);
        EventManager.Instance.RemoveListener(EVENT_TYPE.Stage_Clear, this);
        EventManager.Instance.RemoveListener(EVENT_TYPE.Stage_Start, this);
    }

    public void OnEvent(EVENT_TYPE eventType, Component Sender, object Param = null)
    {
        if (_eventHandlers.TryGetValue(eventType, out var handler))
        {
            handler.Invoke(Param);
        }
        else
        {
            Debug.LogWarning($"Unhandled event: {eventType}");
        }
    }
    #endregion














}


