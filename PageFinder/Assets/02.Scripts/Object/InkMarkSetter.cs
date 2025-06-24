using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum InkMarkType
{
    BASICATTACK,
    DASH,
    INKSKILL,
    INKSKILLEVOLVED,
    INTERACTIVEOBJECT,
    SYNTHESIZED
}

public class InkMarkSetter : Singleton<InkMarkSetter>
{
    public float inkFusionIntersectionAreaThreshold = 0.25f;
    public InkTypeSO inkTypeData; // 0: BA, 1: Dash, 2: Skill, 3: InteractiveObject
    public InkMaskSO inkMask;
    public static int inkId = 1;
    [SerializeField] private GameObject[] effects;

    public InkType SetMergedInkType(InkType inkTypeA, InkType inkTypeB)
    {
        if (inkTypeA == InkType.RED && inkTypeB == InkType.GREEN) return InkType.FIRE;
        if (inkTypeA == InkType.GREEN && inkTypeB == InkType.RED) return InkType.FIRE;
        if (inkTypeA == InkType.BLUE && inkTypeB == InkType.GREEN) return InkType.SWAMP;
        if (inkTypeA == InkType.GREEN && inkTypeB == InkType.BLUE) return InkType.SWAMP;
        if (inkTypeA == InkType.BLUE && inkTypeB == InkType.RED) return InkType.MIST;
        if (inkTypeA == InkType.RED && inkTypeB == InkType.BLUE) return InkType.MIST;
        return InkType.FIRE;
    }

    public void SetInkMarkScaleAndDuration(InkMarkType inkMarkType, Transform inkMarkTransform, ref float duration)
    {
        switch (inkMarkType)
        { 
            case InkMarkType.BASICATTACK:
                inkMarkTransform.localScale = new(3, 3, 1);
                duration = 6;
                break;
            case InkMarkType.DASH:
                inkMarkTransform.localScale = Vector3.zero;
                duration = 6;
                break;
            case InkMarkType.INKSKILL:
                inkMarkTransform.localScale = new(3, 3, 1);
                duration = 6;
                break;
            case InkMarkType.INKSKILLEVOLVED:
                inkMarkTransform.localScale = new(6, 6, 1);
                duration = 6;
                break;
            case InkMarkType.INTERACTIVEOBJECT:
                inkMarkTransform.localScale = new(3, 3, 1);
                duration = 6;
                break;
            case InkMarkType.SYNTHESIZED:
                duration = 8;
                break;
        }
    }
    
    public void AddCollider(InkMarkType inkMarkType, Transform inkMarkTransform)
    {
        switch (inkMarkType)
        {
            case InkMarkType.BASICATTACK:
                SphereCollider sphereCollider = inkMarkTransform.AddComponent<SphereCollider>();
                sphereCollider.radius = 0.8f;
                sphereCollider.isTrigger = true;
                break;
            case InkMarkType.INKSKILL:
            case InkMarkType.INKSKILLEVOLVED:
            case InkMarkType.INTERACTIVEOBJECT:
                sphereCollider = inkMarkTransform.AddComponent<SphereCollider>();
                sphereCollider.radius = 0.5f;
                sphereCollider.isTrigger = true;
                break;
            case InkMarkType.DASH:
                BoxCollider boxCollider = inkMarkTransform.AddComponent<BoxCollider>();
                boxCollider.size = new Vector3(1f, 2f, 0f);
                boxCollider.isTrigger = true;
                boxCollider.center = new Vector3(0f, 0.05f, 0f);
                break;
        }
    }

    public bool SetInkMarkSprite(InkMarkType inkMarkType, InkType inkType, SpriteRenderer inkMarkSpriteRenderer, SpriteMask spriteMask)
    {
        bool result = false;
        if (inkId > 50) inkId = 1;

        switch (inkMarkType)
        {
            case InkMarkType.BASICATTACK:
                result = SetSprite(inkType, inkMarkSpriteRenderer, spriteMask, 2, inkId++);
                break;
            case InkMarkType.DASH:
                result = SetSprite(inkType, inkMarkSpriteRenderer, spriteMask, 1, inkId++);
                break;
            case InkMarkType.INKSKILL:
            case InkMarkType.INKSKILLEVOLVED:
                result = SetSprite(inkType, inkMarkSpriteRenderer, spriteMask, 0, inkId++);
                break;
            case InkMarkType.INTERACTIVEOBJECT:
                result = SetSprite(inkType, inkMarkSpriteRenderer, spriteMask, 0, inkId++);
                break;
        }

        return result;
    }

    private bool SetSprite(InkType inkType, SpriteRenderer spriteRenderer, SpriteMask spriteMask, int maskIndex, int inkId)
    {
        switch (inkType)
        {
            case InkType.RED:
                spriteRenderer.sprite = inkTypeData.images[0];
                break;
            case InkType.GREEN:
                spriteRenderer.sprite = inkTypeData.images[1];
                break;
            case InkType.BLUE:
                spriteRenderer.sprite = inkTypeData.images[2];
                break;
            case InkType.FIRE:
                spriteRenderer.sprite = inkTypeData.images[3];
                break;
            case InkType.MIST:
                spriteRenderer.sprite = inkTypeData.images[4];
                break;
            case InkType.SWAMP:
                spriteRenderer.sprite = inkTypeData.images[5];
                break;
        }

        spriteMask.sprite = inkMask.images[maskIndex];

        // Set Unique Sorting Order (inkId based)
        int baseOrder = inkId * 10;
        spriteRenderer.sortingOrder = baseOrder + 1;

        spriteMask.frontSortingOrder = baseOrder + 1;
        spriteMask.backSortingOrder = baseOrder;
        spriteMask.isCustomRangeActive = true;

        if (spriteRenderer.sprite == null)
        {
            Debug.LogError("Failed to assign the intended ink mark SpriteImage.");
            return false;
        }

        return true;
    }

    public Sprite SetSprite(InkType inkType)
    {
        if (inkType == InkType.FIRE)  return inkTypeData.images[3];
        if (inkType == InkType.MIST)  return inkTypeData.images[4];
        if (inkType == InkType.SWAMP) return inkTypeData.images[5];
        return null;
    }

    public void SetEffect(int index, Transform transform)
    {
        GameObject instantiatedEffect = Instantiate(effects[index], transform);
        Destroy(instantiatedEffect, 7.5f);
    }

    
}