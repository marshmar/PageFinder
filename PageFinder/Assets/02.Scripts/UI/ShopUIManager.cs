using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopUIManager : MonoBehaviour
{
    public enum Category
    {
        ITEM,
        EQUIPMENT
    }

    public Canvas ShopCanvas;
    public GameObject[] Product = new GameObject[4];
    public GameObject ClickedProduct;

    Category category = Category.ITEM;

    ProductData productData;

    private void Start()
    {
        productData = ProductData.Instance;
        Init();
    }

    public void ClickItem()
    {
        category = Category.ITEM;
    }
    public void ClickEquipment()
    {
        category = Category.EQUIPMENT;
    }

    public void ClickProduct()
    {
        string name = EventSystem.current.currentSelectedGameObject.transform.GetChild(2).GetComponent<TMP_Text>().text;
        int productIndex = -1;

        if (category == Category.ITEM)
            productIndex = productData.FindProductIndex(ProductData.Type.ITEM, name);
        else if (category == Category.EQUIPMENT)
            productIndex = productData.FindProductIndex(ProductData.Type.EQUIPMENT, name);
        else
            Debug.LogWarning(category);

        SetClickedProduct(productIndex);
    }

    public void ResetShop()
    {
        
    }

    public void Exit()
    {
        ShopCanvas.gameObject.SetActive(false);
    }

    public void Activate()
    {
        ShopCanvas.gameObject.SetActive(true);
    }

    public void Buy()
    {

    }

    void Init()
    {
        SetProducts();
    }

    void SetProducts()
    {
        for (int i = 0; i < Product.Length; i++)
        {
            Product[i].transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>("/Shop/" + productData.GetProduct(ProductData.Type.ITEM, i).name);
            Product[i].transform.GetChild(2).GetComponent<TMP_Text>().text = productData.GetProduct(ProductData.Type.ITEM, i).name;
            Product[i].transform.GetChild(3).GetComponent<TMP_Text>().text = productData.GetProduct(ProductData.Type.ITEM, i).shortDescription;
            Product[i].transform.GetChild(4).GetComponent<TMP_Text>().text = productData.GetProduct(ProductData.Type.ITEM, i).price;
        }
    }

    void SetClickedProduct(int productIndex)
    {
        ClickedProduct.transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>("/Shop/" + productData.GetProduct(ProductData.Type.ITEM, productIndex).name);
        ClickedProduct.transform.GetChild(2).GetComponent<TMP_Text>().text = productData.GetProduct(ProductData.Type.ITEM, productIndex).name;
        ClickedProduct.transform.GetChild(3).GetComponent<TMP_Text>().text = productData.GetProduct(ProductData.Type.ITEM, productIndex).longDescription;
        ClickedProduct.transform.GetChild(4).GetComponent<TMP_Text>().text = productData.GetProduct(ProductData.Type.ITEM, productIndex).price;
    }
}
