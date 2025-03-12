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
        Debug.Log($"Go to {target} through the portal.");
        GameObject.FindWithTag("PLAYER").transform.position = target;
    }
}