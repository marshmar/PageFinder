using UnityEngine;
using UnityEngine.UI;
using System;

public class CustomToggle : Toggle
{
    
    private Graphic[] m_graphics;
    protected Graphic[] Graphics
    {
        get
        {
            if (m_graphics == null) //캐싱이 되지 않았다면
                m_graphics = targetGraphic.transform.GetComponentsInChildren<Graphic>(); //자식 오브젝트로부터 Graphic 컴포넌트 지정
            return m_graphics;
        }
    }

    protected override void DoStateTransition(SelectionState state, bool instant)
    {
        Color color;
        switch (state)
        {
            case SelectionState.Normal:
                color = colors.normalColor;
                break;
            case SelectionState.Highlighted:
                color = colors.highlightedColor;
                break;
            case SelectionState.Pressed:
                color = colors.pressedColor;
                break;
            case SelectionState.Selected:
                color = colors.selectedColor;
                break;
            case SelectionState.Disabled:
                color = colors.disabledColor;
                break;
            default:
                color = Color.black;
                break;
        }

        if (gameObject.activeInHierarchy)
        {
            switch (transition)
            {
                case Transition.ColorTint: //Color Tint
                    ColorTween(color * colors.colorMultiplier, instant);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }
    }

    private void ColorTween(Color targetColor, bool instant)
    {
        if (targetGraphic == null) return;
        for (int i = 0; i < Graphics.Length; ++i) Graphics[i].CrossFadeColor(targetColor, (!instant) ? colors.fadeDuration : 0f, true, true);
    }
}