using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CoolTimeComponent : MonoBehaviour
{
    public Image coolTimeImage;
    public TMP_Text coolTimeText;

    private float currSkillCoolTime;
    private float leftSkillCoolTime;
    private bool isAbleSkill;
    private Coroutine coolTimeCoroutine;

    public bool IsAbleSkill { get => isAbleSkill; set => isAbleSkill = value; }
    public float CurrSkillCoolTime { get => currSkillCoolTime; set => currSkillCoolTime = value; }
    public float LeftSkillCoolTime { get => leftSkillCoolTime; set => leftSkillCoolTime = value; }

    public void Start()
    {
        isAbleSkill = true;        
    }
    public IEnumerator SkillCoolTime()
    {
        if (!isAbleSkill) yield break;
        leftSkillCoolTime = currSkillCoolTime;
        isAbleSkill = false;
        coolTimeText.enabled = true;
        while (leftSkillCoolTime > 0.0f)
        {
            leftSkillCoolTime -= Time.deltaTime;
            //coolTimeText.text = ((int)LeftSkillCoolTime + 1).ToString();
            coolTimeText.text = Mathf.Ceil(leftSkillCoolTime).ToString();
            coolTimeImage.fillAmount = leftSkillCoolTime / currSkillCoolTime;


            yield return null;
        }
        coolTimeText.enabled = false;
        coolTimeImage.fillAmount = 0;
        isAbleSkill = true;
    }

    public void StartCoolDown()
    {
        if(coolTimeCoroutine != null)
        {
            StopCoroutine(coolTimeCoroutine);
        }
        coolTimeCoroutine = StartCoroutine(SkillCoolTime());
    }
}
