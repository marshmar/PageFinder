using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
public class SkillJoystick : MonoBehaviour, VirtualJoystick
{
    private Image imageBackground;
    private Image imageController;
    private Vector2 touchPosition;
    private Vector3 attackDir;
    private float touchStartTime;
    private float touchEndTime;
    private float touchDuration;
    private float shortSkillTouchDuration;

    private PlayerTarget playerTargetScr;
    private PlayerSkillController playerSkillControllerScr;
    private CoolTimeComponent coolTimeComponent;

    private void Awake()
    {
        imageBackground = DebugUtils.GetComponentWithErrorLogging<Image>(transform.GetChild(0), "Image");
        imageController = DebugUtils.GetComponentWithErrorLogging<Image>(transform.GetChild(1), "Image");
        coolTimeComponent = DebugUtils.GetComponentWithErrorLogging<CoolTimeComponent>(transform, "CoolTimeComponent");
    }

    private void Start()
    {
        imageBackground.enabled = false;
        imageController.enabled = false;
        shortSkillTouchDuration = 0.1f;

        GameObject playerObj = GameObject.FindGameObjectWithTag("PLAYER");

        if (!DebugUtils.CheckIsNullWithErrorLogging<GameObject>(playerObj, this.gameObject))
        {
            playerSkillControllerScr = DebugUtils.GetComponentWithErrorLogging<PlayerSkillController>(playerObj, "PlayerSkillController");

        }

        playerTargetScr = DebugUtils.GetComponentWithErrorLogging<PlayerTarget>(playerObj, "PlayerTarget");

        if (!DebugUtils.CheckIsNullWithErrorLogging<CoolTimeComponent>(coolTimeComponent))
        {
            if (playerSkillControllerScr == null)
            {
                Debug.Log("null!");
            }
            if (!DebugUtils.CheckIsNullWithErrorLogging<PlayerSkillController>(playerSkillControllerScr, this.gameObject))
            {
                coolTimeComponent.CurrSkillCoolTime = playerSkillControllerScr.CurrSkillData.skillCoolTime;
            }
        }

    }

    /// <summary>
    /// 조이스틱 입력 시작 시에 호출되는 함수
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerDown(PointerEventData eventData)
    {
        attackDir = Vector3.zero;
        touchStartTime = Time.time;
    }

    /// <summary>
    /// 조이스틱 드래그시에 호출되는 함수
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
        if (!DebugUtils.CheckIsNullWithErrorLogging<CoolTimeComponent>(coolTimeComponent, this.gameObject))
        {
            if (!coolTimeComponent.IsAbleSkill)
                return;
        }
        else
        {
            return;
        }

        imageBackground.enabled = true;
        imageController.enabled = true;
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


            attackDir = new Vector3(touchPosition.x, 0.1f, touchPosition.y);

            if (!DebugUtils.CheckIsNullWithErrorLogging<PlayerTarget>(playerTargetScr, this.gameObject))
            {
                if (!DebugUtils.CheckIsNullWithErrorLogging<PlayerSkillController>(playerSkillControllerScr, this.gameObject))
                {
                    if (playerSkillControllerScr.CurrSkillData.skillType == SkillTypes.FAN)
                    {
                        FanSkillData fanSkillData = playerSkillControllerScr.CurrSkillData as FanSkillData;
                        playerTargetScr.FanTargeting(attackDir, fanSkillData.skillRange, fanSkillData.fanDegree);
                    }
                }
            }

        }
    }

    /// <summary>
    /// 조이스틱 터치 종료시 호출되는 함수
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerUp(PointerEventData eventData)
    {
        if (!DebugUtils.CheckIsNullWithErrorLogging<CoolTimeComponent>(coolTimeComponent))
        {
            if (!coolTimeComponent.IsAbleSkill)
                return;
        }
        else
        {
            return;
        }

        // 터치 종료 시 이미지의 위치를 중앙으로 다시 옮긴다.
        imageController.rectTransform.anchoredPosition = Vector2.zero;
        // 다른 오브젝트에서 이동 방향으로 사용하기 때문에 이동 방향도 초기화
        touchPosition = Vector2.zero;

        // 터치 시간 측정
        touchEndTime = Time.time;
        touchDuration = touchEndTime - touchStartTime;

        if (!DebugUtils.CheckIsNullWithErrorLogging<PlayerTarget>(playerTargetScr, this.gameObject))
        {
            playerTargetScr.OffAllTargetObjects();       
        }

        if(!DebugUtils.CheckIsNullWithErrorLogging<PlayerSkillController>(playerSkillControllerScr, this.gameObject))
        {
            if (touchDuration <= shortSkillTouchDuration)
            {
                if (playerSkillControllerScr.InstantiateSkill())
                    coolTimeComponent.StartCoolDown();
            }
            else
            {
                if (playerSkillControllerScr.InstantiateSkill(attackDir))
                    coolTimeComponent.StartCoolDown();
            }
        }
        
        imageBackground.enabled = false;
        imageController.enabled = false;
    }


}
