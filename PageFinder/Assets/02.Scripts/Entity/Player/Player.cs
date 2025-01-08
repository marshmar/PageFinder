using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Diagnostics.CodeAnalysis;
using System;
using TMPro;

public class Player : Entity
{
    /*
     * 그 외 필요한 변수들 설정
     */

    // 최승표
    private int coin;
    private PlayerDamageIndicator damageIndicator;

    #region Variables
    private float originalMaxHP;
    private float imgPower;
    private float maxInk;
    private float currInk;
    private float inkGain;
    private float originalInkGain;
    private float attackSpeed;
    private float originalAttackSpeed;
    private float attackRange;
    private WaitForSeconds inkRecoveryDealy;
    private IEnumerator inkRecoverCoroutine;
    [SerializeField]
    private Transform modelTr;
    private Transform tr;
    private Animator anim;
    private Rigidbody rigid;
    protected UtilsManager utilsManager;
    protected EventManager eventManager;
    [SerializeField]
    private TMP_Text hpBarText;
    [SerializeField]
    private SliderBar manaBar;
    [SerializeField]
    //protected Gradation gradation; // 채력 눈금

    private InkType basicAttackInkType;
    private InkType skillInkType;
    private InkType dashInkType;
    private InkType inkMagicInkType;

    [SerializeField]
    private GameObject skillJoystick;
    [SerializeField]
    private GameObject dashJoystick;

    private SkillJoystick skillJoystickScr;
    private DashJoystick dashJoystickScr;

    private PlayerAttackController playerAttackControllerScr;
    private PlayerInkMagicController playerInkMagicControllerScr;
    #endregion

    #region Properties

    private List<IStatModifier> maxHpModifiers = new List<IStatModifier>();
    private List<IStatModifier> currHpModifiers = new List<IStatModifier>();
    private List<IStatModifier> currInkModifiers = new List<IStatModifier>();
    private List<IStatModifier> InkGaiNModifiers = new List<IStatModifier>();
    private List<IStatModifier> atkModifiers = new List<IStatModifier>();
    public InkType BasicAttackInkType 
    { 
        get => basicAttackInkType;
        set {
            basicAttackInkType = value;
            Flamestrike();
            playerAttackControllerScr.SetAttackTypeImage(value);
        } 

    }

    private void Flamestrike()
    {
        switch (basicAttackInkType)
        {
            case InkType.RED:
                this.attackSpeed = attackSpeed * 0.85f;
                playerAttackControllerScr.AttackDelay = new WaitForSeconds(attackSpeed);
                break;
            case InkType.GREEN:
                this.attackSpeed = originalAttackSpeed;
                playerAttackControllerScr.AttackDelay = new WaitForSeconds(originalAttackSpeed);
                break;
            case InkType.BLUE:
                this.attackSpeed = originalAttackSpeed;
                playerAttackControllerScr.AttackDelay = new WaitForSeconds(originalAttackSpeed);
                break;
        }
    }

    public InkType SkillInkType
    {
        get => skillInkType; 
        set
        {
            skillInkType = value;
            if (!DebugUtils.CheckIsNullWithErrorLogging<SkillJoystick>(skillJoystickScr, this.gameObject))
            {
                skillJoystickScr.SetJoystickImage(skillInkType);
            }
        }
    }

    public InkType DashInkType
    {
        get => dashInkType; set
        {
            dashInkType = value;
            if (!DebugUtils.CheckIsNullWithErrorLogging<DashJoystick>(dashJoystickScr, this.gameObject))
            {
                dashJoystickScr.SetJoystickImage(dashInkType);
            }
        }
    }
    public override float HP
    {
        get
        {
            return currHP/* + currShield*/;
        }
        set
        {
            // 입력으로 들어온 데미지 계산
            float inputDamage = currHP - value;

            // 데미지가 음수일 때 = 체력 회복이 됐을 때
            if(inputDamage < 0)
            {
                currHP = currHP + (-1 * inputDamage);
                if (currHP > maxHP) currHP = maxHP;
            }
            // 데미지가 양수일 때 = 데미지를 받았을 때
            else
            {
                Debug.Log("실드 체크");
                // 데미지를 현재 실드량 만큼 차감
                inputDamage -= CurrShield;
                // 차감한 데미지가 0보다 작으면, 즉 현재 실드량이 데미지보다 많으면
                if(inputDamage < 0)
                {
                    CurrShield = -1 * inputDamage;
                    return;
                }

                if (inputDamage >= 0)
                {
                    // 데미지만큼 HP 감소
                    currHP = Mathf.Max(0, currHP - inputDamage);
                    CurrShield = 0;
                    damageIndicator.StartCoroutine(damageIndicator.ShowDamageIndicator());
                }
            }

            // UI 변경
            hpBar.SetCurrValueUI(currHP);
            hpBarText.text = Mathf.Floor(currHP).ToString();
            if (currHP <= 0)
            {
                Die();
                EndGame();
            }
        }
    }

    public override float MAXHP
    {
        get
        {
            return maxHP;
        }
        set
        {
            //float hpDiff = maxHP - currHP;
            float maxHpDiff = value - maxHP;
            float modifiedValue = value;
/*            foreach (IStatModifier m in maxHpModifiers)
            {
                modifiedValue = m.ModifyStat(modifiedValue);
            }
            Debug.Log(modifiedValue);*/
            maxHP = modifiedValue;

            // UI 변경
            hpBar.SetMaxValueUI(maxHP);
            Debug.Log(maxHP);
            currHP +=/* hpDiff +*/ maxHpDiff;
            hpBar.SetCurrValueUI(currHP);
            hpBarText.text = Mathf.Floor(currHP).ToString();
            //gradation.SetGradation(maxHP);
        }
    }

    public float CurrInk
    {
        get
        {
            return currInk;
        }
        set
        {
            currInk = value;
            if (currInk <= 0)
            {
                currInk = 0;
            }

            if (currInk >= maxInk)
            {
                currInk = maxInk;
            }

            manaBar.SetCurrValueUI(currInk);
        }
    }


    public float AttackSpeed
    {
        get { return attackSpeed; }
        set
        {
            attackSpeed = value;
        }
    }

    public float AttackRange
    {
        get { return attackRange; }
        set
        {
            attackRange = value;
        }
    }

    public override float MaxShield
    {
        get
        {
            return maxShield;
        }
        set
        {
            // 실드를 생성한 경우

            maxShield = value;
            //hpBar.SetMaxValueUI(maxHP + maxShield);

            //gradation.SetGradation(maxHP + maxShield);

            shieldBar.SetMaxValueForPlayerUI (maxHP, currHP, maxShield);
            //CurrShield = maxShield;
        }
    }

    public override float CurrShield
    {
        get
        {
            return currShield;
        }
        set
        {
            float minusShield = value;
            if(minusShield < 0)
            {
                minusShield = 0;
            }
            currShield = minusShield;

            Debug.Log(currShield);
            if(maxShield == 0 && currShield != 0)
            {
                shieldBar.SetMaxValueForPlayerUI(maxHP, currHP, value);
            }
            else
            {
                shieldBar.SetMaxValueForPlayerUI(maxHP, currHP, maxShield);
            }
            shieldBar.SetCurrValueForPlayerUI(maxHP, currHP, currShield);

        }
    }

    public float InkGain
    {
        get 
        {
            float modifiedValue = inkGain;
            foreach (IStatModifier m in InkGainModifiers)
            {
                modifiedValue = m.ModifyStat(modifiedValue);
            }
            return modifiedValue;
        }
        set
        {
            inkGain = value;
        }
    }

    internal void SetMaxHP(int greenScriptCounts)
    {
        this.MAXHP = originalMaxHP + greenScriptCounts * originalMaxHP * 0.04f;
    }

    public Animator Anim { get => anim; set => anim = value; }
    public Transform ModelTr { get => modelTr; set => modelTr = value; }
    public Transform Tr { get => tr; set => tr = value; }
    public Rigidbody Rigid { get => rigid; set => rigid = value; }
    public float OriginalInkGain { 
        get => originalInkGain; 
        set
        {
            originalInkGain = value;
            inkGain = value;
        }
    }



    #endregion


    public int Coin
    {
        get
        {
            return coin;
        }
        set
        {
            coin = value;
        }
    }

    public List<IStatModifier> InkGainModifiers { get => InkGaiNModifiers; set => InkGaiNModifiers = value; }
    public List<IStatModifier> MaxHpModifiers { get => maxHpModifiers; set => maxHpModifiers = value; }
    public List<IStatModifier> AtkModifiers { get => atkModifiers; set => atkModifiers = value; }
    public InkType InkMagicInkType { get => inkMagicInkType; 
        set 
        { 
            inkMagicInkType = value;
            if (!DebugUtils.CheckIsNullWithErrorLogging<PlayerInkMagicController>(playerInkMagicControllerScr))
            {
                playerInkMagicControllerScr.SetInkMagicButtonImage(inkMagicInkType);
            }
        } 
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            BasicAttackInkType = InkType.RED;
            SkillInkType = InkType.RED;
            DashInkType = InkType.RED;
            InkMagicInkType = InkType.RED;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            BasicAttackInkType = InkType.BLUE; ;
            SkillInkType = InkType.BLUE;
            DashInkType = InkType.BLUE;
            InkMagicInkType = InkType.BLUE;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            BasicAttackInkType = InkType.GREEN; ;
            SkillInkType = InkType.GREEN;
            DashInkType = InkType.GREEN;
            InkMagicInkType = InkType.GREEN;
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            MaxShield = 20.0f;
            CurrShield = 20.0f;
            Debug.Log("실드 생성");
        }
    }

    public void Awake()
    {
        Hasing();
        SetBasicStatus();
        DontDestroyOnLoad(this.gameObject);
    }
    // Start is called before the first frame update
    public override void Start()
    {

    }

    public void RecoverInk()
    {
        if (inkRecoverCoroutine != null)
        {
            StopCoroutine(inkRecoverCoroutine);
        }
        inkRecoverCoroutine = RecoverInkCoroutine();
        StartCoroutine(inkRecoverCoroutine);
    }
    public IEnumerator RecoverInkCoroutine()
    {
        yield return inkRecoveryDealy;

        while (CurrInk < maxInk)
        {
            CurrInk += InkGain * Time.deltaTime;
            yield return null;
        }
    }

    public Vector3 CalculateDirection(Collider goalObj)
    {
        Vector3 dir = goalObj.gameObject.transform.position - Tr.position;
        return dir;
    }
    public void TurnToDirection(Vector3 dir)
    {
        ModelTr.rotation = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z));
    }

    public virtual void Hasing()
    {
        // 컴포넌트 세팅
        anim = GetComponent<Animator>();
        tr = GetComponentInChildren<Transform>();
        rigid = GetComponentInChildren<Rigidbody>();

        utilsManager = UtilsManager.Instance;
        eventManager = EventManager.Instance;

        dashJoystickScr = DebugUtils.GetComponentWithErrorLogging<DashJoystick>(dashJoystick, "DashJoystick");
        skillJoystickScr = DebugUtils.GetComponentWithErrorLogging<SkillJoystick>(skillJoystick, "SkillJoystick");

        playerAttackControllerScr = DebugUtils.GetComponentWithErrorLogging<PlayerAttackController>(this.gameObject, "PlayerAttackController");
        playerInkMagicControllerScr = DebugUtils.GetComponentWithErrorLogging<PlayerInkMagicController>(this.gameObject, "PlayerInkMagicController");
    }

    // 플레이어 기본 능력치 설정
    public void SetBasicStatus()
    {
        maxHP = 1000;
        originalMaxHP = maxHP;
        atk = 1000;
        currHP = maxHP;
        originalMoveSpeed = 7.0f;
        moveSpeed = 7.0f;
        attackSpeed = 1.0f;
        originalAttackSpeed = 1.0f;
        anim.SetFloat("AttackSpeed", attackSpeed);

        maxInk = 100.0f;
        currInk = maxInk;
        originalInkGain = 20.0f;
        inkGain = originalInkGain;
        inkRecoveryDealy = new WaitForSeconds(0.5f);
        attackRange = 3.0f;

        maxShield = 0;
        currShield = maxShield;

        // HP Bar
        //hpBar = GameObject.Find("Player_UI_Info_HpBar").GetComponent<SliderBar>();
        hpBar.SetMaxValueUI(maxHP);
        hpBar.SetCurrValueUI(currHP);
        hpBarText.text = currHP.ToString();
        //gradation.SetGradation(maxHP); 

        // Mana Bar
        //manaBar = GameObject.Find("Player_UI_Info_ManaBar").GetComponent<SliderBar>();
        manaBar.SetMaxValueUI(currInk);
        manaBar.SetCurrValueUI(currInk);

        // Shield Bar
        //shieldBar = GetComponentInChildren<ShieldBar>();
        shieldBar.SetMaxValueForPlayerUI(maxHP, currHP, maxShield);
        shieldBar.SetCurrValueUI(currShield);
        // Damage Indicator
        damageIndicator = GameObject.Find("Player_UI_Damage_Indicator").GetComponent<PlayerDamageIndicator>();

        BasicAttackInkType = InkType.RED;
        DashInkType = InkType.RED;
        SkillInkType = InkType.RED;
    }

    public void EndGame()
    {
        UIManager.Instance.SetUIActiveState("Defeat");
        //SceneManager.LoadScene("Title");
        //eventManager.PostNotification(EVENT_TYPE.GAME_END, this);
    }


    public virtual void CheckAnimProgress(string animName, float time, ref bool state)
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName(animName))
        {
            if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= time)
            {
                state = false;
            }
            else
            {
                state = true;
            }
        }

    }

    public void ExtraInkGain()
    {
        CurrInk = CurrInk + maxInk * 0.07f;
        Debug.Log(CurrInk);
    }

    public void WaterConservation()
    {
        PlayerDashController playerDashControllerScr = DebugUtils.GetComponentWithErrorLogging<PlayerDashController>(this.gameObject, "PlayerDashController");
        playerDashControllerScr.DashCost = playerDashControllerScr.DashCost - playerDashControllerScr.DashCost * 0.25f;
        PlayerSkillController playerSkillControllerScr = DebugUtils.GetComponentWithErrorLogging<PlayerSkillController>(this.gameObject, "PlayerSkillController");
        playerSkillControllerScr.CurrSkillData.skillCost = playerSkillControllerScr.CurrSkillData.skillCost - playerSkillControllerScr.CurrSkillData.skillCost * 0.25f;
    }

    
}