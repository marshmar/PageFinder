using System;
using UnityEngine;

public class NewPlayerAttackController : MonoBehaviour
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
        //inputInvoker = DebugUtils.GetComponentWithErrorLogging<PlayerInputInvoker>(this.gameObject, "PlayerInputInvoker");
        //inputAction = DebugUtils.GetComponentWithErrorLogging<PlayerInputAction>(this.gameObject, "PlayerInputAction");

        //playerAnim = DebugUtils.GetComponentWithErrorLogging<PlayerAnim>(this.gameObject, "PlayerAnim");
        //playerState = DebugUtils.GetComponentWithErrorLogging<PlayerState>(this.gameObject, "PlayerState");
        //playerUtils = DebugUtils.GetComponentWithErrorLogging<PlayerUtils>(this.gameObject, "PlayerUtils");
        //playerTarget = DebugUtils.GetComponentWithErrorLogging<PlayerTarget>(this.gameObject, "PlayerTarget");
        //targetMarker = DebugUtils.GetComponentWithErrorLogging<TargetObject>(this.gameObject, "TargetMarker");

        //playerDashController = DebugUtils.GetComponentWithErrorLogging<NewPlayerDashController>(this.gameObject, "NewPlayerDashController");
        //playerSkillController = DebugUtils.GetComponentWithErrorLogging<NewPlayerSkillController>(this.gameObject, "NewPlayerSkillController");
    }

    private void Start()
    {
        SetAttackAction();

        BaseScript baseScript = ScriptSystemManager.Instance.CreateScritByID(1);
        baseScript.CopyData(ScriptSystemManager.Instance.GetScriptByIDNew(1));
        SetScript(baseScript);
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
        return script.CanExcuteBehaviour();
    }
}
