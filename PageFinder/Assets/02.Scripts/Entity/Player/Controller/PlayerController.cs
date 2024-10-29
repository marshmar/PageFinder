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

    private INKMARK dashInkMark;

    private PlayerAttackController playerAttackControllerScr;
    private PlayerSkillController playerSkillControllerScr;
    private Player playerScr;
    #endregion

    #region Properties
    public float DashPower { get => dashPower; set => dashPower = value; }
    public float DashDuration { get => dashDuration; set => dashDuration = value; }
    public float DashCooltime { get => dashCooltime; set => dashCooltime = value; }
    public float DashWidth { get => dashWidth; set => dashWidth = value; }
    public INKMARK DashInkMark { get => dashInkMark; set => dashInkMark = value; }
    public float DashCost { get => dashCost; set => dashCost = value; }

    #endregion

    public void Awake()
    {

        dashCooltime = 1.0f;
        dashPower = 4.0f;
        dashDuration = 0.2f;
        DashWidth = 2.0f;
        isDashing = false;
        DashInkMark = INKMARK.RED;
        DashCost = 30.0f;

    }
    public void Start()
    {
        if(TryGetComponent<PlayerInk>(out PlayerInk pi))
        {
            playerInkScr = pi;
        }
        if (TryGetComponent<PlayerSkillController>(out PlayerSkillController PSCS))
        {
            playerSkillControllerScr = PSCS;
        }
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

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            dashInkMark = INKMARK.RED;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            dashInkMark = INKMARK.BLUE;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            dashInkMark = INKMARK.GREEN;
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
        //tr.position += moveDir * moveSpeed * Time.deltaTime;
        playerScr.TurnToDirection(moveDir);
    }

    public void Dash(Vector3? dir = null)
    {
        if(playerScr.CurrInk >= DashCost)
            StartCoroutine(DashCouroutine(dir));
    }
    public IEnumerator DashCouroutine(Vector3? dashDir)
    {
        isDashing = true; 
        playerScr.Anim.SetTrigger("Dash");
        playerScr.CurrInk -= DashCost;
        playerScr.RecoverInk();
        float leftDuration = dashDuration;
        Vector3 dest;
        if(dashDir == null)
        {
            dest = playerScr.Tr.position + playerScr.ModelTr.forward * dashPower;
        }
        else
        {
            playerScr.TurnToDirection(((Vector3)dashDir).normalized);
            dest = playerScr.Tr.position + ((Vector3)dashDir).normalized * dashPower;
        }

        float dashSpeed = dashPower / leftDuration; // 일정한 속도 계산
        float currentDuration = 0f;
        Transform inkObjTransform = null;
        if (playerInkScr)
        {
            Vector3 direction = (dest - playerScr.Tr.position).normalized;
            Vector3 position = playerScr.Tr.position + direction * (dashPower / 2);
            position.y += 0.1f;

            inkObjTransform = playerInkScr.CreateInk(INKTYPE.LINE, position);
            if (inkObjTransform.TryGetComponent<InkMark>(out InkMark inkMarkScr))
            {
                inkMarkScr.CurrMark = DashInkMark;
                inkMarkScr.SetMaterials();
            }
            float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            inkObjTransform.rotation = Quaternion.Euler(90, angle, 0);
        }
        float size = 0;
        Vector3 originPos = playerScr.Tr.position;
        while (currentDuration < leftDuration)
        {
            // 현재 위치에서 목표 위치까지 일정한 속도로 이동
            playerScr.Tr.position = Vector3.MoveTowards(playerScr.Tr.position, dest, dashSpeed * Time.deltaTime);
            size = Vector3.Distance(originPos, playerScr.Tr.position);
            if (inkObjTransform)
            {
                inkObjTransform.localScale = new Vector3(dashWidth, size, 0);
            }

            yield return null; // 다음 프레임까지 대기

            currentDuration += Time.deltaTime;
        }

        isDashing = false;

    }
}
