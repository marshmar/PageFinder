using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// 플레이어 기본 이동에 관한 인터페이스
public interface IPlayerController
{
    // Move
    void OnMove(InputValue value);

    // Attack
    void OnAttack(InputValue value);

}
