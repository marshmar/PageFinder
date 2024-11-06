using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static System.Collections.Specialized.BitVector32;

public class CompensationUIMangaer : MonoBehaviour
{
    [SerializeField]
    private Canvas CompensationCanvas;

    [SerializeField]
    private Button[] compensations;
    [SerializeField]
    private Button selectBtn;

    [SerializeField]
    private Sprite[] compensationIcons;
    [SerializeField]
    private Sprite[] compensationDecorationIcons;

    [SerializeField]
    private TMP_Text coinTxt;
    [SerializeField]
    private TMP_Text priceTxt;

    Player player;
    private void Start()
    {
        player = GameObject.FindWithTag("PLAYER").GetComponent<Player>();

    }

    public void SetCompensationCanvasState(bool value)
    {
        CompensationCanvas.gameObject.SetActive(value);
        
        if (!value)
            return;

        // 초기화
        SetCoin();
        //SetCompensations();
        SetExchangePrice();
        SetSelcecBtnState(false);
    }

    private void SetCompensations()
    {
        Transform compensation;
        for (int i=0; i< compensations.Length; i++)
        {
            compensation = compensations[i].transform;
            // 아이콘
            compensation.GetChild(0).GetComponent<Image>().sprite = compensationIcons[0];
            // 이름
            compensation.GetChild(1).GetComponent<TMP_Text>().text = compensationIcons[0].name;
            // 내용
            compensation.GetChild(2).GetComponent<TMP_Text>().text = "ㅁㄴㅇㄹㅁㄴㅇㄹ";
            // 장식
            compensation.GetChild(3).GetComponent<Image>().sprite = compensationDecorationIcons[0];
        }
    }

    /// <summary>
    /// 보상을 선택했을 경우 선택버튼을 활성화한다.
    /// </summary>
    public void ActivateSelectBtn()
    {
        SetSelcecBtnState(true);
    }

    public void SetSelcecBtnState(bool value)
    {
        selectBtn.interactable = value;
    }

    public void OpenDiary()
    {

    }

    /// <summary>
    /// 보상들을 전부 교체한다.
    /// </summary>
    public void changeCompensations()
    {

    }

    private void SetCoin()
    {
        //coinTxt.text = player.Coin;
    }

    private void SetExchangePrice()
    {
        priceTxt.text = "11";
    }

    public void MoveToPageMap()
    {
        //UIManager.Instance.SetUIActiveState("PageMap");
    }
}
