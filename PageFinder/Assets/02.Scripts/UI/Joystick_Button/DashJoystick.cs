using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class DashJoystick : CoolTimeJoystick, IListener
{
    private Player player;

    public override void Awake()
    {
        base.Awake();

        GameObject playerObj = GameObject.FindGameObjectWithTag("PLAYER");

        player = playerObj.GetComponent<Player>();  
    }

    public override void Start()
    {
        base.Start();

        coolTimeComponent.SetCoolTime(player.DashController.DashCoolTime);
        EventManager.Instance.AddListener(EVENT_TYPE.InkGage_Changed, this);
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
        if (player.DashController.IsDashing  || playerState.CurInk < player.DashController.DashCost)
            return true;

        return false;
    }

    public override void OnEvent(EVENT_TYPE eventType, Component sender, object param)
    {
        base.OnEvent(eventType, sender, param);
        switch (eventType)
        {
            case EVENT_TYPE.InkGage_Changed:
                CheckInkGaugeAndSetImage(player.DashController.DashCost);
                break;
        }
    }
}
