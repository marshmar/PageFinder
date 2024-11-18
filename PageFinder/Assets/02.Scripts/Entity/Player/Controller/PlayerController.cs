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
    // Dash
    private float dashPower;
    private float dashDuration;
    private float dashWidth;
    private float dashCooltime;
    private int dashCount;
    [SerializeField]
    private float dashCost;
    private bool isDashing;
    private Vector3 dashDest;
    private bool isCreatedDashInkMark;
    private Vector3 originPos;
    private Transform inkObjTransform;

    private PlayerAttackController playerAttackControllerScr;
    private PlayerSkillController playerSkillControllerScr;
    private Player playerScr;

    #endregion

    #region Properties
    public float DashPower { get => dashPower; set => dashPower = value; }
    public float DashDuration { get => dashDuration; set => dashDuration = value; }
    public float DashCooltime { get => dashCooltime; set => dashCooltime = value; }
    public float DashWidth { get => dashWidth; set => dashWidth = value; }

    public float DashCost { get => dashCost; set => dashCost = value; }

    #endregion

    public void Awake()
    {

        dashCooltime = 0.5f;
        dashPower = 4.0f;
        dashDuration = 0.2f;
        dashWidth = 2.0f;
        isDashing = false;
        DashCost = 30.0f;

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
        if (!isDashing && !playerSkillControllerScr.IsUsingSkill && !playerAttackControllerScr.IsAttacking)
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
        if (isDashing)
        {
            float size = 0;

            if (playerInkScr && !isCreatedDashInkMark)
            {
                Vector3 direction = (dashDest - playerScr.Tr.position).normalized;
                Vector3 position = playerScr.Tr.position /*+ direction * (dashPower / 2)*/;
                position.y += 0.1f;

                inkObjTransform = playerInkScr.CreateInk(INKTYPE.LINE, position);
                if (inkObjTransform.TryGetComponent<InkMark>(out InkMark inkMarkScr))
                {
                    inkMarkScr.CurrType = playerScr.DashInkType;
                    inkMarkScr.SetSprites();
                }
                float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
                inkObjTransform.rotation = Quaternion.Euler(90, angle, 0);
                isCreatedDashInkMark = true;
            }

            // 거리 = 속도 x 시간 
            // 4 = 속도 x 0.2f
            // 속도 = 4 * 10 / 2 = 20.0f;
            float dashSpeed = dashPower / dashDuration;

            Vector3 NormalizedDest = (dashDest - playerScr.Tr.position).normalized;

            size = Vector3.Distance(originPos, playerScr.Tr.position);
            if (inkObjTransform)
            {
                inkObjTransform.localScale = new Vector3(dashWidth, size, 0);
            }

            // 현재 위치에서 목표 위치까지 일정한 속도로 이동
            playerScr.Rigid.velocity = NormalizedDest * dashSpeed;

        }
        else
        {
            if(inkObjTransform != null)
            {
                Debug.DrawRay(playerScr.Tr.position, playerScr.ModelTr.forward * 3.0f, Color.red);
/*                if (!Physics.BoxCast(playerScr.Tr.position, new Vector3(0.5f, 0.5f, 0.5f), playerScr.ModelTr.forward, Quaternion.identity, 2.0f, 1 << 7))
                {
                    if (inkObjTransform)
                    {
                        inkObjTransform.localScale = new Vector3(dashWidth, 4.0f, 0);
                    }
                }*/
                if (!Physics.Raycast(playerScr.Tr.position, playerScr.ModelTr.forward,1.0f, 1 << 7))
                {
                    if (inkObjTransform)
                    {
                        inkObjTransform.localScale = new Vector3(dashWidth, 4.0f, 0);
                    }
                }

                inkObjTransform = null;
            }
            playerScr.Rigid.velocity = Vector3.zero;
            isCreatedDashInkMark = false;
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
            StartCoroutine(DashCouroutine(dir));
    }
    public IEnumerator DashCouroutine(Vector3? dashDir)
    {
        if (playerAttackControllerScr.IsAttacking) yield break;

        isDashing = true; 
        playerScr.Anim.SetTrigger("Dash");
        playerScr.CurrInk -= DashCost;
        playerScr.RecoverInk();

        float leftDuration = dashDuration;
        if(dashDir == null)
        {
            dashDest = playerScr.Tr.position + playerScr.ModelTr.forward * dashPower;
        }
        else
        {
            playerScr.TurnToDirection(((Vector3)dashDir).normalized);
            dashDest = playerScr.Tr.position + ((Vector3)dashDir).normalized * dashPower;
        }

        originPos = playerScr.Tr.position;

        yield return new WaitForSeconds(0.2f);

        isDashing = false;

    }

}
