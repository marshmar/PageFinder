using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class DraggableUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Transform canvas;               // UI�� �ҼӵǾ� �ִ� �ֻ���� CanvasTransform
    private Transform previousParent;       // �ش� ������Ʈ�� ������ �ҼӵǾ� �־��� �θ� Transform
    private RectTransform rect;             // UI ��ġ ��� ���� RectTransform
    private CanvasGroup canvasGroup;        // UI�� ���İ��� ��ȣ�ۿ� ��� ���� CanvasGroup

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

        // ���� �巡������ UI�� ȭ���� �ֻ�ܿ� ��µǵ��� �ϱ� ����
        tempObj.transform.SetParent(canvas);    // �θ� ������Ʈ�� Canvas�� ����
        tempObj.transform.SetAsLastSibling();   // ���� �տ� ���̵��� ������ �ڽ����� ����

    }

    public void OnDrag(PointerEventData eventData)
    {
        if(!canDrag) return;
        // ���� ��ũ������ ���콺 ��ġ�� UI ��ġ�� ���� (UI�� ���콺�� �Ѿƴٴϴ� ����)
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
