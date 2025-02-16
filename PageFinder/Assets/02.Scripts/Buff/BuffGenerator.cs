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
        buffFactories.Add(BuffType.BuffType_Script, new ScriptBuffFactory());
    }

    public BuffCommand CreateBuffCommand(ref BuffData buffData)
    {
        if(!buffFactories.TryGetValue(buffData.buffType, out BuffFactory factory))
        {
            Debug.LogError($"지원되지 않는 buffType : {buffData.buffType}");
            return null;
        }
        BuffCommand buffCommand = factory.CreateBuffCommand(ref buffData);
        return factory.CreateBuffCommand(ref buffData);
    }
}
