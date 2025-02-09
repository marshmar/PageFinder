using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;

public class SkillJoystick : CoolTimeJoystick, IListener
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
        //base.Start();
        shortTouchThreshold = 0.1f;

        coolTimeComponent.SetCoolTime(playerSkillControllerScr.CurrSkillData.skillCoolTime);
        EventManager.Instance.AddListener(EVENT_TYPE.InkGage_Changed, this);
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

    public void OnEvent(EVENT_TYPE eventType, Component sender, object param)
    {
        switch (eventType)
        {
            case EVENT_TYPE.InkGage_Changed:
                CheckInkGaugeAndSetImage(playerSkillControllerScr.CurrSkillData.skillCost);
                break;
        }
    }
}
