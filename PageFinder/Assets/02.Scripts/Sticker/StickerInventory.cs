using UnityEngine;
using System.Collections.Generic;
using System;


public class StickerInventory : MonoBehaviour
{
    #region Variables
    private List<Sticker> _stickerList = new List<Sticker>();
    #endregion

    #region Properties
    #endregion

    #region Unity Lifecycle
    #endregion

    #region Initialization
    #endregion

    #region Actions
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

            _stickerList.Add(newSticker);
        }


    }

    public Sticker FindPlayerStickerByID(int stickerID)
    {
        return _stickerList.Find(s => s.GetID() == stickerID);
    }

    public List<Sticker> GetPlayerStickerList()
    {
        return _stickerList;
    }

    public List<Sticker> GetUnEquipedStickerList()
    {
        var result = new List<Sticker>();
        foreach (var sticker in _stickerList)
        {
            if (sticker.IsAttached()) continue;
            result.Add(sticker);
        }

        return result;
    }

    public bool RemoveStikcer(Sticker stikcer)
    {
        stikcer.Detach();
        return _stickerList.Remove(stikcer);
    }
    #endregion

    #region Getter
    #endregion

    #region Setter
    #endregion

    #region Utilities
    #endregion

    #region Events
    #endregion
}


