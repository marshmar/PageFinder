using UnityEngine;

public class InkMarkSynthesis : MonoBehaviour
{
    public SpriteRenderer spriteA;
    public SpriteRenderer spriteB;
    public Texture2D seamlessTexture;

    void Start()
    {
        MergeSprites(spriteA, spriteB, seamlessTexture); // Generate New Merged Texture
    }

    public void MergeSprites(SpriteRenderer spriteA, SpriteRenderer spriteB, Texture2D seamlessTexture)
    {
        if (spriteA == null || spriteB == null || seamlessTexture == null)
        {
            Debug.LogError("SpriteRenderers or texture is missing!");
            return;
        }

        // Calculate the center position and rotation values of existing objects
        Vector3 mergedPosition = (spriteA.transform.position + spriteB.transform.position) / 2f;
        Quaternion mergedRotation = spriteA.transform.rotation;

        int width = Mathf.Max(spriteA.sprite.texture.width, spriteB.sprite.texture.width);
        int height = Mathf.Max(spriteA.sprite.texture.height, spriteB.sprite.texture.height);
        Texture2D newTexture = new(width, height);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 worldPos = PixelToWorld(spriteA, x, y, mergedRotation);

                Color colorA = GetPixelFromWorld(spriteA, worldPos, mergedRotation);
                Color colorB = GetPixelFromWorld(spriteB, worldPos, mergedRotation);

                bool isAVisible = colorA.a > 0.1f;
                bool isBVisible = colorB.a > 0.1f;

                Color finalColor = Color.clear;

                if (isAVisible || isBVisible)
                {
                    float u = (float)x / width;
                    float v = (float)y / height;
                    finalColor = seamlessTexture.GetPixelBilinear(u, v);
                }

                newTexture.SetPixel(x, y, finalColor);
            }
        }

        newTexture.Apply();

        CreateNewSpriteObject(newTexture, mergedPosition, mergedRotation, spriteA.flipX, spriteA.flipY, spriteA.sprite.pivot);

        Destroy(spriteA.gameObject);
        Destroy(spriteB.gameObject);
    }

    void CreateNewSpriteObject(Texture2D texture, Vector3 position, Quaternion rotation, bool flipX, bool flipY, Vector2 pivot)
    {
        GameObject newSpriteObj = new("MergedSprite");
        SpriteRenderer newSpriteRenderer = newSpriteObj.AddComponent<SpriteRenderer>();
        Sprite newSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), pivot / new Vector2(texture.width, texture.height));

        newSpriteRenderer.sprite = newSprite;
        newSpriteObj.transform.SetPositionAndRotation(position, rotation);

        // Maintain existing Flip X and Flip Y values
        newSpriteRenderer.flipX = flipX;
        newSpriteRenderer.flipY = flipY;
    }

    Vector2 PixelToWorld(SpriteRenderer sprite, int x, int y, Quaternion rotation)
    {
        Vector2 pivotOffset = sprite.sprite.pivot / new Vector2(sprite.sprite.texture.width, sprite.sprite.texture.height);
        Vector3 localPos = new(((float)x / sprite.sprite.texture.width) - pivotOffset.x, ((float)y / sprite.sprite.texture.height) - pivotOffset.y, 0);
        Vector3 rotatedPos = rotation * localPos;
        return sprite.transform.TransformPoint(rotatedPos);
    }

    Color GetPixelFromWorld(SpriteRenderer sprite, Vector2 worldPos, Quaternion rotation)
    {
        Vector3 localPos = sprite.transform.InverseTransformPoint(worldPos);
        localPos = Quaternion.Inverse(rotation) * localPos;

        Vector2 pivotOffset = sprite.sprite.pivot / new Vector2(sprite.sprite.texture.width, sprite.sprite.texture.height);
        localPos.x += pivotOffset.x;
        localPos.y += pivotOffset.y;

        Texture2D texture = sprite.sprite.texture;
        int pixelX = Mathf.RoundToInt(localPos.x * texture.width);
        int pixelY = Mathf.RoundToInt(localPos.y * texture.height);

        if (pixelX < 0 || pixelY < 0 || pixelX >= texture.width || pixelY >= texture.height)
            return Color.clear;

        return texture.GetPixel(pixelX, pixelY);
    }
}