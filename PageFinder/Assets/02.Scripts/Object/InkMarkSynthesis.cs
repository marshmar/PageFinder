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

        // 월드 상의 SpriteMask 위치 (중심)
        Vector3 posA = maskA.transform.position;
        Vector3 posB = maskB.transform.position;

        // 각 스프라이트의 픽셀 크기를 pixelPerUnit을 이용해 월드 크기로 변환
        float pixelPerUnit = 200;
        float widthWorldA = spriteA.rect.width / pixelPerUnit;
        float heightWorldA = spriteA.rect.height / pixelPerUnit;
        float widthWorldB = spriteB.rect.width / pixelPerUnit;
        float heightWorldB = spriteB.rect.height / pixelPerUnit;

        // 두 스프라이트의 월드 바운딩 박스 계산 (겹침 여부와 상관없이 실제 위치 반영)
        float left = Mathf.Min(posA.x - widthWorldA * 0.5f, posB.x - widthWorldB * 0.5f);
        float right = Mathf.Max(posA.x + widthWorldA * 0.5f, posB.x + widthWorldB * 0.5f);
        float bottom = Mathf.Min(posA.y - heightWorldA * 0.5f, posB.y - heightWorldB * 0.5f);
        float top = Mathf.Max(posA.y + heightWorldA * 0.5f, posB.y + heightWorldB * 0.5f);

        // 최종 합성 텍스처의 크기 (픽셀 단위)
        int compositePixelWidth = Mathf.RoundToInt((right - left) * pixelPerUnit);
        int compositePixelHeight = Mathf.RoundToInt((top - bottom) * pixelPerUnit);

        Texture2D newTexture = new Texture2D(compositePixelWidth, compositePixelHeight, TextureFormat.RGBA32, false);
        Color[] combinedPixels = new Color[compositePixelWidth * compositePixelHeight];

        // 각 스프라이트의 월드 좌측하단 좌표 계산
        float spriteAWorldLowerLeftX = posA.x - (widthWorldA * 0.5f);
        float spriteAWorldLowerLeftY = posA.y - (heightWorldA * 0.5f);
        float spriteBWorldLowerLeftX = posB.x - (widthWorldB * 0.5f);
        float spriteBWorldLowerLeftY = posB.y - (heightWorldB * 0.5f);

        // 합성 텍스처 내에서 각 스프라이트의 픽셀 오프셋 (월드 좌표와 pixelPerUnit을 이용)
        int offsetXA = Mathf.RoundToInt((spriteAWorldLowerLeftX - left) * pixelPerUnit);
        int offsetYA = Mathf.RoundToInt((spriteAWorldLowerLeftY - bottom) * pixelPerUnit);
        int offsetXB = Mathf.RoundToInt((spriteBWorldLowerLeftX - left) * pixelPerUnit);
        int offsetYB = Mathf.RoundToInt((spriteBWorldLowerLeftY - bottom) * pixelPerUnit);

        // 각 스프라이트의 픽셀 데이터를 합성 텍스처의 올바른 위치에 복사합니다.
        CopyMaskedPixels(spriteA, combinedPixels, compositePixelWidth, offsetXA, offsetYA);
        CopyMaskedPixels(spriteB, combinedPixels, compositePixelWidth, offsetXB, offsetYB);

        // seamless sprite의 텍스처 데이터를 이용해 색상을 입힙니다.
        ApplySeamlessTexture(combinedPixels, newSprite, compositePixelWidth, compositePixelHeight);

        newTexture.SetPixels(combinedPixels);
        newTexture.Apply();

        // 합성된 결과를 담을 오브젝트 생성
        GameObject newObject = Instantiate(prefab);
        // 바운딩 박스의 중심으로 위치 설정
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

    // 주어진 Sprite의 픽셀 데이터를 targetPixels에 복사합니다.
    // targetPixels는 합성 텍스처의 픽셀 배열이고, offsetX/Y는 복사 시작 위치(픽셀 단위)입니다.
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

    // newSprite의 텍스처 데이터를 샘플링하여, 합성 텍스처의 불투명 영역에 색상을 적용합니다.
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