using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;

public class BuffGenerator : Singleton<BuffGenerator>
{
    private Dictionary<BuffType, BuffFactory> buffFactories = new Dictionary<BuffType, BuffFactory>();

    private void Start()
    {
        buffFactories.Add(BuffType.BuffType_Permanent, new PermanentBuffFactory());
        buffFactories.Add(BuffType.BuffType_Temporary, new TemporaryBuffFactory());
        buffFactories.Add(BuffType.BuffType_Tickable, new TickableBuffFactory());
    }

    public BuffCommand CreateBuffCommand(in BuffData buffData)
    {
        if(!buffFactories.TryGetValue(buffData.buffType, out BuffFactory factory))
        {
            Debug.LogError($"지원되지 않는 buffType : {buffData.buffType}");
            return null;
        }
        BuffCommand buffCommand = factory.CreateBuffCommand(in buffData);
        return buffCommand;
    }
}
