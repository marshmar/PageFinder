using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTargetingVisualizer : MonoBehaviour, IListener
{
    #region Variables
    private Transform lineTargetingTrans;
    private Transform circleTargetingBGTrans;
    private Transform circleTargetingTrans;
    private Transform fanTargetingTrans;
    private FanShapeSprite fanShapeSpriteScr;

    [SerializeField] private GameObject lineTargetingObj;
    [SerializeField] private GameObject circleTargetingBG;
    [SerializeField] private GameObject circleTargetingObj;
    [SerializeField] private GameObject fanTargetingObj;

    #endregion

    #region Properties
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        if (lineTargetingObj.IsNull()) return;
        if (circleTargetingBG.IsNull()) return;
        if (circleTargetingObj.IsNull()) return;
        if (fanTargetingObj.IsNull()) return;

        // Set line transform
        lineTargetingTrans = lineTargetingObj.GetComponentSafe<Transform>();
        if (lineTargetingTrans.IsNull()) return;
        lineTargetingTrans.position = new Vector3(lineTargetingTrans.position.x, lineTargetingTrans.position.y + 0.11f, lineTargetingTrans.position.z);

        // Set circle targeting BG transform
        circleTargetingBGTrans = circleTargetingBG.GetComponentSafe<Transform>();
        if (circleTargetingBGTrans.IsNull()) return;
        circleTargetingBGTrans.position = new Vector3(circleTargetingBGTrans.position.x, circleTargetingBGTrans.position.y + 0.1f, circleTargetingBGTrans.position.z);

        // Set circle targeting transform
        circleTargetingTrans = circleTargetingObj.GetComponentSafe<Transform>();
        if (circleTargetingBGTrans.IsNull()) return;
        circleTargetingTrans.position = new Vector3(circleTargetingTrans.position.x, circleTargetingTrans.position.y + 0.1f, circleTargetingTrans.position.z);

        // Set fan targeting transform
        fanTargetingTrans = fanTargetingObj.GetComponentSafe<Transform>();
        fanShapeSpriteScr = fanTargetingObj.GetComponentInChildrenSafe<FanShapeSprite>();
        if (fanTargetingObj.IsNull() || fanShapeSpriteScr.IsNull()) return;
        fanTargetingTrans.position = new Vector3(fanTargetingTrans.position.x, fanTargetingTrans.position.y + 0.1f, fanTargetingTrans.position.z);

        OffAllTargetObjects();
    }

    public void Start()
    {
        AddListener();
    }

    private void OnDestroy()
    {
        RemoveListener();
    }
    #endregion

    #region Initialization
    #endregion

    #region Actions
    public void UnFixedLineTargeting(Vector3 direction, float targetingRange, float lineWidth, bool circleBGObj = true)
    {
        // show maximum targeting range
        circleTargetingBG.SetActive(true);
        circleTargetingBGTrans.GetChild(0).gameObject.SetActive(true);
        circleTargetingBGTrans.localScale = new Vector3(targetingRange * 2, 0, targetingRange * 2);

        lineTargetingObj.SetActive(true);
        lineTargetingTrans.GetChild(0).gameObject.SetActive(true);
        if (lineTargetingTrans.localScale.z >= targetingRange)
        {
            lineTargetingTrans.localScale = new Vector3(lineWidth, 0.0f, targetingRange);
        }
        else
        {
            float dist = Vector3.Distance(lineTargetingTrans.position, lineTargetingTrans.position + direction);
            float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            lineTargetingTrans.rotation = Quaternion.Euler(0, angle, 0);
            lineTargetingTrans.localScale = new Vector3(lineWidth, 0.0f, Mathf.Clamp(dist, 0.1f, targetingRange));
        }
    }


    public void FixedLineTargeting(Vector3 direction, float targetingRange, float lineWidth, bool circleBGObj = true)
    {
        circleTargetingBG.SetActive(true);
        circleTargetingBGTrans.localScale = new Vector3(targetingRange * 2, 0, targetingRange * 2);

        lineTargetingObj.SetActive(true);
        lineTargetingTrans.GetChild(0).gameObject.SetActive(true);
        float yAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        lineTargetingTrans.rotation = Quaternion.Euler(0, yAngle, 0);
        lineTargetingTrans.localScale = new Vector3(lineWidth, 0.0f, targetingRange);
    }

    public void FanTargeting(Vector3 direction, float targetingRange, float centerAngle)
    {
        fanTargetingObj.SetActive(true);
        fanTargetingTrans.GetChild(0).gameObject.SetActive(true);

        fanShapeSpriteScr.CreateFanShape(centerAngle, targetingRange);
        float yAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        fanTargetingTrans.rotation = Quaternion.Euler(0, yAngle, 0);
    }


    public void CircleTargeting(Vector3 skillSpawnPos, float skillDist, float skillRange)
    {
        circleTargetingObj.SetActive(true);
        circleTargetingBG.SetActive(true);

        circleTargetingTrans.GetChild(0).gameObject.SetActive(true);
        circleTargetingTrans.localScale = new Vector3(skillRange, 0.0f, skillRange);
        circleTargetingTrans.position = skillSpawnPos + new Vector3(0f, 0.1f, 0f);

        circleTargetingBGTrans.localScale = new Vector3(skillDist * 2, 0, skillDist * 2);
    }

    public void ShowMaximumRange(float maximumRange, float time = 0f)
    {
        StartCoroutine(ShowMaximumRangeCoroutine(maximumRange, time));
    }

    public IEnumerator ShowMaximumRangeCoroutine(float maximumRange, float time)
    {
        circleTargetingBG.SetActive(true);
        circleTargetingBG.transform.localScale = new Vector3(maximumRange * 2, 0, maximumRange * 2);

        yield return new WaitForSeconds(time);
        circleTargetingBG.SetActive(false);
    }
    #endregion

    #region Getter
    #endregion

    #region Setter
    #endregion

    #region Utilities
    public void OffAllTargetObjects()
    {
        lineTargetingObj.SetActive(false);
        circleTargetingBG.SetActive(false);
        circleTargetingObj.SetActive(false);
        fanTargetingObj.SetActive(false);
    }
    #endregion

    #region Events
    public void AddListener()
    {
        // Add event for mobile joystick
        //EventManager.Instance.AddListener(EVENT_TYPE.Joystick_Dragged, this);
        //EventManager.Instance.AddListener(EVENT_TYPE.Joystick_Short_Released, this);
        //EventManager.Instance.AddListener(EVENT_TYPE.Joystick_Long_Released, this);
        //EventManager.Instance.AddListener(EVENT_TYPE.Joystick_Canceled, this);
    }

    public void RemoveListener()
    {
        // Remove event for mobile joystick
        //EventManager.Instance.RemoveListener(EVENT_TYPE.Joystick_Dragged, this);
        //EventManager.Instance.RemoveListener(EVENT_TYPE.Joystick_Short_Released, this);
        //EventManager.Instance.RemoveListener(EVENT_TYPE.Joystick_Long_Released, this);
        //EventManager.Instance.RemoveListener(EVENT_TYPE.Joystick_Canceled, this);
    }

    public void OnEvent(EVENT_TYPE eventType, Component sender, object param)
    {
        // Event for mobile joystick
        /*        if(eventType == EVENT_TYPE.Joystick_Dragged)
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
                }*/
    }
    #endregion
}
