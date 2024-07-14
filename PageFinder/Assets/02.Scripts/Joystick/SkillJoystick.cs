using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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

    public string skillName;
    private PlayerSkillController playerSkillControllerScr;
    private SkillManager<GameObject> skillObjectManager;
    private SkillManager<SkillData> skillDataManager;

    private void Awake()
    {
        imageBackground = transform.GetChild(0).GetComponent<Image>();
        imageController = transform.GetChild(1).GetComponent<Image>();
        skillObjectManager = SkillManager<GameObject>.Instance;
        skillDataManager = SkillManager<SkillData>.Instance;
    }

    private void Start()
    {
        imageBackground.enabled = false;
        imageController.enabled = false;
        shortSkillTouchDuration = 0.1f;
        playerSkillControllerScr = GameObject.FindGameObjectWithTag("PLAYER").GetComponent<PlayerSkillController>();

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
        if (skillObjectManager[skillName] == null 
            || skillDataManager[skillName].skillType == SkillTypes.STROKE) 
            return;

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


            playerSkillControllerScr.OnTargeting(attackDir, skillDataManager[skillName].skillDist);
        }
    }

    /// <summary>
    /// 조이스틱 터치 종료시 호출되는 함수
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerUp(PointerEventData eventData)
    {
        // 터치 종료 시 이미지의 위치를 중앙으로 다시 옮긴다.
        imageController.rectTransform.anchoredPosition = Vector2.zero;
        // 다른 오브젝트에서 이동 방향으로 사용하기 때문에 이동 방향도 초기화
        touchPosition = Vector2.zero;

        // 터치 시간 측정
        touchEndTime = Time.time;
        touchDuration = touchEndTime - touchStartTime;

        if (touchDuration <= shortSkillTouchDuration)
        {
            playerSkillControllerScr.InstantiateSkill(skillName);
        }
        else
        {
            playerSkillControllerScr.InstantiateSkill(skillName, attackDir);
        }
        playerSkillControllerScr.SetTargetObject(false);
        imageBackground.enabled = false;
        imageController.enabled = false;
    }

    /// <summary>
    /// 장착된 스킬을 변경하는 함수
    /// </summary>
    /// <param name="skillName">변경할 스킬 이름</param>
    public void ChangeSkill(string skillName)
    {
        this.skillName = skillName;
    }
}
