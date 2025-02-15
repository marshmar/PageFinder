using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class VirtualJoystick : MonoBehaviour, IVirtualJoystick
{
    protected Image imageBackground;
    protected Image imageController;
    protected Vector2 touchPosition;
    protected Vector3 direction;
    protected float touchStartTime;
    protected float touchEndTime;
    protected float touchDuration;
    protected float shortTouchThreshold;

    public virtual void SetImages()
    {
        imageBackground = DebugUtils.GetComponentWithErrorLogging<Image>(transform, "Image");
        imageController = DebugUtils.GetComponentWithErrorLogging<Image>(transform.GetChild(0), "Image");
    }
    // Start is called before the first frame update
    public virtual void Start()
    {
        SetImages();
    }

    public virtual void OnPointerDown(PointerEventData eventData) 
    {
        ResetImageAndPostion();
        touchStartTime = Time.time;
    }

    public virtual void OnDrag(PointerEventData eventData) 
    {
        Debug.Log("드래그 중");
        MoveImage(eventData, ref touchPosition);
        direction = new Vector3(touchPosition.x, 0f, touchPosition.y);

        EventManager.Instance.PostNotification(EVENT_TYPE.Joystick_Dragged, this, direction);
    }

    public virtual void OnPointerUp(PointerEventData eventData) 
    {
        ResetImageAndPostion();

        touchEndTime = Time.time;
        touchDuration = touchEndTime - touchStartTime;

        if (touchDuration > shortTouchThreshold)
            EventManager.Instance.PostNotification(EVENT_TYPE.Joystick_Long_Released, this);
        else
            EventManager.Instance.PostNotification(EVENT_TYPE.Joystick_Short_Released, this);
    }

    public virtual void MoveImage(PointerEventData eventData, ref Vector2 position)
    {
        // 조이스틱의 위치가 어디에 있든 동일한 값을 연산하기 위해
        // touchPosition의 위치 값은 이미지의 현재 위치를 기준으로
        // 얼마나 떨어져 있는지에 따라 다르게 나온다.
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            imageBackground.rectTransform, eventData.position, eventData.pressEventCamera, out position))
        {
            // touchPosition의 값을 정규화[0 ~ 1]
            // touchPosition을 이미지 크기로 나눔
            position.x = (position.x / imageBackground.rectTransform.sizeDelta.x);
            position.y = (position.y / imageBackground.rectTransform.sizeDelta.y);

            // touchPosition 값의 정규화 [-1 ~ 1]
            // 가상 조이스틱 배경 이미지 밖으로 터치가 나가게 되면 -1 ~ 1보다 큰 값이 나올 수 있다.
            // 이 때 normalized를 이용해 -1 ~ 1 사이의 값으로 정규화
            position = (position.magnitude > 1) ? position.normalized : position;

            // 가상 조이스틱 컨트롤러 이미지 이동 
            imageController.rectTransform.anchoredPosition = new Vector2(
                position.x * imageBackground.rectTransform.sizeDelta.x / 2,
                position.y * imageBackground.rectTransform.sizeDelta.y / 2);

        }
    }

    public virtual void ResetImageAndPostion()
    {
        // 터치 종료 시 이미지의 위치를 중앙으로 다시 옮긴다.
        imageController.rectTransform.anchoredPosition = Vector2.zero;
        // 다른 오브젝트에서 이동 방향으로 사용하기 때문에 이동 방향도 초기화
        touchPosition = Vector2.zero;
    }

    public float Horizontal()
    {
        return touchPosition.x;
    }

    public float Vertical()
    {
        return touchPosition.y;
    }
}
