using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    public GameObject obj;
    // Start is called before the first frame update


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            obj.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            obj.SetActive(true);
        }
    }
}
