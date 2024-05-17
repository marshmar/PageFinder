using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Playables;

public class TokenUIManager : MonoBehaviour
{
    
    public TMP_Text Cnt_Txt;

    // 스크립트 관련
    TokenManager tokenManager;

    private void Awake()
    {
        tokenManager = GameObject.Find("TokenManager").GetComponent<TokenManager>();
    }

    private void Start()
    {
        SetCnt_Txt(0);
    }

    /// <summary>
    /// 토큰의 개수 텍스트를 설정한다.
    /// </summary>
    /// <param name="currentTokenCnt">토큰 개수</param>
    public void SetCnt_Txt(int currentTokenCnt)
    {
        Cnt_Txt.text = currentTokenCnt.ToString();
    }
}
