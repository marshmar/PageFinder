using UnityEngine;

public class StellarStickerFactory : IStickerFactory
{
    public Sticker CreateStickerByID(int scriptID)
    {
        Sticker newSticker = null;

        switch (scriptID)
        {
            case 101:
                newSticker = new PerceivedTemperature();
                break;
            case 102:
                newSticker = new ToughVine();
                break;
            case 103:
                newSticker = new DeepWellNew();
                break;
            case 104:
                newSticker = new StrongStrike();
                break;
            case 105:
                newSticker = new SwiftDash();
                break;
            case 106:
                newSticker = new Capri();
                break;
            case 107:
                newSticker = new ForceOfThePlants();
                break;
            case 108:
                newSticker = new WaterConservationNew();
                break;
            case 109:
                newSticker = new Fernand();
                break;
            default:
                Debug.LogError("No matching ID found.");
                break;
        }

        return newSticker;
    }

}
