using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class DraggableUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Transform canvas;               // UI가 소속되어 있는 최상단의 CanvasTransform
    private Transform previousParent;       // 해당 오브젝트가 직전에 소속되어 있었던 부모 Transform
    private RectTransform rect;             // UI 위치 제어를 위한 RectTransform
    private CanvasGroup canvasGroup;        // UI의 알파값과 상호작용 제어를 위한 CanvasGroup

    public Sprite dragImg;
    private GameObject tempObj;

    public GameObject TempObj { get => tempObj; set => tempObj = value; }
    public Action beginDragEvent;
    public Action dropFailEvent;
    public Action dropSuccessEvent;
    public bool dropSuccessed = false;
    public bool canDrag = false;

    /*    public Transform PreviousParent { get => previousParent; set => previousParent = value; }
public CanvasGroup CanvasGroup { get => canvasGroup; set => canvasGroup = value; }*/

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>().transform;
        //dataComponent = GetComponent<Data>();
        rect = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!canDrag) return;

        beginDragEvent?.Invoke();

        tempObj = new GameObject("Dragging UI");
        Image img = tempObj.AddComponent<Image>();
        img.sprite = dragImg;
        tempObj.transform.SetParent(transform);

        canvasGroup = tempObj.AddComponent<CanvasGroup>();
        canvasGroup.blocksRaycasts = false;

        //tempObj = (GameObject)Instantiate(tempObj, transform);

        rect = tempObj.GetComponent<RectTransform>();

        // 현재 드래그중인 UI가 화면의 최상단에 출력되도록 하기 위해
        tempObj.transform.SetParent(canvas);    // 부모 오브젝트를 Canvas로 설정
        tempObj.transform.SetAsLastSibling();   // 가장 앞에 보이도록 마지막 자식으로 설정

    }

    public void OnDrag(PointerEventData eventData)
    {
        if(!canDrag) return;
        // 현재 스크린상의 마우스 위치를 UI 위치로 설정 (UI가 마우스를 쫓아다니는 상태)
        rect.position = eventData.position;
        rect.localScale = new Vector3(1.2f, 1.2f, 1.2f);
    }

    public void OnEndDrag(PointerEventData eventData)
    {

        Destroy(tempObj);
        tempObj = null;

        if (!dropSuccessed)
        {
            dropFailEvent?.Invoke();
        }
        else
        {
            dropSuccessEvent?.Invoke();
        }

    }
}
