using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RiddlePlayUIManager : MonoBehaviour
{
    [SerializeField]
    private Canvas RiddlePlayUICanavs;

    [SerializeField]
    private TMP_Text timer_Txt;
    float total_time = 60;
    int timer_min;
    float timer_sec;
    bool isEnded = true;


    // Update is called once per frame
    void Update()
    {
        SetTimer();
    }

    public void SetRiddlePlayUICanvasState(bool value)
    {
        RiddlePlayUICanavs.gameObject.SetActive(value);

        if (!value)
            return;

        isEnded = false;
        InitTime();
    }

    /// <summary>
    /// 제한시간을 계산한다. 
    /// </summary>
    void SetTimer()
    {
        if (isEnded)
            return;

        if (total_time < 0 && !isEnded)
        {
            isEnded = true;
            UIManager.Instance.SetUIActiveState("PageMap");
            return;
        } 

        total_time -= Time.deltaTime;
        timer_min = (int)(total_time / 60f);
        timer_sec = (int)(total_time % 60f);
        timer_Txt.text = timer_min + ":" + timer_sec;
    }

    void InitTime()
    {
        total_time = 60;
        timer_min = (int)(total_time / 60f);
        timer_sec = (int)(total_time % 60f);
    }
}
