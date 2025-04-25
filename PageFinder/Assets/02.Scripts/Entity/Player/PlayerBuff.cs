using System.Runtime.CompilerServices;
using NUnit.Framework.Internal.Filters;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;

public class PlayerBuff : EntityBuff, IListener
{
    private PlayerState playerState;
    private PlayerAttackController playerAttackController;
    private PlayerDashController playerDashController;
    private PlayerSkillController playerSkillController;


    [SerializeField]
    public List<BuffCommand> activeCommands;

    private void Awake()
    {
        buffCommandInvoker = new BuffCommandInvoker();
        activeCommands = new List<BuffCommand>();

        playerAttackController = DebugUtils.GetComponentWithErrorLogging<PlayerAttackController>(this.gameObject, "PlayerAttrackController");
        playerDashController = DebugUtils.GetComponentWithErrorLogging<PlayerDashController>(this.gameObject, "PlayerDashController");
        playerSkillController = DebugUtils.GetComponentWithErrorLogging<PlayerSkillController>(this.gameObject, "PlayerSkillController");
        playerState = DebugUtils.GetComponentWithErrorLogging<PlayerState>(this.gameObject, "PlayerState");
    }

    private void Start()
    {
        EventManager.Instance.AddListener(EVENT_TYPE.Buff, this);
        EventManager.Instance.AddListener(EVENT_TYPE.Create_Script, this);
    }
    private void Update()
    {
        buffCommandInvoker.Update(Time.deltaTime);
    }

    public override void AddBuff(int buffID)
    {
        BuffCommand buffCommand = buffCommandInvoker.FindCommand(buffID);
        if (buffCommand == null)
        {
            BuffData buffData = new BuffData();
            buffCommand = BuffGenerator.Instance.CreateBuffCommand(ref buffData);
            buffCommandInvoker.AddCommand(buffCommand);
        }
    }

    public void AddBuff(BuffData buffData)
    {
        BuffCommand buffCommand = buffCommandInvoker.FindCommand(buffData.buffId);
        if (buffCommand == null)
        {
            buffCommand = BuffGenerator.Instance.CreateBuffCommand(ref buffData);
            buffCommandInvoker.AddCommand(buffCommand);
        }
    }

    public override void RemoveBuff(int buffID)
    {
        BuffCommand command = buffCommandInvoker.FindCommand(buffID);
        if (command != null)
        {
            buffCommandInvoker.RemoveCommand(command);
        }
    }

    public override void ChangeBuffLevel(int buffID, int level)
    {
        BuffCommand command = buffCommandInvoker.FindCommand(buffID);
        if (command != null)
        {
            buffCommandInvoker.ChangeCommandLevel(command, level);
        }
    }

    public void OnEvent(EVENT_TYPE eventType, Component sender, object Param)
    {
        switch (eventType)
        {
            case EVENT_TYPE.Buff:
                {
                    var buffId = (int)Param;
                    BuffCommand buffCommand = buffCommandInvoker.FindCommand(buffId);
                    if (buffCommand is null)
                    {
                        BuffData buffData = new BuffData();
                        buffCommand = BuffGenerator.Instance.CreateBuffCommand(ref buffData);
                    }
                    buffCommandInvoker.AddCommand(buffCommand);
                    break;
                }

            case EVENT_TYPE.Create_Script:
                {
                    var scriptParam = (System.Tuple<int, float>)Param;
                    BuffData buffData = CreateScriptDataById(scriptParam.Item1, scriptParam.Item2);
                    BuffCommand buffCommand = BuffGenerator.Instance.CreateBuffCommand(ref buffData);
                    buffCommandInvoker.AddCommand(buffCommand);
                    break;
                }

        }
    }

    public BuffData CreateScriptDataById(int buffId, float value)
    {
        BuffData buffData = new BuffData();
        switch (buffId)
        {
            case 1: // ºÒ²É ÀÏ°Ý
                buffData = new BuffData(BuffType.BuffType_Script, buffId, value, targets: new List<Component>() { playerState });
                Debug.Log("ºÒ²É ÀÏ°Ý °­È­");
                break;
            case 9: // ¾ï¼¾ µ¢Äð
                buffData = new BuffData(BuffType.BuffType_Script, buffId, value, targets: new List<Component>() { playerState });
                Debug.Log("¾ï¼¾ µ¢Äð °­È­");
                break;
            case 14: // ¹° Àý¾à
                buffData = new BuffData(BuffType.BuffType_Script, buffId, value, targets: new List<Component>() { playerDashController, playerSkillController });
                Debug.Log("¹° Àý¾à °­È­");
                break;
            case 15: // ±íÀº ¿ì¹°
                buffData = new BuffData(BuffType.BuffType_Script, buffId, value, targets: new List<Component>() { playerState });
                Debug.Log("±íÀº ¿ì¹° °­È­");
                break;

        }

        return buffData;
    }
}
