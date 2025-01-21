using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTarget : MonoBehaviour, IListener
{
    public GameObject lineObject;
    public GameObject circleBGObject;
    public GameObject circleObject;
    public GameObject fanObject;

    private Transform lineTransform;
    private Transform circleBGTransform;
    private Transform circleTransform;
    private Transform fanTransform;

    FanShapeSprite fanShapeSpriteScr;
    private PlayerDashController playerDashController;
    private PlayerSkillController playerSkillController;
    private void Awake()
    {
        playerDashController = DebugUtils.GetComponentWithErrorLogging<PlayerDashController>(this.gameObject, "PlayerDashController");
        playerSkillController = DebugUtils.GetComponentWithErrorLogging<PlayerSkillController>(this.gameObject, "PlayerSkillController");
    }
    
    public void Start()
    {
        if (lineObject)
        {
            lineTransform = lineObject.GetComponent<Transform>();
            lineTransform.position = new Vector3(lineTransform.position.x, lineTransform.position.y + 0.11f, lineTransform.position.z);
            lineObject.SetActive(false);
        }
        if (circleBGObject)
        {
            circleBGTransform = circleBGObject.GetComponent<Transform>();
            circleBGTransform.position = new Vector3(circleBGTransform.position.x, circleBGTransform.position.y + 0.1f, circleBGTransform.position.z);
            circleBGObject.SetActive(false);
        }
        if (circleObject)
        {
            circleTransform = circleObject.GetComponent<Transform>();
            circleTransform.position = new Vector3(circleTransform.position.x, circleTransform.position.y + 0.1f, circleTransform.position.z);
            circleObject.SetActive(false);
        }
        if (fanObject)
        {
            fanTransform = fanObject.GetComponent<Transform>();
            fanShapeSpriteScr = fanObject.GetComponentInChildren<FanShapeSprite>();
            fanTransform.position = new Vector3(fanTransform.position.x, fanTransform.position.y + 0.1f, fanTransform.position.z);
            fanObject.SetActive(false);
        }

        EventManager.Instance.AddListener(EVENT_TYPE.Joystick_Dragged, this);
        EventManager.Instance.AddListener(EVENT_TYPE.Joystick_Short_Released, this);
        EventManager.Instance.AddListener(EVENT_TYPE.Joystick_Long_Released, this);
        EventManager.Instance.AddListener(EVENT_TYPE.Joystick_Canceled, this);
    }

    public void OffAllTargetObjects()
    {
        lineObject.SetActive(false);
        circleBGObject.SetActive(false);
        circleObject.SetActive(false);
        fanObject.SetActive(false);
    }
    public void UnFixedLineTargeting(Vector3 direction, float targetingRange, float lineWidth, bool circleBGObj = true)
    {
        if (lineTransform == null || circleBGTransform == null)
        {
            Debug.LogError("Line Transform or Circle Transform is null");
            return;
        }
        circleBGObject.SetActive(true);
        circleBGTransform.GetChild(0).gameObject.SetActive(true);
        circleBGTransform.localScale = new Vector3(targetingRange*2, 0, targetingRange*2);

        lineObject.SetActive(true);
        lineTransform.GetChild(0).gameObject.SetActive(true);
        if (lineTransform.localScale.z >= targetingRange)
        {
            lineTransform.localScale = new Vector3(lineWidth, 0.0f, targetingRange);
        }
        else
        {
            float dist = Vector3.Distance(lineTransform.position, lineTransform.position + direction);
            float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            lineTransform.rotation = Quaternion.Euler(0, angle, 0);
            lineTransform.localScale = new Vector3(lineWidth, 0.0f, Mathf.Clamp(dist, 0.1f, targetingRange));
        }
    }
    public void FixedLineTargeting(Vector3 direction, float targetingRange, float lineWidth, bool circleBGObj = true)
    {
        if (lineTransform == null || circleBGTransform == null)
        {
            Debug.LogError("Line Transform or Circle Transform is null");
            return;
        }
        circleBGObject.SetActive(true);
        circleBGTransform.localScale = new Vector3(targetingRange*2, 0, targetingRange*2);

        lineObject.SetActive(true);
        lineTransform.GetChild(0).gameObject.SetActive(true);
        float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        lineTransform.rotation = Quaternion.Euler(0, angle, 0);
        lineTransform.localScale = new Vector3(lineWidth, 0.0f, targetingRange);
    }

    public void FanTargeting(Vector3 direction, float targetingRange, float centerAngle)
    {
        if(fanObject == null || fanTransform == null)
        {
            Debug.LogError("Fan Transform or Fan Transform is null");
            return;
        }
        fanObject.SetActive(true);
        fanTransform.GetChild(0).gameObject.SetActive(true);
        if (fanShapeSpriteScr == null)
        {
            Debug.LogError("FanShapeSpriteScr is null");
            return;
        }

        fanShapeSpriteScr.Angle = centerAngle;
        fanShapeSpriteScr.Radius = targetingRange;
        fanShapeSpriteScr.CreateFanShape();
        float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        fanTransform.rotation = Quaternion.Euler(0, angle, 0);
    }

    public void CircleRangeOn(float size, float time)
    {
        StartCoroutine(CircleRangeOnCoroutine(size, time));
    }

    public IEnumerator CircleRangeOnCoroutine(float size, float time)
    {
        circleBGObject.SetActive(true);
        circleBGObject.transform.localScale = new Vector3(size * 2, 0, size * 2);

        yield return new WaitForSeconds(time);
        circleBGObject.SetActive(false);
    }

    public void OnEvent(EVENT_TYPE eventType, Component sender, object param)
    {
        if(eventType == EVENT_TYPE.Joystick_Dragged)
        {
            Vector3 direction = (Vector3)param;
            switch (sender.name)
            {
                case PlayerUI.playerDashJoystickName:
                    FixedLineTargeting(direction, playerDashController.DashPower, playerDashController.DashWidth);
                    break;
                case PlayerUI.playerSkillJoystickName:
                    FanSkillData skillData = playerSkillController.CurrSkillData as FanSkillData;
                    FanTargeting(direction, skillData.skillRange, skillData.fanDegree);
                    break;
                
            }
        }
        else if(eventType == EVENT_TYPE.Joystick_Long_Released || eventType == EVENT_TYPE.Joystick_Short_Released 
            || eventType == EVENT_TYPE.Joystick_Canceled)
        {
            OffAllTargetObjects();
        }
    }
}
