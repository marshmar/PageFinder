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
        Sprite spriteA = a.GetComponent<SpriteRenderer>().sprite;
        Sprite spriteB = b.GetComponent<SpriteRenderer>().sprite;

        int overlapX = Mathf.Min(spriteA.texture.width, spriteB.texture.width) / 2;
        int overlapY = Mathf.Min(spriteA.texture.height, spriteB.texture.height) / 2;

        int width = spriteA.texture.width + spriteB.texture.width - (2 * overlapX);
        int height = spriteA.texture.height + spriteB.texture.height - (2 * overlapY);

        Texture2D newTexture = new(width, height);
        Color[] pixelsA = spriteA.texture.GetPixels();
        Color[] pixelsB = spriteB.texture.GetPixels();
        Color[] texturePixels = texture.GetPixels();
        Color[] combinedPixels = new Color[width * height];

        int offsetXA = 0;
        int offsetYA = height / 4;
        int offsetXB = spriteA.texture.width - overlapX;
        int offsetYB = height / 2 - overlapY;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int index = y * width + x;
                Color pixelColor = new(0, 0, 0, 0);

                // spriteA pixel
                if (x >= offsetXA && x < offsetXA + spriteA.texture.width &&
                    y >= offsetYA && y < offsetYA + spriteA.texture.height)
                {
                    int spriteAX = x - offsetXA;
                    int spriteAY = y - offsetYA;
                    int spriteIndex = spriteAY * spriteA.texture.width + spriteAX;

                    if (spriteIndex < pixelsA.Length && pixelsA[spriteIndex].a > 0)
                    {
                        pixelColor = pixelsA[spriteIndex];
                    }
                }

                // spriteB pixel
                if (x >= offsetXB && x < offsetXB + spriteB.texture.width &&
                    y >= offsetYB && y < offsetYB + spriteB.texture.height)
                {
                    int spriteBX = x - offsetXB;
                    int spriteBY = y - offsetYB;
                    int spriteIndex = spriteBY * spriteB.texture.width + spriteBX;

                    if (spriteIndex < pixelsB.Length && pixelsB[spriteIndex].a > 0)
                    {
                        pixelColor = pixelsB[spriteIndex];
                    }
                }

                // Filling Seamless Texture by normalizing it when it is opaque
                if (pixelColor.a > 0)
                {
                    float u = (float)x / width; // 0~1 normalized coordinates
                    float v = (float)y / height;

                    int tx = Mathf.FloorToInt(u * texture.width) % texture.width;
                    int ty = Mathf.FloorToInt(v * texture.height) % texture.height;
                    int textureIndex = ty * texture.width + tx;

                    pixelColor = texturePixels[textureIndex]; // Get Texture pixels at normalized locations
                }

                combinedPixels[index] = pixelColor;
            }
        }

        newTexture.SetPixels(combinedPixels);
        newTexture.Apply();

        // Create a new object by duplicating the original prefab
        GameObject newObject = Instantiate(prefab);
        newObject.transform.position = (a.transform.position + b.transform.position) / 2;
        SpriteMask spriteMask = newObject.GetComponentInChildren<SpriteMask>();

        if (spriteMask != null) spriteMask.sprite = Sprite.Create(newTexture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));

        // Create a new game object to apply the Seamless Texture to
        SpriteRenderer sr = newObject.GetComponent<SpriteRenderer>();

        Sprite seamlessSprite = TextureToSprite(seamlessTexture);
        sr.sprite = seamlessSprite;
        sr.drawMode = SpriteDrawMode.Tiled;
        sr.size = new Vector2(1, 1);
        sr.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        sr.flipY = true;

        Destroy(a);
        Destroy(b);
    }

    Sprite TextureToSprite(Texture2D texture)
    {
        if (texture == null) return null;
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }
}