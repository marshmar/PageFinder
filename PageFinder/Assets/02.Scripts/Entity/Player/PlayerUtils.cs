using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerUtils : MonoBehaviour
{
    [SerializeField] private Transform modelTr;
    private Transform tr;
    private Rigidbody rigid;
    private PlayerAnim playerAnim;
    private Transform playerSpine; 
    public Transform ModelTr { get => modelTr; set => modelTr = value; }
    public Transform Tr { get => tr; set => tr = value; }
    public Rigidbody Rigid { get => rigid; set => rigid = value; }

    public float rotateSpeed = 30f;

    private bool rotateSpine = false;
    private Quaternion newRot;
    // Start is called before the first frame update
    void Awake()
    {
        //modelTr = DebugUtils.GetComponentWithErrorLogging<Transform>(this.gameObject, "modelTr");
        tr = DebugUtils.GetComponentWithErrorLogging<Transform>(this.gameObject, "Transform");
        rigid = DebugUtils.GetComponentWithErrorLogging<Rigidbody>(this.gameObject, "Rigidbody");
        playerAnim = DebugUtils.GetComponentWithErrorLogging<PlayerAnim>(this.gameObject, "PlayerAnim");
    }

    public void TurnToDirection(Vector3 dir, bool smoothRot = false)
    {
        if (dir == Vector3.zero) return;


        if (smoothRot)
        {
            Quaternion newRot = Quaternion.LookRotation(new Vector3(dir.x, 0f, dir.z));
            //rigid.MoveRotation(Quaternion.Slerp(modelTr.rotation, newRot, rotateSpeed * Time.deltaTime));
            modelTr.rotation = Quaternion.Slerp(modelTr.rotation, newRot, rotateSpeed * Time.deltaTime);
        }
        else /*rigid.MoveRotation(Quaternion.LookRotation(new Vector3(dir.x, 0f, dir.z)));*/modelTr.rotation = Quaternion.LookRotation(new Vector3(dir.x, 0f, dir.z));
    }

    public void RotateSpine()
    {

        playerSpine = playerAnim.GetPlayerSpine();
        if (playerSpine != null)
        {
            playerSpine.rotation = newRot;
        }
        else
            Debug.LogError("Spine is null");
    }

    public void SetSpineRotation(bool val, Vector3 dir)
    {
        if (dir == Vector3.zero) return;

        rotateSpine = val;
        newRot = Quaternion.LookRotation(new Vector3(dir.x, 0f, dir.z));
    }

    private void LateUpdate()
    {
        //RotateSpine();
    }

    public Vector3 CalculateDirectionFromPlayer(Component component)
    {
        Vector3 dir = component.transform.position - tr.position;
        return dir;
    }
}
