using UnityEngine;

public enum StickerType
{
    Dedicated,
    General
}

public enum DedicatedStickerTarget
{
    BasicAttack,
    Dash,
    Skill
}

[CreateAssetMenu(fileName = "StickerData", menuName = "ScriptSystem/StickerData")]
public class StickerData : ScriptableObject
{
    
}
