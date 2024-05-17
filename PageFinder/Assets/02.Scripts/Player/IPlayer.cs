using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayer
{

    // 대상과의 방향 구하기
    public Vector3 CaculateDirection(Collider goalObj);
    // 지정 방향으로 회전
    public void TurnToDirection(Vector3 dir);
}
