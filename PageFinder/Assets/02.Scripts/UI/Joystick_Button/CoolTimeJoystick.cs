using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class CoolTimeJoystick : VirtualJoystick
{
    [SerializeField] protected Image coolTimeImage;
    [SerializeField] protected Image joystickIcon;
    [SerializeField] protected Image cancelImage;
    [SerializeField] protected CoolTimeComponent coolTimeComponent;

    public float enabledBrightness = 255.0f;
    public float disabledBrightness = 180.0f;
    protected PlayerState playerState;

    [SerializeField]
    protected Sprite[] joystickIcons;

/*    public override void SetImages()
    {
        joystickImage = DebugUtils.GetComponentWithErrorLogging<Image>(transform, "Image");
        imageBackground = DebugUtils.GetComponentWithErrorLogging<Image>(transform.GetChild(0), "Image");
        imageController = DebugUtils.GetComponentWithErrorLogging<Image>(transform.GetChild(1), "Image");
        coolTimeImage = DebugUtils.GetComponentWithErrorLogging<Image>(transform.GetChild(2), "Image");
        coolTimeComponent = DebugUtils.GetComponentWithErrorLogging<CoolTimeComponent>(transform, "CoolTimeComponent");
    }*/

    public virtual void Awake()
    {
        //SetImages();
        coolTimeComponent = DebugUtils.GetComponentWithErrorLogging<CoolTimeComponent>(transform, "CoolTimeComponent");
        SetImageState(false);

        GameObject playerObj = GameObject.FindGameObjectWithTag("PLAYER");
        playerState = playerObj.GetComponent<PlayerState>();
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
        /* �ֽ�ǥ�� �߰��� �ڵ�
         *  �߻��� ���� : CoolTimeJoyStick 135��° �ٿ��� joystick�� Null�� ������.
         *  
         *  <����Ǵ� ���� ��ġ>
         *  VirtualJoyStick()::Start() : SetImage() 
            Player::Awake() : SetBasicStatus()
            ȣ�� ���� ������ ��

            <�ذ���>
            �Ʒ��� ���� �ʱ�ȭ �ٽ� ��
         */
/*         if (!joystickImage)
            SetImages();*/


        switch (inkType)
        {
            case InkType.Red:
                joystickIcon.sprite = joystickIcons[0];
                coolTimeImage.sprite = joystickIcons[0];
               
                break;
            case InkType.Green:
                joystickIcon.sprite = joystickIcons[1];
                coolTimeImage.sprite = joystickIcons[1];
                break;
            case InkType.Blue:
                joystickIcon.sprite = joystickIcons[2];
                coolTimeImage.sprite = joystickIcons[2];
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

        direction = new Vector3(touchPosition.x, 0f, touchPosition.y);

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

        // ��� ��ư
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

            // ���� �������� ��� ��ư ���� �ִ��� Ȯ���ϱ�
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
        if(joystickBackground != null)
            joystickBackground.enabled = value;

        if(joystickController != null)
            joystickController.enabled = value;

        if(cancelImage != null)
            cancelImage.enabled = value;
    }

    public void ChangeBrightnessJoystickImage(float alphaValue)
    {
        joystickIcon.color = new Color(joystickIcon.color.r, joystickIcon.color.g, joystickIcon.color.b, alphaValue / 255f);
        //coolTimeImage.color = new Color(cancelImage.color.r, cancelImage.color.g, cancelImage.color.b, alphaValue / 255f);
    }

    public virtual void Refresh() 
    {
        coolTimeComponent.Refresh();
    }
}
