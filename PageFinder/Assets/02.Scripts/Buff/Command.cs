using System.Collections.Generic;
using UnityEngine;

public abstract class Command
{
    public abstract void Execute();
}

public abstract class BuffCommand : Command
{
    public int buffId;
    public bool active = true;
    private float buffValue;
    protected BuffType buffType;
    public BuffType BuffType {  get; set; }
    public virtual float BuffValue 
    {
        get
        {
            return buffValue;
        }
        set
        {
            buffValue = value;
        }
    }

    public abstract void EndBuff();
}

public interface ITemporary
{
    public float ElapsedTime { get; set; }
    public float Duration { get; }

    public void Update(float deltaTime);
}

public interface ITickable
{
    public float ElapsedTime { get; set; }
    public float TickThreshold { get; set; }
    public void Tick(float deltaTime);
}
