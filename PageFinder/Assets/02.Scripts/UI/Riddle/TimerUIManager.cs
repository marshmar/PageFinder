using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimerUIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text timer_Txt;
    
    private bool isTimerOn;
    private int min, sec;
    private float currTime;
    private readonly float maxTime = 30;
    private List<CanvasType> canvasTypes = new(){ CanvasType.RESULT, CanvasType.BATTLE };

    private void OnEnable()
    {
        InitTime();
    }

    private void OnDisable()
    {
        isTimerOn = false;
    }

    private void SetTimer()
    {
        currTime -= Time.deltaTime;
        min = (int)(currTime / 60f);
        sec = (int)(currTime % 60f);
        timer_Txt.text = $"{min}:{sec}";
    }

    private void InitTime()
    {
        isTimerOn = true;
        currTime = maxTime;
        min = (int)(currTime / 60f);
        sec = (int)(currTime % 60f);

        StartCoroutine(Timer());
    }

    private IEnumerator Timer()
    {
        while(currTime > 0)
        {
            if (!isTimerOn) yield break;
            SetTimer();
            yield return null;
        }

        var canvasTypes = new List<CanvasType> { CanvasType.BATTLE, CanvasType.RESULT, CanvasType.PLAYERUIOP, CanvasType.PLAYERUIOP};
        EnemyPooler.Instance.ReleaseAllEnemy(Enemy.EnemyType.Fugitive);
    }
}