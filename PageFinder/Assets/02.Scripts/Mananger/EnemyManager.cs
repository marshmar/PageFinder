using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public GameObject[] Enemy = new GameObject[5]; // 0~3 가오리, 4 상어

    bool[] isFirstClear = { false, false };


    StageManager stageManager;
    // Start is called before the first frame update
    void Start()
    {
        stageManager = GameObject.Find("StageManager").GetComponent<StageManager>();
    }

    private void Update()
    {
        CheckIfEnemyIsDied();

    }

    void CheckIfEnemyIsDied()
    {
        for(int i=0; i<Enemy.Length; i++)
        {
            if (Enemy[i] != null)
                break;

            if (i == 3 && !isFirstClear[0])
            {
                Debug.Log("2스테이지 클리어");
                isFirstClear[0] = true;
                stageManager.ClearStage(1);
            }
            else if (i==4 && !isFirstClear[1])
            {
                Debug.Log("3스테이지 클리어");
                isFirstClear[1] = true;
                stageManager.ClearStage(2);
            }
        }
    }
}
