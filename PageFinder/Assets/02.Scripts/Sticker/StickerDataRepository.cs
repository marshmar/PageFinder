using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class StickerDataRepository : MonoBehaviour
{
    private List<StickerData> stickerDatas;

    public List<StickerData> StickerDatas { get => stickerDatas; }

    public void SaveStickerDatas(List<StickerData> parsedStickerDatas)
    {
        stickerDatas = new List<StickerData>();

        foreach (var stickerData in parsedStickerDatas)
        {
            StickerData copyData = ScriptableObject.CreateInstance<StickerData>();
            copyData.CopyData(stickerData);

            stickerDatas.Add(copyData);
        }
    }

    public List<StickerData> GetRandomStickers(StickerInventory stickerInventory, int count)
    {
        var result = new List<StickerData>();

        while (result.Count < count)
        {
            int index = Random.Range(0, stickerDatas.Count);

            result.Add(stickerDatas[index]);
        }

        return result;
    }

    public List<StickerData> GetDistinctRandomStickers(StickerInventory stickerInventory, int count)
    {
        var result = new List<StickerData>();

        while (result.Count < count)
        {
            int index = Random.Range(0, stickerDatas.Count);

            if (result.Any(s => s.stickerID == stickerDatas[index].stickerID))
                continue;

            Sticker playerSticker = stickerInventory.FindPlayerStickerByID(stickerDatas[index].stickerID);
            if (playerSticker != null)
            {
                if (playerSticker.IsMaxRarity()) continue;
                StickerData upgradedStickerData = ScriptableObject.CreateInstance<StickerData>();
                upgradedStickerData.CopyData(playerSticker.GetCopiedData());
                upgradedStickerData.rarity = playerSticker.GetCurrRarity() + 1;
                result.Add(upgradedStickerData);
                continue;
            }

            result.Add(stickerDatas[index]);
        }

        /*Debug.Log("============Each script info============");
        foreach (var data in result)
        {
            Debug.Log($"scriptID: {data.scriptID}");
            Debug.Log($"scriptName: {data.scriptName}");
            Debug.Log($"scriptRarity: {data.rarity}");
            Debug.Log($"scriptMaxRarity: {data.maxRarity}");
            Debug.Log($"scriptType: {data.scriptType}");
            Debug.Log($"scriptInkType: {data.inkType}");
            Debug.Log("============================================");
        }*/

        return result;
    }
}
