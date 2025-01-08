using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MarkData")]
public class InkMarkData : ScriptableObject
{
    public Vector3 scale;
    public Sprite[] inkMarkImages;
}
