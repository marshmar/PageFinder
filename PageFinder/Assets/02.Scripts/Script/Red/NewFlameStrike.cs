using UnityEngine;

public class NewFlameStrike : BAScript, IAnimatedBasedScript
{
    public NewFlameStrike()
    {
        scriptBehaviour = new BasicAttackBehaviour();
    }

    public virtual void ExcuteAnim()
    {
        scriptBehaviour.ExcuteAnim();
    }
}
