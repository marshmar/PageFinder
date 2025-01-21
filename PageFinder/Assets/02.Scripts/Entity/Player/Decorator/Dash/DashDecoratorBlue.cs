using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashDecoratorBlue : Dash
{
    #region Variables
    // Dash
    #endregion


    public DashDecoratorBlue(PlayerDashController playerDashController) : base(playerDashController)
    {
        playerDashControllerScr = playerDashController;
        dashCooltime = playerDashController.DashCooltime;
        dashPower = playerDashController.DashPower;
        dashDuration = playerDashController.DashDuration;
        dashWidth = playerDashController.DashWidth;
        dashCost = playerDashController.DashCost;
    }


/*    public override IEnumerator DashCoroutine(Vector3? dashDir,Player playerScr)
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

        yield return new WaitForSeconds(0.2f);

        playerDashControllerScr.IsDashing = false;
        
        // 관련 로직 추가하기

    }*/


    public override IEnumerator ExtraEffectCoroutine(Component component = null)
    {
        yield break;
    }
}
