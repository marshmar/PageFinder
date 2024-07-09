using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPBar : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        //transform.LookAt(Camera.main.transform);
        //transform.rotation  = Quaternion.Euler(Camera.main.transform.rotation.x, 180, Camera.main.transform.rotation.z);
        //transform.rotation = Camera.main.transform.rotation;
        transform.rotation = Quaternion.Euler(-Camera.main.transform.eulerAngles.x, 180, Camera.main.transform.eulerAngles.z);
    }
}
