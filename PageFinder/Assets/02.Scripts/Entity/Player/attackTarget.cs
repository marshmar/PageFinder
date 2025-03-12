using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class attackTarget : MonoBehaviour
{
    private List<Collider> enemies;
    public List<Collider> Enimes
    {
        get
        {
            return enemies;
        }
    }

    Transform tr;
    Collider attackEnemy;

    void Start()
    {
        tr = GetComponent<Transform>();
    }
    private void OnEnable()
    {
        enemies = new List<Collider>();
    }


    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("ENEMY"))
        {
            if (enemies.Contains(other)) return;
            enemies.Add(other);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("ENEMY"))
        {
            enemies.Remove(other);
        }
    }

    public Collider GetClosestEnemy()
    {
        if (Enimes == null || Enimes.Count == 0) return null;
        attackEnemy = Utils.FindMinDistanceObject(tr.position, Enimes);
        return attackEnemy;
    }
}
