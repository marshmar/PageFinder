using System;
using UnityEngine;

public class NewPlayerSkillController : MonoBehaviour, IListener
{
    private Player player;
    private BaseScript script;

    private bool canUseSkill = true;
    private bool isChargingSkill = false;
    private bool skillCanceled = false;
    private bool isUsingSkill = false;
    public bool IsChargingSkill { get => isChargingSkill; set => isChargingSkill = value; }
    public bool IsUsingSkill { get => isUsingSkill; set => isUsingSkill = value; }

    public float SkillCost
    {
        get
        {
            if(script != null && script is SkillScript skillScript)
            {
                return skillScript.SkillCost;   
            }

            return 0f;
        }
    }

    public float SkillCoolTime
    {
        get
        {
            if (script != null && script is SkillScript skillScript)
            {
                return skillScript.SkillCoolTime;
            }

            return 0f;
        }
    }

    private void Awake()
    {
        player = DebugUtils.GetComponentWithErrorLogging<Player>(this.gameObject, "Player");

        AddListener();
    }

    private void Start()
    {
        InitializeSkillAction();
        InitializeCancelAction();
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

        if (isUsingSkill)
        {
            isUsingSkill = player.Anim.HasAnimPassedTime("Player_Skill_Turning", 0.8f);
            if (!isUsingSkill)
            {
                player.MoveController.CanMove = true;
                player.MoveController.MoveTurn = true;
            }
        }
    }

    private void OnDestroy()
    {
        RemoveListener();
    }

    private void InitializeSkillAction()
    {
        var skillAction = player.InputAction.GetInputAction(PlayerInputActionType.Skill);
        if (skillAction == null)
        {
            Debug.LogError("Skill Action is null");
            return;
        }


        skillAction.started += context =>
        {

        };

        skillAction.performed += context =>
        {
            if (script.CanExcuteBehaviour())
                isChargingSkill = true;
        };

        skillAction.canceled += context =>
        {
            SkillCommand skillCommand = new SkillCommand(this, Time.time);
            player.InputInvoker.AddInputCommand(skillCommand);
        };

        
    }

    private void InitializeCancelAction()
    {
        var cancelAction = player.InputAction.GetInputAction(PlayerInputActionType.Cancel);
        if (cancelAction == null)
        {
            Debug.LogError("Cancel Action is null.");
            return;
        }

        cancelAction.started += context =>
        {
            player.TargetingVisualizer.OffAllTargetObjects();
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
        return script.CanExcuteBehaviour() && canUseSkill;
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

        SkillContext skillContext = new SkillContext()
        {
            player = this.player,
        };

        script.SetContext(skillContext);
    }

    public void AddListener()
    {
        EventManager.Instance.AddListener(EVENT_TYPE.Open_Panel_Exclusive, this);
        EventManager.Instance.AddListener(EVENT_TYPE.Open_Panel_Stacked, this);
        EventManager.Instance.AddListener(EVENT_TYPE.InkDashWating, this);
        EventManager.Instance.AddListener(EVENT_TYPE.InkDashTutorialCleared, this);
    }

    public void RemoveListener()
    {
        EventManager.Instance.RemoveListener(EVENT_TYPE.Open_Panel_Exclusive, this);
        EventManager.Instance.RemoveListener(EVENT_TYPE.Open_Panel_Stacked, this);
        EventManager.Instance.RemoveListener(EVENT_TYPE.InkDashWating, this);
        EventManager.Instance.RemoveListener(EVENT_TYPE.InkDashTutorialCleared, this);
    }

    public void OnEvent(EVENT_TYPE eventType, Component Sender, object Param = null)
    {
        switch (eventType)
        {
            case EVENT_TYPE.Open_Panel_Exclusive:
            case EVENT_TYPE.Open_Panel_Stacked:
                isUsingSkill = false;
                break;
            case EVENT_TYPE.InkDashWating:
                canUseSkill = false;
                break;
            case EVENT_TYPE.InkDashTutorialCleared:
                canUseSkill = true;
                break;
        }
    }
}
