using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ConstellationScroll : MonoBehaviour
{
    public Image[] ConstellationImg = new Image[5];

    private void Start()
    {
        SetContellationImgSize(null);
    }

    public void MakeContellationImgBigger()
    {
        GameObject clickedObject = EventSystem.current.currentSelectedGameObject;

        SetContellationImgSize(clickedObject);
    }

    void SetContellationImgSize(GameObject clickedObject) 
    {
        foreach (Image img in ConstellationImg)
        {
            if (img.gameObject.Equals(clickedObject))
                img.rectTransform.sizeDelta = new Vector2(250, 250);
            else
                img.rectTransform.sizeDelta = new Vector2(200, 200);
        }
    }
}
