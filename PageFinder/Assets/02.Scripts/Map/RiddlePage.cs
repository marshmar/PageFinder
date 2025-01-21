using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class RiddlePage : Page
{
    // 0번째는 Target, 나머지는 일반 몹
    public string[] types = { "" };
    public Vector3[] spawnPos = { Vector3.zero };
    public float[] maxHp;
    public float[] playerCognitiveDist;// 플레이어 인지 거리
    public float[] fugitiveCognitiveDist; // 도망자 인지 거리
    public float[] moveDistance;
    public float[] moveSpeed;

    public Vector3[] rallyPoints;
}
