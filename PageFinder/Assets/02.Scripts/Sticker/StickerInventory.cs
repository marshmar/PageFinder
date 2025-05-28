using UnityEngine;
using System.Collections.Generic;
using System;


public class StickerInventory : MonoBehaviour
{
    private List<Sticker> stickerList = new List<Sticker>();

    public void AddSticker(StickerData newStickerData)
    {
        Sticker playerSticker = FindPlayerStickerByID(newStickerData.stickerID);
        if (playerSticker != null)
        {
            playerSticker.UpgradeSticker(newStickerData.rarity);
        }
        else
        {
            Sticker newSticker = ScriptSystemManager.Instance.CreateStickerByID(newStickerData.stickerID);
            newSticker.CopyData(newStickerData);

            stickerList.Add(newSticker);
        }


    }

    public Sticker FindPlayerStickerByID(int stickerID)
    {
        return stickerList.Find(s => s.GetID() == stickerID);
    }

    public List<Sticker> GetPlayerStickerList()
    {
        return stickerList;
    }

    public List<Sticker> GetUnEquipedStickerList()
    {
        var result = new List<Sticker>();
        foreach(var sticker in stickerList)
        {
            if (sticker.IsAttached()) continue;
            result.Add(sticker);
        }

        return result;
    }

    public bool RemoveStikcer(Sticker stikcer)
    {
        stikcer.Detach();
        return stickerList.Remove(stikcer);
    }
}


