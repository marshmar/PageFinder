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

    public bool IsAbleSkill { get => isAbleSkill; set => isAbleSkill = value; }
    public float CurrSkillCoolTime { get => currSkillCoolTime; set => currSkillCoolTime = value; }
    public float LeftSkillCoolTime { get => leftSkillCoolTime; set => leftSkillCoolTime = value; }

    public void Start()
    {
        isAbleSkill = true;        
    }
    public IEnumerator SkillCoolTime()
    {
        LeftSkillCoolTime = CurrSkillCoolTime;
        IsAbleSkill = false;
        coolTimeText.enabled = true;
        while (LeftSkillCoolTime > 0.0f)
        {
            LeftSkillCoolTime -= Time.deltaTime;
            coolTimeText.text = ((int)LeftSkillCoolTime + 1).ToString();
            coolTimeImage.fillAmount = LeftSkillCoolTime / CurrSkillCoolTime;


            yield return new WaitForFixedUpdate();
        }
        coolTimeText.enabled = false;
        IsAbleSkill = true;
    }
}
