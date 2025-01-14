using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어의 대쉬에 관한 것을 처리하는 클래스
/// </summary>
public class PlayerDashController : MonoBehaviour
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

    private Player playerScr;
    private PlayerAttackController playerAttackControllerScr;
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

    #endregion

    
    private void Awake()
    {
        dashCoolTime = 0.5f;
        dashPower = 4.0f;
        dashDuration = 0.2f;
        dashWidth = 2.0f;
        dashCost = 30.0f;
        isDashing = false;

        playerAttackControllerScr = DebugUtils.GetComponentWithErrorLogging<PlayerAttackController>(this.gameObject, "PlayerAttackController");
        playerScr = DebugUtils.GetComponentWithErrorLogging<Player>(this.gameObject, "Player");

        dash = new Dash(this);         // 기본 대쉬로 데코레이터 설정
    }
    

    public void SetDecoratorByInkType(InkType dashInkType)
    {
        switch (dashInkType)
        {
            case InkType.RED:
                dash = new DashDecoratorRed(this);
                break;
            case InkType.GREEN:
                dash = new DashDecoratorGreen(this);
                break;
            case InkType.BLUE:
                dash = new DashDecoratorBlue(this);
                break;
        }
    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            dash.GenerateInkMark(playerScr);
            dash.DashMovement(playerScr);
        }
        else
        {
            dash.EndDash(playerScr);
        }
    }

    public void Dash(Vector3? dir = null)
    {
        if(playerScr.CurrInk >= DashCost && !playerAttackControllerScr.IsAttacking)
        {
            StartCoroutine(dash.DashCoroutine(dir, playerScr));
            if(extraEffectCoroutine != null)
            {
                StopCoroutine(extraEffectCoroutine);
            }

            extraEffectCoroutine = StartCoroutine(dash.ExtraEffectCoroutine(playerScr));
        }
    }
}
