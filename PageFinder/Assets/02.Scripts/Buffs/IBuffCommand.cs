using UnityEngine;

public enum BuffType
{
    BuffType_PermanentBuff,
    BuffType_PermanentMultiplier,
    BuffType_PermanentDebuff,
    BuffType_TemporaryBuff,
    BuffType_TemporaryDebuff
}

public abstract class IBuffCommand : ICommand
{
    public BuffType Type { get; set; }
    public float Value { get; set; }
}

// 일시적인 버프
public abstract class ITemporaryBuffCommand : IBuffCommand
{
    private float elapsedTime;
    private float duration;

    public float ElapsedTime { get; set; }
    public float Duration { get; set; } // -1: 영구적 버프

    public abstract void Tick();
    public abstract void EndBuff();
}
