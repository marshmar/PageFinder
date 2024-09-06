using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScr : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(GameObject.Find("Jiruru"));
        Debug.Log(GameObject.Find("Jiruru").GetComponent<EnemyAnimation>().DefaultAtkPercent);
        
    }

   
}
