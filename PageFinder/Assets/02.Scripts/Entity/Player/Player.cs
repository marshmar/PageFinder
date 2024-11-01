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

    #region Variables
    private float img;
    private float maxInk;
    private float currInk;
    private float inkGain;
    private float originalInkGain;
    private float attackSpeed;
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
    private SliderBar manaBar;
    [SerializeField]
    //protected Gradation gradation; // 채력 눈금

    private InkType basicAttackInkType;
    private InkType skillInkType;
    private InkType dashInkType;

    [SerializeField]
    private GameObject skillJoystick;
    [SerializeField]
    private GameObject dashJoystick;

    private SkillJoystick skillJoystickScr;
    private DashJoystick dashJoystickScr;
    #endregion

    #region Properties

    public InkType BasicAttackInkType 
    { 
        get => basicAttackInkType;
        set {
            basicAttackInkType = value;
            UpGradeBasicAttack();
        } 

    }

    private void UpGradeBasicAttack()
    {
        switch (basicAttackInkType)
        {
            case InkType.RED:
                this.attackSpeed = attackSpeed * 0.85f;
                break;
            case InkType.GREEN:
                break;
            case InkType.BLUE:
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
                skillJoystickScr.SetJoystickImage(dashInkType);
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
            return currHP + currShield;
        }
        set
        {
            // 감소시켜도 쉴드가 남아있는 경우
            if (value > currHP)
            {
                CurrShield = value - currHP;
            }
            else // 감소시켜도 쉴드가 남아있지 않은 경우
            {
                CurrShield = 0;
                currHP = value;
            }

            // UI 변경
            hpBar.SetCurrValueUI(currHP);
            hpBarText.text = currHP.ToString();
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
            maxHP = value;

            // UI 변경
            hpBar.SetMaxValueUI(maxHP);
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
            hpBar.SetMaxValueUI(maxHP + maxShield);

            //gradation.SetGradation(maxHP + maxShield);

            shieldBar.SetMaxValueUI(maxHP, currHP, maxShield);
            CurrShield = maxShield;
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
            currShield = value;

            shieldBar.SetCurrValueUI(currShield);

            // 쉴드를 다 사용했을 경우
            if (currShield <= 0)
            {
                currShield = 0;
                //gradation.SetGradation(maxHP);
            }
        }
    }

    public float InkGain { get => inkGain; set => inkGain = value; }

    public Animator Anim { get => anim; set => anim = value; }
    public Transform ModelTr { get => modelTr; set => modelTr = value; }
    public Transform Tr { get => tr; set => tr = value; }
    public Rigidbody Rigid { get => rigid; set => rigid = value; }
    public float OriginalInkGain { get => originalInkGain; set => originalInkGain = value; }



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


    // Start is called before the first frame update
    public override void Start()
    {
        Hasing();
        SetBasicStatus();
        DontDestroyOnLoad(this.gameObject);
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
            CurrInk += inkGain * Time.deltaTime;
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
    }

    // 플레이어 기본 능력치 설정
    public void SetBasicStatus()
    {
        maxHP = 1000.0f;
        atk = 1000;
        currHP = maxHP;
        moveSpeed = 7.0f;
        attackSpeed = 1.0f;
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
        shieldBar.SetMaxValueUI(maxHP, currHP, maxShield);
        shieldBar.SetCurrValueUI(currShield);
    }

    public void EndGame()
    {
        eventManager.PostNotification(EVENT_TYPE.GAME_END, this);
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

}