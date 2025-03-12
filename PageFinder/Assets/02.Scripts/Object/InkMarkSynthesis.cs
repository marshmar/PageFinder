using UnityEngine;

public class InkMarkSynthesis : MonoBehaviour
{
    public GameObject A;
    public GameObject B;
    public Texture2D seamlessTexture;
    public GameObject prefab;
    private bool isProcessed = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C) && !isProcessed)
        {
            isProcessed = true;
            CombineSprites(A, B, seamlessTexture);
        }
    }

    void CombineSprites(GameObject a, GameObject b, Texture2D texture)
    {
        if (a == null || b == null || texture == null) return;

        SpriteRenderer srA = a.GetComponent<SpriteRenderer>();
        SpriteRenderer srB = b.GetComponent<SpriteRenderer>();

        if (srA == null || srB == null) return;

        Sprite spriteA = srA.sprite;
        Sprite spriteB = srB.sprite;

        float pixelsPerUnit = spriteA.pixelsPerUnit;

        // Tiled Size를 고려한 텍스처 크기 계산
        Vector2 tiledSizeA = srA.size;
        Vector2 tiledSizeB = srB.size;

        int widthA = Mathf.RoundToInt(tiledSizeA.x * pixelsPerUnit);
        int heightA = Mathf.RoundToInt(tiledSizeA.y * pixelsPerUnit);
        int widthB = Mathf.RoundToInt(tiledSizeB.x * pixelsPerUnit);
        int heightB = Mathf.RoundToInt(tiledSizeB.y * pixelsPerUnit);

        int overlapX = Mathf.Min(widthA, widthB) / 2;
        int newWidth = widthA + widthB - overlapX;
        int newHeight = Mathf.Max(heightA, heightB);

        Texture2D newTexture = new Texture2D(newWidth, newHeight, TextureFormat.RGBA32, false);
        Color[] combinedPixels = new Color[newWidth * newHeight];

        // 스프라이트 A의 픽셀 복사 (Tiled Size 고려)
        CopyMaskedPixels(spriteA, srA.size, combinedPixels, newWidth, 0, (newHeight - heightA) / 2, pixelsPerUnit);

        // 스프라이트 B의 픽셀 복사 (겹치는 부분 적용)
        CopyMaskedPixels(spriteB, srB.size, combinedPixels, newWidth, widthA - overlapX, (newHeight - heightB) / 2, pixelsPerUnit);

        ApplySeamlessTexture(combinedPixels, texture, newWidth, newHeight);

        newTexture.SetPixels(combinedPixels);
        newTexture.Apply();

        GameObject newObject = Instantiate(prefab);
        newObject.transform.position = (srA.transform.position + srB.transform.position) / 2;

        SpriteRenderer srNew = newObject.GetComponent<SpriteRenderer>();
        srNew.sprite = Sprite.Create(newTexture, new Rect(0, 0, newWidth, newHeight), new Vector2(0.5f, 0.5f), pixelsPerUnit);
        SpriteMask spriteMask = newObject.GetComponentInChildren<SpriteMask>();
        if (spriteMask != null) spriteMask.sprite = srNew.sprite;

        srNew.drawMode = SpriteDrawMode.Tiled;
        srNew.size = new Vector2(newWidth / pixelsPerUnit, newHeight / pixelsPerUnit);
        srNew.flipY = true;

        newObject.transform.localScale = srA.transform.localScale;

        Destroy(a);
        Destroy(b);
    }

    void CopyMaskedPixels(Sprite sprite, Vector2 tiledSize, Color[] targetPixels, int targetWidth, int offsetX, int offsetY, float pixelsPerUnit)
    {
        Texture2D texture = sprite.texture;
        Rect rect = sprite.textureRect;

        int width = Mathf.RoundToInt(tiledSize.x * pixelsPerUnit);
        int height = Mathf.RoundToInt(tiledSize.y * pixelsPerUnit);

        Color[] spritePixels = texture.GetPixels(
            (int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int index = (y + offsetY) * targetWidth + (x + offsetX);
                int spriteIndex = (y % (int)rect.height) * (int)rect.width + (x % (int)rect.width);

                if (spritePixels[spriteIndex].a > 0) // 알파 값이 있는 픽셀만 복사
                {
                    targetPixels[index] = spritePixels[spriteIndex];
                }
            }
        }
    }

    void ApplySeamlessTexture(Color[] pixels, Texture2D texture, int width, int height)
    {
        Color[] texturePixels = texture.GetPixels();
        int textureWidth = texture.width;
        int textureHeight = texture.height;

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

                // 불투명한 영역을 seamlessTexture 색상으로 덮어씌움
                if (pixels[index].a > 0)
                {
                    pixels[index] = texturePixels[textureIndex];
                }
            }
        }
    }
}