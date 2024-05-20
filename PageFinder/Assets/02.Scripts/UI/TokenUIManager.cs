using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Playables;

public class TokenUIManager : MonoBehaviour
{
    
    public TMP_Text CurrentTokenCnt_Txt;
    public TMP_Text StoragedTokenCnt_Txt;

    // 스크립트 관련
    TokenManager tokenManager;

    private void Awake()
    {
        tokenManager = GameObject.Find("TokenManager").GetComponent<TokenManager>();
    }

    private void Start()
    {
        SetCurrentTokenCnt_Txt(0);
        SetStoragedTokenCnt_Txt(tokenManager.ReturnTokenCntAboutNextScene());
    }

    /// <summary>
    /// 현재 가지고 있는 토큰의 개수 텍스트를 설정한다.
    /// </summary>
    /// <param name="currentTokenCnt">토큰 개수</param>
    public void SetCurrentTokenCnt_Txt(int currentTokenCnt)
    {
        CurrentTokenCnt_Txt.text = currentTokenCnt.ToString();
    }

    /// <summary>
    /// 블랙홀에 저장한 토큰의 개수 텍스트를 설정한다.
    /// </summary>
    /// <param name="storagedTokenCnt"></param>
    public void SetStoragedTokenCnt_Txt(int storagedTokenCnt)
    {
        StoragedTokenCnt_Txt.text = storagedTokenCnt.ToString();
    }
}
