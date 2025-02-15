using System;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public static event Action<Portal> OnPortalEnter;

    public static void Teleport(Vector3 target)
    {
        Debug.Log($"��Ż�� ���� {target}���� �̵��մϴ�.");
        GameObject.FindWithTag("PLAYER").transform.position = target;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PLAYER"))  // �÷��̾ ��Ż�� �������� ���� �̵�
        {
            Portal.OnPortalEnter?.Invoke(this);
        }
    }
}