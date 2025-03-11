using UnityEngine;

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

    public bool SetInkMarkSprite(InkMarkType inkMarkType, InkType inkType, SpriteRenderer inkMarkSpriteRenderer, SpriteMask spriteMask)
    {
        bool result = false;

        switch (inkMarkType)
        {
            case InkMarkType.BASICATTACK:
                result = SetSprite(0, inkType, inkMarkSpriteRenderer, spriteMask);
                break;
            case InkMarkType.DASH:
                result = SetSprite(1, inkType, inkMarkSpriteRenderer, spriteMask);
                break;
            case InkMarkType.INKSKILL:
                result = SetSprite(2, inkType, inkMarkSpriteRenderer, spriteMask);
                break;
            case InkMarkType.INTERACTIVEOBJECT:
                result = SetSprite(3, inkType, inkMarkSpriteRenderer, spriteMask);
                break;
        }

        return result;
    }

    private bool SetSprite(int index, InkType inkType, SpriteRenderer spriteRenderer, SpriteMask spriteMask)
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

        spriteMask.sprite = inkMarksDatas[index].inkMarkImages[6];

        if (spriteRenderer.sprite == null)
        {
            Debug.LogError("�Ҵ��Ϸ��� ��ũ��ũ SpriteImage�� �������� �ʽ��ϴ�.");
            return false;
        }

        return true;
    }
}