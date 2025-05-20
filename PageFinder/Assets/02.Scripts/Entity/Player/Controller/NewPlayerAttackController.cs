using System;
using UnityEngine;

public class NewPlayerAttackController : MonoBehaviour
{
    private BaseScript script;

    private PlayerInputInvoker inputInvoker;
    private PlayerInputAction inputAction;
    private PlayerAnim playerAnim;
    private PlayerUtils playerUtils;
    private PlayerTarget playerTarget;
    private TargetObject targetMarker;
    private PlayerState playerState;
    private bool isAttacking = false;
    private int comboCount = 0;
    private bool isNextAttackBuffered = false;

    [SerializeField] private PlayerBasicAttackCollider basicAttackCollider;

    [Header("Effects")]
    [SerializeField] private GameObject[] baEffectRed;
    [SerializeField] private GameObject[] baEffectGreen;
    [SerializeField] private GameObject[] baEffectBlue;

    public bool IsAttacking { get => isAttacking; set => isAttacking = value; }
    public int ComboCount { get => comboCount; set => comboCount = value; }
    public bool IsNextAttackBuffered { get => isNextAttackBuffered; set => isNextAttackBuffered = value; }

    private void Awake()
    {
        inputInvoker = DebugUtils.GetComponentWithErrorLogging<PlayerInputInvoker>(this.gameObject, "PlayerInputInvoker");
        inputAction = DebugUtils.GetComponentWithErrorLogging<PlayerInputAction>(this.gameObject, "PlayerInputAction");

        playerAnim = DebugUtils.GetComponentWithErrorLogging<PlayerAnim>(this.gameObject, "PlayerAnim");
        playerState = DebugUtils.GetComponentWithErrorLogging<PlayerState>(this.gameObject, "PlayerState");
        playerUtils = DebugUtils.GetComponentWithErrorLogging<PlayerUtils>(this.gameObject, "PlayerUtils");
        playerTarget = DebugUtils.GetComponentWithErrorLogging<PlayerTarget>(this.gameObject, "PlayerTarget");
        targetMarker = DebugUtils.GetComponentWithErrorLogging<TargetObject>(this.gameObject, "TargetMarker");
    }

    private void Start()
    {
        SetAttackAction();

        SetScript(new BAScript());
    }

    private void SetAttackAction()
    {
        if (inputAction == null)
        {
            Debug.LogError("PlayerInputAction is null");
            return;
        }

        if (inputAction.AttackAction is null)
        {
            Debug.LogError("Attack Action is null");
            return;
        }

        inputAction.AttackAction.canceled += context =>
        {
            NewBasicAttackCommand basicAttackCommand = new NewBasicAttackCommand(this, Time.time);
            inputInvoker.AddInputCommand(basicAttackCommand);
        };
    }

    public void SetScript(BaseScript script)
    {
        this.script = script;

        NewScriptData scriptData = ScriptableObject.CreateInstance<NewScriptData>();
        scriptData.scriptID = 1;
        scriptData.scriptType = NewScriptData.ScriptType.BasicAttack;
        scriptData.inkCost = 0;
        scriptData.scriptName = "∫“≤… ¿œ∞›";
        scriptData.scriptDesc = "æ»≥Á";
        scriptData.inkType = InkType.RED;
        scriptData.rarity = 0;
        scriptData.maxRarity = 3;
        scriptData.price = new int[4] { 0, 120, 240, 360 };
        scriptData.levelData = new float[4] { 0.01f, 0.05f, 0.1f, 0.5f } ;

        script.CopyData(scriptData);

        BasicAttackContext baContext = new BasicAttackContext()
        {
            playerAnim = this.playerAnim,
            playerState = this.playerState,
            playerTarget = this.playerTarget,
            playerUtils = this.playerUtils,
            basicAttackCollider = this.basicAttackCollider,
            baEffectRed = this.baEffectRed,
            baEffectGreen = this.baEffectGreen,
            baEffectBlue = this.baEffectBlue,
            targetMarker = this.targetMarker,
            playerAttackController = this
        };

        script.SetContext(baContext);
    }

    public void ExcuteBehaviour()
    {
        script.ExcuteBehaviour();
    }

    public void ExcuteAnim()
    {
        Debug.Log("Excute Anim");
        script.ExcuteAnim();
    }

    public bool CanExcuteBehaviour()
    {
        return script.CanExcuteBehaviour();
    }
}
