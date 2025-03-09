using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Portal : MonoBehaviour, IInteractable
{
    public static event Action<Portal> OnPortalEnter;

    public Action<InputAction.CallbackContext> InteractAction()
    {
        return context => OnPortalEnter?.Invoke(this);
    }

    public static void Teleport(Vector3 target)
    {
        Debug.Log($"포탈을 통해 {target}으로 이동합니다.");
        GameObject.FindWithTag("PLAYER").transform.position = target;
    }

/*    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PLAYER"))  // 플레이어가 포탈에 접근했을 때만 이동
        {
            Portal.OnPortalEnter?.Invoke(this);
        }
    }*/
}