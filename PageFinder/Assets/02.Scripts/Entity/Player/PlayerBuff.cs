using System.Runtime.CompilerServices;
using NUnit.Framework.Internal.Filters;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerBuff : MonoBehaviour, IListener
{
    private BuffCommandInvoker buffCommandInvoker;

    private void Awake()
    {
        buffCommandInvoker = new BuffCommandInvoker();
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
                var buffData = (BuffData)Param;
                BuffCommand buffCommand = buffCommandInvoker.FindCommand(buffData.buffId);
                if(buffCommand is null)
                {
                    buffCommand = BuffGenerator.Instance.CreateBuffCommand(ref buffData);
                }
                buffCommandInvoker.AddCommand(buffCommand);
                break;
        }
    }
}
