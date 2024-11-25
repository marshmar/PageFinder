using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInkMagicController : MonoBehaviour
{
    [SerializeField]
    private Image inkMagicButtonImage;
    [SerializeField]
    private Image inkMagicCooltimeImage;
    [SerializeField]
    private Button inkMagicButton;
    [SerializeField]
    private CoolTimeComponent inkMagicButtoncoolTimeComponent;

    [SerializeField]
    private Sprite[] backgroundImages;

    private Collider[] inkMarkColliders;
    private List<InkMark> inkMarks;
    private List<InkMark> nearbyDifferntTypeInkMarks;
    private Player playerScr;
    private float inkMagicRange;
    private float inkMagicCoolTime;

    private bool isUsingInkMagic;
    public List<InkMark> InkMarks { get => inkMarks; set => inkMarks = value; }
    public bool IsUsingInkMagic { get => isUsingInkMagic; set => isUsingInkMagic = value; }

    // Start is called before the first frame update
    void Awake()
    {
        inkMagicRange = 7.0f;
        inkMagicCoolTime = 15.0f;
        playerScr = DebugUtils.GetComponentWithErrorLogging<Player>(this.gameObject, "Player");
        inkMarks = new List<InkMark>();
        nearbyDifferntTypeInkMarks = new List<InkMark>();

        if (!DebugUtils.CheckIsNullWithErrorLogging<CoolTimeComponent>(inkMagicButtoncoolTimeComponent, this.gameObject))
        {
            inkMagicButtoncoolTimeComponent.CurrSkillCoolTime = inkMagicCoolTime;
        }
    }

    // Update is called once per frame
    void Update()
    {
        FindNearByDifferentTypesOfInkMarks();
        SetInkMarkButton();
    }

    private void FindNearByDifferentTypesOfInkMarks()
    {
        nearbyDifferntTypeInkMarks.Clear();
        foreach (InkMark inkMark in inkMarks)
        {
            if(inkMark.CurrType != playerScr.InkMagicInkType && !inkMark.IsFusioned)
            {
                float distance = Vector3.Distance(playerScr.ModelTr.position, inkMark.transform.position);
                if (distance <= inkMagicRange)
                {
                    nearbyDifferntTypeInkMarks.Add(inkMark);
                }
            }
        }
    }

    public void SetInkMarkButton()
    {
        if(nearbyDifferntTypeInkMarks.Count > 0)
        {
            if (!DebugUtils.CheckIsNullWithErrorLogging<CoolTimeComponent>(inkMagicButtoncoolTimeComponent, this.gameObject))
            {
                if (inkMagicButtoncoolTimeComponent.IsAbleSkill)
                {
                    inkMagicButton.interactable = true;
                }
            }
        }
        else
        {
            inkMagicButton.interactable = false;
        }
    }

    // 잉크 매직 애니메이션 설정
    public void SetInkAnim()
    {
        if (!DebugUtils.CheckIsNullWithErrorLogging<CoolTimeComponent>(inkMagicButtoncoolTimeComponent, this.gameObject))
        {
            if (inkMagicButtoncoolTimeComponent.IsAbleSkill)
            {
                playerScr.Anim.SetTrigger("InkMagic");
                isUsingInkMagic = true;
            }
        }
    }

    // 잉크 퓨전은 애니메이션 이벤트를 통해 진행.
    public void InkFusion()
    {
        for (int i = 0; i < nearbyDifferntTypeInkMarks.Count; i++)
        {
            nearbyDifferntTypeInkMarks[i].InkFusion(playerScr.InkMagicInkType);
        }
        inkMagicButtoncoolTimeComponent.StartCoolDown();
    }

    internal void SetInkMagicButtonImage(InkType inkMagicInkType)
    {
        switch (inkMagicInkType)
        {
            case InkType.RED:
                inkMagicButtonImage.sprite = backgroundImages[0];
                inkMagicCooltimeImage.sprite = backgroundImages[0];
                break;
            case InkType.GREEN:
                inkMagicButtonImage.sprite = backgroundImages[1];
                inkMagicCooltimeImage.sprite = backgroundImages[1];
                break;
            case InkType.BLUE:
                inkMagicButtonImage.sprite = backgroundImages[2];
                inkMagicCooltimeImage.sprite = backgroundImages[2];
                break;
        }
    }
}
