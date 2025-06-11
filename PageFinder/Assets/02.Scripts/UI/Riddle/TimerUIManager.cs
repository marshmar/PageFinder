using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimerUIManager : MonoBehaviour, IListener
{
    [SerializeField] private TMP_Text timer_Txt;
    
    private bool isTimerOn = true;
    private int min, sec;
    private float currTime;
    private readonly float maxTime = 30;
    //private List<CanvasType> canvasTypes = new(){ CanvasType.RESULT, CanvasType.BATTLE };

    private void Awake()
    {
        AddListener();
    }

    private void OnDestroy()
    {
        RemoveListener();
    }

    private void SetTimer()
    {
        currTime -= Time.deltaTime;
        min = (int)(currTime / 60f);
        sec = (int)(currTime % 60f);
        timer_Txt.text = $"{min}:{sec}";
    }

    public void InitTime()
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
            if (!isTimerOn) {
                this.gameObject.SetActive(false);
                yield break;
            }
            SetTimer();
            yield return null;
        }

        EventManager.Instance.PostNotification(EVENT_TYPE.Stage_Failed, this);
        //var canvasTypes = new List<CanvasType> { CanvasType.BATTLE, CanvasType.RESULT, CanvasType.PLAYERUIOP, CanvasType.PLAYERUIOP};
        EnemyPooler.Instance.ReleaseAllEnemy(Enemy.EnemyType.Fugitive);
    }

    public void AddListener()
    {
        EventManager.Instance.AddListener(EVENT_TYPE.EndTimer, this);
    }

    public void RemoveListener()
    {
        EventManager.Instance.RemoveListener(EVENT_TYPE.EndTimer, this);
    }
    public void OnEvent(EVENT_TYPE eventType, Component Sender, object Param = null)
    {
        switch (eventType)
        {
            case EVENT_TYPE.EndTimer:
                isTimerOn = false;
                StopAllCoroutines();
                this.gameObject.SetActive(false);
                break;
        }
    }
}