using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashDecoratorRed : Dash
{
    // Dash
    private float speedUpTimer = 0f;
    private const float speedUpDuration = 1.5f;


    public DashDecoratorRed(PlayerDashController playerDashController) : base(playerDashController)
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

        // Debug.Log("속도 증가");
        playerScr.MoveSpeed = playerScr.OriginalMoveSpeed * 1.15f;
        yield return new WaitForSeconds(0.2f);

        playerDashControllerScr.IsDashing = false;



        yield return new WaitForSeconds(1.5f);

    }

    public override IEnumerator ExtraEffectCoroutine(Component component = null)
    {
        Player playerScr = component as Player;
        speedUpTimer = speedUpDuration;
        // 타이머가 끝날 때까지 대기
        while (speedUpTimer > 0)
        {
            speedUpTimer -= Time.deltaTime;
            yield return null;
        }

        Debug.Log("속도 원래대로");
        playerScr.MoveSpeed = playerScr.OriginalMoveSpeed;
    }
}
