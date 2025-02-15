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
            case 9:
                if (buffData.targets[0] is PlayerState) Debug.Log("플레이어 스테이트 진입");
                else Debug.Log("이상한 값 진입");

                command = new ThickVine(buffData.targets[0] as PlayerState);
                break;
            case 14:
                command = new WaterConservation(buffData.targets[0] as PlayerDashController, buffData.targets[1] as PlayerSkillController);
                break;
            case 15:
                command = new DeepWell(buffData.targets[0] as PlayerState);
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
                command = new TemporaryMovementBuff(buffData.targets[0] as IEntityState, buffData.buffValue, buffData.duration);
                break;
        }

        return command;
    }
}
