using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DestroyAllDontDestroyOnLoadObjects();
    }

    void DestroyAllDontDestroyOnLoadObjects()
    {
        var objects = new List<GameObject>();

        var root = new GameObject("Root");
        DontDestroyOnLoad(root);

        root.scene.GetRootGameObjects(objects);
        Destroy(root);

        foreach (var obj in objects)
        {
            Destroy(obj);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
