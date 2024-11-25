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
    protected Player playerScr;
    protected PlayerTarget playerTargetScr;
    protected Vector3 direction;
    protected PlayerAttackController playerAttackControllerScr;
    protected PlayerInkMagicController playerInkMagicControllerScr;
    private float iconBrightness = 180.0f;
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

    public void Awake()
    {
        SetImages();
        SetImageState(false);
    }
    public override void Start()
    {


        FindPlayerObjectAndSetGameObject();
    }

    public void StartCoolDown()
    {
        if (!DebugUtils.CheckIsNullWithErrorLogging<CoolTimeComponent>(coolTimeComponent, this.gameObject))
        {
            coolTimeComponent.StartCoolDown();
        }
    }

    public virtual void SetImageState(bool value)
    {
        imageBackground.enabled = value;
        imageController.enabled = value;
        cancelImage.enabled = value;
    }

    public virtual void FindPlayerObjectAndSetGameObject()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("PLAYER");

        if (!DebugUtils.CheckIsNullWithErrorLogging<GameObject>(playerObj, this.gameObject))
        {
            playerScr = DebugUtils.GetComponentWithErrorLogging<Player>(playerObj, "Player");
            playerTargetScr = DebugUtils.GetComponentWithErrorLogging<PlayerTarget>(playerObj, "PlayerTarget");
            playerAttackControllerScr = DebugUtils.GetComponentWithErrorLogging<PlayerAttackController>(playerObj, "PlayerAttackController");
            playerInkMagicControllerScr = DebugUtils.GetComponentWithErrorLogging<PlayerInkMagicController>(playerObj, "PlayerInkMagicController");
        }
    }

    public virtual void SetCoolTime(float value)
    {
        if (!DebugUtils.CheckIsNullWithErrorLogging<CoolTimeComponent>(coolTimeComponent, this.gameObject))
        {
            coolTimeComponent.CurrSkillCoolTime = value;
        }
    }

    public virtual bool CheckInkGaugeAndSetImage(float value)
    {
        if (playerScr.CurrInk < value)
        {
            joystickImage.color = new Color(iconBrightness / 255f, iconBrightness / 255f, iconBrightness / 255f);
            return false;
        }
        else
        {
            joystickImage.color = Color.white;
            return true;
        }
    }

    public virtual bool CheckCancel(PointerEventData eventData)
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
                HighlightButtonOnTouch(255.0f);
                return true;
            }
            HighlightButtonOnTouch(150.0f);
        }
        return false;
    }

    public void HighlightButtonOnTouch(float alphaValue)
    {
        cancelImage.color = new Color(cancelImage.color.r, cancelImage.color.g, cancelImage.color.b, alphaValue/ 255f);
    }

    public void OffTargetObject()
    {
        if (!DebugUtils.CheckIsNullWithErrorLogging<PlayerTarget>(playerTargetScr, this.gameObject))
        {
            playerTargetScr.OffAllTargetObjects();
        }
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
}
