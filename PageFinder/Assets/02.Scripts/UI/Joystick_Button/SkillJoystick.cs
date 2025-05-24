using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;

public class SkillJoystick : CoolTimeJoystick, IListener
{
    private Player player;

    public override void Awake()
    {

        base.Awake();

        GameObject playerObj = GameObject.FindGameObjectWithTag("PLAYER");

        player = playerObj.GetComponent<Player>();


        EventManager.Instance.AddListener(EVENT_TYPE.Skill_Successly_Used, this);
    }

    public override void Start()
    {
        base.Start();
        shortTouchThreshold = 0.1f;

        coolTimeComponent.SetCoolTime(player.SkillController.SkillCoolTime);
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
        if (player.DashController.IsDashing || playerState.CurInk < player.SkillController.SkillCost)
            return true;

        return false;
    }

    public override void OnEvent(EVENT_TYPE eventType, Component sender, object param = null)
    {
        base.OnEvent(eventType, sender, param);
        switch (eventType)
        {
            case EVENT_TYPE.InkGage_Changed:
                CheckInkGaugeAndSetImage(player.SkillController.SkillCost);
                break;
            case EVENT_TYPE.Skill_Successly_Used:
                coolTimeComponent.StartCoolDown();
                break;         
        }
    }
}
