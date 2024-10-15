using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Diagnostics.CodeAnalysis;
using System;

public class Player : Entity
{
    /*
     * 그 외 필요한 변수들 설정
     */

    #region Variables
    protected float img;
    protected float maxInk;
    protected float currInk;
    protected float inkGain;
    protected float attackSpeed;
    protected float attackRange;

    [SerializeField]
    protected Transform modelTr;
    protected Transform tr;
    protected Animator anim;
    protected Rigidbody rigid;
    protected UtilsManager utilsManager;
    protected EventManager eventManager;
    protected Palette palette;
    protected RangedEntity rangedEntity;

    [SerializeField]
    private GameObject targetObject;
    protected Transform targetObjectTr;

    private SliderBar manaBar;
    [SerializeField]
    protected Gradation gradation; // 채력 눈금

    #endregion

    #region Properties
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
            gradation.SetGradation(maxHP);
        } 
    }

    public float CurrInk
    {
        get { 
            return currInk; 
        }
        set 
        { 
            currInk = value;
            
            if(currInk <= 0)
            {
                currInk = 0;
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
            anim.SetFloat("AttackSpeed", attackSpeed);
        }
    }

    public float AttackRange
    {
        get { return attackRange; }
        set
        {
            attackSpeed = value;
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

            gradation.SetGradation(maxHP + maxShield);

            shieldBar.SetMaxShieldValueUI(maxHP, currHP, maxShield);
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
                gradation.SetGradation(maxHP);
            }
        }
    }

    public float InkGain { get => inkGain; set => inkGain = value; }
    public GameObject TargetObject{ get { return targetObject; } }



    #endregion

    public virtual void Awake()
    {
        palette = GameObject.FindWithTag("PLAYER").GetComponent<Palette>();
    }
    // Start is called before the first frame update
    public override void Start()
    {
        Hasing();
        SetBasicStatus();
        DontDestroyOnLoad(this.gameObject);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3 CalculateDirection(Collider goalObj)
    {
        Vector3 dir = goalObj.gameObject.transform.position - tr.position;
        return dir;
    }
    public void TurnToDirection(Vector3 dir)
    {
        modelTr.rotation = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z));
    }

    public virtual void Hasing()
    {
        // 컴포넌트 세팅
        anim = GetComponentInChildren<Animator>();
        tr = GetComponentInChildren<Transform>();
        rigid = GetComponentInChildren<Rigidbody>();
        
        utilsManager = UtilsManager.Instance;
        eventManager = EventManager.Instance;
        targetObjectTr = targetObject.GetComponent<Transform>();
        targetObject.SetActive(false);

        rangedEntity = GetComponent<RangedEntity>();
    }

    // 플레이어 기본 능력치 설정
    public void SetBasicStatus()
    {
        maxHP = 1000.0f;
        atk = 10;
        currHP = maxHP;
        moveSpeed = 10.0f;
        attackSpeed = 2.5f;
        currInk = 500.0f;
        currInk = maxInk;
        anim.SetFloat("AttackSpeed", attackSpeed);
        attackRange = 7.0f;

        maxShield = 0;
        currShield = maxShield;

        // HP Bar
        hpBar = GetComponentInChildren<SliderBar>();
        hpBar.SetMaxValueUI(maxHP);
        hpBar.SetCurrValueUI(currHP);
        gradation.SetGradation(maxHP); 

        // Mana Bar
        manaBar = GameObject.Find("Player_UI_Info_ManaBar").GetComponent<SliderBar>();
        manaBar.SetMaxValueUI(currInk);
        manaBar.SetCurrValueUI(currInk);

        // Shield Bar
        shieldBar.SetMaxShieldValueUI(maxHP, currHP, maxShield);
        shieldBar.SetCurrValueUI(currShield);
    }

    /// <summary>
    /// 타겟팅 객체 움직이기
    /// </summary>
    /// <param name="targetingRange">공격 범위</param>
    public virtual void OnTargeting(Vector3 attackDir, float targetingRange)
    {
        SetTargetObject(true);

        // 사거리를 벗어날 경우 제자리 고정
        if (Vector3.Distance(tr.position, targetObjectTr.position) >= targetingRange)
        {
            targetObjectTr.position = (tr.position - targetObjectTr.position).normalized * targetingRange;
        }
        // 타겟팅 오브젝트 움직이기
        else
        {
            targetObjectTr.position = (tr.position + (attackDir) * (targetingRange - 0.1f));
            targetObjectTr.position = new Vector3(targetObjectTr.position.x, 0.1f, targetObjectTr.position.z);
        }
    }

    public void SetTargetObject(bool isActive)
    {
        targetObjectTr.position = tr.position;
        targetObject.SetActive(isActive);
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
