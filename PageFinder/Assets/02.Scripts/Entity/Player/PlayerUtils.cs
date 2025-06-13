using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerUtils : MonoBehaviour
{
    #region Variables
    public float rotateSpeed = 30f;

    // Hashing
    private Transform tr;
    private Rigidbody rigid;
    private Player player;
    private Transform playerSpine;
    [SerializeField] private Transform modelTr;
    #endregion

    #region Properties
    public Transform ModelTr { get => modelTr; set => modelTr = value; }
    public Transform Tr { get => tr; set => tr = value; }
    public Rigidbody Rigid { get => rigid; set => rigid = value; }
    #endregion

    #region Unity Lifecycle
    void Awake()
    {
        tr = this.GetComponentSafe<Transform>();
        rigid = this.GetComponentSafe<Rigidbody>();
        player = this.GetComponentSafe<Player>();
    }
    #endregion

    #region Initialization
    #endregion

    #region Actions
    public void TurnToDirection(Vector3 dir, bool smoothRot = false)
    {
        if (dir == Vector3.zero) return;
        if (modelTr.IsNull()) return;

        if (smoothRot)
            modelTr.rotation = Quaternion.Slerp(modelTr.rotation, Quaternion.LookRotation(
                new Vector3(dir.x, 0f, dir.z)), rotateSpeed * Time.deltaTime);
        else 
            modelTr.rotation = Quaternion.LookRotation(new Vector3(dir.x, 0f, dir.z));
    }

    #endregion

    #region Getter
    #endregion

    #region Setter
    #endregion

    #region Utilities
    public Vector3 CalculateDirectionFromPlayer(Component component)
    {
        Vector3 dir = component.transform.position - tr.position;
        return dir;
    }
    #endregion

    #region Events
    #endregion
}
