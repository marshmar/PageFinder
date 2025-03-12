using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetObject : MonoBehaviour
{
    [SerializeField]
    private GameObject uiObject;
    private bool isActive;
    private Transform targetTransform;

    public bool IsActive
    {
        get => isActive;
        set {
            isActive = value;
            if (isActive) uiObject.SetActive(true);
            else uiObject.SetActive(false);
        }
    }

    public Transform TargetTransform { get => targetTransform; set => targetTransform = value; }

    void Start()
    {
        uiObject.SetActive(false);
    }

    void Update()
    {
        if(targetTransform == null) uiObject.SetActive(false);
        if (isActive) MoveTargetObject();
    }

    private void MoveTargetObject()
    {
        if(targetTransform) uiObject.transform.position = new Vector3(TargetTransform.position.x, 1.1f, TargetTransform.position.z);
    }
}