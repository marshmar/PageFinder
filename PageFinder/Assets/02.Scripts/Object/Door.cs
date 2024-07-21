using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public IEnumerator Open()
    {
        int speed = 3;
        while(transform.position.y >= -4.5f)
        {
            transform.Translate(Vector3.down * speed * Time.deltaTime, Space.World);
            yield return null;
        }
    }
}
