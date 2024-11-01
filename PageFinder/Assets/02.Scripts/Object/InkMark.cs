using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum InkType
{
    NONE,
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
    private InkType currType;
    private InkType otherType;
    private SpriteRenderer spriterenderer;
    public Button QTEButton;
    private Collider myCollider;
    private Collider fusionColl;
    private Player playerScr;
    [SerializeField]
    private Sprite[] inkMarkImgs; // 0: Red, 1: Green,  2: Blue, 3:Swamp
    #endregion

    #region Properties
    public InkType CurrType { get => currType; set => currType = value; }
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
            if(currType != inkMark.CurrType && !inkMark.isFusioned)
            {
                isOtherMarkInTrigger = haveToCheck;
                otherType = inkMark.CurrType;
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
        if(currType == InkType.RED)
        {
            spriterenderer.sprite = inkMarkImgs[0];
        }
        else if (currType == InkType.GREEN)
        {
            spriterenderer.sprite = inkMarkImgs[1];
        }
        else if(currType == InkType.BLUE)
        {
            spriterenderer.sprite = inkMarkImgs[2];
        }
        else if(currType == InkType.FIRE)
        {
            spriterenderer.material.color = Color.yellow;
        }
        else if(currType == InkType.MIST)
        {
            spriterenderer.material.color = Color.cyan;
        }
        else if(currType == InkType.SWAMP)
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
        if (currType == InkType.RED)
        {
            spriterenderer.material.color = Color.red;
        }
        else if (currType == InkType.GREEN)
        {
            spriterenderer.material.color = Color.green;
        }
        else if (currType == InkType.BLUE)
        {
            spriterenderer.material.color = Color.blue;
        }
        else if (currType == InkType.FIRE)
        {
            spriterenderer.material.color = Color.yellow;
        }
        else if (currType == InkType.MIST)
        {
            spriterenderer.material.color = Color.cyan;
        }
        else if (currType == InkType.SWAMP)
        {
            spriterenderer.sprite = inkMarkImgs[0];
        }
    }



    public InkType InkFusion(InkType baseType, InkType subType)
    {
        InkType fusionInk = baseType;
        if (baseType == InkType.RED)
        {
            if (subType == InkType.GREEN)
                fusionInk = InkType.FIRE;
            else if (subType == InkType.BLUE)
                fusionInk = InkType.MIST;
        }
        else if (baseType == InkType.GREEN)
        {
            if (subType == InkType.RED)
                fusionInk = InkType.FIRE;
            else if (subType == InkType.BLUE)
                fusionInk = InkType.SWAMP;
        }
        else if (baseType == InkType.BLUE)
        {
            if (subType == InkType.RED)
                fusionInk = InkType.MIST;
            else if (subType == InkType.GREEN)
                fusionInk = InkType.SWAMP;
        }

        return fusionInk;
    }

    public void MarkFusion(Collider subColl)
    {
        if(fusionColl == null || subColl == null)
        {
            return;
        }

        InkType fusionType = InkFusion(currType, otherType);
        this.isFusioned = true;
        this.currType = fusionType;
        this.spawnTime = 0.0f;
        this.duration = fusionDuration;
        this.SetSprites();
        if (subColl.TryGetComponent<InkMark>(out InkMark inkMarkScr))
        {
            inkMarkScr.SpawnTime = 0.0f;
            inkMarkScr.IsFusioned = true;
            inkMarkScr.CurrType = fusionType;
            inkMarkScr.duration = fusionDuration;
            inkMarkScr.SetSprites();
        }
        SetQTEButtonStatus(false);
    }
}
