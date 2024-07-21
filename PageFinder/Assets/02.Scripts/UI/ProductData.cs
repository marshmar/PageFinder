using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class ProductData : Singleton<ProductData>
{
    public struct Product
    {
        public string name;
        public int price;
        public string shortDescription;
        public string longDescription;
        public Sprite sprite;
    }

    public enum Type
    {
        ITEM,
        EQUIPMENT
    }

    List<Product> items = new List<Product>();
    List<Product> equipments = new List<Product>();

    int[,] itemsIndexForStage = new int[5, 4]
        {{0,1,2,3},
        {0,1,2,3},
        {0,1,2,3},
        {0,1,2,3},
        {0,1,2,3}};

    int[,] equipmentsIndexForStage = new int[5, 4]
        {{0,1,2,3},
        {0,1,2,3},
        {0,1,2,3},
        {0,1,2,3},
        {0,1,2,3}};


    void AddProduct(Type type, string name, int price, string shortDescription, string longDescription)
    {
        Product product = new Product();

        product.name = name;
        product.price = price;
        product.shortDescription = shortDescription;
        product.longDescription = longDescription;
        product.sprite = Resources.Load<Sprite>("Shop/" + product.name);

        if (type == Type.ITEM)
            items.Add(product);
        else if (type == Type.EQUIPMENT)
            equipments.Add(product);
        else
            Debug.LogWarning(type);
    }

    public Product GetProduct(Type type, int stageNum, int productIndex)
    {
        int itemsIndex = -1;

        if (type == Type.ITEM)
        {
            itemsIndex = itemsIndexForStage[stageNum, productIndex];
            return items[itemsIndex];
        }  
        else if (type == Type.EQUIPMENT)
        {
            itemsIndex = equipmentsIndexForStage[stageNum, productIndex];
            return equipments[itemsIndex];
        }
            
        Debug.LogWarning(type);
        return items[productIndex];
    }

    /// <summary>
    /// 스테이지별 상품을 설정한다. 
    /// </summary>
    void SetProductForStage(Type type, int stageNum, int productNum, int value)
    {
        if (type == Type.ITEM)
            itemsIndexForStage[stageNum, productNum] = value;
        else if (type == Type.EQUIPMENT)
            equipmentsIndexForStage[stageNum, productNum] = value;
        else
            Debug.LogWarning(type);
    }

    int GetProductIndexForStage(Type type, int stageNum, int productNum)
    {
        if (type == Type.ITEM)
            return itemsIndexForStage[stageNum, productNum];
        else if (type == Type.EQUIPMENT)
            return equipmentsIndexForStage[stageNum, productNum];
        
        Debug.LogWarning(type);
        return -1;
    }

    public int FindProductIndex(Type type, string name)
    {
        if (type == Type.ITEM)
            return items.FindIndex(x => x.name.Equals(name));
        else if (type == Type.EQUIPMENT)
            return equipments.FindIndex(x => x.name.Equals(name));

        Debug.LogWarning(type);
        return -1;
    }

    void SetItems()
    {
        Type type = Type.ITEM;
        
        AddProduct(type, "Item1", 100, "----------------", "--------------------------------------------------------------------------------------");
        AddProduct(type, "Item2", 200, "----------------", "--------------------------------------------------------------------------------------");
        AddProduct(type, "Item3", 300, "----------------", "--------------------------------------------------------------------------------------");
        AddProduct(type, "Item4", 400, "----------------", "--------------------------------------------------------------------------------------");
        AddProduct(type, "Item5", 500, "----------------", "--------------------------------------------------------------------------------------");
        AddProduct(type, "Item6", 600, "----------------", "--------------------------------------------------------------------------------------");
        AddProduct(type, "Item7", 700, "----------------", "--------------------------------------------------------------------------------------");
        AddProduct(type, "Item8", 800, "----------------", "--------------------------------------------------------------------------------------");
        AddProduct(type, "Item9", 900, "----------------", "--------------------------------------------------------------------------------------");
        AddProduct(type, "Item10", 1000, "----------------", "--------------------------------------------------------------------------------------");
        AddProduct(type, "Item11", 1100, "----------------", "--------------------------------------------------------------------------------------");
        AddProduct(type, "Item12", 1200, "----------------", "--------------------------------------------------------------------------------------");
    }

    void SetEquipment()
    {
        Type type = Type.EQUIPMENT;

        AddProduct(type, "Brush1", 100, "----------------", "--------------------------------------------------------------------------------------");
        AddProduct(type, "Brush2", 200, "----------------", "--------------------------------------------------------------------------------------");
        AddProduct(type, "Brush3", 300, "----------------", "--------------------------------------------------------------------------------------");
        AddProduct(type, "Brush4", 400, "----------------", "--------------------------------------------------------------------------------------");
        AddProduct(type, "Brush5", 500, "----------------", "--------------------------------------------------------------------------------------");
        AddProduct(type, "Brush6", 600, "----------------", "--------------------------------------------------------------------------------------");
        AddProduct(type, "Brush7", 700, "----------------", "--------------------------------------------------------------------------------------");
        AddProduct(type, "Brush8", 800, "----------------", "--------------------------------------------------------------------------------------");
        AddProduct(type, "Brush9", 900, "----------------", "--------------------------------------------------------------------------------------");
        AddProduct(type, "Brush10", 1000, "----------------", "--------------------------------------------------------------------------------------");
        AddProduct(type, "Brush11", 1100, "----------------", "--------------------------------------------------------------------------------------");
        AddProduct(type, "Brush12", 1200, "----------------", "--------------------------------------------------------------------------------------");
    }

    public void InitProducts()
    {
        SetItems();
        SetEquipment();
    }

    public void SetNewProducts(int currentStage)
    {
        // 새 상품들
        int[] newItems = new int[itemsIndexForStage.GetLength(1)];
        int[] newEquipments = new int[equipmentsIndexForStage.GetLength(1)];

        // 새 상품으로 변경
        ChangeToNewProducts(ProductData.Type.ITEM, currentStage);
        ChangeToNewProducts(ProductData.Type.EQUIPMENT, currentStage);
    }

    void ChangeToNewProducts(ProductData.Type type, int currentStage)
    {
        int randProductIndex = -1;

        for (int totalProductIndex = 0; totalProductIndex < itemsIndexForStage.GetLength(1); totalProductIndex++)
        {

            randProductIndex = Random.Range(0, items.Count);
            // 랜덤한 상품과 기존 상품이 같은지 확인           
            for (int productIndex = 0; productIndex < itemsIndexForStage.GetLength(1); productIndex++)
            {
                // 랜덤한 상품과 기존 상품이 같다면 다시 설정 
                if (randProductIndex == GetProductIndexForStage(type, currentStage, productIndex))
                {
                    randProductIndex = Random.Range(0, items.Count);
                    productIndex = -1;
                    continue;
                }
            }

            // 새로운 상품일 경우
            SetProductForStage(type, currentStage, totalProductIndex, randProductIndex);
        }
    }
}
