using UnityEngine;
using System;

public class TargetMarker : MonoBehaviour
{
    #region Variables

    [SerializeField] private GameObject targetMarkerObject;
    private bool _isActive;
    private Transform _targetTransform;
    #endregion

    #region Properties
    public bool IsActive
    {
        get => _isActive;
        set
        {
            _isActive = value;

            if (targetMarkerObject.IsNull()) return;
            targetMarkerObject.SetActive(value);
        }
    }

    public Transform TargetTransform { get => _targetTransform; set => _targetTransform = value; }
    #endregion

    #region Unity Lifecycle

    void Start()
    {
        targetMarkerObject.SetActive(false);
    }

    void Update()
    {
        if (_targetTransform == null || !_targetTransform.gameObject.activeSelf)
        {
            if (targetMarkerObject.IsNull()) return;
            targetMarkerObject.SetActive(false);
        }
        if (_isActive)
        {
            MoveTargetObject();
        }
    }

    #endregion

    #region Initialization
    #endregion

    #region Actions
    private void MoveTargetObject()
    {
        if (_targetTransform.IsNull())
            return;

        if (_targetTransform)
        {
            targetMarkerObject.transform.position = Utils.GetSpawnPosRayCast(TargetTransform.position); ;
        }
    }
    #endregion

    #region Getter
    #endregion

    #region Setter
    #endregion

    #region Utilities
    #endregion

    #region Events
    #endregion
}