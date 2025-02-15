using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour, IListener, IObserver, /*IInvoker,*/ IEntityState
{
    #region defaultValue
    private const float defaultMaxHp = 500f;
    private const float defaultMaxInk = 100f;
    private const float defaultInkGain = 20f;
    private const float defaultAttackSpeed = 1f;
    private const float defaultAttackRange = 3f;
    private const float defaultAtk = 50f;
    private const float defaultMoveSpeed = 7f;
    private const float defaultCritical = 0.1f;
    private const float defaultCriticalDmg = 1.5f;
    #endregion

    #region currValue
    private float maxHp;
    private float curHp;
    private float maxInk;
    private float curInk;
    private float curInkGain;
    private float curAttackSpeed;
    private float curAttackRange;
    private float curAtk;
    private float curDef;
    private float curMoveSpeed;
    private float curCritical;
    private float curCriticalDmg;
    private float maxShield;
    private float maxShieldPercentage = 0.3f;
    private float curShield;
    private int coin;
    private float dmgBonus;
    private float dmgResist;

    #endregion

    #region Default Value Properties
    public float DefaultMaxHp { get => defaultMaxHp; }
    public float DefaultMaxInk { get => defaultMaxInk; }
    public float DefaultAttackSpeed { get => defaultAttackSpeed; }
    public float DefaultAttackRange { get => defaultAttackRange; }
    public float DefaultAtk { get => defaultAtk; }
    public float DefaultMoveSpeed { get => defaultMoveSpeed; }
    public float DefaultCritical { get => defaultCritical; }
    public float DefaultInkGain { get => defaultInkGain; }
    public float DefaultCriticalDmg { get => defaultCriticalDmg; }

    #endregion

    #region Cur value Properties
    public float MaxHp { 
        get => maxHp;
        set 
        {
            // maxHP�� �þ ���, �þ ��ŭ�� ü���� ���� hp���� �����ֱ�
            float hpInterval = value - maxHp;

            maxHp = value;
            playerUI.SetMaxHPBarUI(maxHp);

            CurHp += hpInterval;
        }
    }
    public float CurHp
    {
        get => curHp;
        set { 
            // ������ ��� ���� ���� �ʿ�
            float inputDamage = curHp - value;

            if(inputDamage < 0)
            {
                curHp = curHp + -inputDamage;
                if (curHp > maxHp) curHp = maxHp;
            }
            else
            {
                float damage = shieldManager.CalculateDamageWithDecreasingShield(inputDamage);
                if (damage <= 0) 
                {
                    playerUI.SetStateBarUIForCurValue(maxHp, curHp, CurShield);
                    return;
                }

                playerUI.StartDamageFlash(curHp, damage, maxHp);
                curHp -= damage;
                playerUI.ShowDamageIndicator(); // ToDo: �� �κе� Event��� ���α׷������� ���� �� ���� ������?
                //playerUI.SetCurrHPBarUI(curHp);

                
               /* // ���������� ���� ���常ŭ ����
                inputDamage -= CurShield;
                // ���� ���尡 ���������� ���� ��� ���常 ����
                if(inputDamage <= 0)
                {
                    CurShield = -inputDamage;
                    return;
                }

                // ���� ���尡 ���������� ������� �ʰ��� ��ŭ hp ����
                if(inputDamage > 0)
                {
                    curHp = Mathf.Max(0, curHp - inputDamage);
                    CurShield = 0;
                    playerUI.ShowDamageIndicator();
                }*/
            }
            playerUI.SetStateBarUIForCurValue(maxHp, curHp, CurShield);
            

            if (curHp <= 0)
            {
                //UIManager.Instance.SetUIActiveState("Defeat");

                // �ֽ�ǥ ����
                // �ٷ� Ÿ��Ʋ �̵� ���� �й� UI ǥ���� ResultUIManager���� GameEnd�� �̵�
                EventManager.Instance.PostNotification(EVENT_TYPE.UI_Changed, this, UIType.Defeat);
                // EventManager.Instance.PostNotification(EVENT_TYPE.GAME_END, this);
            }
        }
    }

    public float MaxInk { get => maxInk; set { maxInk = value; playerUI.SetMaxInkUI(maxInk); } }
    public float CurInk { 
        get => curInk;
        set 
        {
            curInk = Mathf.Clamp(value, 0, maxInk);
            playerUI.SetCurrInkBarUI(curInk);

            if(curInk < maxInk)
            {
                RecoverInk();
            }
        }
    }
    public float CurInkGain { 
        get => curInkGain; set => curInkGain = value; }
    public float CurAttackSpeed { get => curAttackSpeed; set => curAttackSpeed = value; }
    public float CurAttackRange { get => curAttackRange; set => curAttackRange = value; }
    public float CurAtk { get => curAtk; set => curAtk = value; }
    public float CurDef { get => curDef; set => curDef = value; }
    public float CurMoveSpeed { get => curMoveSpeed; set => curMoveSpeed = value; }
    public float CurCritical { get => curCritical; set => curCritical = value; }

    public float CurShield {
        get => shieldManager.CurShield;
        set
        {
            curShield = Mathf.Max(0, value);
            playerUI.SetStateBarUIForCurValue(MaxHp, curHp, value);
        }
    }
    public float MaxShield { get => shieldManager.MaxShield;
        set
        {
            shieldManager.MaxShield = value;
            //playerUI.SetMaxShieldUI(MaxHp, CurHp, maxShield);
        }
    }

    public int Coin { get => coin; set => coin = value; }
    public float CurCriticalDmg { get => curCriticalDmg; set => curCriticalDmg = value; }
    public float DmgBonus { get => dmgBonus; set => dmgBonus = value; }
    public float DmgResist { get => dmgResist; set => dmgResist = value; }
    #endregion

    #region Hashing
    private ShieldManager shieldManager;
    private PlayerUI playerUI;
    private WaitForSeconds inkRecoveryDelay;
    private IEnumerator inkRecoveryCoroutine;
    #endregion

    #region Buff
    // ����
    private SortedList<float, ICommand> commands;
/*    private Dictionary<BuffState, List<IBuff>> permanentBuffs;
    private Dictionary<BuffState, List<IBuff>> permanentMultiplier;
    private Dictionary<BuffState, List<IBuff>> permanentDebuff;*/

    private float[] permanentBuffStates; // buffState : ���� ���� �ɷ�ġ�� �����ϴ� �迭
    private float[] permanentMultiplierStates;
    private float[] permanentDebuffStates;
    #endregion

    private void Awake()
    {
        // Hashing
        playerUI = DebugUtils.GetComponentWithErrorLogging<PlayerUI>(this.gameObject, "PlayerUI");
        shieldManager = DebugUtils.GetComponentWithErrorLogging<ShieldManager>(this.gameObject, "ShieldManager");
        shieldManager.Attach(this);
    }

    private void Start()
    {
        SetBasicState();

        inkRecoveryDelay = new WaitForSeconds(0.5f);

        EventManager.Instance.AddListener(EVENT_TYPE.Generate_Shield_Player, this);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            EventManager.Instance.PostNotification(EVENT_TYPE.Generate_Shield_Player, this, new System.Tuple<float, float>(50f, 3f));
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            CurHp -= 50.0f;
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            CurHp += 50.0f;
        }
    }
    private void SetBasicState()
    {
        // �⺻�� ����
        MaxHp = defaultMaxHp;
        CurHp = MaxHp;
        MaxInk = defaultMaxInk;
        CurInk = MaxInk;
        curInkGain = defaultInkGain;
        CurAttackSpeed = defaultAttackSpeed;
        CurAttackRange = defaultAttackRange;
        CurAtk = defaultAtk;
        CurMoveSpeed = defaultMoveSpeed;
        CurCritical = defaultCritical;
        CurCriticalDmg = defaultCriticalDmg;
        MaxShield = curHp * maxShieldPercentage;
        dmgBonus = 0f;
        dmgResist = 0f;
    }

/*    /// <summary>
    /// Ŀ�ǵ� �������� �����غ���
    /// </summary>
    private void InitializeBuffDictionaries()
    {
        permanentBuffs = new Dictionary<BuffState, List<IBuff>>();
        foreach(BuffState type in System.Enum.GetValues(typeof(BuffState)))
        {
            permanentBuffs[type] = new List<IBuff>();
        }

        permanentMultiplier = new Dictionary<BuffState, List<IBuff>>();
        foreach (BuffState type in System.Enum.GetValues(typeof(BuffState)))
        {
            permanentMultiplier[type] = new List<IBuff>();
        }

        permanentDebuff = new Dictionary<BuffState, List<IBuff>>();
        foreach (BuffState type in System.Enum.GetValues(typeof(BuffState)))
        {
            permanentDebuff[type] = new List<IBuff>();
        }

        
        // ���� ������ �ʵ�� ���� State
    }*/

    // UniTask ����ϸ� ����
    public void RecoverInk()
    {
        // is not null
        if(inkRecoveryCoroutine is not null)
        {
            StopCoroutine(inkRecoveryCoroutine);
        }
        inkRecoveryCoroutine = RecoverInkCoroutine();
        StartCoroutine(inkRecoveryCoroutine);
    }

    private void OnDestroy()
    {
        inkRecoveryCoroutine = null;
    }

    private IEnumerator RecoverInkCoroutine()
    {
        yield return inkRecoveryDelay;

        while(CurInk < MaxInk)
        {
            curInk += CurInkGain * Time.deltaTime;
            curInk = Mathf.Clamp(curInk, 0, maxInk);
            playerUI.SetCurrInkBarUI(curInk);

            // ��ũ ������ ���� ȸ���� ������ �̺�Ʈ ���ֱ�
            EventManager.Instance.PostNotification(EVENT_TYPE.InkGage_Changed, this, curInk);
            yield return null;
        }
    }

    private float FinalStatCalculator(float baseStat, float permanentBuff, float permanentMultiplier, float permanentDebuff, float temporaryBuff, float temporaryDebuff)
    {
        return (baseStat + permanentBuff) * (permanentMultiplier) * (1 - permanentDebuff) * (1 + temporaryBuff - temporaryDebuff);
    }

    public void OnEvent(EVENT_TYPE eventType, Component Sender, object Param)
    {
        switch (eventType)
        {
/*            case EVENT_TYPE.Buff:
                var buffInfo = (System.Tuple<BuffType, BuffState, float, float>)Param;
                break;*/
            case EVENT_TYPE.Generate_Shield_Player:
                float shieldAmount = ((System.Tuple<float, float>)Param).Item1;
                float shieldDuration = ((System.Tuple<float, float>)Param).Item2;

                shieldManager.GenerateShield(shieldAmount, shieldDuration);
                playerUI.SetStateBarUIForCurValue(maxHp, curHp, CurShield);
                break;
        }
    }

    public void Notify(Subject subject)
    {
        playerUI.SetStateBarUIForCurValue(maxHp, curHp, CurShield);
    }

    public void ExecuteCommand(ICommand command)
    {
        command.Execute();
    }

    public void AddCommand(ICommand command)
    {
        commands.Add(1, command);
    }
}
