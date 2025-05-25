using UnityEngine;

public enum StickerType
{
    Dedicated,
    General
}


[CreateAssetMenu(fileName = "StickerData", menuName = "ScriptSystem/StickerData")]
public class StickerData : ScriptSystemData
{
    public int stickerID;
    public string stickerName;  
    public StickerType stickerType;
    public NewScriptData.ScriptType dedicatedScriptTarget;
    public InkType dedicatedInkType;
    public int rarity;
    public int maxRarity = 3;
    public string stickerDesc;
    public int[] price;
    public float[] levelData;

    public void CopyData(StickerData copyData)
    {
        stickerID = copyData.stickerID;
        stickerName = copyData.stickerName;
        stickerType = copyData.stickerType;
        dedicatedScriptTarget = copyData.dedicatedScriptTarget;
        dedicatedInkType = copyData.dedicatedInkType;
        rarity = copyData.rarity;
        maxRarity = copyData.maxRarity;
        stickerDesc = copyData.stickerDesc;

        price = new int[4] { copyData.price[0], copyData.price[1], copyData.price[2], copyData.price[3] };
        levelData = new float[4] {copyData.levelData[0], copyData.levelData[1], copyData.levelData[2], copyData.levelData[3] };
    }
}
