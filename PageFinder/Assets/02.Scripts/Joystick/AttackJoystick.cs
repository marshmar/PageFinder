using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class AttackJoystick : MonoBehaviour, VirtualJoystick
{
    private Image imageBackground;
    private Image imageController;
    private Vector2 touchPosition;
    private Vector3 attackDir;
    private float touchStartTime;
    private float touchEndTime;
    private float touchDuration;
    private float shortAttackTouchDuration;
    
    private PlayerAttackController playerAttackScr;
    private void Awake()
    {
        imageBackground = GetComponent<Image>();
        imageController = transform.GetChild(0).GetComponent<Image>();
    }

    private void Start()
    {
        shortAttackTouchDuration = 0.1f;
        playerAttackScr = GameObject.FindGameObjectWithTag("PLAYER").GetComponent<PlayerAttackController>();
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        touchStartTime = Time.time;
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

            if(playerAttackScr == null)
            {
                Debug.LogError("playerAttackScr 객체가 없습니다.");
                return;
            }
            attackDir = new Vector3(touchPosition.x, 0, touchPosition.y);
            
            playerAttackScr.OnTargeting(attackDir, playerAttackScr.AttackRange);
        }
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        // 터치 종료 시 이미지의 위치를 중앙으로 다시 옮긴다.
        imageController.rectTransform.anchoredPosition = Vector2.zero;
        // 다른 오브젝트에서 이동 방향으로 사용하기 때문에 이동 방향도 초기화
        touchPosition = Vector2.zero;

        // 터치 시간 측정
        touchEndTime = Time.time;
        touchDuration = touchEndTime - touchStartTime;

        if (touchDuration <= shortAttackTouchDuration)
        {
            Debug.Log("짧은 공격");
            playerAttackScr.AttackType = AttackType.SHORTATTCK;
        }
        else
        {
            Debug.Log("타겟 공격");
            playerAttackScr.AttackType = AttackType.LONGATTACK;
        }
        StartCoroutine(playerAttackScr.Attack());
    }
}
