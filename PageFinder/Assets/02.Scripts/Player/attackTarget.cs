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
    UtilsManager utilsManager;
    Transform tr;
    Collider attackEnemy;

    void Start()
    {
        utilsManager = UtilsManager.Instance;
        tr = GetComponent<Transform>();
    }
    private void OnEnable()
    {
        enemies = new List<Collider>();
    }

    private void Update()
    {
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
        attackEnemy = utilsManager.FindMinDistanceObject(tr.position, Enimes);
        return attackEnemy;
    }
}
