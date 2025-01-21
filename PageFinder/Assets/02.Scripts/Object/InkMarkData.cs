using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MarkData")]
public class InkMarkData : ScriptableObject
{
    public float duration;
    public Vector3 scale;
    public Sprite[] inkMarkImages;  // 0: Red, 1: Green, 2: Blue, 3: Fire, 4: Mist, 5: Swamp
}
