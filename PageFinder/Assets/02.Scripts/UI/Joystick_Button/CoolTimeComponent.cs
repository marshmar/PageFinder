using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CoolTimeComponent : MonoBehaviour, IListener
{
    public Image coolTimeImage;
    public TMP_Text coolTimeText;
    public bool ShowCoolTimeText;

    private float currSkillCoolTime;
    private float leftSkillCoolTime;
    private bool isAbleSkill;
    private Coroutine coolTimeCoroutine;
    private float elapsedTime;

    public bool IsAbleSkill { get => isAbleSkill; set => isAbleSkill = value; }
    public float CurrSkillCoolTime { get => currSkillCoolTime; set => currSkillCoolTime = value; }
    public float LeftSkillCoolTime { get => leftSkillCoolTime; set => leftSkillCoolTime = value; }

    private void Awake()
    {
    }
    private void Start()
    {
        isAbleSkill = true;
        coolTimeImage.enabled = false;

        EventManager.Instance.AddListener(EVENT_TYPE.Joystick_Short_Released, this);
        EventManager.Instance.AddListener(EVENT_TYPE.Joystick_Long_Released, this);
        EventManager.Instance.AddListener(EVENT_TYPE.Restart_CoolTime, this);
        EventManager.Instance.AddListener(EVENT_TYPE.Reset_CoolTime, this);
        EventManager.Instance.AddListener(EVENT_TYPE.Pause_CoolTime, this);
    }

    public void SetCoolTime(float settingCoolTime)
    {
        this.currSkillCoolTime = settingCoolTime;
    }

    public IEnumerator SkillCoolTime(float startTime)
    {
        if (!isAbleSkill && (startTime <= 0)) yield break;
        coolTimeImage.enabled = true;
        leftSkillCoolTime = currSkillCoolTime;
        leftSkillCoolTime -= startTime;
        isAbleSkill = false;
        if (ShowCoolTimeText)
        {
            coolTimeText.enabled = true;
        }

        while (leftSkillCoolTime > 0.0f)
        {
            leftSkillCoolTime -= Time.deltaTime;
            elapsedTime += Time.deltaTime;

            //coolTimeText.text = ((int)LeftSkillCoolTime + 1).ToString();
            if (ShowCoolTimeText)
            {
                coolTimeText.text = Mathf.Ceil(leftSkillCoolTime).ToString();
            }

            coolTimeImage.fillAmount = leftSkillCoolTime / currSkillCoolTime;


            yield return null;
        }
        if (ShowCoolTimeText)
        {
            coolTimeText.enabled = false;
        }

        coolTimeImage.fillAmount = 0;
        isAbleSkill = true;
        coolTimeImage.enabled = false;
        elapsedTime = 0f;
    }

    public void StartCoolDown()
    {
/*        if (coolTimeCoroutine != null)
        {
            StopCoroutine(coolTimeCoroutine);
        }*/
        coolTimeCoroutine = StartCoroutine(SkillCoolTime(0f));
    }

    public void Refresh()
    {
        StopAllCoroutines();
        coolTimeImage.fillAmount = 0;
        isAbleSkill = true;
        coolTimeImage.enabled = false;
        elapsedTime = 0f;
        leftSkillCoolTime = 0f;
        if (ShowCoolTimeText)
        {
            coolTimeText.text = Mathf.Ceil(leftSkillCoolTime).ToString();
            coolTimeText.enabled = false;
        }
    }

    public void OnEvent(EVENT_TYPE eventType, Component sender, object param)
    {
        switch (eventType)
        {
            case EVENT_TYPE.Joystick_Short_Released:
            case EVENT_TYPE.Joystick_Long_Released:
                if(sender.name.Equals(this.name) && !sender.name.Equals(PlayerUI.playerSkillJoystickName))
                    StartCoolDown();
                break;
            case EVENT_TYPE.Reset_CoolTime:
                if (coolTimeCoroutine == null) break;
                StopCoroutine(coolTimeCoroutine);
                coolTimeImage.fillAmount = 0;
                isAbleSkill = true;
                if (ShowCoolTimeText)
                {
                    coolTimeText.text = Mathf.Ceil(leftSkillCoolTime).ToString();
                    coolTimeText.enabled = false;
                }
                coolTimeImage.enabled = false;
                elapsedTime = 0f;
                break;
            case EVENT_TYPE.Pause_CoolTime:
                if (coolTimeCoroutine is not null)
                    StopCoroutine(coolTimeCoroutine);
                break;
            case EVENT_TYPE.Restart_CoolTime:
                if (elapsedTime > 0)
                {
                    Debug.Log(this.gameObject.name + "s restart CoolTime");
                    StartCoroutine(SkillCoolTime(elapsedTime));
                }

                break;
        }
    }
}
