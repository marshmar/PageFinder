using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTargetingVisualizer : MonoBehaviour, IListener
{
    #region Variables
    private Transform _lineTargetingTrans;
    private Transform _circleTargetingBGTrans;
    private Transform _circleTargetingTrans;
    private Transform _fanTargetingTrans;
    private FanShapeSprite _fanShapeSpriteScr;

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
        _lineTargetingTrans = lineTargetingObj.GetComponentSafe<Transform>();
        if (_lineTargetingTrans.IsNull()) return;
        _lineTargetingTrans.position = new Vector3(_lineTargetingTrans.position.x, _lineTargetingTrans.position.y + 0.11f, _lineTargetingTrans.position.z);

        // Set circle targeting BG transform
        _circleTargetingBGTrans = circleTargetingBG.GetComponentSafe<Transform>();
        if (_circleTargetingBGTrans.IsNull()) return;
        _circleTargetingBGTrans.position = new Vector3(_circleTargetingBGTrans.position.x, _circleTargetingBGTrans.position.y + 0.1f, _circleTargetingBGTrans.position.z);

        // Set circle targeting transform
        _circleTargetingTrans = circleTargetingObj.GetComponentSafe<Transform>();
        if (_circleTargetingBGTrans.IsNull()) return;
        _circleTargetingTrans.position = new Vector3(_circleTargetingTrans.position.x, _circleTargetingTrans.position.y + 0.1f, _circleTargetingTrans.position.z);

        // Set fan targeting transform
        _fanTargetingTrans = fanTargetingObj.GetComponentSafe<Transform>();
        _fanShapeSpriteScr = fanTargetingObj.GetComponentInChildrenSafe<FanShapeSprite>();
        if (fanTargetingObj.IsNull() || _fanShapeSpriteScr.IsNull()) return;
        _fanTargetingTrans.position = new Vector3(_fanTargetingTrans.position.x, _fanTargetingTrans.position.y + 0.1f, _fanTargetingTrans.position.z);

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
        _circleTargetingBGTrans.GetChild(0).gameObject.SetActive(true);
        _circleTargetingBGTrans.localScale = new Vector3(targetingRange * 2, 0, targetingRange * 2);

        lineTargetingObj.SetActive(true);
        _lineTargetingTrans.GetChild(0).gameObject.SetActive(true);
        if (_lineTargetingTrans.localScale.z >= targetingRange)
        {
            _lineTargetingTrans.localScale = new Vector3(lineWidth, 0.0f, targetingRange);
        }
        else
        {
            float dist = Vector3.Distance(_lineTargetingTrans.position, _lineTargetingTrans.position + direction);
            float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            _lineTargetingTrans.rotation = Quaternion.Euler(0, angle, 0);
            _lineTargetingTrans.localScale = new Vector3(lineWidth, 0.0f, Mathf.Clamp(dist, 0.1f, targetingRange));
        }
    }


    public void FixedLineTargeting(Vector3 direction, float targetingRange, float lineWidth, bool circleBGObj = true)
    {
        circleTargetingBG.SetActive(true);
        _circleTargetingBGTrans.localScale = new Vector3(targetingRange * 2, 0, targetingRange * 2);

        lineTargetingObj.SetActive(true);
        _lineTargetingTrans.GetChild(0).gameObject.SetActive(true);
        float yAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        _lineTargetingTrans.rotation = Quaternion.Euler(0, yAngle, 0);
        _lineTargetingTrans.localScale = new Vector3(lineWidth, 0.0f, targetingRange);
    }

    public void FanTargeting(Vector3 direction, float targetingRange, float centerAngle)
    {
        fanTargetingObj.SetActive(true);
        _fanTargetingTrans.GetChild(0).gameObject.SetActive(true);

        _fanShapeSpriteScr.CreateFanShape(centerAngle, targetingRange);
        float yAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        _fanTargetingTrans.rotation = Quaternion.Euler(0, yAngle, 0);
    }


    public void CircleTargeting(Vector3 skillSpawnPos, float skillDist, float skillRange)
    {
        circleTargetingObj.SetActive(true);
        circleTargetingBG.SetActive(true);

        _circleTargetingTrans.GetChild(0).gameObject.SetActive(true);
        _circleTargetingTrans.localScale = new Vector3(skillRange, 0.0f, skillRange);
        _circleTargetingTrans.position = skillSpawnPos + new Vector3(0f, 0.1f, 0f);

        _circleTargetingBGTrans.localScale = new Vector3(skillDist * 2, 0, skillDist * 2);
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
