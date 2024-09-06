using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stel_BA_1 : MonoBehaviour
{
    private Vector3 dir;
    public Vector3 Dir { get; set; }
    private Transform tr;

    // Start is called before the first frame update
    void Start()
    {
        tr = GetComponent<Transform>();
        tr.Rotate(dir);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
