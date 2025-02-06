using UnityEngine;

/// <summary>
/// 공통 프로퍼티 지정 필요
/// </summary>
public interface IEntityState
{
    public float MaxHp { get; set; }
    public float CurHp { get; set; }
    public float Atk { get; set; }
    public float CurMoveSpeed { get; set; }
    public float CurAttackSpeed { get; set; }
}
