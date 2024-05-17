using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    /// <summary>
    /// 무기의 색깔을 변경한다.
    /// </summary>
    /// <param name="color">변경할 색깔</param>
    public void ChangeColor(Color color)
    {
        //Debug.Log("무기 색 변경");
        gameObject.GetComponent<MeshRenderer>().material.color = color;
    }
}
