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
    [SerializeField] private ShieldBar shiledBar;
 
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
    public void SetCurrHPBarUI(float value)
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

    public void SetCurrShieldUI(float maxHP, float currHP, float currShield)
    {
        if (shiledBar == null)
        {
            Debug.LogError("shiledBar is not assignment");
            return;
        }
        shiledBar.SetCurrValueForPlayerUI(maxHP, currHP, currShield);
    }

    public void SetMaxShieldUI(float maxHP, float currHP, float maxShield)
    {
        if (shiledBar == null)
        {
            Debug.LogError("shiledBar is not assignment");
            return;
        }
        shiledBar.SetMaxValueForPlayerUI(maxHP, currHP, maxShield);
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
