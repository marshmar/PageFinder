using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
public class SkillJoystick : CoolTimeJoystick
{

    private PlayerSkillController playerSkillControllerScr;

    private void Update()
    {
        CheckInkGaugeAndSetImage(playerSkillControllerScr.CurrSkillData.skillCost);
    }

    public override void Start()
    {
        SetImages();
        SetImageState(false);
        shortTouchDuration = 0.1f;
        FindPlayerObjectAndSetGameObject();

        GameObject playerObj = GameObject.FindGameObjectWithTag("PLAYER");

        
        if (!DebugUtils.CheckIsNullWithErrorLogging<GameObject>(playerObj, this.gameObject))
        {
            playerSkillControllerScr = DebugUtils.GetComponentWithErrorLogging<PlayerSkillController>(playerObj, "PlayerSkillController");
            //SetCoolTime(playerSkillControllerScr.CurrSkillData.skillCoolTime);
        }

    }

    /// <summary>
    /// 조이스틱 입력 시작 시에 호출되는 함수
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnPointerDown(PointerEventData eventData)
    {
        if(!DebugUtils.CheckIsNullWithErrorLogging<Player>(playerScr, this.gameObject))
        {
            if(!CheckInkGaugeAndSetImage(playerSkillControllerScr.CurrSkillData.skillCost))
            {
                return;
            }
        }
        if (playerAttackControllerScr.IsAttacking || playerInkMagicControllerScr.IsUsingInkMagic) return;
        direction = Vector3.zero;
        touchStartTime = Time.time;
    }

    /// <summary>
    /// 조이스틱 드래그시에 호출되는 함수
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnDrag(PointerEventData eventData)
    {
        CheckCancel(eventData);
        if (!DebugUtils.CheckIsNullWithErrorLogging<CoolTimeComponent>(coolTimeComponent, this.gameObject))
        {
            if (!coolTimeComponent.IsAbleSkill)
                return;
        }
        else
        {
            return;
        }

        if(!DebugUtils.CheckIsNullWithErrorLogging<Player>(playerScr, this.gameObject))
        {
            if(!CheckInkGaugeAndSetImage(playerSkillControllerScr.CurrSkillData.skillCost))
            {
                return;
            }
        }
        else
        {
            return;
        }
        if (playerAttackControllerScr.IsAttacking || playerInkMagicControllerScr.IsUsingInkMagic) return;

        SetImageState(true);
        touchPosition = Vector2.zero;
        MoveImage(eventData, ref touchPosition);

        direction = new Vector3(touchPosition.x, 0.1f, touchPosition.y);

        if (!DebugUtils.CheckIsNullWithErrorLogging<PlayerTarget>(playerTargetScr, this.gameObject))
        {
            if (!DebugUtils.CheckIsNullWithErrorLogging<PlayerSkillController>(playerSkillControllerScr, this.gameObject))
            {
                if (playerSkillControllerScr.CurrSkillData.skillType == SkillTypes.FAN)
                {
                    FanSkillData fanSkillData = playerSkillControllerScr.CurrSkillData as FanSkillData;
                    playerTargetScr.FanTargeting(direction, fanSkillData.skillRange, fanSkillData.fanDegree);
                }
            }
        }
    }

    /// <summary>
    /// 조이스틱 터치 종료시 호출되는 함수
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnPointerUp(PointerEventData eventData)
    {
        if (playerAttackControllerScr.IsAttacking || playerInkMagicControllerScr.IsUsingInkMagic) return;

        if (!DebugUtils.CheckIsNullWithErrorLogging<CoolTimeComponent>(coolTimeComponent))
        {
            if (!coolTimeComponent.IsAbleSkill)
                return;
        }
        else
        {
            return;
        }

        if (!DebugUtils.CheckIsNullWithErrorLogging<Player>(playerScr, this.gameObject))
        {
            if (!CheckInkGaugeAndSetImage(playerSkillControllerScr.CurrSkillData.skillCost))
            {
                return;
            }
        }
        else
        {
            return;
        }

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

        OffTargetObject();

        if (!DebugUtils.CheckIsNullWithErrorLogging<PlayerSkillController>(playerSkillControllerScr, this.gameObject))
        {
            if (touchDuration <= shortTouchDuration || direction == Vector3.zero)
            {
                if (playerSkillControllerScr.InstantiateSkill())
                    coolTimeComponent.StartCoolDown();
            }
            else
            {
                if (playerSkillControllerScr.InstantiateSkill(direction))
                    coolTimeComponent.StartCoolDown();
            }
        }

        SetImageState(false);
    }

}
