using UnityEngine;

public enum BuffType
{
    BuffType_PermanentBuff,
    BuffType_PermanentMultiplier,
    BuffType_PermanentDebuff,
    BuffType_TemporaryBuff,
    BuffType_TemporaryDebuff
}

public interface IBuffCommand : ICommand
{
    public BuffType Type { get; set; }
    public float Value { get; set; }

}

// 일시적인 버프
public interface ITemporaryBuffCommand : IBuffCommand
{
    public float Duration { get; set; } // -1: 영구적 버프

    public void Tick();
}
