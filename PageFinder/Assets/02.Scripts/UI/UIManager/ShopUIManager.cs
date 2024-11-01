using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopUIManager : MonoBehaviour
{
    [SerializeField]
    private Canvas ShopUICanvas;


    public void SetShopUICanvasState(bool value)
    {
        ShopUICanvas.gameObject.SetActive(value);

        if (!value)
        {

        }

        // √ ±‚»≠
    }
}
