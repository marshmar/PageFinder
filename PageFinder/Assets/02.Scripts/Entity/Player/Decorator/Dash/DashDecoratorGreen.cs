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

    public override IEnumerator DashCoroutine(Vector3? dashDir, PlayerUtils playerUtils, PlayerAnim playerAnim, PlayerState playerState)
    {
        playerDashControllerScr.IsDashing = true;
        playerAnim.SetAnimationTrigger("Dash");
        playerState.CurInk -= dashCost;

        float leftDuration = dashDuration;
        if (dashDir == null)
        {
            dashDest = playerUtils.Tr.position + playerUtils.ModelTr.forward * dashPower;
        }
        else
        {
            playerUtils.TurnToDirection(((Vector3)dashDir).normalized);
            dashDest = playerUtils.Tr.position + ((Vector3)dashDir).normalized * dashPower;
        }

        originPos = playerUtils.Tr.position;

        Debug.Log("실드 생성");


        yield return new WaitForSeconds(0.2f);

        playerDashControllerScr.IsDashing = false;
    }

    public override IEnumerator ExtraEffectCoroutine(Component component)
    {
        PlayerState playerState = component as PlayerState;
        EventManager.Instance.PostNotification(EVENT_TYPE.Generate_Shield_Player, component, new System.Tuple<float, float>(playerState.MaxHp * 0.07f, 1.5f));

        yield return null;
        /*shieldTimer = shieldDuration;

        if (!createdShield)
        {
            playerState.CurShield += playerState.MaxHp * 0.07f;
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
        playerState.CurShield -= playerState.MaxHp * 0.07f;
        createdShield = false;*/
    }
}
