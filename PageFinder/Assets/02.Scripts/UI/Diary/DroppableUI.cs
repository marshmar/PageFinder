using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class DropResult
{
    public bool Success = false;
}
public class DroppableUI : MonoBehaviour, IPointerEnterHandler, IDropHandler, IPointerExitHandler
{
    public bool canDroppbable = true;
    private RectTransform rect;
    public Action<Sticker, DropResult> dropEvent;
    //private PanelPSData panelPSDataScr;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        //panelPSDataScr = GetComponentInParent<PanelPSData>();
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        //image.color = Color.yellow;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //image.color = Color.white;
    }

    public void OnDrop(PointerEventData eventData)
    {
        // pointerDrag는 현재 드래그하고 있는 대상(=아이템)
        if (eventData.pointerDrag != null && canDroppbable)
        {
            if (eventData.pointerDrag.transform.TryGetComponent<DraggableUI>(out DraggableUI dui))
            {
                var result = new DropResult();

                if (eventData.pointerDrag.transform.TryGetComponent<DiaryElement>(out DiaryElement de))
                {
                    this.dropEvent?.Invoke(de.Sticker, result);

                    if (result.Success)
                    {
                        Debug.Log("Drop 성공");
                        dui.dropSuccessed = true;

                    }
                }
                return;
            }

        }

        Debug.Log("Drop Fail");
    }
}
