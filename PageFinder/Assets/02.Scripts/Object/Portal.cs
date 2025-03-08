using System;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public static event Action<Portal> OnPortalEnter;

    public static void Teleport(Vector3 target)
    {
        Debug.Log($"Go to {target} through the portal.");
        GameObject.FindWithTag("PLAYER").transform.position = target;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PLAYER"))  // Move only when the player approaches the portal
        {
            Portal.OnPortalEnter?.Invoke(this);
        }
    }
}