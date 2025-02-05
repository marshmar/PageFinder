using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class PlayerUI : MonoBehaviour
{
    public const string playerDashJoystickName = "Player_UI_OP_Dash";
    public const string playerSkillJoystickName = "Player_UI_OP_Skill";

    [Header("StatusBar")]
    [SerializeField] private SliderBar hpBar;
    [SerializeField] private SliderBar inkBar;
    [SerializeField] private ShieldBar shieldBar;
 
    [SerializeField] private TMP_Text hpBarText;
    [SerializeField] private PlayerDamageIndicator damageIndicator;

    [Header("Joystick Sprites")]
    [SerializeField]
    private Sprite[] attackTypeImages;

    [Header("Joystick Objects")]
    [SerializeField] private Image basicAttackInkTypeImage;
    [SerializeField] private SkillJoystick skillJoystick;
    [SerializeField] private DashJoystick dashJoystick;

    public void SetBasicAttackInkTypeImage(InkType inkType)
    {
        switch (inkType)
        {
            case InkType.RED:
                basicAttackInkTypeImage.sprite = attackTypeImages[0];
                break;
            case InkType.GREEN:
                basicAttackInkTypeImage.sprite = attackTypeImages[1];
                break;
            case InkType.BLUE:
                basicAttackInkTypeImage.sprite = attackTypeImages[2];
                break;
        }
    }
    private void SetCurrHPBarUI(float value)
    {
        if (hpBar == null)
        {
            Debug.LogError("hpBar is not assignment");
            return;
        }
        hpBar.SetCurrValueUI(value);
        hpBarText.text = Mathf.Floor(value).ToString();
    }

    public void SetMaxHPBarUI(float value)
    {
        if (hpBar == null)
        {
            Debug.LogError("hpBar is not assignment");
            return;
        }
        hpBar.SetMaxValueUI(value);
        shieldBar.SetMaxValueUI(value);
    }

    public void SetCurrInkBarUI(float value)
    {
        if (inkBar == null)
        {
            Debug.LogError("inkBar is not assignment");
            return;
        }
        inkBar.SetCurrValueUI(value);
    }

    private void SetCurrShieldUI(float maxHP, float currHP, float currShield)
    {
        if (shieldBar == null)
        {
            Debug.LogError("shieldBar is not assignment");
            return;
        }
        shieldBar.SetCurrValueForPlayerUI(maxHP, currHP, currShield);
    }

    private void SetMaxShieldUI(float maxHP, float currHP, float maxShield)
    {
        if (shieldBar == null)
        {
            Debug.LogError("shieldBar is not assignment");
            return;
        }
        shieldBar.SetMaxValueForPlayerUI(maxHP, currHP, maxShield);
    }

    public void SetStateBarUIForCurValue(float maxHP, float curHP, float shieldValue)
    {
        if(curHP + shieldValue >= maxHP)
        {
            float hpRatio = curHP * (curHP / (curHP + shieldValue));
            hpBar.SetCurrValueUI(hpRatio);
            shieldBar.SetCurrValueUI(maxHP);
        }
        else
        {
            shieldBar.SetCurrValueUI(curHP + shieldValue);
            hpBar.SetCurrValueUI(curHP);
        }

        hpBarText.text = Mathf.Floor(curHP).ToString();

    }

    public void ShowDamageIndicator()
    {
        if(damageIndicator == null)
        {
            Debug.LogError("damageIndicator is not assignment");
            return;
        }
        damageIndicator.StartCoroutine(damageIndicator.ShowDamageIndicator());
    }

    public void SetMaxInkUI(float value)
    {
        if (inkBar == null)
        {
            Debug.LogError("inkBar is not assignment");
            return;
        }
        inkBar.SetMaxValueUI(value);
    }
    
    public void SetSkillJoystickImage(InkType inkType)
    {
        skillJoystick.SetJoystickImage(inkType);
    }

    public void SetDashJoystickImage(InkType inkType)
    {
        dashJoystick.SetJoystickImage(inkType);
    }
}
