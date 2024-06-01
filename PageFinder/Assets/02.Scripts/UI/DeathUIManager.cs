using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathUIManager : MonoBehaviour
{
    public Canvas Death_Canvas;

    StageManager stageManager;

    private void Awake()
    {
        DontDestroyOnLoad(this);

        stageManager = GameObject.Find("StageManager").GetComponent<StageManager>();
    }

    private void Start()
    {
        ChangeDeathCanvasState(false);
    }

    void ChangeDeathCanvasState(bool value)
    {
        Death_Canvas.gameObject.SetActive(value);
    }
    
    // 플레이어 죽는 조건쪽에서 호출하기
    public IEnumerator ActivateDeathUI()
    {
        ChangeDeathCanvasState(true);

        yield return new WaitForSeconds(1);

        ChangeDeathCanvasState(false);
        stageManager.MoveTitleScene();
    }
}
