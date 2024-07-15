using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductData : Singleton<ProductData>
{
    public struct Product
    {
        public string name;
        public string price;
        public string shortDescription;
        public string longDescription;
    }

    public enum Type
    {
        ITEM,
        EQUIPMENT
    }

    List<Product> items = new List<Product>();
    List<Product> equipments = new List<Product>();

    private void Start()
    {
        SetProducts();
    }

    void AddProduct(Type type, string name, string price, string shortDescription, string longDescription)
    {
        Product product = new Product();

        product.name = name;
        product.name = price;
        product.name = shortDescription;
        product.name = longDescription;

        if (type == Type.ITEM)
            items.Add(product);
        else if (type == Type.EQUIPMENT)
            equipments.Add(product);
        else
            Debug.LogWarning(type);
    }

    public Product GetProduct(Type type, int index)
    {
        if (type == Type.ITEM)
            return items[index];
        else if (type == Type.EQUIPMENT)
            return items[index];

        Debug.LogWarning(type);
        return items[index];
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
        
        AddProduct(type, "item1", "100G", "-------------------", "-------------------------------------------");
        AddProduct(type, "item2", "200G", "-------------------", "-------------------------------------------");
        AddProduct(type, "item3", "300G", "-------------------", "-------------------------------------------");
        AddProduct(type, "item4", "400G", "-------------------", "-------------------------------------------");
        AddProduct(type, "item5", "500G", "-------------------", "-------------------------------------------");
        AddProduct(type, "item6", "600G", "-------------------", "-------------------------------------------");
        AddProduct(type, "item7", "700G", "-------------------", "-------------------------------------------");
        AddProduct(type, "item8", "800G", "-------------------", "-------------------------------------------");
    }

    void SetEquipment()
    {
        Type type = Type.EQUIPMENT;

        AddProduct(type, "Brush1", "100G", "-------------------", "-------------------------------------------");
        AddProduct(type, "Brush2", "200G", "-------------------", "-------------------------------------------");
        AddProduct(type, "Brush3", "300G", "-------------------", "-------------------------------------------");
        AddProduct(type, "Brush4", "400G", "-------------------", "-------------------------------------------");
        AddProduct(type, "Brush5", "500G", "-------------------", "-------------------------------------------");
        AddProduct(type, "Brush6", "600G", "-------------------", "-------------------------------------------");
        AddProduct(type, "Brush7", "700G", "-------------------", "-------------------------------------------");
        AddProduct(type, "Brush8", "800G", "-------------------", "-------------------------------------------");
    }

    void SetProducts()
    {
        SetItems();
        SetEquipment();
    }
}
