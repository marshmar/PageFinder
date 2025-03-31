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
    public void Synthesize(GameObject a, GameObject b)
    {
        if (a == null || b == null) return;

        var inkMarkA = a.GetComponent<InkMark>();
        var inkMarkB = b.GetComponent<InkMark>();
        if (inkMarkA == null || inkMarkB == null) return;

        var newInkType = InkMarkSetter.Instance.SetMergedInkType(inkMarkA.CurrType, inkMarkB.CurrType);
        var newSprite = InkMarkSetter.Instance.SetSprite(newInkType);
        if (newSprite == null) return;

        SpriteMask maskA = a.GetComponentInChildren<SpriteMask>();
        SpriteMask maskB = b.GetComponentInChildren<SpriteMask>();
        if (maskA == null || maskB == null) return;

        Sprite spriteA = maskA.sprite;
        Sprite spriteB = maskB.sprite;
        if (spriteA == null || spriteB == null) return;

        // ���� ���� SpriteMask ��ġ (�߽�)
        Vector3 posA = maskA.transform.position;
        Vector3 posB = maskB.transform.position;

        // �� ��������Ʈ�� �ȼ� ũ�⸦ pixelPerUnit�� �̿��� ���� ũ��� ��ȯ
        float pixelPerUnit = 200;
        float widthWorldA = spriteA.rect.width / pixelPerUnit;
        float heightWorldA = spriteA.rect.height / pixelPerUnit;
        float widthWorldB = spriteB.rect.width / pixelPerUnit;
        float heightWorldB = spriteB.rect.height / pixelPerUnit;

        // �� ��������Ʈ�� ���� �ٿ�� �ڽ� ��� (��ħ ���ο� ������� ���� ��ġ �ݿ�)
        float left = Mathf.Min(posA.x - widthWorldA * 0.5f, posB.x - widthWorldB * 0.5f);
        float right = Mathf.Max(posA.x + widthWorldA * 0.5f, posB.x + widthWorldB * 0.5f);
        float bottom = Mathf.Min(posA.y - heightWorldA * 0.5f, posB.y - heightWorldB * 0.5f);
        float top = Mathf.Max(posA.y + heightWorldA * 0.5f, posB.y + heightWorldB * 0.5f);

        // ���� �ռ� �ؽ�ó�� ũ�� (�ȼ� ����)
        int compositePixelWidth = Mathf.RoundToInt((right - left) * pixelPerUnit);
        int compositePixelHeight = Mathf.RoundToInt((top - bottom) * pixelPerUnit);

        Texture2D newTexture = new Texture2D(compositePixelWidth, compositePixelHeight, TextureFormat.RGBA32, false);
        Color[] combinedPixels = new Color[compositePixelWidth * compositePixelHeight];

        // �� ��������Ʈ�� ���� �����ϴ� ��ǥ ���
        float spriteAWorldLowerLeftX = posA.x - (widthWorldA * 0.5f);
        float spriteAWorldLowerLeftY = posA.y - (heightWorldA * 0.5f);
        float spriteBWorldLowerLeftX = posB.x - (widthWorldB * 0.5f);
        float spriteBWorldLowerLeftY = posB.y - (heightWorldB * 0.5f);

        // �ռ� �ؽ�ó ������ �� ��������Ʈ�� �ȼ� ������ (���� ��ǥ�� pixelPerUnit�� �̿�)
        int offsetXA = Mathf.RoundToInt((spriteAWorldLowerLeftX - left) * pixelPerUnit);
        int offsetYA = Mathf.RoundToInt((spriteAWorldLowerLeftY - bottom) * pixelPerUnit);
        int offsetXB = Mathf.RoundToInt((spriteBWorldLowerLeftX - left) * pixelPerUnit);
        int offsetYB = Mathf.RoundToInt((spriteBWorldLowerLeftY - bottom) * pixelPerUnit);

        // �� ��������Ʈ�� �ȼ� �����͸� �ռ� �ؽ�ó�� �ùٸ� ��ġ�� �����մϴ�.
        CopyMaskedPixels(spriteA, combinedPixels, compositePixelWidth, offsetXA, offsetYA);
        CopyMaskedPixels(spriteB, combinedPixels, compositePixelWidth, offsetXB, offsetYB);

        // seamless sprite�� �ؽ�ó �����͸� �̿��� ������ �����ϴ�.
        ApplySeamlessTexture(combinedPixels, newSprite, compositePixelWidth, compositePixelHeight);

        newTexture.SetPixels(combinedPixels);
        newTexture.Apply();

        // �ռ��� ����� ���� ������Ʈ ����
        GameObject newObject = Instantiate(prefab);
        // �ٿ�� �ڽ��� �߽����� ��ġ ����
        newObject.transform.position = new Vector3((left + right) * 0.5f, (bottom + top) * 0.5f, (posA.z + posB.z) * 0.5f);
        newObject.GetComponent<InkMark>().CurrType = newInkType;

        SpriteRenderer srNew = newObject.GetComponentInChildren<SpriteRenderer>();
        SpriteMask newSpriteMask = newObject.GetComponentInChildren<SpriteMask>();

        if (newSpriteMask != null)
        {
            newSpriteMask.sprite = Sprite.Create(newTexture, new Rect(0, 0, compositePixelWidth, compositePixelHeight), new Vector2(0.5f, 0.5f), pixelPerUnit);
        }
        srNew.sprite = newSpriteMask.sprite;

        // Set Unique Sorting Order (inkId based)
        int baseOrder = InkMarkSetter.inkId * 10;
        InkMarkSetter.inkId++;
        srNew.sortingOrder = baseOrder + 1;

        newSpriteMask.frontSortingOrder = baseOrder + 1;
        newSpriteMask.backSortingOrder = baseOrder;
        newSpriteMask.isCustomRangeActive = true;

        Destroy(a);
        Destroy(b);
    }

    // �־��� Sprite�� �ȼ� �����͸� targetPixels�� �����մϴ�.
    // targetPixels�� �ռ� �ؽ�ó�� �ȼ� �迭�̰�, offsetX/Y�� ���� ���� ��ġ(�ȼ� ����)�Դϴ�.
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
                int targetIndex = (y + offsetY) * targetWidth + (x + offsetX);
                int spriteIndex = y * width + x;
                if (spritePixels[spriteIndex].a > 0)
                {
                    targetPixels[targetIndex] = spritePixels[spriteIndex];
                }
            }
        }
    }

    // newSprite�� �ؽ�ó �����͸� ���ø��Ͽ�, �ռ� �ؽ�ó�� ������ ������ ������ �����մϴ�.
    void ApplySeamlessTexture(Color[] pixels, Sprite sprite, int width, int height)
    {
        Texture2D texture = sprite.texture;
        Rect spriteRect = sprite.rect;
        int textureWidth = (int)spriteRect.width;
        int textureHeight = (int)spriteRect.height;
        Color[] texturePixels = texture.GetPixels((int)spriteRect.x, (int)spriteRect.y, textureWidth, textureHeight);

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
                if (pixels[index].a > 0)
                {
                    pixels[index] = texturePixels[textureIndex];
                }
            }
        }
    }
}