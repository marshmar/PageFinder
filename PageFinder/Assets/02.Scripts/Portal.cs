using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{

    // 스크립트 관련
    ReinforceUIManager reinforceUIManager;

    private void Awake()
    {
        reinforceUIManager = GameObject.Find("UIManager").GetComponent<ReinforceUIManager>();
    }
    private void Start()
    {
        SetActiveState(false);
        SceneManager.sceneLoaded += LoadedSceneEvent;
    }

    /// <summary>
    /// 포탈의 색깔을 변경한다. 
    /// </summary>
    /// <param name="color">변경할 색</param>
    public void ChangeColor(Color color)
    {
        ParticleSystem.MainModule main = GetComponent<ParticleSystem>().main;
        main.startColor = color; // <- or whatever color you want to assign    
        StartCoroutine(MoveNextScene());
    }

    /// <summary>
    /// 다음씬으로 전환한다. 
    /// </summary>
    /// <returns></returns>
    public IEnumerator MoveNextScene()
    {
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene("Scene1");
    }

    /// <summary>
    /// 포탈 활성화/비활성화 여부를 변경한다. 
    /// </summary>
    /// <param name="value">변경할 활성화 여부</param>
    public void SetActiveState(bool value)
    {
        gameObject.SetActive(value);
    }

    /// <summary>
    /// 씬이 로딩될 때 이벤트를 처리한다. 
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="mode"></param>
    void LoadedSceneEvent(Scene scene, LoadSceneMode mode)
    {
        reinforceUIManager.StartCoroutine(reinforceUIManager.ActivateReinforceUI()); // 기억 시스템 UI 동작
    }
}
