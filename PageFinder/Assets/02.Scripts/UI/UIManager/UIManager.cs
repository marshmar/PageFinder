using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CanvasType
{
    BATTLE = 0,
    RIDDLE = 1,
    RESULT = 2,
    REWARD = 3,
    SHOP = 4,
    SETTING = 5,
    DIARY = 6,
    PLAYERUIOP = 7,
    PLAYERUIINFO = 8,
    TIMER = 9,
    PAGEMAP = 10,
    TREASURE = 11,
    COMMA = 12
}

public enum UIType
{
    Battle,
    RiddleBook,
    Shop,
    Reward,
    Win,
    Defeat,
    Goal_Fail,
    Setting,
    Diary,
    Help,
    RewardToDiary,
    BackDiaryFromReward,
    ShopToDiary,
    BackDiaryFromShop,
    PageMap,
    RiddlePlay,
    Treasure,
    Comma
}

public class UIManager : Singleton<UIManager>, IListener
{
    // 강해담 추가  - bgm 용도
    // -----------------------------------------
    [SerializeField] private AudioSource bgmAudioSource;
    private bool audioFirstPlay;
    // -----------------------------------------
    ShopUIManager shopUIManager;
    BattleUIManager battleUIManager;
    SettingUIManager settingUIManager;
    ScriptManager reward;
    ResultUIManager resultUIManager;
    TreasureUIManager treasureUIManager;
    CommaUIManager commaUIManager;

    [SerializeField] private bool isFixedMap = false;
    // 스크립트 -> 캔버스로 변경 중
    [SerializeField] List<Canvas> canvases;

    // 강해담 추가
    DiaryManager diary;

    Canvas plyaerUiOp;
    Canvas plyaerUiInfo;

    bool isSetting;

    public UIType uiType;

    private void Start()
    {
        battleUIManager = DebugUtils.GetComponentWithErrorLogging<BattleUIManager>(canvases[(int)CanvasType.BATTLE].gameObject, "BattleUIManager");
        shopUIManager = DebugUtils.GetComponentWithErrorLogging<ShopUIManager>(gameObject, "ShopUIManager");
        //plyaerUiOp = DebugUtils.GetComponentWithErrorLogging<Canvas>(GameObject.Find("Player_UI_OP"), "Canvas");
        //plyaerUiInfo = DebugUtils.GetComponentWithErrorLogging<Canvas>(GameObject.Find("Player_UI_Info"), "Canvas");
        reward = gameObject.GetComponent<ScriptManager>();
        resultUIManager = DebugUtils.GetComponentWithErrorLogging<ResultUIManager>(canvases[(int)CanvasType.RESULT].gameObject, "ResultUIManager");
        treasureUIManager = DebugUtils.GetComponentWithErrorLogging<TreasureUIManager>(canvases[(int)CanvasType.TREASURE].gameObject, "TreasureUIManager");
        commaUIManager = DebugUtils.GetComponentWithErrorLogging<CommaUIManager>(canvases[(int)CanvasType.COMMA].gameObject, "CommaUIManager");

        isSetting = false;
        Time.timeScale = 1;

        diary = DebugUtils.GetComponentWithErrorLogging<DiaryManager>(this.gameObject, "DiaryManager");
        audioFirstPlay = false;

        // ToDo: UI Changed;
        //EventManager.Instance.AddListener(EVENT_TYPE.UI_Changed, this);

        // 모든 UI 끄기
        //SetUIActiveState(new List<CanvasType>());
        //EventManager.Instance.PostNotification(EVENT_TYPE.UI_Changed, this, UIType.Battle);
    }

    private IEnumerator RewardCoroutine(bool active)
    {
        battleUIManager.SetBattleUICanvasState(!active, isSetting);
        //riddleBookUIManager.SetRiddleUICanvasState(!active);
        //shopUIManager.SetShopUICanvasState(!active);
        settingUIManager.SetSettingUICanvasState(!active);

        yield return new WaitForSeconds(1.0f);

        plyaerUiOp.enabled = !active;
        plyaerUiInfo.enabled = !active;
        reward.SetScriptUICanvasState(active);
        // 강해담 추가
        // -----------------------------
        reward.SetScripts();
        // -----------------------------
    }

    /// <summary>
    /// UI의 활성화 상태를 설정
    /// </summary>
    /// <param name="canvasTypes">활성화할 캔버스들</param>
    /// <param name="resultType">결과화면 활성화시 옵션</param>
    private void SetUIActiveState(List<CanvasType> canvasTypes)
    {
        var canvasIndexes = new List<int>();

        for (int i = 0; i < canvases.Count; i++) canvasIndexes.Add(i);

        // 활성화할 캔버스들 설정
        foreach (CanvasType canvasType in canvasTypes)
        {
            int i = (int)canvasType;

            // 활성화할 캔버스가 현재 비활성화인 경우
            if (!canvases[i].gameObject.activeSelf) canvases[i].gameObject.SetActive(true);

            canvasIndexes.Remove(i);
        }
           
        // 비활성화할 캔버스들 설정
        foreach(int i in canvasIndexes)
        {
            // 활성화 되어있다면
            if(canvases[i].gameObject.activeSelf) canvases[i].gameObject.SetActive(false);
        }
    }


    public void OnEvent(EVENT_TYPE eventType, Component Sender, object Param = null)
    {
        switch (eventType)
        {
            // ToDo: UI Changed;
            /*            case EVENT_TYPE.UI_Changed:
                            if (Param == null) Debug.LogWarning($"{Param}");
                            else SetUI((UIType)Param);
                            break;*/
        }
    }

    // 최신 버전
    private void SetUI(UIType uiType)
    {
        bool active = true;
        //Debug.Log($"UI 변경 : {this.uiType} -> {uiType}");
        switch (uiType)
        {
            case UIType.Battle:
                if (Time.timeScale == 0) Time.timeScale = 1;

                //shopUIManager.SetShopUICanvasState(!active);
                reward.SetScriptUICanvasState(!active);

                SetUIActiveState(new List<CanvasType> { CanvasType.BATTLE, CanvasType.PLAYERUIINFO, CanvasType.PLAYERUIOP });
                EventManager.Instance.PostNotification(EVENT_TYPE.Restart_CoolTime, this);
/*                // 강해담 추가
                if (!audioFirstPlay)
                {
                    bgmAudioSource.Play();
                    audioFirstPlay = true;
                }
                else
                {
                    bgmAudioSource.UnPause();
                }*/
                break;

            case UIType.RiddlePlay:
                if (Time.timeScale == 0) Time.timeScale = 1;
                //shopUIManager.SetShopUICanvasState(!active);
                reward.SetScriptUICanvasState(!active);
                SetUIActiveState(new List<CanvasType> { CanvasType.BATTLE, CanvasType.PLAYERUIINFO, CanvasType.PLAYERUIOP, CanvasType.TIMER });
                EventManager.Instance.PostNotification(EVENT_TYPE.Restart_CoolTime, this);
                break;
            case UIType.RiddleBook:
                //shopUIManager.SetShopUICanvasState(!active);
                reward.SetScriptUICanvasState(!active);
                SetUIActiveState(new List<CanvasType> { CanvasType.RIDDLE });
                EventManager.Instance.PostNotification(EVENT_TYPE.Reset_CoolTime, this);
                //bgmAudioSource.UnPause();
                break;
            case UIType.Shop:
                //shopUIManager.SetShopUICanvasState(active);
                reward.SetScriptUICanvasState(!active);
                SetUIActiveState(new List<CanvasType> { CanvasType.SHOP });
                //bgmAudioSource.UnPause();
                break;
            case UIType.Reward:
                //shopUIManager.SetShopUICanvasState(!active);
                reward.SetScriptUICanvasState(active);
                EventManager.Instance.PostNotification(EVENT_TYPE.Reset_CoolTime, this);
                SetUIActiveState(new List<CanvasType> { CanvasType.REWARD });
                //bgmAudioSource.UnPause();
                break;
            case UIType.Win:
                //shopUIManager.SetShopUICanvasState(!active);
                reward.SetScriptUICanvasState(!active);
                resultUIManager.SetResultData(ResultType.WIN, 3);
                SetUIActiveState(new List<CanvasType> { CanvasType.RESULT });
                break;
            case UIType.Defeat:
                //shopUIManager.SetShopUICanvasState(!active);
                reward.SetScriptUICanvasState(!active);
                resultUIManager.SetResultData(ResultType.DEFAT, 3);
                SetUIActiveState(new List<CanvasType> { CanvasType.RESULT });
                break;
            case UIType.Goal_Fail:
                //shopUIManager.SetShopUICanvasState(!active);
                reward.SetScriptUICanvasState(!active);
                resultUIManager.SetResultData(ResultType.GOAL_FAIL, 1.5f);
                SetUIActiveState(new List<CanvasType> { CanvasType.BATTLE, CanvasType.RESULT });
                break;
            case UIType.Setting:
                Time.timeScale = 0;
                //shopUIManager.SetShopUICanvasState(!active);
                reward.SetScriptUICanvasState(!active);
                EventManager.Instance.PostNotification(EVENT_TYPE.Pause_CoolTime, this);
                SetUIActiveState(new List<CanvasType> { CanvasType.SETTING });
                //bgmAudioSource.Pause();
                break;
            case UIType.Diary:
                Time.timeScale = 0;
                //shopUIManager.SetShopUICanvasState(!active);
                reward.SetScriptUICanvasState(!active);
                //diary.SetDiaryUICanvasState(active, "Battle");
                SetUIActiveState(new List<CanvasType> { CanvasType.DIARY });
                //bgmAudioSource.Pause();
                break;
            case UIType.Help:
                //shopUIManager.SetShopUICanvasState(!active);
                //diary.SetDiaryUICanvasState(active, "Battle");
                SetUIActiveState(new List<CanvasType> { CanvasType.DIARY });
                //bgmAudioSource.Pause();
                break;
            case UIType.RewardToDiary:
                //shopUIManager.SetShopUICanvasState(!active);
                reward.SetScriptUICanvasState(!active);
                //diary.SetDiaryUICanvasState(active, "Reward");
                SetUIActiveState(new List<CanvasType> { CanvasType.DIARY });
                //bgmAudioSource.Pause();
                break;
            case UIType.BackDiaryFromReward:
                //shopUIManager.SetShopUICanvasState(!active);
                reward.SetScriptUICanvasState(active, false);
                //diary.SetDiaryUICanvasState(!active);
                SetUIActiveState(new List<CanvasType> { CanvasType.REWARD });
                //bgmAudioSource.UnPause();
                break;
            case UIType.ShopToDiary:
                //shopUIManager.SetShopUICanvasState(!active);
                reward.SetScriptUICanvasState(!active);
                //diary.SetDiaryUICanvasState(active, "Shop");
                SetUIActiveState(new List<CanvasType> { CanvasType.SHOP });
                //bgmAudioSource.Pause();
                break;
            case UIType.BackDiaryFromShop:
                //shopUIManager.SetShopUICanvasState(active, false);
                reward.SetScriptUICanvasState(!active);
                //diary.SetDiaryUICanvasState(!active);
                SetUIActiveState(new List<CanvasType> { CanvasType.SHOP });
                //bgmAudioSource.UnPause();
                break;
            case UIType.PageMap:
                //shopUIManager.SetShopUICanvasState(!active);
                treasureUIManager.SetUICanvasState(!active);
                reward.SetScriptUICanvasState(!active);
                //diary.SetDiaryUICanvasState(!active);
                SetUIActiveState(new List<CanvasType> { CanvasType.PLAYERUIINFO, CanvasType.PLAYERUIOP });
                if(isFixedMap) canvases[10].GetComponentInParent<FixedMap>().playerNode.portal.gameObject.SetActive(true);
                else canvases[10].GetComponentInParent<ProceduralMapGenerator>().playerNode.portal.gameObject.SetActive(true);
                //bgmAudioSource.UnPause();
                break;
            case UIType.Treasure:
                treasureUIManager.SetUICanvasState(active);
                break;
            case UIType.Comma:
                commaUIManager.gameObject.SetActive(active);
                break;
            default:
                Debug.LogWarning($"Param : {uiType}");
                break;
        }

        this.uiType = uiType;
    }

    // 이전 버전
    /*public void SetUIActiveState(string name)
    {
        bool active = true;
        switch (name)
        {
            case "Battle":
                if (isSetting)
                    Time.timeScale = 1;

                battleUIManager.SetBattleUICanvasState(active, isSetting);
                shopUIManager.SetShopUICanvasState(!active);
                settingUIManager.SetSettingUICanvasState(!active);

                plyaerUiOp.enabled = active;
                plyaerUiInfo.enabled = active;
                reward.SetScriptUICanvasState(!active);

                if (isSetting)
                    isSetting = false;
                diary.SetDiaryUICanvasState(!active);

                // 강해담 추가
                if (!audioFirstPlay)
                {
                    bgmAudioSource.Play();
                    audioFirstPlay = true;
                }
                else
                {
                    bgmAudioSource.UnPause();
                }
                break;

            case "RiddleBook":
                battleUIManager.SetBattleUICanvasState(!active, isSetting);
                //riddleBookUIManager.SetRiddleUICanvasState(active);
                shopUIManager.SetShopUICanvasState(!active);
                settingUIManager.SetSettingUICanvasState(!active);

                plyaerUiOp.enabled = !active;
                plyaerUiInfo.enabled = !active;
                reward.SetScriptUICanvasState(!active);

                bgmAudioSource.UnPause();
                break;

            case "Shop":
                battleUIManager.SetBattleUICanvasState(!active, isSetting);
                //riddleBookUIManager.SetRiddleUICanvasState(!active);
                shopUIManager.SetShopUICanvasState(active);
                settingUIManager.SetSettingUICanvasState(!active);

                plyaerUiOp.enabled = !active;
                plyaerUiInfo.enabled = !active;
                reward.SetScriptUICanvasState(!active);

                bgmAudioSource.UnPause();
                break;

            case "Reward":
                battleUIManager.SetBattleUICanvasState(!active, isSetting);
                //riddleBookUIManager.SetRiddleUICanvasState(!active);
                shopUIManager.SetShopUICanvasState(!active);
                settingUIManager.SetSettingUICanvasState(!active);

                plyaerUiOp.enabled = !active;
                plyaerUiInfo.enabled = !active;
                reward.SetScriptUICanvasState(active);

                bgmAudioSource.UnPause();
                break;

            case "Success":
                battleUIManager.SetBattleUICanvasState(!active, isSetting);
                //riddleBookUIManager.SetRiddleUICanvasState(!active);
                shopUIManager.SetShopUICanvasState(!active);
                settingUIManager.SetSettingUICanvasState(!active);

                plyaerUiOp.enabled = !active;
                plyaerUiInfo.enabled = !active;
                reward.SetScriptUICanvasState(!active);

                Invoke("LoadNextScene", 3);
                break;

            case "Defeat":
                battleUIManager.SetBattleUICanvasState(!active, isSetting);
                //riddleBookUIManager.SetRiddleUICanvasState(!active);
                shopUIManager.SetShopUICanvasState(!active);
                settingUIManager.SetSettingUICanvasState(!active);

                plyaerUiOp.enabled = !active;
                plyaerUiInfo.enabled = !active;
                reward.SetScriptUICanvasState(!active);

                Invoke("LoadNextScene", 3);
                break;


            case "Setting":
                isSetting = true;
                battleUIManager.SetBattleUICanvasState(!active, isSetting);
                //riddleBookUIManager.SetRiddleUICanvasState(!active);
                shopUIManager.SetShopUICanvasState(!active);
                settingUIManager.SetSettingUICanvasState(active);
                plyaerUiOp.enabled = !active;
                plyaerUiInfo.enabled = !active;
                reward.SetScriptUICanvasState(!active);

                bgmAudioSource.Pause();
                break;
            // 강해담 추가
            //------------------------------------------------
            case "Diary":
                battleUIManager.SetBattleUICanvasState(!active, isSetting);
                //riddleBookUIManager.SetRiddleUICanvasState(!active);
                shopUIManager.SetShopUICanvasState(!active);
                settingUIManager.SetSettingUICanvasState(!active);
                plyaerUiOp.enabled = !active;
                plyaerUiInfo.enabled = !active;
                reward.SetScriptUICanvasState(!active);

                diary.SetDiaryUICanvasState(active, "Battle");
                bgmAudioSource.Pause();
                break;

            case "Help":
                battleUIManager.SetBattleUICanvasState(!active, isSetting);
                //riddleBookUIManager.SetRiddleUICanvasState(!active);
                shopUIManager.SetShopUICanvasState(!active);
                settingUIManager.SetSettingUICanvasState(!active);
                diary.SetDiaryUICanvasState(active, "Battle");
                bgmAudioSource.Pause();
                break;

            case "RewardToDiary":
                battleUIManager.SetBattleUICanvasState(!active, isSetting);
                //riddleBookUIManager.SetRiddleUICanvasState(!active);
                shopUIManager.SetShopUICanvasState(!active);

                plyaerUiOp.enabled = !active;
                plyaerUiInfo.enabled = !active;
                reward.SetScriptUICanvasState(!active);

                diary.SetDiaryUICanvasState(active, "Reward");
                bgmAudioSource.Pause();
                break;

            case "BackDiaryFromReward":
                battleUIManager.SetBattleUICanvasState(!active, isSetting);
                //riddleBookUIManager.SetRiddleUICanvasState(!active);
                shopUIManager.SetShopUICanvasState(!active);

                plyaerUiOp.enabled = !active;
                plyaerUiInfo.enabled = !active;
                reward.SetScriptUICanvasState(active, false);

                diary.SetDiaryUICanvasState(!active);
                bgmAudioSource.UnPause();
                break;

            case "ShopToDiary":
                battleUIManager.SetBattleUICanvasState(!active, isSetting);
                //riddleBookUIManager.SetRiddleUICanvasState(!active);
                shopUIManager.SetShopUICanvasState(!active);

                //plyaerUiOp.enabled = !active;
                //plyaerUiInfo.enabled = !active;
                reward.SetScriptUICanvasState(!active);

                diary.SetDiaryUICanvasState(active, "Shop");
                bgmAudioSource.Pause();
                break;

            case "BackDiaryFromShop":
                battleUIManager.SetBattleUICanvasState(!active, isSetting);
                //riddleBookUIManager.SetRiddleUICanvasState(!active);
                shopUIManager.SetShopUICanvasState(active, false);

                plyaerUiOp.enabled = !active;
                plyaerUiInfo.enabled = !active;
                reward.SetScriptUICanvasState(!active);

                diary.SetDiaryUICanvasState(!active);
                bgmAudioSource.UnPause();
                break;

            default:
                
                break;
        }
    }*/
}