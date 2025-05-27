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
        if (targetTransform == null || !targetTransform.gameObject.activeSelf)
        {
            uiObject.SetActive(false);
        }
        if (isActive)
        {
            MoveTargetObject();
        }   
    }

    private void MoveTargetObject()
    {

        if (targetTransform)
        {
            // 13: Ground Layer;
            int targetLayer = 1 << 13;
            Vector3 targetPos = new Vector3(TargetTransform.position.x, targetTransform.position.y + 0.1f, TargetTransform.position.z);
            Ray groundRay = new Ray(targetPos, Vector3.down);
            RaycastHit hit;
            
            if (Physics.Raycast(groundRay, out hit, Mathf.Infinity, targetLayer))
            {
                targetPos = hit.point + new Vector3(0f, 0.15f, 0f);
            }
            uiObject.transform.position = targetPos;
        }
    }
}