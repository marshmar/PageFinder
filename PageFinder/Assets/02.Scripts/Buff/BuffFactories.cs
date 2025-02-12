using NUnit.Framework.Constraints;
using NUnit.Framework.Internal.Execution;
using UnityEngine;

public class PermanentBuffFactory : BuffFactory
{
    public override BuffCommand CreateBuffCommand(ref BuffData buffData)
    {
        BuffCommand command = null;
        switch (buffData.buffId)
        {
            case 0:
                break;
        }

        return command;
    }
}

public class TemporaryBuffFactory : BuffFactory
{
    public override BuffCommand CreateBuffCommand(ref BuffData buffData)
    {
        BuffCommand command = null;
        switch (buffData.buffId)
        {
            case 0:
                command = new TemporaryMovementBuff(buffData.target as IEntityState, buffData.buffValue, buffData.duration);
                break;
        }

        return command;
    }
}
