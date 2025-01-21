using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    #region defaultValue
    private const float defaultMaxHp = 100f;
    private const float defaultMaxInk = 100f;
    private const float defaultInkGain = 20f;
    private const float defaultAttackSpeed = 1f;
    private const float defaultAttackRange = 3f;
    private const float defaultAtk = 1f;
    private const float defaultDef = 1f;
    private const float defaultMoveSpeed = 7f;
    private const float defaultCritical = 0.1f;
    private const float defaultImag = 1f; // 상상력
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
    private float curImag;
    private float maxShield;
    private float curShield;
    private int coin;
    #endregion

    #region Default Value Properties
    public float DefaultMaxHp { get => defaultMaxHp; }
    public float DefaultMaxInk { get => defaultMaxInk; }
    public float DefaultAttackSpeed { get => defaultAttackSpeed; }
    public float DefaultAttackRange { get => defaultAttackRange; }
    public float DefaultAtk { get => defaultAtk; }
    public float DefaultDef { get => defaultDef; }
    public float DefaultMoveSpeed { get => defaultMoveSpeed; }
    public float DefaultCritical { get => defaultCritical; }
    public float DefaultImag { get => defaultImag; }
    public float DefaultInkGain { get => defaultInkGain; }

    #endregion

    #region Cur value Properties
    public float MaxHp { 
        get => maxHp;
        set 
        {
            // maxHP가 늘어날 경우, 늘어난 만큼의 체력을 현재 hp에서 더해주기
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
            // 데미지 계산 공식 적용 필요
            float inputDamage = curHp - value;

            if(inputDamage < 0)
            {
                curHp = curHp + -inputDamage;
                if (curHp > maxHp) curHp = maxHp;
            }
            else
            {
                inputDamage -= CurShield;
                if(inputDamage < 0)
                {
                    CurShield = -inputDamage;
                    return;
                }

                if(inputDamage >= 0)
                {
                    curHp = Mathf.Max(0, curHp - inputDamage);
                    CurShield = 0;
                    playerUI.ShowDamageIndicator();
                }
            }
            playerUI.SetCurrHPBarUI(curHp);


            if (curHp <= 0)
            {
                UIManager.Instance.SetUIActiveState("Defeat");
                EventManager.Instance.PostNotification(EVENT_TYPE.GAME_END, this);
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
/*            // 잉크 게이지를 소모한 경우 게이지 회복
            if(Mathf.Min(curInk, maxInk) == maxInk)
            {
                Debug.Log("잉크 회복");
                RecoverInk();
            }*/
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
    public float CurImag { get => curImag; set => curImag = value; }
    public float CurShield { 
        get => curShield; 
        set 
        {
            curShield = Mathf.Max(0, value);
            if(MaxShield == 0 && curShield != 0)
            {
                playerUI.SetMaxShieldUI(MaxHp, CurHp, value);
            }
            else
            {
                playerUI.SetMaxShieldUI(MaxHp, CurHp, MaxShield);
            }
            playerUI.SetCurrShieldUI(MaxHp, CurHp, CurShield);
        } 
    }
    public float MaxShield { get => maxShield; 
        set 
        { 
            maxShield = value;
            playerUI.SetMaxShieldUI(MaxHp, CurHp, maxShield);
        } 
    }

    public int Coin { get => coin; set => coin = value; }
    #endregion

    #region Hashing
    private PlayerUI playerUI;
    private WaitForSeconds inkRecoveryDelay;
    private IEnumerator inkRecoveryCoroutine;

    #endregion

    private void Awake()
    {
        // Hashing
        playerUI = DebugUtils.GetComponentWithErrorLogging<PlayerUI>(this.gameObject, "PlayerUI");
    }

    private void Start()
    {
        // 기본값 설정
        MaxHp = defaultMaxHp;
        CurHp = MaxHp;
        MaxInk = defaultMaxInk;
        CurInk = MaxInk;
        curInkGain = defaultInkGain;
        CurAttackSpeed = defaultAttackSpeed;
        CurAttackRange = defaultAttackRange;
        CurAtk = defaultAtk;
        CurDef = defaultDef;
        CurMoveSpeed = defaultMoveSpeed;
        CurCritical = defaultCritical;
        CurImag = defaultImag;

        inkRecoveryDelay = new WaitForSeconds(0.5f);
    }

    public void RecoverInk()
    {
        if(inkRecoveryCoroutine != null)
        {
            StopCoroutine(inkRecoveryCoroutine);
        }
        inkRecoveryCoroutine = RecoverInkCoroutine();
        StartCoroutine(inkRecoveryCoroutine);
    }

    private IEnumerator RecoverInkCoroutine()
    {
        yield return inkRecoveryDelay;

        while(CurInk < MaxInk)
        {
            curInk += CurInkGain * Time.deltaTime;
            curInk = Mathf.Clamp(curInk, 0, maxInk);
            playerUI.SetCurrInkBarUI(curInk);
            yield return null;
        }
    }
}
