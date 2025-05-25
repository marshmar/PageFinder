using UnityEngine;
using System.Collections.Generic;

public class StickerFactory : MonoBehaviour
{
    private Dictionary<CharacterType, IStickerFactory> stickerFactories = new Dictionary<CharacterType, IStickerFactory>();

    private void Start()
    {
        stickerFactories.Add(CharacterType.Stellar, new StellarStickerFactory());
    }

    public Sticker CreateStickerByID(CharacterType charType, int scriptID)
    {
        if (!stickerFactories.TryGetValue(charType, out IStickerFactory stickerFactory))
        {
            Debug.LogError($"Not surpported character Type : {charType}");
            return null;
        }
        Sticker newSticker = stickerFactory.CreateStickerByID(scriptID);
        return newSticker;
    }
}
