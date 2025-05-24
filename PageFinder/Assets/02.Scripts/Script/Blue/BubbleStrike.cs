using UnityEngine;

public class BubbleStrike : BAScript, IAnimatedBasedScript
{
    public BubbleStrike()
    {
        scriptBehaviour = new BasicAttackBehaviour();
    }

    public void ExcuteAnim()
    {
        scriptBehaviour.ExcuteAnim();
    }
}
