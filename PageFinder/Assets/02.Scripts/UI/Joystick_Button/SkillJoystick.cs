using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;

public class SkillJoystick : CoolTimeJoystick
{
    private PlayerDashController playerDashControllerScr;
    private PlayerAttackController playerAttackControllerScr;
    private PlayerSkillController playerSkillControllerScr;

    public override void Awake()
    {
        playerSkillControllerScr = GetComponentInParent<PlayerSkillController>();
        playerDashControllerScr = GetComponentInParent<PlayerDashController>();
        playerAttackControllerScr = GetComponentInParent<PlayerAttackController>();

        base.Awake();
    }

    public override void Start()
    {
        base.Start();

        coolTimeComponent.SetCoolTime(playerSkillControllerScr.CurrSkillData.skillCoolTime);
    }
    private void Update()
    {
        CheckInkGaugeAndSetImage(playerSkillControllerScr.CurrSkillData.skillCost);
    }


    public override void OnPointerDown(PointerEventData eventData)
    {
        if (CheckIsNotAbleSkill()) return;

        base.OnPointerDown(eventData);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        if (CheckIsNotAbleSkill()) return;

        base.OnDrag(eventData);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (CheckIsNotAbleSkill()) return;

        base.OnPointerUp(eventData);
    }

    public bool CheckIsNotAbleSkill()
    {
        if (playerDashControllerScr.IsDashing || playerAttackControllerScr.IsAttacking 
            || playerState.CurInk < playerSkillControllerScr.CurrSkillData.skillCost)
            return true;

        return false;
    }
}
