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

    public virtual void DashMovement(PlayerUtils playerUtils, Vector3? dir = null)
    {

        float dashSpeed = dashPower / dashDuration;

        Vector3 NormalizedDest = (dashDest - playerUtils.Tr.position).normalized;

        float size = Vector3.Distance(originPos, playerUtils.Tr.position);
        if (inkObjTransform)
        {
            inkObjTransform.localScale = new Vector3(dashWidth, size * 0.5f, 0);
            inkObjTransform.position = playerUtils.Tr.position - size * 0.5f * NormalizedDest;
            inkObjTransform.position = new Vector3(inkObjTransform.transform.position.x, playerUtils.Tr.position.y + 0.1f, inkObjTransform.transform.position.z);
        }

        playerUtils.Rigid.linearVelocity = NormalizedDest * dashSpeed;
    }

    public virtual void EndDash(PlayerUtils playerUtils)
    {
        if (inkObjTransform != null)
        {
            Debug.DrawRay(playerUtils.Tr.position, playerUtils.ModelTr.forward * 3.0f, Color.red);

            if (!Physics.Raycast(playerUtils.Tr.position, playerUtils.ModelTr.forward, 1.0f, 1 << 7))
            {
                if (inkObjTransform)
                {
                    inkObjTransform.localScale = new Vector3(dashWidth, dashPower * 0.5f, 0);
                    BoxCollider dashMarkColl = inkObjTransform.GetComponent<BoxCollider>();
                    if (dashMarkColl is not null) dashMarkColl.size = new Vector3(1f, inkObjTransform.localScale.y + 0.1f, 0f);
                }
            }

            InkMark inkMark = DebugUtils.GetComponentWithErrorLogging<InkMark>(inkObjTransform, "InkMark");
            if (!DebugUtils.CheckIsNullWithErrorLogging<InkMark>(inkMark)) inkMark.IsAbleFusion = true;

            inkObjTransform = null;
        }
        playerUtils.Rigid.linearVelocity = Vector3.zero;
        isCreatedDashInkMark = false;
    }

    public virtual IEnumerator DashCoroutine(Vector3? dashDir, PlayerUtils playerUtils, PlayerAnim playerAnim, PlayerState playerState)
    {
        playerDashControllerScr.IsDashing = true;
        playerAnim.SetAnimationTrigger("Dash");
        playerState.CurInk -= dashCost;
        playerState.RecoverInk();

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

        yield return new WaitForSeconds(0.2f);

        playerDashControllerScr.IsDashing = false;
    }

    public virtual void GenerateInkMark(PlayerInkType playerInkType, PlayerUtils playerUtils)
    {
        if (!isCreatedDashInkMark)
        {
            Vector3 direction = (dashDest - playerUtils.Tr.position).normalized;
            Vector3 position = playerUtils.Tr.position /*+ direction * (dashPower / 2)*/;
            position.y += 0.1f;

            InkMark inkMark = InkMarkPooler.Instance.Pool.Get();

            inkMark.SetInkMarkData(InkMarkType.DASH, playerInkType.DashInkType);
            inkMark.IsAbleFusion = false;

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