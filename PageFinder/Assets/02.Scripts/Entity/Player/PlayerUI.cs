using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PlayerUI : MonoBehaviour, IUIElement
{
    #region Variables
    private Player _player;

    public const string PlayerDashJoystickName  = "Player_UI_OP_Dash";
    public const string PlayerSkillJoystickName = "Player_UI_OP_Skill";

    [SerializeField] private GameObject hudPlayer;

    [Header("StatusBar")]
    [SerializeField] private SliderBar hpBar;
    [SerializeField] private SliderBar inkBar;
    [SerializeField] private SliderBar shieldBar;
    [SerializeField] private SliderBar damageFlashBar;

    [SerializeField] private TMP_Text hpBarText;
    [SerializeField] private PlayerDamageIndicator damageIndicator;

    [Header("Joystick Sprites")]
    [SerializeField] private Sprite[] attackTypeImages;

    [Header("Joystick Objects")]
    [SerializeField] private Image basicAttackImage;
    [SerializeField] private SkillJoystick skillJoystick;
    [SerializeField] private DashJoystick dashJoystick;
    [SerializeField] private GameObject interactButton;
    #endregion

    #region Properties
    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        _player = this.GetComponentSafe<Player>();
        //SetUIPosByDevice();
    }

    private void Start()
    {
        InitializePauseAction();
        BindPlayerStatsToUI();
    }
    #endregion

    #region Initialization

    private void InitializePauseAction()
    {
        var pauseAction = _player.InputAction.GetInputAction(PlayerInputActionType.Pause);
        if (pauseAction == null)
        {
            Debug.LogError("Pause Action is null");
            return;
        }

        pauseAction.canceled += context =>
        {
            EventManager.Instance.PostNotification(EVENT_TYPE.Open_Panel_Exclusive, this, PanelType.Setting);
        };
    }
    #endregion

    #region Actions
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

    public void SetInteractButton(bool active)
    {
        skillJoystick.gameObject.SetActive(!active);
        interactButton.SetActive(active);
    }

    public void SetBasicAttackInkTypeImage(InkType inkType)
    {
        if(attackTypeImages.Length < 3)
        {
            Debug.LogError("need at least three sprite images for the basic attack.");
            return;
        }

        basicAttackImage.sprite = attackTypeImages[(int)inkType];
    }

    private void SetCurrHPBarUI(float value)
    {
        if (hpBar.IsNull()) return;

        hpBar.SetCurrValueUI(value);
        hpBarText.text = Mathf.Floor(value).ToString();
    }

    public void SetMaxHPBarUI()
    {
        if (hpBar.IsNull()) return;
        if (shieldBar.IsNull()) return;
        if (damageFlashBar.IsNull()) return;

        float maxHpVal = _player.State.MaxHp.Value;
        hpBar.SetMaxValueUI(maxHpVal);
        shieldBar.SetMaxValueUI(maxHpVal);
        damageFlashBar.SetMaxValueUI(maxHpVal);
    }

    public void SetCurrInkBarUI(float value)
    {
        if (inkBar.IsNull()) return;

        inkBar.SetCurrValueUI(value);
    }

    public void SetStateBarUIForCurValue(float maxHP, float curHP, float shieldValue)
    {
        if (hpBar.IsNull()) return;
        if (shieldBar.IsNull()) return;
        if (hpBarText.IsNull()) return;

        if (curHP + shieldValue >= maxHP)
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

    public void StartDamageFlash(float curHp, float damageAmount, float maxHp)
    {
        StartCoroutine(DamageFlash(curHp, damageAmount, maxHp));
    }

    public void ShowDamageIndicator()
    {
        if (damageIndicator.IsNull()) return;

        damageIndicator.StartCoroutine(damageIndicator.ShowDamageIndicator());
    }

    public void SetMaxInkUI()
    {
        if (inkBar.IsNull()) return;

        inkBar.SetMaxValueUI(_player.State.MaxInk.Value);
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
        float elapsedTimeSec = 0f;
        float damageFlashTimeSec = 0.3f;

        while (elapsedTimeSec < damageFlashTimeSec)
        {
            elapsedTimeSec += Time.deltaTime;
            damageFlashBar.SetCurrValueUI(Mathf.Lerp(curHp, curHp - damage, elapsedTimeSec / damageFlashTimeSec));
            yield return null;
        }
    }
    #endregion

    #region Getter
    #endregion

    #region Setter
    #endregion

    #region Utilities
    #endregion

    #region Events
    private void BindPlayerStatsToUI()
    {
        _player.State.MaxHp.OnModified += SetMaxHPBarUI;
        _player.State.MaxInk.OnModified += SetMaxInkUI;
    }
    #endregion

    public void Open()
    {
        hudPlayer.SetActive(true);
        Refresh();
    }

    public void Close()
    {
        hudPlayer.SetActive(false);
    }

    public void Refresh()
    {
        skillJoystick.Refresh();
        dashJoystick.Refresh();
    }
}
