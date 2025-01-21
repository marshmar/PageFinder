using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class DashJoystick : CoolTimeJoystick
{
    private PlayerDashController playerDashControllerScr;
    private PlayerAttackController playerAttackControllerScr;

    public override void Awake()
    {
        base.Awake();

        playerDashControllerScr = GetComponentInParent<PlayerDashController>();
        playerAttackControllerScr = GetComponentInParent<PlayerAttackController>();

    }

    public override void Start()
    {
        base.Start();

        coolTimeComponent.SetCoolTime(playerDashControllerScr.DashCooltime);
    }

    private void Update()
    {
        CheckInkGaugeAndSetImage(playerDashControllerScr.DashCost);
    }
    public override void OnPointerDown(PointerEventData eventData)
    {
        if (CheckIsNotAbleDash()) return;

        base.OnPointerDown(eventData);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        if (CheckIsNotAbleDash()) return;

        base.OnDrag(eventData);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (CheckIsNotAbleDash()) return;

        base.OnPointerUp(eventData);
    }

    public bool CheckIsNotAbleDash()
    {
        if (playerDashControllerScr.IsDashing || playerAttackControllerScr.IsAttacking || playerState.CurInk < playerDashControllerScr.DashCost)
            return true;

        return false;
    }
}
