using UnityEngine;

public class VineStrike : BAScript, IAnimatedBasedScript
{
    public VineStrike()
    {
        scriptBehaviour = new BasicAttackBehaviour();
    }

    public virtual void ExcuteAnim()
    {
        scriptBehaviour.ExcuteAnim();
    }
}
