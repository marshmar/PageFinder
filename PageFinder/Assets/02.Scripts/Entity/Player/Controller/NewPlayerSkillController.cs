using System;
using UnityEngine;

public class NewPlayerSkillController : MonoBehaviour
{
    private PlayerAnim playerAnim;
    private PlayerState playerState;
    private PlayerUtils playerUtils;
    private PlayerInputAction inputAction;
    private PlayerTarget playerTarget;
    private PlayerInputInvoker inputInvoker;
    private BaseScript script;

    private bool isChargingSkill = false;
    private bool skillCanceled = false;

    public bool IsChargingSkill { get => isChargingSkill; set => isChargingSkill = value; }

    private void Awake()
    {
        playerState = DebugUtils.GetComponentWithErrorLogging<PlayerState>(this.gameObject, "PlayerState");
        playerAnim = DebugUtils.GetComponentWithErrorLogging<PlayerAnim>(this.gameObject, "PlayerAnim");
        playerUtils = DebugUtils.GetComponentWithErrorLogging<PlayerUtils>(this.gameObject, "PlayerUtils");
        playerTarget = DebugUtils.GetComponentWithErrorLogging<PlayerTarget>(this.gameObject, "PlayerTarget");

        inputAction = DebugUtils.GetComponentWithErrorLogging<PlayerInputAction>(this.gameObject, "PlayerInputAction");
        inputInvoker = DebugUtils.GetComponentWithErrorLogging<PlayerInputInvoker>(this.gameObject, "PlayerInputInvoker");
    }

    private void Start()
    {
        SetSkillAction();
    }

    private void Update()
    {
        if (isChargingSkill)
        {
            if(script is IChargableScript chargableSkillScript)
            {
                chargableSkillScript.ChargeBehaviour();
            }
        }
    }
    private void SetSkillAction()
    {
        if (inputAction is null)
        {
            Debug.LogError("PlayerInput 컴포넌트가 존재하지 않습니다.");
            return;
        }

        if (inputAction.SkillAction is null)
        {
            Debug.LogError("Skill Action이 존재하지 않습니다.");
            return;
        }

        inputAction.SkillAction.started += context =>
        {

        };

        inputAction.SkillAction.performed += context =>
        {
            if (script.CanExcuteBehaviour())
                isChargingSkill = true;
        };

        inputAction.SkillAction.canceled += context =>
        {
            NewSkillCommand skillCommand = new NewSkillCommand(this, Time.time);
            inputInvoker.AddInputCommand(skillCommand);
        };

        if (inputAction.CancelAction is null)
        {
            Debug.LogError("Cancel Action이 존재하지 않습니다.");
            return;
        }

        inputAction.CancelAction.started += context =>
        {
            playerTarget.OffAllTargetObjects();
            isChargingSkill = false;
            skillCanceled = true;
        };
    }

    public bool CanExcuteBehaviour()
    {
        if (script == null)
        {
            Debug.LogError("Skill script is not Assigned");
            return false;
        }
        return script.CanExcuteBehaviour();
    }

    public void ExcuteBehaviour()
    {
        if (script == null)
        {
            Debug.LogError("Skill script is not Assigned");
            return;
        }

        script.ExcuteBehaviour();
    }

    public void SetScript(BaseScript script)
    {
        this.script = script;

        SkillContext baContext = new SkillContext()
        {
            playerAnim = this.playerAnim,
            playerState = this.playerState,
            playerTarget = this.playerTarget,
            playerUtils = this.playerUtils,
            playerSkillController = this
        };

        script.SetContext(baContext);
    }
}
