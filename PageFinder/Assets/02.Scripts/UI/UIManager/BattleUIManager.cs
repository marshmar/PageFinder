using Google.GData.AccessControl;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;


public class BattleUIManager : MonoBehaviour
{
    [SerializeField]
    private Canvas BattleUICanvas;

    [SerializeField]
    private TMP_Text pageTypeTxt;

    [SerializeField]
    private GameObject goalContentImg;
    [SerializeField]
    private TMP_Text goalContentTxt;

    [SerializeField]
    private GameObject goalDetailContentImg;
    [SerializeField]
    private TMP_Text goalDetailContentTxt;

    // 이전 활성화 버전
    public void SetBattleUICanvasState(bool value, bool isSetting)
    {
        //BattleUICanvas.gameObject.SetActive(value);

        //if (!value)
        //    return;

        //// 설정에서 빈 곳을 눌러 원래 화면인 Battle로 전환될 때는 이미 BattleUI가 켜져 있으므로 밑의 함수 동작하지 않게 리턴
        //if (isSetting)
        //    return;

        //bool isBattle = GameData.Instance.GetCurrPageType() == PageType.BATTLE;

        //// 좌상단 타이틀 이름 설정
        //SetPageTypeTxt(isBattle ? "배틀 페이지" : "수수께끼 페이지");

        //// 중앙, 좌측 목표 설정
        //StartCoroutine(SetGoalData());
    }

    // 목표가 띄워지는 여부의 경우를 나눠서 코루틴 호출해야하는데 현재 안됨.


    private void OnEnable()
    {
        bool isBattle = false;
        NodeType currNodeType = GameData.Instance.GetCurrPageType();
        if(currNodeType == NodeType.Start || currNodeType == NodeType.Battle_Elite
            || currNodeType == NodeType.Battle_Normal || currNodeType == NodeType.Boss)
        {
            isBattle = true;
        }

        SetPageTypeTxt(isBattle ? "배틀 페이지" : "수수께끼 페이지");
        StartCoroutine(SetGoalData());
    }

    private void SetPageTypeTxt(string value)
    {
        pageTypeTxt.text = value;
    }

    IEnumerator SetGoalData()
    {
        SetGoalContent(true);
        SetGoalDetailContent(false);

        float currentTime = 0.0f;
        float goalContentFadeTime = 2f;

        while (currentTime / goalContentFadeTime < 1)
        {
            currentTime += Time.deltaTime;

            goalContentImg.GetComponent<Image>().color = new Color(1, 1, 1, Mathf.Lerp(1, 0, currentTime / goalContentFadeTime));
            goalContentTxt.color = new Color(169/255.0f, 109/255.0f, 79/255.0f, Mathf.Lerp(1, 0, currentTime / goalContentFadeTime));
            yield return null;
        }

        SetGoalContent(false);
        SetGoalDetailContent(true);
    }

    /// <summary>
    /// 배틀페이지 시작시 화면 중앙에 뜨는 목표 내용 설정
    /// </summary>
    /// <param name="value"></param>
    void SetGoalContent(bool value)
    {
        goalContentImg.SetActive(value);

        if (!value)
            return;
       
        goalContentImg.GetComponent<Image>().color = Color.white;
        goalContentTxt.color = new Color(169 / 255.0f, 109 / 255.0f, 79 / 255.0f, 1);

        if(GameData.Instance.isBattlePage())
            goalContentTxt.text = $"모든 적을 처치하세요!";
        else
            goalContentTxt.text = $"색이 다른 지루루를 처치하세요!";
    }

    /// <summary>
    /// 배틀페이지 시작후 화면 좌측에 뜨는 세부 목표 내용 설정
    /// </summary>
    /// <param name="value"></param>
    void SetGoalDetailContent(bool value)
    {
        goalDetailContentImg.SetActive(value);
        goalDetailContentTxt.fontStyle = FontStyles.Normal;
        if (!value)
            return;

        if (GameData.Instance.isBattlePage())
            goalDetailContentTxt.text = $"모든 적을 처치하기"; // ex ) 우두머리 지루루를 처치하기
        else
            goalDetailContentTxt.text = $"색이 다른\n지루루 처치하기"; // ex ) 우두머리 지루루를 처치하기
    }

    /// <summary>
    /// 설정 버튼 누를 시 동작
    /// </summary>
    public void ActivateSettingUI()
    {
        EventManager.Instance.PostNotification(EVENT_TYPE.UI_Changed, this, UIType.Setting);
    }
}
