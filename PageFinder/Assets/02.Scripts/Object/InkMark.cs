using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum InkType
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
    private InkType currType;
    private InkMarkType currInkMarkType;

    private SpriteRenderer spriterenderer;

    private Player playerScr;

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
    public InkMarkType CurrInkMarkType { get => currInkMarkType; set => currInkMarkType = value; }
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

    public void SetInkMarkData(InkMarkType inkMarkType, InkType inkType)
    {
        currInkMarkType = inkMarkType;
        currType = inkType;

        SetSprites();
    }

    private void OnDestroy()
    {
        playerInkMagicControllerScr.InkMarks.Remove(this);
    }

    private void OnDisable()
    {
        ResetInkMark();
    }

    private void ResetInkMark()
    {
        spriterenderer.color = new Color(spriterenderer.color.r, spriterenderer.color.g, spriterenderer.color.b, 1f);
        spawnTime = 0f;
        duration = 0f;
        isFusioned = false;
        isPlayerInTrigger = false;
        decreasingTransparency = false;
    }

    private void Update()
    {
        spawnTime += Time.deltaTime;
        if (spawnTime >= duration - 1.0f && !decreasingTransparency)
        {
            decreasingTransparency = true;
            StartCoroutine(DecreaseTransparency());

        }
        if (spawnTime >= duration)
        {
            InkMarkPooler.Instance.Pool.Release(this);
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
        if(spriterenderer == null)
        {
            Debug.Log(gameObject.name + "'s spriterenderer is null");
        }

        InkMarkSetter.Instance.SetInkMarkScaleAndDuration(currInkMarkType, transform, ref duration);

        if(!InkMarkSetter.Instance.SetInkMarkSprite(currInkMarkType, currType, spriterenderer))
        {
            Debug.LogError("잉크마크 스프라이트 할당 실패");
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
