using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public enum ResultType
{
    NONE = 0,
    WIN = 1, // ���� óġ
    DEFEAT = 2, // �÷��̾� ���
    GOAL_FAIL = 3, // ����Ʈ ���������� ��ǥ ����
    StageClear = 4,
}

public class ResultUIManager : MonoBehaviour, IUIPanel
{
    [SerializeField] private bool isFixedMap = false;
    [SerializeField]
    private Sprite[] resultSprites;

    [SerializeField]
    private Image resultImage;

    [SerializeField]
    private TMP_Text resultTxt;

    private ResultType resultType;
    private float resultDuration;

    [SerializeField] private ProceduralMapGenerator proceduralMapGenerator;
    [SerializeField] private FixedMap fixedMap;

    [SerializeField] private VideoPlayer stageClear;
    [SerializeField] private VideoPlayer gameOver;

    public PanelType PanelType => PanelType.Result;

    private void InitResult()
    {
        stageClear.gameObject.SetActive(false);
        gameOver.gameObject.SetActive(false);
        resultImage.sprite = resultSprites[(int)resultType - 1];

        switch (resultType)
        {
            case ResultType.StageClear:
                resultTxt.enabled = false;
                OnStageClear();
                break;
            case ResultType.WIN:
                resultTxt.enabled = true;
                break;
            case ResultType.DEFEAT:
                resultTxt.enabled = false;
                OnGameOver();
                break;
            case ResultType.GOAL_FAIL:
                resultTxt.enabled = false;
                break;
        }
    }

    private void OnGameOver()
    {
        gameOver.gameObject.SetActive(true);
        gameOver.loopPointReached += ((VideoPlayer vp) => EventManager.Instance.PostNotification(EVENT_TYPE.Open_Panel_Exclusive, this, PanelType.Reward));
        AudioManager.Instance.Play(Sound.dead, AudioClipType.SequenceSfx);
        gameOver.Play();
    }

    private void OnStageClear()
    {
        stageClear.gameObject.SetActive(true);
        stageClear.loopPointReached += ((VideoPlayer vp) => EventManager.Instance.PostNotification(EVENT_TYPE.Open_Panel_Exclusive, this, PanelType.Reward));
        AudioManager.Instance.Play(Sound.victory, AudioClipType.SequenceSfx);
        stageClear.Play();
    }

    private void SetResultTxt()
    {
        switch (resultType)
        {
            case ResultType.WIN:
                resultTxt.text = $"���ӿ��� ���� �¸��Ͽ����ϴ�!\n{(int)resultDuration}�� �� ����ȭ������ �Ѿ�ϴ�.";
                break;
        }
    }

    private IEnumerator CloseResultScreen()
    {
        switch (resultType)
        {
            case ResultType.WIN:
                AudioManager.Instance.Play(Sound.victory, AudioClipType.SequenceSfx);
                break;
            case ResultType.GOAL_FAIL:
                AudioManager.Instance.Play(Sound.defeat, AudioClipType.SequenceSfx);
                break;
        }

        while (resultDuration >= 0)
        {
            resultDuration -= Time.deltaTime;
            SetResultTxt();
            yield return null;
        }
        resultDuration = 0;
        SetResultTxt();

        // ���� �̵��� ȭ�� ���ϱ�
        switch (resultType)
        {
            case ResultType.WIN: // ���� óġ��
                EventManager.Instance.PostNotification(EVENT_TYPE.GAME_END, this);
                break;
            // �������� ��ǥ ���н�
            case ResultType.GOAL_FAIL:
                EventManager.Instance.PostNotification(EVENT_TYPE.Open_Panel_Exclusive, this, PanelType.HUD);
                if(isFixedMap) fixedMap.playerNode.portal.gameObject.SetActive(true);
                else proceduralMapGenerator.playerNode.portal.gameObject.SetActive(true);
                break;
        }
    }

    public void SetResultData(ResultType resultType, float resultDuration)
    {
        this.resultType = resultType;
        this.resultDuration = resultDuration;
    }

    public void Open()
    {
        this.gameObject.SetActive(true);
        InitResult();
        if (resultType == ResultType.StageClear || resultType == ResultType.DEFEAT) return;
        StartCoroutine(CloseResultScreen());
    }

    public void Close()
    {
        this.gameObject.SetActive(false);
    }
}
