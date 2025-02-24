using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerUtils : MonoBehaviour
{
    [SerializeField] private Transform modelTr;
    private Transform tr;
    private Rigidbody rigid;

    public Transform ModelTr { get => modelTr; set => modelTr = value; }
    public Transform Tr { get => tr; set => tr = value; }
    public Rigidbody Rigid { get => rigid; set => rigid = value; }

    // Start is called before the first frame update
    void Awake()
    {
        //modelTr = DebugUtils.GetComponentWithErrorLogging<Transform>(this.gameObject, "modelTr");
        tr = DebugUtils.GetComponentWithErrorLogging<Transform>(this.gameObject, "Transform");
        rigid = DebugUtils.GetComponentWithErrorLogging<Rigidbody>(this.gameObject, "Rigidbody");
    }

    public void TurnToDirection(Vector3 dir)
    {
        if (dir == Vector3.zero) return;
        //modelTr.rotation = Quaternion.LookRotation(new Vector3(dir.x, 0f, dir.z));
        modelTr.rotation = Quaternion.LookRotation(new Vector3(-dir.z, 0f, dir.x));
    }

    public Vector3 CalculateDirectionFromPlayer(Component component)
    {
        Vector3 dir = component.transform.position - Tr.position;
        return dir;
    }
}
