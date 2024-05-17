using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : MonoBehaviour
{
    TokenManager tokenManager;

    public void Awake()
    {
        tokenManager = GameObject.Find("TokenManager").GetComponent<TokenManager>();
    }
    /// <summary>
    /// 현재 상태를 변경한다. 
    /// </summary>
    /// <param name="value">변경할 활성화 상태 값</param>
    public void SetActiveState(bool value)
    {
        gameObject.SetActive(value);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PLAYER"))
        {
            tokenManager.StorageCurrentToken();
        }
    }
}
