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

    #region #Run-time �߿� RenderingMode �ٲٴ� �޼ҵ��
    /// <summary>
    /// Material ��ü�� ������ ��带 �����ϴµ� ����ϴ� �޼���
    /// -----------------------------------------------------------------------
    /// 0: Opaque(������), 1: Cutout(�߶󳻱�), 2:Fade(������ �����), 3: Transparent(����)
    /// -----------------------------------------------------------------------
    /// SrcBlend(Source Blend)�� �ҽ� ����� ���� ���� ȥ�� ����� ����.
    /// �ҽ� ������ ���� �׸� ������ �ǹ�.
    /// SrcAlhpa�� �ҽ� ������ ���� ���� ���� �ȼ��� ������ ȥ��.
    /// -----------------------------------------------------------------------
    /// DstBlend(Destination Blend)�� ��� ����� ���� ���� ȥ�� ����� ����.
    /// ��� ������ �̹� ȭ�鿡 �׷��� �ִ� ������ �ǹ�
    /// OneMinusAlpha�� (1-�ҽ� ���� ��)�� ���� �Ȼ��� ������ ȥ��.
    /// -----------------------------------------------------------------------
    /// ZWrite�� ���� ���� ���θ� ����.
    /// ZWrite ���� 0�̸�, ��ü�� �������ϰ� �׷��� �� �ڿ� �ִ� ��ü�� ����� ���̰� ��.
    /// ZWrite ���� 1�̸�, ���� ���ۿ� ��(��, �ڿ� �ִ� ��ü�� �Ⱥ���)
    /// -----------------------------------------------------------------------
    /// DisableKeyword("_ALPHATEST_ON") : ���� �׽�Ʈ�� ��Ȱ��ȭ.
    /// �����׽�Ʈ: ���� ���� Ư�� �Ӱ谪 �̻��� ��쿡�� �ȼ��� �׸��� ���
    /// -----------------------------------------------------------------------
    /// EnableKeyword("_ALPHABLEND_ON") : ���� ������ Ȱ��ȭ.
    /// ���ĺ���: �ȼ��� ���İ��� ���� ������ ȥ���ϴ� ���
    /// -----------------------------------------------------------------------
    /// DisablwKeyword("_ALPHAPREMULTIPLY_ON"): ������Ƽ�ö��̵� ���� ���� ��Ȱ��ȭ
    /// ������Ƽ�ö��̵� ���� ����: ���� ���� ���� ������ �̸� ȥ���ϴ� ���
    /// -----------------------------------------------------------------------
    /// </summary>
    /// <param name="material">������ ��带 ������ ���׸���</param>
    /// <param name="mode">������ ����� �Ӽ�</param>
    /// <param name="renderQueue">������ ������ ����, ���� ���� �ٸ� ��ü�� �ڿ� �׷���</param>
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
    /// Material�� Transparent, �� �����ϰ� ����
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
    /// Material�� Opaque, �� �������ϰ� ����
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
    /// Material�� �⺻������ �ǵ�����(�� �������ϰ� �ǵ�����)
    /// </summary>
    public void ResetOriginalTransparent()
    {
        SetMaterialOpaque();
        resetCoroutine = StartCoroutine(ResetOriginalTransparentCoroutine());
    }

    /// <summary>
    /// ������Ʈ�� ������ �����ϰ� ����� �ڷ�ƾ
    /// </summary>
    /// <returns>delay</returns>
    private IEnumerator BecomeTransparentCoroutine()
    {
        while (true)
        {
            bool isComplete = true;
            for(int i = 0; i < renderers.Length; i++)
            {
                // ���׸����� ���İ��� ��ǥ ���İ� �̻��̸� �ݺ�
                if (renderers[i].material.color.a > THRESHOLD_ALPHA)
                    isComplete = false;

                Color color = renderers[i].material.color;
                color.a -= Time.deltaTime;
                renderers[i].material.color = color;

                yield return delay;
            }

            // ��ǥ ���İ�(����)�� �����Ͽ����� ���� �ð� �Ŀ� ������Ʈ�� �������ϰ� ����
            if (isComplete)
            {
                CheckTimer();
                break;
            }
        }
    }

    /// <summary>
    /// ������Ʈ�� ������ �������ϰ� ����� �ڷ�ƾ
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
                // ���׸����� ���İ��� ��ǥ ���İ� �����̸� �ݺ�
                if (renderers[i].material.color.a < 1f)
                    isComplete = false;

                Color color = renderers[i].material.color;
                color.a += Time.deltaTime;
                renderers[i].material.color = color;
            }
            // ���׸����� ���İ��� ��ǥ ���İ�(����)�� �����Ͽ����� �ݺ� ����
            if (isComplete)
            {
                isReseting = false;
                break;
            }

            yield return resetDelay;
        }
    }

    /// <summary>
    /// Ÿ�̸� �ڷ�ƾ�� �����ϴ� �Լ�
    /// </summary>
    public void CheckTimer()
    {
        // ���� �������� Ÿ�̸� �ڷ�ƾ�� ������ �����ϰ� �ٽ� ����
        if (timeCheckCoroutine != null)
            StopCoroutine(timeCheckCoroutine);
        timeCheckCoroutine = StartCoroutine(CheckTimerCoroutine());
    }

    /// <summary>
    /// Ÿ�̸� �ڷ�ƾ(�����ð� �Ŀ� ������Ʈ�� �������ϰ� �����.)
    /// </summary>
    /// <returns></returns>
    private IEnumerator CheckTimerCoroutine()
    {
        timer = 0f;
        while (true)
        {
            timer += Time.deltaTime;
            // ���� �ð� �������� Ȯ��
            if(timer > TRHESHOLD_MAX_TIMER)
            {
                isReseting = true;
                ResetOriginalTransparent();
                break;
            }
            yield return null;
        }
    }
}