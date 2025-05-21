using System;
using UnityEngine;
using System.Collections.Generic;

public class NewPlayerDashController : MonoBehaviour
{
    private PlayerState playerState;
    private PlayerUtils playerUtils;
    private PlayerAnim playerAnim;
    private PlayerAttackController playerAttackControllerScr;
    private PlayerSkillController playerSkillController;
    private PlayerTarget playerTarget;
    private PlayerInputInvoker inputInvoker;

    private PlayerInputAction inputAction;
    private bool chargingDash = false;
    private BaseScript script;
    private bool isDashing = false;
    public Action FixedUpdateDashAction { get; set; }
    public bool IsDashing { get => isDashing; set => isDashing = value; }

    private void Awake()
    {

        playerAttackControllerScr = DebugUtils.GetComponentWithErrorLogging<PlayerAttackController>(this.gameObject, "PlayerAttackController");
        playerSkillController = DebugUtils.GetComponentWithErrorLogging<PlayerSkillController>(this.gameObject, "PlayerSkillController");
        playerState = DebugUtils.GetComponentWithErrorLogging<PlayerState>(this.gameObject, "PlayerState");
        playerUtils = DebugUtils.GetComponentWithErrorLogging<PlayerUtils>(this.gameObject, "PlayerUtils");
        playerAnim = DebugUtils.GetComponentWithErrorLogging<PlayerAnim>(this.gameObject, "PlayerAnim");
        playerTarget = DebugUtils.GetComponentWithErrorLogging<PlayerTarget>(this.gameObject, "PlayerTarget");
        inputAction = DebugUtils.GetComponentWithErrorLogging<PlayerInputAction>(this.gameObject, "PlayerInputAction");
        inputInvoker = DebugUtils.GetComponentWithErrorLogging<PlayerInputInvoker>(this.gameObject, "PlayerInputInvoker");

    }

    private void Start()
    {
        SetDashAction();

        SetScript(new ChargableDashScriipt());
    }

    private void SetDashAction()
    {
        if (inputAction == null)
        {
            Debug.LogError("PlayerInput 컴포넌트가 존재하지 않습니다.");
            return;
        }

        if (inputAction.DashAction == null)
        {
            Debug.LogError("DashAction이 존재하지 않습니다.");
            return;
        }

        inputAction.DashAction.started += context =>
        {

        };

        inputAction.DashAction.performed += context =>
        {
            chargingDash = true;
        };

        inputAction.DashAction.canceled += context =>
        {
            NewDashCommand dashCommand = new NewDashCommand(this, Time.time);
            inputInvoker.AddInputCommand(dashCommand);
        };

        if (inputAction.CancelAction is null)
        {
            Debug.LogError("CancelAction이 존재하지 않습니다.");
            return;
        }

        inputAction.CancelAction.started += context =>
        {
            chargingDash = false;
            //dashCanceld = true;
            playerTarget.OffAllTargetObjects();
        };
    }

    public bool CanExcuteBehaviour()
    {
        return script.CanExcuteBehaviour();
    }

    public void ExcuteBehaviour()
    {
        script.ExcuteBehaviour();
        chargingDash = false;
    }

    private void FixedUpdate()
    {
        FixedUpdateDashAction?.Invoke();
    }
    private void Update()
    {
        if (chargingDash)
        {
            if(script is ChargableDashScriipt chargableDashScript)
            {
                chargableDashScript.ChargeBehaviour();
            }
        }
    }

    public void SetScript(BaseScript script)
    {
        this.script = script;

        NewScriptData scriptData = ScriptableObject.CreateInstance<NewScriptData>();
        scriptData.scriptID = 2;
        scriptData.scriptType = NewScriptData.ScriptType.Dash;
        scriptData.inkCost = 40;
        scriptData.scriptName = "잉크 대쉬";
        scriptData.scriptDesc = "하이";
        scriptData.inkType = InkType.RED;
        scriptData.rarity = 0;
        scriptData.maxRarity = 3;
        scriptData.price = new int[4] { 0, 120, 240, 360 };
        scriptData.levelData = new float[4] { 0.01f, 0.05f, 0.1f, 0.5f };

        script.CopyData(scriptData);

        DashContext baContext = new DashContext()
        {
            playerAnim = this.playerAnim,
            playerState = this.playerState,
            playerTarget = this.playerTarget,
            playerUtils = this.playerUtils,
            playerDashController = this
        };

        script.SetContext(baContext);
    }
}
