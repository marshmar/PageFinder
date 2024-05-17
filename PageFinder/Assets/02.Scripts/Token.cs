using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Token : MonoBehaviour
{
    // 스크립트 관련
    TokenManager tokenManager;
    private void Awake()
    {
        tokenManager = GameObject.Find("TokenManager").GetComponent<TokenManager>();
    }

    private void OnTriggerEnter(Collider coll)
    {
        if (!coll.CompareTag("Player"))
            return;

        // 플레이어와 충돌했을 경우
        SetActiveState(false);
        tokenManager.IncreaseCurrentTokenCnt();

    }

    /// <summary>
    /// 토큰 오브젝트 활성화 상태를 변경한다. 
    /// </summary>
    /// <param name="value">변경할 상태 값</param>
    void SetActiveState(bool value)
    {
        gameObject.SetActive(value);
    }
}
