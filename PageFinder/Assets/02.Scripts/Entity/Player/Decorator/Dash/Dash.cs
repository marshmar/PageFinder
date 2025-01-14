using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash : DashDecorator
{
    // Dash
    protected Vector3 dashDest;
    protected Vector3 originPos;
    protected float dashPower;
    protected float dashDuration;
    protected float dashWidth;
    protected float dashCooltime;
    protected float dashCost;
    protected bool isDashing;
    protected bool isCreatedDashInkMark;
    protected Transform inkObjTransform;
    protected PlayerDashController playerDashControllerScr;
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

    public virtual void DashMovement( Player playerScr, Vector3? dir = null)
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

    public virtual void EndDash(Player playerScr)
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
    public virtual IEnumerator DashCoroutine(Vector3? dashDir, Player playerScr)
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

    public virtual void GenerateInkMark(Player playerScr)
    {
        if (!isCreatedDashInkMark)
        {
            Vector3 direction = (dashDest - playerScr.Tr.position).normalized;
            Vector3 position = playerScr.Tr.position /*+ direction * (dashPower / 2)*/;
            position.y += 0.1f;

            InkMark inkMark = InkMarkPooler.Instance.Pool.Get();

            inkMark.SetInkMarkData(InkMarkType.DASH, playerScr.DashInkType);

            float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

            inkObjTransform = inkMark.transform;
            inkObjTransform.position = position;
            inkObjTransform.rotation = Quaternion.Euler(90, angle, 0);

            isCreatedDashInkMark = true;
        }
    }
    
    public virtual IEnumerator ExtraEffectCoroutine(Component component)
    {
        yield break;
    }
}
