using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Diagnostics;

public class TimerUIManager : MonoBehaviour
{
    [SerializeField]
    private TMP_Text timer_Txt;

    [SerializeField]
    private const float maxTime = 30;
    private float currTime;
    private int min;
    private int sec;

    bool timerControlller;

    List<CanvasType> canvasTypes = new List<CanvasType> { CanvasType.RESULT, CanvasType.BATTLE };

    private void OnEnable()
    {
        InitTime();
    }

    private void OnDisable()
    {
        // Timer 코루틴이 돌고있을 경우 끄기 위함
        timerControlller = false;
    }

    /// <summary>
    /// 제한시간 계산
    /// </summary>
    private void SetTimer()
    {
        currTime -= Time.deltaTime;
        min = (int)(currTime / 60f);
        sec = (int)(currTime % 60f);
        timer_Txt.text = $"{min}:{sec}";
    }

    private void InitTime()
    {
        timerControlller = true;

        currTime = maxTime;
        min = (int)(currTime / 60f);
        sec = (int)(currTime % 60f);

        StartCoroutine(Timer());
    }

    private IEnumerator Timer()
    {
        while(currTime > 0)
        {
            if (!timerControlller)
                yield break;

            SetTimer();
            yield return null;
        }

        List<CanvasType> canvasTypes = new List<CanvasType> { CanvasType.BATTLE, CanvasType.RESULT, CanvasType.PLAYERUIOP, CanvasType.PLAYERUIOP};
        EventManager.Instance.PostNotification(EVENT_TYPE.UI_Changed, this, UIType.Goal_Fail);
    }
}