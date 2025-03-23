using UnityEngine;

public class InkMarkSynthesis : Singleton<InkMarkSynthesis>
{
    public GameObject prefab;

    /// <summary>
    /// Synthesize two InkMark Objects
    /// </summary>
    /// <param name="a">First InkMark</param>
    /// <param name="b">Second InkMark</param>
    /// <param name="pixelPerUnit">Pixel Per Unit</param>
    public void Synthesize(GameObject a, GameObject b, float pixelPerUnit)
    {
        var newInkType = InkMarkSetter.Instance.SetMergedInkType(a.GetComponent<InkMark>().CurrType, b.GetComponent<InkMark>().CurrType);
        var newSprite = InkMarkSetter.Instance.SetSprite(newInkType);
        if (a == null || b == null || newSprite == null) return;
        SpriteMask srA = a.GetComponentInChildren<SpriteMask>();
        SpriteMask srB = b.GetComponentInChildren<SpriteMask>();
        if (srA == null || srB == null) return;

        // Calculated based on original size
        int widthA = Mathf.RoundToInt(srA.sprite.rect.width);
        int heightA = Mathf.RoundToInt(srA.sprite.rect.height);
        int widthB = Mathf.RoundToInt(srB.sprite.rect.width);
        int heightB = Mathf.RoundToInt(srB.sprite.rect.height);
        int overlapX = Mathf.RoundToInt(Mathf.Min(widthA, widthB) * 0.4f);
        int newWidth = widthA + widthB - overlapX;
        int newHeight = Mathf.Max(heightA, heightB);

        Texture2D newTexture = new(newWidth, newHeight, TextureFormat.RGBA32, false);
        Color[] combinedPixels = new Color[newWidth * newHeight];

        CopyMaskedPixels(srA.sprite, combinedPixels, newWidth, 0, (newHeight - heightA) / 2);
        CopyMaskedPixels(srB.sprite, combinedPixels, newWidth, widthA - overlapX, (newHeight - heightB) / 2);
        ApplySeamlessTexture(combinedPixels, newSprite, newWidth, newHeight);

        newTexture.SetPixels(combinedPixels);
        newTexture.Apply();

        GameObject newObject = Instantiate(prefab);
        newObject.transform.position = (srA.transform.position + srB.transform.position) / 2;
        newObject.GetComponent<InkMark>().CurrType = newInkType;

        SpriteRenderer srNew = newObject.GetComponentInChildren<SpriteRenderer>();
        SpriteMask spriteMask = newObject.GetComponentInChildren<SpriteMask>();

        srNew.sprite = newSprite;
        if (spriteMask != null) spriteMask.sprite = Sprite.Create(newTexture, new Rect(0, 0, newWidth, newHeight), new Vector2(0.5f, 0.5f), pixelPerUnit);
        srNew.flipY = true;
    }

    void CopyMaskedPixels(Sprite sprite, Color[] targetPixels, int targetWidth, int offsetX, int offsetY)
    {
        Rect rect = sprite.textureRect;
        int width = (int)rect.width;
        int height = (int)rect.height;

        Color[] spritePixels = sprite.texture.GetPixels((int)rect.x, (int)rect.y, width, height);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int index = (y + offsetY) * targetWidth + (x + offsetX);
                int spriteIndex = y * width + x;
                if (spritePixels[spriteIndex].a > 0) targetPixels[index] = spritePixels[spriteIndex];
            }
        }
    }

    void ApplySeamlessTexture(Color[] pixels, Sprite sprite, int width, int height)
    {
        Texture2D texture = sprite.texture;
        Rect spriteRect = sprite.rect;

        // Extract only the actual pixel data of the sprite
        int textureWidth = (int)spriteRect.width;
        int textureHeight = (int)spriteRect.height;
        Color[] texturePixels = texture.GetPixels(
            (int)spriteRect.x,
            (int)spriteRect.y,
            textureWidth,
            textureHeight
        );

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int index = y * width + x;

                float u = (float)x / width;
                float v = (float)y / height;

                int tx = Mathf.FloorToInt(u * textureWidth) % textureWidth;
                int ty = Mathf.FloorToInt(v * textureHeight) % textureHeight;
                int textureIndex = ty * textureWidth + tx;

                // Cover opaque areas with seamlessTexture color
                if (pixels[index].a > 0) pixels[index] = texturePixels[textureIndex];
            }
        }
    }
}