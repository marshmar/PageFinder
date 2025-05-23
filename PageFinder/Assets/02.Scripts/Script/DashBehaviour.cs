using UnityEngine;
using System.Collections;
using System;

public class DashContext : ScriptContext
{
    public PlayerUtils playerUtils;
    public PlayerAnim playerAnim;
    public PlayerTarget playerTarget;
    public PlayerState playerState;
    public NewPlayerDashController playerDashController;
}

public class DashBehaviour : IChargeBehaviour
{
    private NewScriptData scriptData;

    private Vector3 dashDir;
    private Vector3 dashDest;
    private Vector3 originPos;

    private float dashPower = 4.0f;
    private float dashWidth = 2.0f;
    private float dashDuration = 0.2f;
    private float dashCoolTime = 0.3f;
    private float dashCost = 30.0f;
    private bool isCharged = false;
    private Transform inkMarkTransform;

    private PlayerUtils playerUtils;
    private PlayerAnim playerAnim;
    private PlayerState playerState;
    private PlayerTarget playerTarget;
    private NewPlayerDashController playerDashController;
    public bool CanExcuteBehaviour()
    {
        return true;
    }

    public void ChargingBehaviour()
    {
        SetDashDirection();
        playerTarget.FixedLineTargeting(dashDir, dashPower, dashWidth);
        isCharged = true;
    }

    private void SetDashDirection()
    {
        Vector2 screenDirection = Input.mousePosition - Camera.main.WorldToScreenPoint(playerUtils.Tr.position);
        dashDir = new Vector3(screenDirection.x, 0f, screenDirection.y).normalized;
    }

    public void ExcuteAnim()
    {
        playerAnim.ResetAnim();
        playerAnim.SetAnimationTrigger("Dash");
    }

    public void ExcuteBehaviour()
    {
        playerTarget.OffAllTargetObjects();

        PlayAudio();

        ExcuteAnim();

        if (isCharged)
            Dash(dashDir);
        else
            Dash();
    }

    private void Dash(Vector3? dir = null)
    {
        CoroutineRunner.Instance.RunCoroutine(DashCoroutine(dir, playerUtils, playerAnim, playerState));
    }

    private void PlayAudio()
    {
        // 대쉬 효과음 재생
        AudioManager.Instance.Play(Sound.dashVfx1, AudioClipType.DashSfx);
    }

    public void GenerateInkMark(Vector3 position)
    {
        //throw new System.NotImplementedException();
    }

    public void SetContext(ScriptContext context)
    {
        DashContext dashContext = context as DashContext;
        if(dashContext != null)
        {
            playerAnim = dashContext.playerAnim;
            playerState = dashContext.playerState;
            playerTarget = dashContext.playerTarget;
            playerUtils = dashContext.playerUtils;
            playerDashController = dashContext.playerDashController;
        }
    }

    public void SetScriptData(NewScriptData scriptData)
    {
        this.scriptData = scriptData;
    }

    /*    public Action DashMovement(Vector3? dir = null)
        {
            return () =>
            {
                float dashSpeed = dashPower / dashDuration;

                Vector3 currentPos = playerUtils.Tr.position;
                Vector3 toDest = dashDest - currentPos;
                Vector3 normalizedDir = toDest.normalized;

                // 이동 거리 계산
                float moveDistance = dashSpeed * Time.fixedDeltaTime;

                // 목표 지점까지 남은 거리
                float remainingDistance = toDest.magnitude;

                // 초과하지 않도록 거리 제한
                Vector3 movement = normalizedDir * Mathf.Min(moveDistance, remainingDistance);

                // 실제 이동
                playerUtils.Rigid.MovePosition(currentPos + movement);

                // 잉크 마크 처리
                float size = Vector3.Distance(originPos, playerUtils.Tr.position);

                if (inkMarkTransform)
                {
                    inkMarkTransform.localScale = new Vector3(dashWidth, size * 0.5f, 0);
                    inkMarkTransform.position = playerUtils.Tr.position - size * 0.5f * normalizedDir;
                    inkMarkTransform.position = new Vector3(
                        inkMarkTransform.position.x,
                        playerUtils.Tr.position.y + 0.1f,
                        inkMarkTransform.position.z
                    );
                }
            };
        }*/

    public Action DashMovement(Vector3? dir = null)
    {
        return () =>
        {
            float dashSpeed = dashPower / dashDuration;

            Vector3 NormalizedDest = (dashDest - playerUtils.Tr.position).normalized;

            float size = Vector3.Distance(originPos, playerUtils.Tr.position);
            if (inkMarkTransform)
            {
                inkMarkTransform.localScale = new Vector3(dashWidth, size * 0.5f, 0);
                inkMarkTransform.position = playerUtils.Tr.position - size * 0.5f * NormalizedDest;
                inkMarkTransform.position = new Vector3(inkMarkTransform.transform.position.x, playerUtils.Tr.position.y + 0.1f, inkMarkTransform.transform.position.z);
            }

            playerUtils.Rigid.linearVelocity = NormalizedDest * dashSpeed;
        };
    }

    public virtual void EndDash(PlayerUtils playerUtils)
    {
        if (inkMarkTransform != null)
        {
            Debug.DrawRay(playerUtils.Tr.position, playerUtils.ModelTr.forward * 3.0f, Color.red);

/*            if (!Physics.Raycast(playerUtils.Tr.position, playerUtils.ModelTr.forward, 1.0f, 1 << 7) && inkMarkTransform)
            {
                inkMarkTransform.localScale = new Vector3(dashWidth, dashPower * 0.5f, 0);
                BoxCollider dashMarkColl = inkMarkTransform.GetComponent<BoxCollider>();
                if (dashMarkColl != null) dashMarkColl.size = new Vector3(1f, inkMarkTransform.localScale.y + 0.1f, 0f);
            }*/

            Vector3 boxSize = new Vector3(0.5f, 1.0f, 0.5f);
            if(!Physics.CheckBox(playerUtils.Tr.position + playerUtils.ModelTr.forward +  new Vector3(0f, 1.0f, 0f), boxSize, Quaternion.identity, 1 << 7))
            {
                inkMarkTransform.localScale = new Vector3(dashWidth, dashPower * 0.5f, 0);
                BoxCollider dashMarkColl = inkMarkTransform.GetComponent<BoxCollider>();
                if (dashMarkColl != null) dashMarkColl.size = new Vector3(1f, inkMarkTransform.localScale.y + 0.1f, 0f);
            }

            InkMark inkMark = DebugUtils.GetComponentWithErrorLogging<InkMark>(inkMarkTransform, "InkMark");
            if (!DebugUtils.CheckIsNullWithErrorLogging<InkMark>(inkMark))
            {
                inkMark.IsAbleFusion = true;
                inkMark.AddCollider();
            }

            inkMarkTransform = null;
        }
        playerUtils.Rigid.linearVelocity = Vector3.zero;
        dashDir = Vector3.zero;
        isCharged = false;
    }

    private IEnumerator DashCoroutine(Vector3? dashDir, PlayerUtils playerUtils, PlayerAnim playerAnim, PlayerState playerState)
    {
        if (playerDashController.IsDashing) yield break;
        playerDashController.IsDashing = true;

        playerState.CurInk -= dashCost;
        playerState.RecoverInk();

        if (dashDir == null) dashDest = playerUtils.Tr.position + playerUtils.ModelTr.forward * dashPower;
        else
        {
            playerUtils.TurnToDirection(((Vector3)dashDir).normalized);
            dashDest = playerUtils.Tr.position + ((Vector3)dashDir).normalized * dashPower;
        }



        originPos = playerUtils.Tr.position;;
        GenerateInkMark();

        playerDashController.FixedUpdateDashAction += DashMovement();

        yield return new WaitForSeconds(0.2f);

        playerDashController.FixedUpdateDashAction -= DashMovement();
        playerDashController.IsDashing = false;
        EndDash(playerUtils);
    }

    public virtual void GenerateInkMark()
    {
        Vector3 direction = (dashDest - playerUtils.Tr.position).normalized;
        Vector3 position = playerUtils.Tr.position /*+ direction * (dashPower / 2)*/;
        position.y += 0.1f;

        InkMark inkMark = InkMarkPooler.Instance.Pool.Get();

        inkMark.SetInkMarkData(InkMarkType.DASH, scriptData.inkType, false);
        inkMark.IsAbleFusion = false;

        float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

        inkMarkTransform = inkMark.transform;
        inkMarkTransform.SetPositionAndRotation(position, Quaternion.Euler(90, angle, 0));
    }
}
