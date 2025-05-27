using System;
using UnityEngine;
using System.Collections;

public class NewPlayerAttackController : MonoBehaviour, IListener
{
    private BaseScript script;
    private Player player;
    //private PlayerInputInvoker inputInvoker;
    //private PlayerInputAction inputAction;
    //private PlayerAnim playerAnim;
    //private PlayerUtils playerUtils;
    //private PlayerTarget playerTarget;
    //private TargetObject targetMarker;
    //private PlayerState playerState;
    private bool isAttacking = false;
    private int comboCount = 0;
    private bool isNextAttackBuffered = false;
    private bool isAbleAttack = true;

    //[SerializeField] private PlayerBasicAttackCollider basicAttackCollider;
    //private NewPlayerDashController playerDashController;
    //private NewPlayerSkillController playerSkillController; 

    [Header("Effects")]
    [SerializeField] private GameObject[] baEffectRed;
    [SerializeField] private GameObject[] baEffectGreen;
    [SerializeField] private GameObject[] baEffectBlue;

    public bool IsAttacking { get => isAttacking; set => isAttacking = value; }
    public int ComboCount { get => comboCount; set => comboCount = value; }
    public bool IsNextAttackBuffered { get => isNextAttackBuffered; set => isNextAttackBuffered = value; }

    private void Awake()
    {
        player = GetComponent<Player>();
        EventManager.Instance.AddListener(EVENT_TYPE.Open_Panel_Exclusive, this);
        EventManager.Instance.AddListener(EVENT_TYPE.Open_Panel_Stacked, this);
        EventManager.Instance.AddListener(EVENT_TYPE.Stage_Clear, this);
        EventManager.Instance.AddListener(EVENT_TYPE.Stage_Start, this);
    }

    private void Start()
    {
        SetAttackAction();

    }

    private void SetAttackAction()
    {
        if (player.InputAction == null)
        {
            Debug.LogError("PlayerInputAction is null");
            return;
        }

        if (player.InputAction.AttackAction is null)
        {
            Debug.LogError("Attack Action is null");
            return;
        }

        player.InputAction.AttackAction.canceled += context =>
        {
            NewBasicAttackCommand basicAttackCommand = new NewBasicAttackCommand(this, Time.time);
            player.InputInvoker.AddInputCommand(basicAttackCommand);
        };
    }

    public void SetScript(BaseScript script)
    {
        this.script = script;

        BasicAttackContext baContext = new BasicAttackContext()
        {
/*            playerAnim = this.playerAnim,
            playerState = this.playerState,
            playerTarget = this.playerTarget,
            playerUtils = this.playerUtils,
            basicAttackCollider = this.basicAttackCollider,*/
            player = this.player,
            baEffectRed = this.baEffectRed,
            baEffectGreen = this.baEffectGreen,
            baEffectBlue = this.baEffectBlue,
/*            targetMarker = this.targetMarker,
            playerAttackController = this,
            playerDashController = this.playerDashController,
            playerSkillController = this.playerSkillController,*/
        };

        script.SetContext(baContext);
    }

    public void ExcuteBehaviour()
    {
        if (script == null)
        {
            Debug.LogError("BasicAttack script is not Assigned");
            return;
        }

        script.ExcuteBehaviour();
    }

    public void ExcuteAnim()
    {
        if (script == null)
        {
            Debug.LogError("BasicAttack script is not Assigned");
            return;
        }

        if (script is IAnimatedBasedScript animatedBasedScript)
        {
            animatedBasedScript.ExcuteAnim();
        }
    }

    public bool IsAnimatedBasedAttack()
    {
        return script is IAnimatedBasedScript;
    }
    public bool CanExcuteBehaviour()
    {
        if (script == null)
        {
            Debug.LogError("BasicAttack script is not Assigned");
            return false;
        }
        return script.CanExcuteBehaviour() && isAbleAttack;
    }

    public IEnumerator DelayedSetAttack()
    {
        yield return new WaitForSeconds(0.5f);
        isAbleAttack = true;
    }
    public void OnEvent(EVENT_TYPE eventType, Component Sender, object Param = null)
    {
        switch (eventType)
        {
            case EVENT_TYPE.Open_Panel_Exclusive:
            case EVENT_TYPE.Open_Panel_Stacked:
                PanelType nextPanel = (PanelType)Param;
                if (nextPanel == PanelType.HUD)
                    StartCoroutine(DelayedSetAttack());
                else
                    isAbleAttack = false;
                break;
            case EVENT_TYPE.Stage_Clear:
                isAbleAttack = false;
                break;
            case EVENT_TYPE.Stage_Start:
                NodeType nodeType = ((Node)Param).type;
                switch (nodeType)
                {
                    case NodeType.Treasure:
                    case NodeType.Comma:
                    case NodeType.Market:
                    case NodeType.Quest:
                        isAbleAttack = false;
                        break;
                    default:
                        StartCoroutine(DelayedSetAttack());
                        break;
                }
                break;
        }
    }
}
