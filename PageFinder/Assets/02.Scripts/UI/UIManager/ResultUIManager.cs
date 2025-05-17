using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum ResultType
{
    NONE = 0,
    WIN = 1, // ���� óġ
    DEFAT = 2, // �÷��̾� ���
    GOAL_FAIL = 3 // ����Ʈ ���������� ��ǥ ����
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
    public PanelType PanelType => PanelType.Result;

    private void InitResult()
    {
        resultImage.sprite = resultSprites[(int)resultType - 1];
        switch (resultType)
        {
            case ResultType.WIN:
            case ResultType.DEFAT:
                resultTxt.enabled = true;
                break;

            case ResultType.GOAL_FAIL:
                resultTxt.enabled = false;
                break;
        }
    }


    private void SetResultTxt()
    {
        switch (resultType)
        {
            case ResultType.WIN:
                resultTxt.text = $"���ӿ��� ���� �¸��Ͽ����ϴ�!\n{(int)resultDuration}�� �� ����ȭ������ �Ѿ�ϴ�.";
                break;

            case ResultType.DEFAT:
                resultTxt.text = $"���ڶ��� ü���� ��� �����Ͽ����ϴ�...\n{(int)resultDuration}�� �� ����ȭ������ �Ѿ�ϴ�.";
                break;
        }
    }

    private IEnumerator CloseResultScreen()
    {
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
            case ResultType.DEFAT: // �÷��̾� �����
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
        StartCoroutine(CloseResultScreen());
    }

    public void Close()
    {
        this.gameObject.SetActive(false);
    }
}
