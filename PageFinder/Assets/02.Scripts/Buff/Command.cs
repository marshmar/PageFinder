using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public abstract class Command : MonoBehaviour
{
    public abstract void Execute();
}

public abstract class BuffCommand : Command
{
    public int buffID;
    public bool active;
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
}

public abstract class TemporaryBuffCommand : BuffCommand
{
    private float elapsedTime;
    private float duration;

    public virtual float ElapsedTime { get; set; }
    public virtual float Duration { get; set; }

    public virtual void Tick(float deltaTime)
    {
        elapsedTime += deltaTime;
        if(elapsedTime > duration && active)
        {
            active = false;
            elapsedTime = 0f;
            EndBuff();
        }
    }
    public abstract void EndBuff();
}
