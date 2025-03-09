using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public enum InkMarkType
{
    BASICATTACK,
    DASH,
    INKSKILL,
    INTERACTIVEOBJECT
}

public class InkMarkSetter : Singleton<InkMarkSetter>
{
    public InkMarkData[] inkMarksDatas; // 0: BA, 1: Dash, 2: Skill, 3: InteractiveObject

    public void SetInkMarkScaleAndDuration(InkMarkType inkMarkType, Transform inkMarkTransform, ref float duration)
    {
        switch (inkMarkType)
        { 
            case InkMarkType.BASICATTACK:
                inkMarkTransform.localScale = inkMarksDatas[0].scale;
                duration = inkMarksDatas[0].duration;
                break;
            case InkMarkType.DASH:
                inkMarkTransform.localScale = inkMarksDatas[1].scale;
                duration = inkMarksDatas[1].duration;
                break;
            case InkMarkType.INKSKILL:
                inkMarkTransform.localScale = inkMarksDatas[2].scale;
                duration = inkMarksDatas[2].duration;
                break;
            case InkMarkType.INTERACTIVEOBJECT:
                inkMarkTransform.localScale = inkMarksDatas[3].scale;
                duration = inkMarksDatas[3].duration;
                break;
        }
        
    }

    public void AddCollider(InkMarkType inkMarkType, Transform inkMarkTransform)
    {
        switch (inkMarkType)
        {
            case InkMarkType.BASICATTACK:
            case InkMarkType.INKSKILL:
            case InkMarkType.INTERACTIVEOBJECT:
                SphereCollider sphereCol = inkMarkTransform.AddComponent<SphereCollider>();
                sphereCol.radius = 0.5f;
                sphereCol.isTrigger = true;
                break;
            case InkMarkType.DASH:
                BoxCollider boxCol = inkMarkTransform.AddComponent<BoxCollider>();
                boxCol.size = new Vector3(1f, 1f, 0f);
                boxCol.isTrigger = true;
                break;
        }
    }

    public bool SetInkMarkSprite(InkMarkType inkMarkType, InkType inkType, SpriteRenderer inkMarkSpriteRenderer)
    {
        bool result = false;

        switch (inkMarkType)
        {
            case InkMarkType.BASICATTACK:
                result = SetSprite(0, inkType, inkMarkSpriteRenderer);
                break;
            case InkMarkType.DASH:
                result = SetSprite(1, inkType, inkMarkSpriteRenderer);
                break;
            case InkMarkType.INKSKILL:
                result = SetSprite(2, inkType, inkMarkSpriteRenderer);
                break;
            case InkMarkType.INTERACTIVEOBJECT:
                result = SetSprite(3, inkType, inkMarkSpriteRenderer);
                break;
        }

        return result;
    }

    private bool SetSprite(int index, InkType inkType, SpriteRenderer spriteRenderer)
    {
        switch (inkType)
        {
            case InkType.RED:
                spriteRenderer.sprite = inkMarksDatas[index].inkMarkImages[0];
                break;
            case InkType.GREEN:
                spriteRenderer.sprite = inkMarksDatas[index].inkMarkImages[1];
                break;
            case InkType.BLUE:
                spriteRenderer.sprite = inkMarksDatas[index].inkMarkImages[2];
                break;
            case InkType.FIRE:
                spriteRenderer.sprite = inkMarksDatas[index].inkMarkImages[3];
                break;
            case InkType.MIST:
                spriteRenderer.sprite = inkMarksDatas[index].inkMarkImages[4];
                break;
            case InkType.SWAMP:
                spriteRenderer.sprite = inkMarksDatas[index].inkMarkImages[5];
                break;
        }

        if(spriteRenderer.sprite == null)
        {
            Debug.LogError("�Ҵ��Ϸ��� ��ũ��ũ SpriteImage�� �������� �ʽ��ϴ�.");
            return false;
        }

        return true;
    }
}
