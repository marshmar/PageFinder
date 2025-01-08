using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash : DashDecorator
{
    // Dash
    private Vector3 dashDest;
    private Vector3 originPos;
    private float dashPower;
    private float dashDuration;
    private float dashWidth;
    private float dashCooltime;
    private float dashCost;
    private bool isDashing;
    private bool isCreatedDashInkMark;
    private Transform inkObjTransform;
    private PlayerDashController playerDashControllerScr;
    public float DashPower { get => dashPower; set => dashPower = value; }
    public float DashDuration { get => dashDuration; set => dashDuration = value; }
    public float DashWidth { get => dashWidth; set => dashWidth = value; }
    public float DashCooltime { get => dashCooltime; set => dashCooltime = value; }
    public float DashCost { get => dashCost; set => dashCost = value; }
    public bool IsDashing { get => isDashing; set => isDashing = value; }

    public Dash(PlayerDashController playerDashController)
    {
        playerDashControllerScr = playerDashController;
        dashCooltime = playerDashController.DashCooltime;
        dashPower = playerDashController.DashPower;
        dashDuration = playerDashController.DashDuration;
        dashWidth = playerDashController.DashWidth;
        dashCost = playerDashController.DashCost;
    }

    public void DashMovement( Player playerScr, Vector3? dir = null)
    {
        // 거리 = 속도 x 시간 
        // 4 = 속도 x 0.2f
        // 속도 = 4 * 10 / 2 = 20.0f;
        float dashSpeed = dashPower / dashDuration;

        Vector3 NormalizedDest = (dashDest - playerScr.Tr.position).normalized;
        
        float size = Vector3.Distance(originPos, playerScr.Tr.position);
        if (inkObjTransform)
        {
            inkObjTransform.localScale = new Vector3(dashWidth, size, 0);
        }

        // 현재 위치에서 목표 위치까지 일정한 속도로 이동
        playerScr.Rigid.velocity = NormalizedDest * dashSpeed;
    }

    public void EndDash(Player playerScr)
    {
        if (inkObjTransform != null)
        {
            Debug.DrawRay(playerScr.Tr.position, playerScr.ModelTr.forward * 3.0f, Color.red);
            /*                if (!Physics.BoxCast(playerScr.Tr.position, new Vector3(0.5f, 0.5f, 0.5f), playerScr.ModelTr.forward, Quaternion.identity, 2.0f, 1 << 7))
                            {
                                if (inkObjTransform)
                                {
                                    inkObjTransform.localScale = new Vector3(dashWidth, 4.0f, 0);
                                }
                            }*/
            if (!Physics.Raycast(playerScr.Tr.position, playerScr.ModelTr.forward, 1.0f, 1 << 7))
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
    public IEnumerator DashCoroutine(Vector3? dashDir, Player playerScr)
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
    }

    public void GenerateInkMark(PlayerInk playerInkScr, Player playerScr)
    {
        if (playerInkScr && !isCreatedDashInkMark)
        {
            Vector3 direction = (dashDest - playerScr.Tr.position).normalized;
            Vector3 position = playerScr.Tr.position /*+ direction * (dashPower / 2)*/;
            position.y += 0.1f;

            inkObjTransform = playerInkScr.CreateInk(INKMARKTYPE.DASH, position);
            if (inkObjTransform.TryGetComponent<InkMark>(out InkMark inkMarkScr))
            {
                inkMarkScr.CurrType = playerScr.DashInkType;
                inkMarkScr.SetSprites();
            }
            float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            inkObjTransform.rotation = Quaternion.Euler(90, angle, 0);
            isCreatedDashInkMark = true;
        }
    }
    
    public IEnumerator ExtraEffectCoroutine(Component component)
    {
        yield break;
    }
}
