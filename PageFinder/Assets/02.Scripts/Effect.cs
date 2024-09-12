using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour
{
    public GameObject[] Prefabs = new GameObject[2];

    GameObject[] gameObjects = new GameObject[2];

    Transform playerTr;



    private void Start()
    {
        playerTr = GameObject.FindWithTag("PLAYER").GetComponent<Transform>();

    }

    public void PlayerAttack(Vector3 enemyPos)
    {
        gameObjects[0] = Instantiate(Prefabs[0], enemyPos, Quaternion.identity);
        Destroy(gameObjects[0], 0.5f);
    }

    public void JiruruAttack()
    {
        gameObjects[1] = Instantiate(Prefabs[1], playerTr.position + Vector3.up, Quaternion.identity);
        Destroy(gameObjects[1], 0.5f);
    }

}
