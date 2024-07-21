using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPBar : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(-Camera.main.transform.eulerAngles.x, 180, Camera.main.transform.eulerAngles.z);
    }
}
