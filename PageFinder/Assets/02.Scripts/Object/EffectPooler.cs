using UnityEngine;

public class EffectPooler : ObjectPooler<ParticleSystem, EffectPooler>
{
    public override void Awake()
    {
        base.Awake();

        defaultPoolCapacity = 10;
        maxPoolSize = 40;
    }


}
