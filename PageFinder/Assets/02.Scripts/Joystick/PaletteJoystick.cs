using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class PaletteJoystick : MonoBehaviour, VirtualJoystick
{
    private Image imageBackground;
    private Image imageController;
    private Vector2 touchPosition;

    //private PaletteUIManager paletteUIManager;
    private void Awake()
    {
        imageBackground = GetComponent<Image>();
        imageController = transform.GetChild(2).GetComponent<Image>();
       // paletteUIManager = GameObject.Find("UIManager").GetComponent<PaletteUIManager>();
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // 이미지 크기 조절
        imageBackground.transform.localScale = new Vector3(4f, 4f, 1);
        imageController.transform.localScale = new Vector3(0.3f, 0.3f, 1);

        Debug.Log("PointerDown");
        //paletteUIManager.ChangePaletteObjectsActiveState(true);
    }


    public void OnDrag(PointerEventData eventData)
    {
        touchPosition = Vector2.zero;

        // 조이스틱의 위치가 어디에 있든 동일한 값을 연산하기 위해
        // touchPosition의 위치 값은 이미지의 현재 위치를 기준으로
        // 얼마나 떨어져 있는지에 따라 다르게 나온다.
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            imageBackground.rectTransform, eventData.position, eventData.pressEventCamera, out touchPosition))
        {
            // touchPosition의 값을 정규화[0 ~ 1]
            // touchPosition을 이미지 크기로 나눔
            touchPosition.x = (touchPosition.x / imageBackground.rectTransform.sizeDelta.x);
            touchPosition.y = (touchPosition.y / imageBackground.rectTransform.sizeDelta.y);

            // touchPosition 값의 정규화 [-1 ~ 1]
            // 가상 조이스틱 배경 이미지 밖으로 터치가 나가게 되면 -1 ~ 1보다 큰 값이 나올 수 있다.
            // 이 때 normalized를 이용해 -1 ~ 1 사이의 값으로 정규화
            touchPosition = (touchPosition.magnitude > 1) ? touchPosition.normalized : touchPosition;

            // 가상 조이스틱 컨트롤러 이미지 이동 
            imageController.rectTransform.anchoredPosition = new Vector2(
                touchPosition.x * imageBackground.rectTransform.sizeDelta.x / 2,
                touchPosition.y * imageBackground.rectTransform.sizeDelta.y / 2);

            //paletteUIManager.ChangePaletteObjectsColorTransparency(VectorToRadian(touchPosition));
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        double rot = VectorToRadian(touchPosition);

        // 조이스틱을 놓은 위치의 색깔로 현재 색깔 변경
        //paletteUIManager.ChangeCurrentColor(rot);

        // 투명도 초기화
        //paletteUIManager.ChangePaletteObjectsColorTransparency();

        // 터치 종료 시 이미지의 위치를 중앙으로 다시 옮긴다.
        imageController.rectTransform.anchoredPosition = Vector2.zero;
        // 다른 오브젝트에서 이동 방향으로 사용하기 때문에 이동 방향도 초기화
        touchPosition = Vector2.zero;

        // 이미지 크기 조절
        imageBackground.transform.localScale = new Vector3(1.4f, 1.4f, 1);
        imageController.transform.localScale = new Vector3(0.5f, 0.5f, 1);

        //paletteUIManager.ChangePaletteObjectsActiveState(false);
    }

    /// <summary>
    /// 원의 (0,1)을 기준으로 입력한 벡터 사이의 각을 반환한다.
    /// </summary>
    /// <param name="to"></param>
    /// <returns></returns>
    double VectorToRadian(Vector2 to)
    {
        return Quaternion.FromToRotation(to, Vector3.up).eulerAngles.z;
    }
}
