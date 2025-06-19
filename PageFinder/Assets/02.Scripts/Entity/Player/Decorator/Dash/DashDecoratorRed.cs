using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashDecoratorRed : Dash
{
    // Dash
    private float speedUpTimer = 0f;
    private const float speedUpDuration = 1.5f;
    private float scriptValue;

    public DashDecoratorRed(PlayerDashController playerDashController, float scriptValue) : base(playerDashController)
    {
        playerDashControllerScr = playerDashController;
/*        dashCooltime = playerDashController.DashCooltime;
        dashPower = playerDashController.DashPower;
        dashDuration = playerDashController.DashDuration;
        dashWidth = playerDashController.DashWidth;
        dashCost = playerDashController.DashCost;*/
        this.scriptValue = scriptValue;
    }

    public override IEnumerator DashCoroutine(Vector3? dashDir, PlayerUtils playerUtils, PlayerAnim playerAnim, PlayerState playerState)
    {
        playerDashControllerScr.IsDashing = true;
        playerAnim.SetAnimationTrigger("Dash");
        playerState.CurInk -= dashCost;
        //playerState.RecoverInk();

        float leftDuration = dashDuration;
        if (dashDir is null) dashDest = playerUtils.Tr.position + playerUtils.ModelTr.forward * dashPower;
        else
        {
            playerUtils.TurnToDirection(((Vector3)dashDir).normalized);
            dashDest = playerUtils.Tr.position + ((Vector3)dashDir).normalized * dashPower;
        }

        originPos = playerUtils.Tr.position;

        playerState.CurMoveSpeed.AddModifier(new StatModifier(scriptValue, StatModifierType.PercentAddTemporary, this));
        //playerState.CurMoveSpeed = playerState.DefaultMoveSpeed * (1 +scriptValue);
        yield return new WaitForSeconds(0.2f);

        playerDashControllerScr.IsDashing = false;
        EndDash(playerUtils);

        yield return new WaitForSeconds(1.5f);
    }

    public override IEnumerator ExtraEffectCoroutine(Component component = null)
    {
        PlayerState playerState = component as PlayerState;
        speedUpTimer = speedUpDuration;
        // Ÿ�̸Ӱ� ���� ������ ���
        while (speedUpTimer > 0)
        {
            speedUpTimer -= Time.deltaTime;
            yield return null;
        }

        playerState.CurMoveSpeed.RemoveAllFromSource(this);
        //playerState.CurMoveSpeed = playerState.DefaultMoveSpeed;
    }
}