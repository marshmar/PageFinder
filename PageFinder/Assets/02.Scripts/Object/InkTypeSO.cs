using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Object/Ink/Type", fileName = "TypeData")]
public class InkTypeSO : ScriptableObject
{
    public Sprite[] images;  // 0: Red, 1: Green, 2: Blue, 3: Fire, 4: Mist, 5: Swamp
}