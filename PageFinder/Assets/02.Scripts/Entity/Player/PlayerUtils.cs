using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerUtils : MonoBehaviour
{
    #region Variables
    public float RotateSpeed = 30f;

    // Hashing
    private Transform _tr;
    private Rigidbody _rigid;
    private Player _player;
    private Transform _playerSpine;
    [SerializeField] private Transform modelTr;
    #endregion

    #region Properties
    public Transform ModelTr { get => modelTr; set => modelTr = value; }
    public Transform Tr { get => _tr; set => _tr = value; }
    public Rigidbody Rigid { get => _rigid; set => _rigid = value; }
    #endregion

    #region Unity Lifecycle
    void Awake()
    {
        _tr = this.GetComponentSafe<Transform>();
        _rigid = this.GetComponentSafe<Rigidbody>();
        _player = this.GetComponentSafe<Player>();
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
                new Vector3(dir.x, 0f, dir.z)), RotateSpeed * Time.deltaTime);
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
        Vector3 dir = component.transform.position - _tr.position;
        return dir;
    }
    #endregion

    #region Events
    #endregion
}
