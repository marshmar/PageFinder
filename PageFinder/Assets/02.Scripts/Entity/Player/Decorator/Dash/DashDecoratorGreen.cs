using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashDecoratorGreen : Dash
{
    // Dash

    private float shieldTimer = 0f;
    private const float shieldDuration = 1.5f;
    private bool createdShield = false;


    public DashDecoratorGreen(PlayerDashController playerDashController) : base(playerDashController)
    {
        playerDashControllerScr = playerDashController;
        dashCooltime = playerDashController.DashCooltime;
        dashPower = playerDashController.DashPower;
        dashDuration = playerDashController.DashDuration;
        dashWidth = playerDashController.DashWidth;
        dashCost = playerDashController.DashCost;
    }

    public override IEnumerator DashCoroutine(Vector3? dashDir, Player playerScr)
    {
        playerDashControllerScr.IsDashing = true;
        playerScr.Anim.SetTrigger("Dash");
        playerScr.CurrInk -= dashCost;
        playerScr.RecoverInk();

        float leftDuration = dashDuration;
        if (dashDir == null)
        {
            dashDest = playerScr.Tr.position + playerScr.ModelTr.forward * dashPower;
        }
        else
        {
            playerScr.TurnToDirection(((Vector3)dashDir).normalized);
            dashDest = playerScr.Tr.position + ((Vector3)dashDir).normalized * dashPower;
        }

        originPos = playerScr.Tr.position;

        Debug.Log("실드 생성");


        yield return new WaitForSeconds(0.2f);

        playerDashControllerScr.IsDashing = false;
    }

    public override IEnumerator ExtraEffectCoroutine(Component component)
    {
        Player playerScr = component as Player;
        shieldTimer = shieldDuration;

        if (!createdShield)
        {
            playerScr.CurrShield += playerScr.MAXHP * 0.07f;
            createdShield = true;
        }

        // 타이머가 끝날 때까지 대기
        while (shieldTimer > 0)
        {
            shieldTimer -= Time.deltaTime;
            yield return null;
        }

        // 실드 제거
        Debug.Log("실드 제거");
        playerScr.CurrShield -= playerScr.MAXHP * 0.07f;
        createdShield = false;
    }
}
