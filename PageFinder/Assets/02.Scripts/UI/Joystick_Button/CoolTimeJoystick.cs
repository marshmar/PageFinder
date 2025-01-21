using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CoolTimeJoystick : VirtualJoystick
{
    
    protected Image coolTimeImage;
    protected Image joystickImage;
    [SerializeField]
    protected Image cancelImage;
    protected CoolTimeComponent coolTimeComponent;

    public float enabledBrightness = 255.0f;
    public float disabledBrightness = 180.0f;
    protected PlayerState playerState;

    [SerializeField]
    protected Sprite[] backgroundImages;

    public override void SetImages()
    {
        joystickImage = DebugUtils.GetComponentWithErrorLogging<Image>(transform, "Image");
        imageBackground = DebugUtils.GetComponentWithErrorLogging<Image>(transform.GetChild(0), "Image");
        imageController = DebugUtils.GetComponentWithErrorLogging<Image>(transform.GetChild(1), "Image");
        coolTimeImage = DebugUtils.GetComponentWithErrorLogging<Image>(transform.GetChild(2), "Image");
        coolTimeComponent = DebugUtils.GetComponentWithErrorLogging<CoolTimeComponent>(transform, "CoolTimeComponent");
    }

    public virtual void Awake()
    {
        SetImages();
        SetImageState(false);

        playerState = GetComponentInParent<PlayerState>();
    }

    public override void Start()
    {
        shortTouchThreshold = 0.1f;
    }

    public void CheckInkGaugeAndSetImage(float skillCost)
    {
        if (playerState.CurInk < skillCost)
            ChangeBrightnessJoystickImage(disabledBrightness);
        else
            ChangeBrightnessJoystickImage(enabledBrightness);
    }

    public void SetJoystickImage(InkType inkType)
    {
        switch (inkType)
        {
            case InkType.RED:
                joystickImage.sprite = backgroundImages[0];
                coolTimeImage.sprite = backgroundImages[0];
                break;
            case InkType.GREEN:
                joystickImage.sprite = backgroundImages[1];
                coolTimeImage.sprite = backgroundImages[1];
                break;
            case InkType.BLUE:
                joystickImage.sprite = backgroundImages[2];
                coolTimeImage.sprite = backgroundImages[2];
                break;
        }
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (!coolTimeComponent.IsAbleSkill) return;

        base.OnPointerDown(eventData);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        if (!coolTimeComponent.IsAbleSkill) return;

        SetImageState(true);
        if (CheckPositionOnCancelButton(eventData))
            ChangeBrightnessCancelButton(enabledBrightness);
        else
            ChangeBrightnessCancelButton(disabledBrightness);
        base.OnDrag(eventData);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (!coolTimeComponent.IsAbleSkill) return;

        ResetImageAndPostion();
        if (CheckPositionOnCancelButton(eventData)) 
        {
            ChangeBrightnessCancelButton(disabledBrightness);
            SetImageState(false);
            EventManager.Instance.PostNotification(EVENT_TYPE.Joystick_Canceled, this);
            return;
        }

        touchEndTime = Time.time;
        touchDuration = touchEndTime - touchStartTime;

        if (touchDuration > shortTouchThreshold)
            EventManager.Instance.PostNotification(EVENT_TYPE.Joystick_Long_Released, this, direction);
        else
            EventManager.Instance.PostNotification(EVENT_TYPE.Joystick_Short_Released, this, direction);

        SetImageState(false);
    }

    public virtual bool CheckPositionOnCancelButton(PointerEventData eventData)
    {
        Vector2 localPostion = Vector2.zero;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            cancelImage.rectTransform, eventData.position, eventData.pressEventCamera, out localPostion))
        {
            float halfWidth = cancelImage.rectTransform.sizeDelta.x / 2;
            float halfHeight = cancelImage.rectTransform.sizeDelta.y / 2;

            // 현재 포지션이 취소 버튼 위에 있는지 확인하기
            if ((localPostion.x >= -halfWidth && localPostion.x <= halfWidth)
                && (localPostion.y >= -halfHeight && localPostion.y <= halfHeight)
                )
            {
                return true;
            }
        }
        return false;
    }

    public void ChangeBrightnessCancelButton(float alphaValue)
    {
        cancelImage.color = new Color(cancelImage.color.r, cancelImage.color.g, cancelImage.color.b, alphaValue / 255f);
    }

    public virtual void SetImageState(bool value)
    {
        imageBackground.enabled = value;
        imageController.enabled = value;
        cancelImage.enabled = value;
    }

    public void ChangeBrightnessJoystickImage(float alphaValue)
    {
        joystickImage.color = new Color(joystickImage.color.r, joystickImage.color.g, joystickImage.color.b, alphaValue / 255f);
        //coolTimeImage.color = new Color(cancelImage.color.r, cancelImage.color.g, cancelImage.color.b, alphaValue / 255f);
    }
}
