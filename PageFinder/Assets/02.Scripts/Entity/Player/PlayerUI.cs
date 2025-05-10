using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PlayerUI : MonoBehaviour, IUIElement
{
    private PlayerInputAction input;
    private PlayerState playerState;

    public const string playerDashJoystickName = "Player_UI_OP_Dash";
    public const string playerSkillJoystickName = "Player_UI_OP_Skill";

    [SerializeField] private GameObject HUD_Player;

    [Header("StatusBar")]
    [SerializeField] private SliderBar hpBar;
    [SerializeField] private SliderBar inkBar;
    [SerializeField] private SliderBar shieldBar;
    [SerializeField] private SliderBar damageFlashBar;
 
    [SerializeField] private TMP_Text hpBarText;
    [SerializeField] private PlayerDamageIndicator damageIndicator;

    [Header("Joystick Sprites")]
    [SerializeField]
    private Sprite[] attackTypeImages;

    [Header("Joystick Objects")]
    [SerializeField] private Image basicAttackImage;
    [SerializeField] private SkillJoystick skillJoystick;
    [SerializeField] private DashJoystick dashJoystick;
    [SerializeField] private GameObject interactButton;

    private void Awake()
    {
        input = DebugUtils.GetComponentWithErrorLogging<PlayerInputAction>(this.gameObject, "PlayerInputAction");
        playerState = DebugUtils.GetComponentWithErrorLogging<PlayerState>(this.gameObject, "PlayerState");
        //SetUIPosByDevice();
    }

    private void SetUIPosByDevice()
    {
        RectTransform hpBarRect = hpBar.GetComponent<RectTransform>();
        RectTransform inkBarRect = inkBar.GetComponent<RectTransform>();
#if UNITY_STANDALONE
        hpBarRect.anchorMin = new Vector2(0f, 0f);
        hpBarRect.anchorMax = new Vector2(0f, 0f);
        hpBarRect.pivot = new Vector2(0f, 0f);
        hpBarRect.anchoredPosition = new Vector2(37.5f, 60f);

        inkBarRect.anchorMin = new Vector2(0f, 0f);
        inkBarRect.anchorMax = new Vector2(0f, 0f);
        inkBarRect.pivot = new Vector2(0f, 0f);
        inkBarRect.anchoredPosition = new Vector2(55.5f, 45f);
#elif UNITY_ANDROID || UNITY_IOS
        hpBarRect.anchorMin = new Vector2(0.5f, 0f);
        hpBarRect.anchorMax = new Vector2(0.5f, 0f);
        hpBarRect.pivot = new Vector2(0.5f, 0f);
        hpBarRect.anchoredPosition.Set(-37.5f, 60f);

        inkBarRect.anchorMin = new Vector2(0.5f, 0f);
        inkBarRect.anchorMax = new Vector2(0.5f, 0f);
        inkBarRect.pivot = new Vector2(0.5f, 0f);
        inkBarRect.anchoredPosition.Set(-37.5f, 45f);
#endif

    }

    private void Start()
    {
        SetPauseAction();
        BindPlayerStatsToUI();
    }

    private void BindPlayerStatsToUI()
    {
        playerState.MaxHp.OnModified += SetMaxHPBarUI;
        playerState.MaxInk.OnModified += SetMaxInkUI;
    }

    private void SetPauseAction()
    {
        if (input is null)
        {
            Debug.LogError("PlayerInput 컴포넌트가 존재하지 않습니다.");
            return;
        }

        if (input.PauseAction is null)
        {
            Debug.LogError("Attack Action이 존재하지 않습니다.");
            return;
        }

        input.PauseAction.canceled += context =>
        {
            // ToDo: UI Changed;
            //EventManager.Instance.PostNotification(EVENT_TYPE.UI_Changed, this, UIType.Setting);
        };
    }
    public void SetInteractButton(bool active)
    {
        if (active)
        {
            skillJoystick.gameObject.SetActive(false);
            interactButton.SetActive(true);
        }
        else
        {
            skillJoystick.gameObject.SetActive(true);
            interactButton.SetActive(false);
        }
    }

    public void SetBasicAttackInkTypeImage(InkType inkType)
    {
        Debug.Log("기본공격 아이콘 바구기");
        /*switch (inkType)
        {
            case InkType.RED:
                basicAttackImage.sprite = attackTypeImages[0];
                break;
            case InkType.GREEN:
                basicAttackImage.sprite = attackTypeImages[1];
                break;
            case InkType.BLUE:
                basicAttackImage.sprite = attackTypeImages[2];
                break;
        }*/
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

    public void SetMaxHPBarUI()
    {
        if (hpBar == null)
        {
            Debug.LogError("hpBar is not assignment");
            return;
        }

        float value = playerState.MaxHp.Value;
        hpBar.SetMaxValueUI(value);
        shieldBar.SetMaxValueUI(value);
        damageFlashBar.SetMaxValueUI(value);
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

/*    private void SetCurrShieldUI(float maxHP, float currHP, float currShield)
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
    }*/

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

    internal void StartDamageFlash(float curHp, float damage, float maxHp)
    {
        StartCoroutine(DamageFlash(curHp, damage, maxHp));
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

    public void SetMaxInkUI()
    {
        if (inkBar == null)
        {
            Debug.LogError("inkBar is not assignment");
            return;
        }

        inkBar.SetMaxValueUI(playerState.MaxInk.Value);
    }
    
    public void SetSkillJoystickImage(InkType inkType)
    {
        skillJoystick.SetJoystickImage(inkType);
    }

    public void SetDashJoystickImage(InkType inkType)
    {
        dashJoystick.SetJoystickImage(inkType);
    }

    private IEnumerator DamageFlash(float curHp, float damage, float maxHp)
    {
        float elapsed = 0f;
        float time = 0.3f;
        
        while(elapsed < time)
        {
            elapsed += Time.deltaTime;
            damageFlashBar.SetCurrValueUI(Mathf.Lerp(curHp, curHp - damage, elapsed / time));
            yield return null;
        }
        //SetStateBarUIForCurValue(maxHp, Mathf.Lerp(curHp, curHp - damage, elapsed / time), 0);
    }

    public void Open()
    {
        HUD_Player.SetActive(true);
        Refresh();
    }

    public void Close()
    {
        HUD_Player.SetActive(false);
    }

    public void Refresh()
    {
        skillJoystick.Refresh();
        dashJoystick.Refresh();
    }
}
