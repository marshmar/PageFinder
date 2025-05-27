using System;
using UnityEngine;

public class NewPlayerSkillController : MonoBehaviour, IListener
{
    private Player player;
    private BaseScript script;


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

        EventManager.Instance.AddListener(EVENT_TYPE.Open_Panel_Exclusive, this);
        EventManager.Instance.AddListener(EVENT_TYPE.Open_Panel_Stacked, this);
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

        if (isUsingSkill)
        {
            player.Anim.CheckAnimProgress("Player_Skill_Turning", 0.8f, ref isUsingSkill);
            if (!isUsingSkill)
            {
                        player.MoveController.CanMove = true;
        player.MoveController.MoveTurn = true;
            }
        }
    }
    private void SetSkillAction()
    {
        if (player.InputAction is null)
        {
            Debug.LogError("PlayerInput 컴포넌트가 존재하지 않습니다.");
            return;
        }

        if (player.InputAction.SkillAction is null)
        {
            Debug.LogError("Skill Action이 존재하지 않습니다.");
            return;
        }

        player.InputAction.SkillAction.started += context =>
        {

        };

        player.InputAction.SkillAction.performed += context =>
        {
            if (script.CanExcuteBehaviour())
                isChargingSkill = true;
        };

        player.InputAction.SkillAction.canceled += context =>
        {
            NewSkillCommand skillCommand = new NewSkillCommand(this, Time.time);
            player.InputInvoker.AddInputCommand(skillCommand);
        };

        if (player.InputAction.CancelAction is null)
        {
            Debug.LogError("Cancel Action이 존재하지 않습니다.");
            return;
        }

        player.InputAction.CancelAction.started += context =>
        {
            player.Target.OffAllTargetObjects();
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

        SkillContext skillContext = new SkillContext()
        {
            player = this.player,
        };

        script.SetContext(skillContext);
    }

    public void OnEvent(EVENT_TYPE eventType, Component Sender, object Param = null)
    {
        switch (eventType)
        {
            case EVENT_TYPE.Open_Panel_Exclusive:
            case EVENT_TYPE.Open_Panel_Stacked:
                isUsingSkill = false;
                break;
        }
    }
}
