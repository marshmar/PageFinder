using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BattlePage : Page
{
    // 일단 이용하기 쉽게 public으로 설정
    public string[] types = { "" };

    public Vector3[] spawnPos = { Vector3.zero };

    public Vector3[] dir = { Vector3.zero };

    public int[] moveDist = { 0 };

    public int[] maxHP;
    public int[] atk;
    public float[] atkSpeed;
}
