using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.WSA;

public class ReinforceUIManager : MonoBehaviour
{
    public Canvas Reinforce_Canvas;
    public Image[] Content_Img = new Image[3];  // 증강 내용 이미지
    public TMP_Text[] Title_Txt = new TMP_Text[3];      // 이미지에 따른 증강 제목 
    public TMP_Text[] Content_Txt = new TMP_Text[3];    // 증강 제목에 따른 내용 
    public Sprite[] content_Spr = new Sprite[6];
    // 현재 띄워진 증강체에 어떤 내용이 들어가있는지를 표시해야함 
    List<int> icurrentReinforceBodys = new List<int>() { 4, 1, 5 };

    bool didSelectReinforceBody = false;

    // 스크립트 관련
    ExpUIManager expUIManager;
    Exp exp;
    Level level;

    private void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("PLAYER");
        expUIManager = GameObject.Find("UIManager").GetComponent<ExpUIManager>();
        exp = player.GetComponent<Exp>();
        level = player.GetComponent<Level>();
    }
    private void Start()
    {
        SetUI();
        ChangeReinforceCanvasState(false);
    }

    

    /// <summary>
    /// 증강 내용 이미지를 변경하는 함수
    /// </summary>
    public void ChangeImgsContent()
    {
        for (int i = 0; i < Content_Img.Length; i++)
        {
            Content_Img[i].sprite = content_Spr[icurrentReinforceBodys[i]]; 
        }  
    }

    /// <summary>
    /// 기억 제목들을 변경한다.
    /// </summary>
    void ChangeTitleTxts()
    {
        for (int i = 0; i < Title_Txt.Length; i++)
            Title_Txt[i].text = ReturnTitleTxt(icurrentReinforceBodys[i]);
    }

    /// <summary>
    /// 기억 제목 텍스트를 리턴한다. 
    /// </summary>
    /// <param name="n">기억 요소</param>
    /// <returns>기억 제목 문자열</returns>
    string ReturnTitleTxt(int n)
    {
        /*
       *  0 : 최대 체력
       *  1 : 공격력
       *  2 : 주문력
       *  3 : 최대 마나
       *  4 : 팔레트
       *  5 : 이속
       */
        switch (n)
        {
            case 0:
                return "MaxHp";
            case 1:
                return "AttackPower";
            case 2:
                return "MagicalPower";
            case 3:
                return "MaxMana";
            case 4:
                return "Palette";
            case 5:
                return "Speed";
            default:
                Debug.LogWarning(n  + " 인덱스 값 받음 잘못된 입력");
                return "Error";
        }
    }

    /// <summary>
    /// 기억 내용 텍스트들을 변경한다. 
    /// </summary>
    void ChangeContentTxts()
    {
        for (int i = 0; i < Content_Txt.Length; i++)
            Content_Txt[i].text = ReturnContentTxt(icurrentReinforceBodys[i]);
    }

    /// <summary>
    /// 기억 내용 텍스트를 리턴한다. 
    /// </summary>
    /// <param name="n">기억 요소</param>
    /// <returns>기억 내용 문자열</returns>
    string ReturnContentTxt(int n)
    {
        /*
       *  0 : 최대 체력
       *  1 : 공격력
       *  2 : 주문력
       *  3 : 최대 마나
       *  4 : 팔레트
       *  5 : 이속
       */
        switch (n)
        {
            case 0:
                return "Max Hp ++";
            case 1:
                return "AttackPower ++";
            case 2:
                return "MagicalPower ++";
            case 3:
                return "MaxMana ++";
            case 4:
                return "Color A ++";
            case 5:
                return "Speed ++";
            default:
                Debug.LogWarning(n + " 인덱스 값 받음 잘못된 입력");
                return "Error";
        }
    }

    /// <summary>
    /// 증강체를 선택했을 경우 동작하는 함수
    /// </summary>
    public void SelectReinforcedBody()
    {
        GameObject clickBtn = EventSystem.current.currentSelectedGameObject;

        for (int i = 0; i < Content_Img.Length; i++)
        {
            if (clickBtn.name.Contains(i.ToString()))
            {
                ChangeDidSelectReinforceBody(true);
                ReinforceSelectedBody(i);
                ChangeReinforceCanvasState(false);
                expUIManager.ResetExpBar();
                exp.ResetExp();
                level.IncreaseCurrentLevel(1); // 레벨 증가
                break;
            }
        }
    }

    /// <summary>
    /// 선택한 증강체를 강화한다. 
    /// </summary>
    /// <param name="n">강화할 증강체 번호</param>
    void ReinforceSelectedBody(int n)
    {
        /*
         *  0 : 최대 체력
         *  1 : 공격력
         *  2 : 주문력
         *  3 : 최대 마나
         *  4 : 팔레트
         *  5 : 이속
         */
        switch (n)
        {
            case 0:
                // playerScripts.MaxHp 값 증가 
                break;
            case 1:
                // playerScripts.attackPower 값 증가 
                break;
            case 2:
                // playerScripts.magicalPower 값 증가 
                break;
            case 3:
                // playerScripts. 값 증가 
                break;
            case 4:
                // paletteScripts.totalColor 값 추가
                break;
            case 5:
                // playerScripts.skill 추가 
                break;

        }
    }

    /// <summary>
    /// icurrentReinforceBodys 리스트에 값을 추가한다. 
    /// </summary>
    /// <param name="reinforcedBodyNum">추가할 증강체의 번호</param>
    void AddReinforceBody(int reinforcedBodyNum)
    {
        icurrentReinforceBodys.Add(reinforcedBodyNum);
    }

    /// <summary>
    /// icurrentReinforceBodys 리스트 내에 값을 전부 지운다. 
    /// </summary>
    void ResetICurrentReinforceBodys()
    {
        icurrentReinforceBodys.Clear();
    }

    /// <summary>
    /// icurrentReinforceBodys의 값을 설정한다. 
    /// </summary>
    void SetICurrentReinforceBodys()
    {
        int randomNum = 5;
        for (int i = 0; i < 3; i++)
        { 
            if(i!=0) //  첫 칸에는 무조건 이속이 나오게 설정
                randomNum = ReturnReinforcedBodyRandNum();
            AddReinforceBody(randomNum);
        }     
    }

    /// <summary>
    /// 강화할 증강체의 번호를 랜덤으로 리턴한다. 
    /// </summary>
    /// <returns>강화할 증강체의 번호</returns>
    int ReturnReinforcedBodyRandNum()
    {
        int randomNum = Random.Range(0, 6); // 0 ~ 6
        while (icurrentReinforceBodys.IndexOf(randomNum) != -1) // icurrentReinforceBodys 리스트에 이미 증강체 번호가 있는 경우
            randomNum = Random.Range(0, 6); // 다시 랜덤으로 색깔 배정
        return randomNum;
    }

    /// <summary>
    /// UI를 설정한다. 
    /// </summary>
    void SetUI()
    {
        ResetICurrentReinforceBodys();  // icurrentReinforceBodys 리셋
        SetICurrentReinforceBodys();    // icurrentReinforceBodys 값 설정
        //Debug.Log(icurrentReinforceBodys.Count);
        ChangeImgsContent(); // 이미지 변경
        ChangeTitleTxts();    // 제목 텍스트 변경
        ChangeContentTxts();  // 내용 텍스트 변경
    }

    /// <summary>
    /// 증강 캔버스의 상태를 변경한다. 
    /// </summary>
    /// <param name="value">변경할 상태 값</param>
    public void ChangeReinforceCanvasState(bool value)
    {
        Reinforce_Canvas.gameObject.SetActive(value);
    }

    /// <summary>
    /// 증강 UI를 활성화한다. 
    /// </summary>
    /// <returns></returns>
    public IEnumerator ActivateReinforceUI()
    {
        yield return new WaitForSeconds(0.25f);
        ChangeReinforceCanvasState(true);
        SetUI();
    }

    /// <summary>
    /// 증강체를 선택했는지 확인하는 변수의 값을 변경한다. 
    /// </summary>
    /// <param name="value">변경할 값</param>
    public void ChangeDidSelectReinforceBody(bool value)
    {
        didSelectReinforceBody = value;
    }

    /// <summary>
    /// 증강체를 선택했는지 확인하는 변수의 값을 리턴한다. 
    /// </summary>
    /// <returns>증강체를 선택했는지 확인하는 변수의 값</returns>
    public bool ReturnDidSelectReinforceBody()
    {
        return didSelectReinforceBody;
    }
}
