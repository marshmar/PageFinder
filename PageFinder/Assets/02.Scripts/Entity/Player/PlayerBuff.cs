using System.Runtime.CompilerServices;
using NUnit.Framework.Internal.Filters;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;

public class PlayerBuff : EntityBuff IListener
{
    private BuffCommandInvoker buffCommandInvoker;
    private PlayerState playerState;
    private PlayerAttackController playerAttackController;
    private PlayerDashController playerDashController;
    private PlayerSkillController playerSkillController;

    private void Awake()
    {
        buffCommandInvoker = new BuffCommandInvoker();
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

    private void RemoveBuff(int buffID)
    {
        BuffCommand command = buffCommandInvoker.FindCommand(buffID);
        if (command is not null)
        {
            buffCommandInvoker.RemoveCommand(command);
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
            case 1: // �Ҳ� �ϰ�
                buffData = new BuffData(BuffType.BuffType_Script, buffId, value, targets: new List<Component>() { playerState });
                Debug.Log("�Ҳ� �ϰ� ��ȭ");
                break;
            case 9: // �＾ ����
                buffData = new BuffData(BuffType.BuffType_Script, buffId, value, targets: new List<Component>() { playerState });
                Debug.Log("�＾ ���� ��ȭ");
                break;
            case 14: // �� ����
                buffData = new BuffData(BuffType.BuffType_Script, buffId, value, targets: new List<Component>() { playerDashController, playerSkillController });
                Debug.Log("�� ���� ��ȭ");
                break;
            case 15: // ���� �칰
                buffData = new BuffData(BuffType.BuffType_Script, buffId, value, targets: new List<Component>() { playerState });
                Debug.Log("���� �칰 ��ȭ");
                break;

        }

        return buffData;
    }
}
