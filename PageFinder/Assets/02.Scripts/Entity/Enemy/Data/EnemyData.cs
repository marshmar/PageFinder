using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class EnemyData : ScriptableObject
{
    public int id;
    
    public Enemy.Rank rank;
    public Enemy.EnemyType enemyType;
    public Enemy.PosType posType;
    public Enemy.Personality personality;
    public Enemy.PatrolType patrolType;
    public InkType inkType;

    public int hp;
    public int atk;
    public int def;
    public int cognitiveDist;
    public int inkTypeResistance;
    public int staggerResistance;

    public float atkSpeed;
    public float moveSpeed;
    public float firstWaitTime;
    public float attackWaitTime;

    public int dropItem; // public Item.Type dropItem;

    public Vector3 spawnDir;
    public List<Vector3> destinations = new List<Vector3>();

    // 상급 이상인 경우
    public List<float> skillCoolTimes = new List<float>(); // 스킬 쿨타임 
    public List<int> skillPriority = new List<int>(); // 스킬 우선도
    public List<bool> skillConditions = new List<bool>(); // 스킬 조건
}
