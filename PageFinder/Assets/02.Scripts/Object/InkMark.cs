using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum INKMARK
{
    RED,
    GREEN,
    BLUE,   
    FIRE,   // 불바다
    MIST,   // 안개
    SWAMP   // 습지
}
public class InkMark : MonoBehaviour
{
    #region Variables
    public float duration;
    public float fusionDuration;
    private float spawnTime;
    private bool isFusioned;
    private bool isPlayerInTrigger;
    private bool isOtherMarkInTrigger;
    private INKMARK currMark;
    private INKMARK otherMark;
    private SpriteRenderer spriterenderer;
    public Button QTEButton;
    private Collider myCollider;
    private Collider fusionColl;
    private Player playerScr;
    [SerializeField]
    private Sprite[] inkMarkImgs; // 0: Red, 1: Green,  2: Blue, 3:Swamp
    #endregion

    #region Properties
    public INKMARK CurrMark { get => currMark; set => currMark = value; }
    public float SpawnTime { get => spawnTime; set => spawnTime = value; }
    public bool IsFusioned { get => isFusioned; set => isFusioned = value; }
    public bool IsPlayerInTrigger { get => isPlayerInTrigger; set => isPlayerInTrigger = value; }
    #endregion


    private void Awake()
    {
        isFusioned = false;
        IsPlayerInTrigger = false;
        SpawnTime = 0.0f;
        spriterenderer = GetComponent<SpriteRenderer>();
        QTEButton = GameObject.Find("Player_UI_OP").transform.GetChild(2).GetComponent<Button>();
        myCollider = GetComponent<Collider>();
        fusionColl = null;
        if (QTEButton && myCollider)
            QTEButton.onClick.AddListener(() => MarkFusion(fusionColl));
        playerScr = DebugUtils.GetComponentWithErrorLogging<Player>
            (GameObject.FindGameObjectWithTag("PLAYER"), "Player"
            );
    }

    private void OnDestroy()
    {
        if (isOtherMarkInTrigger)
        {
            SetQTEButtonStatus(false);
        }
        QTEButton.onClick.RemoveListener(() => MarkFusion(fusionColl));
    }

    private void Update()
    {
        spawnTime += Time.deltaTime;
        if (spawnTime >= duration)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        CheckPlayerInTrigger(other, true);

        CheckOtherMarkInTrigger(other, true);

        if (isPlayerInTrigger )
        {
            playerScr.InkGain = playerScr.OriginalInkGain * 1.6f;
            if (isOtherMarkInTrigger)
            {
                if (other.TryGetComponent<InkMark>(out InkMark inkMark))
                {
                    if (!inkMark.isFusioned && !isFusioned && inkMark.IsPlayerInTrigger)
                    {
                        SetQTEButtonStatus(true);
                        fusionColl = other;
                    }
                }
            }
            
        }
    }
    private void OnTriggerExit(Collider other)
    {
        CheckPlayerInTrigger(other, false);

        CheckOtherMarkInTrigger(other, false);

        if (!isPlayerInTrigger || !isOtherMarkInTrigger)
        {
            fusionColl = null;
            SetQTEButtonStatus(false);
        }

        playerScr.InkGain = playerScr.OriginalInkGain;
    }

    private void CheckPlayerInTrigger(Collider coll, bool haveToCheck)
    {
        if (coll.TryGetComponent<Player>(out Player playerScr))
        {
            IsPlayerInTrigger = haveToCheck;
        }
    }

    private void CheckOtherMarkInTrigger(Collider coll, bool haveToCheck)
    {
        if (coll.TryGetComponent<InkMark>(out InkMark inkMark))
        {
            if(currMark != inkMark.CurrMark && !inkMark.isFusioned)
            {
                isOtherMarkInTrigger = haveToCheck;
                otherMark = inkMark.CurrMark;
            }
           
        }
    }

    public void SetQTEButtonStatus(bool setting)
    {
        if(QTEButton == null)
        {
            Debug.LogError($"{this.gameObject.name}'s QTEButton is null");
            return;
        }
        QTEButton.gameObject.SetActive(setting);
    }

    public bool CompareSpawnTime(Collider coll)
    {
        bool result = false;
        if(coll.TryGetComponent<InkMark>(out InkMark inkMarkScr))
        {
            result =  spawnTime >= inkMarkScr.spawnTime; 
        }
        return result;
    }
    public void SetSprites()
    {
        if(spriterenderer == null)
        {
            Debug.LogError("Renderer is null");
        }
        if(currMark == INKMARK.RED)
        {
            spriterenderer.sprite = inkMarkImgs[0];
        }
        else if (currMark == INKMARK.GREEN)
        {
            spriterenderer.sprite = inkMarkImgs[1];
        }
        else if(currMark == INKMARK.BLUE)
        {
            spriterenderer.sprite = inkMarkImgs[2];
        }
        else if(currMark == INKMARK.FIRE)
        {
            spriterenderer.material.color = Color.yellow;
        }
        else if(currMark == INKMARK.MIST)
        {
            spriterenderer.material.color = Color.cyan;
        }
        else if(currMark == INKMARK.SWAMP)
        {
            spriterenderer.sprite = inkMarkImgs[3];
        }
    }

    public void SetMaterials()
    {
        if (spriterenderer == null)
        {
            Debug.LogError("Renderer is null");
        }
        if (currMark == INKMARK.RED)
        {
            spriterenderer.material.color = Color.red;
        }
        else if (currMark == INKMARK.GREEN)
        {
            spriterenderer.material.color = Color.green;
        }
        else if (currMark == INKMARK.BLUE)
        {
            spriterenderer.material.color = Color.blue;
        }
        else if (currMark == INKMARK.FIRE)
        {
            spriterenderer.material.color = Color.yellow;
        }
        else if (currMark == INKMARK.MIST)
        {
            spriterenderer.material.color = Color.cyan;
        }
        else if (currMark == INKMARK.SWAMP)
        {
            spriterenderer.sprite = inkMarkImgs[0];
        }
    }



    public INKMARK InkFusion(INKMARK baseMark, INKMARK subMark)
    {
        INKMARK fusionInk = baseMark;
        if (baseMark == INKMARK.RED)
        {
            if (subMark == INKMARK.GREEN)
                fusionInk = INKMARK.FIRE;
            else if (subMark == INKMARK.BLUE)
                fusionInk = INKMARK.MIST;
        }
        else if (baseMark == INKMARK.GREEN)
        {
            if (subMark == INKMARK.RED)
                fusionInk = INKMARK.FIRE;
            else if (subMark == INKMARK.BLUE)
                fusionInk = INKMARK.SWAMP;
        }
        else if (baseMark == INKMARK.BLUE)
        {
            if (subMark == INKMARK.RED)
                fusionInk = INKMARK.MIST;
            else if (subMark == INKMARK.GREEN)
                fusionInk = INKMARK.SWAMP;
        }

        return fusionInk;
    }

    public void MarkFusion(Collider subColl)
    {
        if(fusionColl == null || subColl == null)
        {
            return;
        }

        INKMARK fusionMark = InkFusion(currMark, otherMark);
        this.isFusioned = true;
        this.currMark = fusionMark;
        this.spawnTime = 0.0f;
        this.duration = fusionDuration;
        this.SetSprites();
        if (subColl.TryGetComponent<InkMark>(out InkMark inkMarkScr))
        {
            inkMarkScr.SpawnTime = 0.0f;
            inkMarkScr.IsFusioned = true;
            inkMarkScr.CurrMark = fusionMark;
            inkMarkScr.duration = fusionDuration;
            inkMarkScr.SetSprites();
        }
        SetQTEButtonStatus(false);
    }
}
