using UnityEngine;
using System.Collections.Generic;
using System;


public class StickerInventory : MonoBehaviour
{
    private List<Sticker> stickerList = new List<Sticker>();

    public void AddSticker(StickerData stickerData)
    {
        //stickerList.Add(sticker);
    }

    public Sticker FindPlayerStickerByID(int stickerID)
    {
        return stickerList.Find(s => s.GetID() == stickerID);
    }
}


