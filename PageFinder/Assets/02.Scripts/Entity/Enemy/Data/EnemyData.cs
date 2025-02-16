using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Object/EnemyData")]
public class EnemyData : ScriptableObject
{
    public int id;
    
    public Enemy.Rank rank;
    public Enemy.EnemyType enemyType;
    public Enemy.PosType posType;
    public Enemy.Personality personality;
    public Enemy.PatrolType patrolType;
    public Enemy.AttackDistType attackDistType;
    public InkType inkType;

    public float hp;
    public float atk;
    public int cognitiveDist;
    public int inkTypeResistance;
    public int staggerResistance;

    public float atkSpeed;
    public float moveSpeed;
    public float firstWaitTime;
    public float attackWaitTime;

    public int dropItem; 

    public Vector3 spawnDir;
    public List<Vector3> destinations = new List<Vector3>();

    // 상급 이상인 경우
    public List<float> skillCoolTimes = new List<float>(); // 스킬 쿨타임 
    public List<int> skillPriority = new List<int>(); // 스킬 우선도
    public List<bool> skillConditions = new List<bool>();
}
