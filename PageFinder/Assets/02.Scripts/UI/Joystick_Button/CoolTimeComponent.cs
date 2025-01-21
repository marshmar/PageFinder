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

    public bool IsAbleSkill { get => isAbleSkill; set => isAbleSkill = value; }
    public float CurrSkillCoolTime { get => currSkillCoolTime; set => currSkillCoolTime = value; }
    public float LeftSkillCoolTime { get => leftSkillCoolTime; set => leftSkillCoolTime = value; }

    private PlayerDashController playerDashController;
    private PlayerSkillController playerSkillController;

    private void Awake()
    {
        playerDashController = GetComponentInParent<PlayerDashController>();
        playerSkillController = GetComponentInParent<PlayerSkillController>();
    }
    private void Start()
    {
        isAbleSkill = true;
        coolTimeImage.enabled = false;

        EventManager.Instance.AddListener(EVENT_TYPE.Joystick_Short_Released, this);
        EventManager.Instance.AddListener(EVENT_TYPE.Joystick_Long_Released, this);
    }

    public void SetCoolTime(float settingCoolTime)
    {
        this.currSkillCoolTime = settingCoolTime;
    }

    public IEnumerator SkillCoolTime()
    {
        if (!isAbleSkill) yield break;
        coolTimeImage.enabled = true;
        leftSkillCoolTime = currSkillCoolTime;
        isAbleSkill = false;
        if (ShowCoolTimeText)
        {
            coolTimeText.enabled = true;
        }

        while (leftSkillCoolTime > 0.0f)
        {
            leftSkillCoolTime -= Time.deltaTime;
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
    }

    public void StartCoolDown()
    {
        Debug.Log("ƒ≈∏¿” Ω√¿€");
/*        if (coolTimeCoroutine != null)
        {
            StopCoroutine(coolTimeCoroutine);
        }*/
        coolTimeCoroutine = StartCoroutine(SkillCoolTime());
    }

    public void OnEvent(EVENT_TYPE eventType, Component sender, object param)
    {
        switch (eventType)
        {
            case EVENT_TYPE.Joystick_Short_Released:
            case EVENT_TYPE.Joystick_Long_Released:
                if(sender.name.Equals(this.name))
                    StartCoolDown();
                break;
        }
    }
}
