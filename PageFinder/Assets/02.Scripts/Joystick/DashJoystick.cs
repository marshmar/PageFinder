using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class DashJoystick : CoolTimeJoystick
{
    private PlayerDashController playerControllerScr;
    private bool isDraged;


    public override void Start()
    {
        SetImages();
        SetImageState(false);
        FindPlayerObjectAndSetGameObject();

        isDraged = false;
        shortTouchDuration = 0.1f;

        GameObject playerObj = GameObject.FindGameObjectWithTag("PLAYER");
        if(!DebugUtils.CheckIsNullWithErrorLogging<GameObject>(playerObj, this.gameObject))
        {
            playerControllerScr = DebugUtils.GetComponentWithErrorLogging<PlayerDashController>(playerObj, "PlayerController");
            SetCoolTime(playerControllerScr.DashCooltime);
        }
    }

    private void Update()
    {
        CheckInkGaugeAndSetImage(playerControllerScr.DashCost);
    }
    public override void OnPointerDown(PointerEventData eventData)
    {
        if (!coolTimeComponent.IsAbleSkill || (playerScr.CurrInk < playerControllerScr.DashCost) || playerAttackControllerScr.IsAttacking || playerInkMagicControllerScr.IsUsingInkMagic) return;
        direction = Vector3.zero;
        touchStartTime = Time.time;
    }

    public override void OnDrag(PointerEventData eventData)
    {
        if (!coolTimeComponent.IsAbleSkill || (playerScr.CurrInk < playerControllerScr.DashCost) || playerAttackControllerScr.IsAttacking || playerInkMagicControllerScr.IsUsingInkMagic)
            return;

        CheckCancel(eventData);
        isDraged = true;
        SetImageState(true);

        touchPosition = Vector2.zero;
        MoveImage(eventData, ref touchPosition);

        direction = new Vector3(touchPosition.x, 0.1f, touchPosition.y);

        if (playerTargetScr)
        {
            playerTargetScr.FixedLineTargeting(direction, playerControllerScr.DashPower, playerControllerScr.DashWidth);
        }
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (!coolTimeComponent.IsAbleSkill || (playerScr.CurrInk < playerControllerScr.DashCost) || playerAttackControllerScr.IsAttacking || playerInkMagicControllerScr.IsUsingInkMagic) return;
        if (CheckCancel(eventData))
        {
            OffTargetObject();
            ResetImageAndPostion();
            SetImageState(false);
            return;
        }

        ResetImageAndPostion();

        // 터치 시간 측정
        touchEndTime = Time.time;
        touchDuration = touchEndTime - touchStartTime;

        if (touchDuration <= shortTouchDuration || isDraged == false)
        {
            playerControllerScr.Dash();
            isDraged = false;
        }
        else
        {
            playerControllerScr.Dash(new Vector3(direction.x, 0.0f, direction.z));
            isDraged = false;
        }
        StartCoolDown();

        playerTargetScr.OffAllTargetObjects();
        SetImageState(false);
    }
}
