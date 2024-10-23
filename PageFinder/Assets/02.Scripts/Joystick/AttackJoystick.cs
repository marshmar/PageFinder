using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class AttackJoystick : VirtualJoystick
{
    
    private PlayerAttackController playerAttackScr;


    public override void Start()
    {
        shortTouchDuration = 0.1f;
        GameObject playerObj = GameObject.FindGameObjectWithTag("PLAYER");
        if(!DebugUtils.CheckIsNullWithErrorLogging<GameObject>(playerObj, this.gameObject))
        {
            playerAttackScr = DebugUtils.GetComponentWithErrorLogging<PlayerAttackController>(playerObj, "PlayerAttackController");
        }    
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        touchStartTime = Time.time;
    }

    public override void OnDrag(PointerEventData eventData)
    {
        touchPosition = Vector2.zero;

        MoveImage(eventData, ref touchPosition);
    }
    public override void OnPointerUp(PointerEventData eventData)
    {
        ResetImageAndPostion();

        // 터치 시간 측정
        touchEndTime = Time.time;
        touchDuration = touchEndTime - touchStartTime;

        if (touchDuration <= shortTouchDuration)
        {
            Debug.Log("짧은 공격");
            playerAttackScr.AttackType = AttackType.SHORTATTCK;
        }
        else
        {
            Debug.Log("타겟 공격");
            playerAttackScr.AttackType = AttackType.LONGATTACK;
        }
    }
}
