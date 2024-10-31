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
            if (isActive)
            {
                uiObject.SetActive(true);
            }
            else
            {
                uiObject.SetActive(false);
            }
        }
    }

    public Transform TargetTransform { get => targetTransform; set => targetTransform = value; }

    // Start is called before the first frame update
    void Start()
    {
        uiObject.SetActive(false);
    }

    
    // Update is called once per frame
    void Update()
    {
        if (IsActive)
        {
            MoveTargetObject();
        }   
    }

    private void MoveTargetObject()
    {
        if(targetTransform)
            uiObject.transform.position = TargetTransform.position;
    }
}
