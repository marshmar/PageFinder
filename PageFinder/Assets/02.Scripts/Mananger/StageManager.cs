using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class StageManager : MonoBehaviour
{
    public Door[] DoorSrc = new Door[3];
    public Vector3[] stageStartPos = new Vector3[3];
    int currentStage = 0;

    GameObject Player;

    private void Start()
    {
        Player = GameObject.FindWithTag("PLAYER");
        Player.transform.position = stageStartPos[0]; // 플레이어 스테이지 1 시작 위치에서 시작
    }

    private void Update()
    {
        if (Player.transform.position.z <= -36) // 해당 스테이지 클리어 후 문 바깥으로 나갈 경우
        {
            if (currentStage >= 3) // 마지막 스테이지
                currentStage = 0; // 맨 처음 스테이지로 이동하게 설정

            Debug.Log("이동할 스테이지 : "+stageStartPos[currentStage]);
            Player.transform.position = stageStartPos[currentStage];
        }
            
    }


    public void ClearStage(int i)
    {
        currentStage = i+1;
        DoorSrc[i].StartCoroutine(DoorSrc[i].Open());
    }
}
