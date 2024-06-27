using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonManager : MonoBehaviour
{
    public GameObject[] Balloon = new GameObject[4];

    bool firstClear = false;

    StageManager stageManager;

    private void Start()
    {
        stageManager = GameObject.Find("StageManager").GetComponent<StageManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!firstClear && CheckIfAllBalloonColorIsRed())
        {
            firstClear = true;
            stageManager.ClearStage(0);
        }
    }

    bool CheckIfAllBalloonColorIsRed()
    {
        for(int i=0; i< Balloon.Length; i++)
        {
            if (Balloon[i].GetComponent<MeshRenderer>().material.color != Color.red)
                return false;
        }
        return true;
    }
}
