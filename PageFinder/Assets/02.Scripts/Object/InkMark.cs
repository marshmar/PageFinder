using System;
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


    private SpriteRenderer spriterenderer;

    private Player playerScr;
    [SerializeField]
    private Sprite[] inkMarkImgs; // 0: Red, 1: Green,  2: Blue, 3:Swamp
    private bool decreasingTransparency;
    private PlayerInkMagicController playerInkMagicControllerScr;

    private Coroutine transparencyCoroutine;
    #endregion

    #region Properties
    public InkType CurrType { get => currType; set => currType = value; }
    public float SpawnTime { get => spawnTime; set 
        { 
            spawnTime = value;     
            spriterenderer.color = new Color(spriterenderer.color.r, spriterenderer.color.g, spriterenderer.color.b, 1.0f);
        } 
    }
    public bool IsFusioned { get => isFusioned; set => isFusioned = value; }
    public bool IsPlayerInTrigger { get => isPlayerInTrigger; set => isPlayerInTrigger = value; }
    #endregion


    private void Awake()
    {
        isFusioned = false;
        IsPlayerInTrigger = false;
        spawnTime = 0.0f;
        spriterenderer = GetComponent<SpriteRenderer>();


        playerScr = DebugUtils.GetComponentWithErrorLogging<Player>
            (GameObject.FindGameObjectWithTag("PLAYER"), "Player"
            );
        playerInkMagicControllerScr = DebugUtils.GetComponentWithErrorLogging<PlayerInkMagicController>
            (GameObject.FindGameObjectWithTag("PLAYER"), "PlayerInkMagicController"
            );
        playerInkMagicControllerScr.InkMarks.Add(this);
    }

    private void OnDestroy()
    {
        playerInkMagicControllerScr.InkMarks.Remove(this);
/*        if (isOtherMarkInTrigger)
        {
            SetQTEButtonStatus(false);
        }
        QTEButton.onClick.RemoveListener(() => MarkFusion(fusionColl));*/
    }

    private void Update()
    {
        spawnTime += Time.deltaTime;
        if (spawnTime >= duration - 1.0f )
        {
            decreasingTransparency = true;
            if(transparencyCoroutine == null)
            {
                transparencyCoroutine = StartCoroutine(DecreaseTransparency());
            }
        }
        if (spawnTime >= duration)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {

        CheckPlayerInTrigger(other, true);

        if (isPlayerInTrigger )
        {
            playerScr.InkGain = playerScr.OriginalInkGain * 1.6f;
        }

        InkTypeAction(other);

    }

    private void InkTypeAction(Collider other)
    {
        switch (currType)
        {
            case InkType.FIRE:
                if(other.TryGetComponent<Entity>(out Entity entity) && other.CompareTag("ENEMY"))
                {
                    entity.HP -= 0.5f;
                }
                break;
            case InkType.SWAMP:
                if(other.TryGetComponent<Entity>(out Entity player) && other.CompareTag("PLAYER"))
                {
                    player.HP += 0.5f;
                }
                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        CheckPlayerInTrigger(other, false);

        playerScr.InkGain = playerScr.OriginalInkGain;
    }

    private void CheckPlayerInTrigger(Collider coll, bool haveToCheck)
    {
        if (coll.TryGetComponent<Player>(out Player playerScr))
        {
            IsPlayerInTrigger = haveToCheck;
        }
    }

    public void SetSprites()
    {
        Debug.Log("스프라이트 설정");
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
            spriterenderer.sprite = inkMarkImgs[4];
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




    public void InkFusion(InkType fusionType)
    {
        if (currType == InkType.RED)
        {
            if (fusionType == InkType.GREEN)
                currType = InkType.FIRE;
            else if (fusionType == InkType.BLUE)
                currType = InkType.MIST;
        }
        else if (currType == InkType.GREEN)
        {
            if (fusionType == InkType.RED)
                currType = InkType.FIRE;
            else if (fusionType == InkType.BLUE)
                currType = InkType.SWAMP;
        }
        else if (currType == InkType.BLUE)
        {
            if (fusionType == InkType.RED)
                currType = InkType.MIST;
            else if (fusionType == InkType.GREEN)
                currType = InkType.SWAMP;
        }

        spawnTime = 0.0f;
        duration = fusionDuration;
        isFusioned = true;
        SetSprites();
        if(transparencyCoroutine != null)
        {
            StopCoroutine(transparencyCoroutine);
            transparencyCoroutine = null;
            spriterenderer.color = new Color(spriterenderer.color.r, spriterenderer.color.g, spriterenderer.color.b, 1.0f);
        }
    }


    public IEnumerator DecreaseTransparency()
    {
        float time = 0.0f;

        while (time <= 1.0f)
        {
            time += Time.deltaTime;
            spriterenderer.color = new Color(spriterenderer.color.r, spriterenderer.color.g, spriterenderer.color.b, 1 - time);

            yield return null;
        }

        yield break;
    }
}
