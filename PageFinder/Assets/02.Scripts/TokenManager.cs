using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class TokenManager : MonoBehaviour
{
    // 프리팹
    public GameObject Token_Prefab;

    // 사용할 오브젝트
    public GameObject BlackHole_Obj;
    public GameObject Portal_Obj;

    int currentTokenCnt = 0;
    int storagedTokenCnt = 0;

    int tokenCntAboutNextScene = 5;

    // 스크립트 관련
    TokenUIManager tokenUIManager;
    private void Awake()
    {
        DontDestroyOnLoad(this);
        tokenUIManager = GameObject.Find("UIManager").GetComponent<TokenUIManager>();
    }
    
    /// <summary>
    /// 토큰을 생성한다.
    /// </summary>
    /// <param name="pos">토큰 생성 위치</param>
    public void MakeToken(Vector3 pos)
    {
        Instantiate(Token_Prefab, pos, Quaternion.identity, gameObject.transform);
    }

    /// <summary>
    /// 토큰 개수를 1 증가시킨다. 
    /// </summary>
    public void IncreaseCurrentTokenCnt()
    {
        ++currentTokenCnt;
        tokenUIManager.SetCnt_Txt(currentTokenCnt);
    }

    /// <summary>
    /// 현재 토큰 개수를 0으로 리셋시킨다.
    /// </summary>
    void ResetCurrentTokenCnt()
    {
        currentTokenCnt = 0;
        tokenUIManager.SetCnt_Txt(currentTokenCnt);
    }

    /// <summary>
    /// 저장토큰이 목표 토큰에 도달했는지 체크한다. 
    /// </summary>
    void CheckStoragedTokenHasReachedTargetToken()
    {
        if (storagedTokenCnt < tokenCntAboutNextScene)
            return;

        // 저장 토큰이 목표 토큰에 도달했을 경우 처리코드
        BlackHole_Obj.GetComponent<BlackHole>().SetActiveState(false); // 블랙홀 비활성화
        Portal_Obj.GetComponent<Portal>().SetActiveState(true);
        ResetStoragedTokenCnt();
    }

    /// <summary>
    /// 현재 토큰을 저장 토큰에 저장한다. 
    /// </summary>
    public void StorageCurrentToken()
    {
        storagedTokenCnt += currentTokenCnt;
        ResetCurrentTokenCnt();
        CheckStoragedTokenHasReachedTargetToken();
    }

    /// <summary>
    /// 저장 토큰을 0으로 리셋시킨다. 
    /// </summary>
    void ResetStoragedTokenCnt()
    {
        storagedTokenCnt = 0;
    }
}
