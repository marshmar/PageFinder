using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TransparentObject : MonoBehaviour
{
    public bool IsTransparent { get; private set; } = false;

    private MeshRenderer[] renderers;
    private WaitForSeconds delay = new WaitForSeconds(0.001f);
    private WaitForSeconds resetDelay = new WaitForSeconds(0.005f);
    private const float THRESHOLD_ALPHA = 0.25f;
    private const float TRHESHOLD_MAX_TIMER = 0.5f;

    private bool isReseting = false;
    private float timer = 0f;
    private Coroutine timeCheckCoroutine;
    private Coroutine resetCoroutine;
    private Coroutine becomeTransparentCoroutine;

    private void Awake()
    {
        renderers = GetComponentsInChildren<MeshRenderer>();
    }

    public void BecomeTransParent()
    {
        if (IsTransparent)
        {
            timer = 0f;
            return;
        }
        if(resetCoroutine != null && isReseting)
        {
            isReseting = false;
            IsTransparent = false;
            StopCoroutine(resetCoroutine);
        }
        SetMaterialTransparent();
        IsTransparent = true;
        becomeTransparentCoroutine = StartCoroutine(BecomeTransparentCoroutine());
    }

    #region #Run-time 중에 RenderingMode 바꾸는 메소드들
    /// <summary>
    /// Material 객체의 렌더링 모드를 변경하는데 사용하는 메서드
    /// -----------------------------------------------------------------------
    /// 0: Opaque(불투명), 1: Cutout(잘라내기), 2:Fade(서서히 사라짐), 3: Transparent(투명)
    /// -----------------------------------------------------------------------
    /// SrcBlend(Source Blend)는 소스 색상과 알파 값의 혼합 방식을 지정.
    /// 소스 색상은 새로 그릴 색상을 의미.
    /// SrcAlhpa는 소스 색상의 알파 값에 따라 픽셀의 색상을 혼합.
    /// -----------------------------------------------------------------------
    /// DstBlend(Destination Blend)는 대상 색상과 알파 값의 혼합 방식을 지정.
    /// 대상 색상은 이미 화면에 그려져 있는 색상을 의미
    /// OneMinusAlpha는 (1-소스 알파 값)에 따라 픽샐의 색상을 혼합.
    /// -----------------------------------------------------------------------
    /// ZWrite는 깊이 쓰기 여부를 설정.
    /// ZWrite 값이 0이면, 객체가 반투명하게 그려질 때 뒤에 있는 객체가 제대로 보이게 함.
    /// ZWrite 값이 1이면, 깊이 버퍼에 씀(즉, 뒤에 있는 객체가 안보임)
    /// -----------------------------------------------------------------------
    /// DisableKeyword("_ALPHATEST_ON") : 알파 테스트를 비활성화.
    /// 알파테스트: 알파 값이 특정 임계값 이상일 경우에만 픽셀을 그리는 기능
    /// -----------------------------------------------------------------------
    /// EnableKeyword("_ALPHABLEND_ON") : 알파 블렌딩을 활성화.
    /// 알파블렌딩: 픽셀의 알파값에 따라 색상을 혼합하는 기능
    /// -----------------------------------------------------------------------
    /// DisablwKeyword("_ALPHAPREMULTIPLY_ON"): 프리멀티플라이드 알파 블렌딩 비활성화
    /// 프리멀티플라이드 알파 블렌딩: 알파 값에 따라 색상을 미리 혼합하는 기능
    /// -----------------------------------------------------------------------
    /// </summary>
    /// <param name="material">렌더링 모드를 변경할 머테리얼</param>
    /// <param name="mode">렌더링 모드의 속성</param>
    /// <param name="renderQueue">재질의 랜더링 순서, 높을 수록 다른 객체들 뒤에 그려짐</param>
    private void SetMaterialRenderingMode(Material material, float mode, int renderQueue)
    {   
        
        material.SetFloat("_Mode", mode);
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("ZWrite", 0);
        material.DisableKeyword("_ALPHATEST_ON");
        material.EnableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = renderQueue;
    }

    /// <summary>
    /// Material을 Transparent, 즉 투명하게 설정
    /// </summary>
    private void SetMaterialTransparent()
    {
        for(int i = 0; i < renderers.Length; i++)
        {
            foreach(Material material in renderers[i].materials)
            {
                SetMaterialRenderingMode(material, 3f, 3000);
            }
        }
    }

    /// <summary>
    /// Material을 Opaque, 즉 불투명하게 설정
    /// </summary>
    private void SetMaterialOpaque()
    {
        for(int i = 0; i < renderers.Length; i++)
        {
            foreach(Material material in renderers[i].materials)
            {
                SetMaterialRenderingMode(material, 0f, -1);
            }
        }
    }
    #endregion

    /// <summary>
    /// Material을 기본값으로 되돌리기(즉 불투명하게 되돌리기)
    /// </summary>
    public void ResetOriginalTransparent()
    {
        SetMaterialOpaque();
        resetCoroutine = StartCoroutine(ResetOriginalTransparentCoroutine());
    }

    /// <summary>
    /// 오브젝트를 서서히 투명하게 만드는 코루틴
    /// </summary>
    /// <returns>delay</returns>
    private IEnumerator BecomeTransparentCoroutine()
    {
        while (true)
        {
            bool isComplete = true;
            for(int i = 0; i < renderers.Length; i++)
            {
                // 머테리얼의 알파값이 목표 알파값 이상이면 반복
                if (renderers[i].material.color.a > THRESHOLD_ALPHA)
                    isComplete = false;

                Color color = renderers[i].material.color;
                color.a -= Time.deltaTime;
                renderers[i].material.color = color;

                yield return delay;
            }

            // 목표 알파값(투명도)에 도달하였으면 일정 시간 후에 오브젝트를 불투명하게 만듬
            if (isComplete)
            {
                CheckTimer();
                break;
            }
        }
    }

    /// <summary>
    /// 오브젝트를 서서히 불투명하게 만드는 코루틴
    /// </summary>
    /// <returns>resetDelay</returns>
    private IEnumerator ResetOriginalTransparentCoroutine()
    {
        IsTransparent = false;

        while (true)
        {

            bool isComplete = true;

            for(int i = 0; i < renderers.Length; i++)
            {
                // 머테리얼의 알파값이 목표 알파값 이하이면 반복
                if (renderers[i].material.color.a < 1f)
                    isComplete = false;

                Color color = renderers[i].material.color;
                color.a += Time.deltaTime;
                renderers[i].material.color = color;
            }
            // 머테리얼의 알파값이 목표 알파값(투명도)에 도달하였으면 반복 종료
            if (isComplete)
            {
                isReseting = false;
                break;
            }

            yield return resetDelay;
        }
    }

    /// <summary>
    /// 타이머 코루틴을 시작하는 함수
    /// </summary>
    public void CheckTimer()
    {
        // 현재 진행중인 타이머 코루틴이 있으면 중지하고 다시 시작
        if (timeCheckCoroutine != null)
            StopCoroutine(timeCheckCoroutine);
        timeCheckCoroutine = StartCoroutine(CheckTimerCoroutine());
    }

    /// <summary>
    /// 타이머 코루틴(일정시간 후에 오브젝트를 불투명하게 만든다.)
    /// </summary>
    /// <returns></returns>
    private IEnumerator CheckTimerCoroutine()
    {
        timer = 0f;
        while (true)
        {
            timer += Time.deltaTime;
            // 일정 시간 지났는지 확인
            if(timer > TRHESHOLD_MAX_TIMER)
            {
                isReseting = true;
                ResetOriginalTransparent();
                break;
            }
            yield return null;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
