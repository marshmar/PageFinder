using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CoolTimeComponent : MonoBehaviour
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

    public void Start()
    {
        isAbleSkill = true;
        coolTimeImage.enabled = false;
        
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
}
