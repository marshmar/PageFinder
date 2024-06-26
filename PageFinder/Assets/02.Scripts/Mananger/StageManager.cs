using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class StageManager : MonoBehaviour
{
    public Door[] DoorSrc = new Door[3];
    public Vector3[] stageStartPos = new Vector3[3];
    int currentStage = 0;

    bool[] clearStage = { false, false, false };
    GameObject Player;

    private void Start()
    {
        Player = GameObject.FindWithTag("PLAYER");
        Player.transform.position = stageStartPos[0]; // 플레이어 스테이지 1 시작 위치에서 시작
    }

    private void Update()
    {
        if (Player.transform.position.z <= -36) // 문 바깥으로 나갈 경우
            Player.transform.position = stageStartPos[currentStage];
    }


    public void ClearStage(int i)
    {
        if (i >= 3)
            i = 0; 

        currentStage = i;
        clearStage[i - 1] = true;
        DoorSrc[i-1].StartCoroutine(DoorSrc[i - 1].Open());
    }
}
