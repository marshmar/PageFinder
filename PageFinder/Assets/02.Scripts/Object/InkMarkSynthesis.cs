using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class InkMarkSynthesis : Singleton<InkMarkSynthesis>
{
    public GameObject prefab;

    public async void Synthesize(GameObject a, GameObject b)
    {
        await Task.Delay(1000);
        var inkMarkA = a.GetComponent<InkMark>();
        var inkMarkB = b.GetComponent<InkMark>();

        var newInkType = InkMarkSetter.Instance.SetMergedInkType(inkMarkA.CurrType, inkMarkB.CurrType);
        var seamlessSprite = InkMarkSetter.Instance.SetSprite(newInkType);
        var seamlessTexture = seamlessSprite.texture;

        inkMarkA.FadeOut = true;
        inkMarkB.FadeOut = true;

        // 1. Set both ink prefabs and all child objects to the INKMARK layer.
        int inkMarkLayer = LayerMask.NameToLayer("INKMARK");
        SetLayerRecursively(a, inkMarkLayer);
        SetLayerRecursively(b, inkMarkLayer);

        // 2. Calculating the center positions of a and b (based on SpriteRenderer)
        SpriteRenderer aSR = a.GetComponentInChildren<SpriteRenderer>();
        SpriteRenderer bSR = b.GetComponentInChildren<SpriteRenderer>();

        Vector3 centerA = aSR.bounds.center;
        Vector3 centerB = bSR.bounds.center;
        Vector3 inkCenter = (centerA + centerB) / 2f;

        // 3. Create a temporary camera and take a picture from above
        GameObject tempCameraObj = new("InkTempCamera");
        Camera tempCamera = tempCameraObj.AddComponent<Camera>();
        tempCamera.clearFlags = CameraClearFlags.SolidColor;
        tempCamera.backgroundColor = new Color(0, 0, 0, 0);
        tempCamera.cullingMask = LayerMask.GetMask("INKMARK");
        tempCamera.transform.position = inkCenter + new Vector3(0, 10f, 0);
        tempCamera.transform.rotation = Quaternion.Euler(90f, 180f, 0f);

        // 4. Create a RenderTexture (adjust resolution to suit your needs)
        int width = 1024;
        int height = 1024;
        RenderTexture rt = new(width, height, 24);
        tempCamera.targetTexture = rt;
        tempCamera.Render();

        // 5. Reading Texture2D from RenderTexture
        RenderTexture.active = rt;
        Texture2D capturedTexture = new(width, height, TextureFormat.ARGB32, false);
        capturedTexture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        capturedTexture.Apply();

        // 6. Initialize RenderTexture
        tempCamera.targetTexture = null;
        RenderTexture.active = null;
        rt.Release();
        Destroy(tempCameraObj);

        // 7. Color mapping of seamlessTexture to the part of the captured texture where the alpha value is greater than 0
        Color[] capturedPixels = capturedTexture.GetPixels();
        Color[] seamlessPixels = seamlessTexture.GetPixels();
        int seamlessWidth = seamlessTexture.width;
        int seamlessHeight = seamlessTexture.height;

        Color[] processedPixels = await Task.Run(() =>
        {
            Color[] result = new Color[width * height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int i = y * width + x;
                    Color capturedPixel = capturedPixels[i];

                    if (capturedPixel.a > 0)
                    {
                        float u = (float)x / width;
                        float v = (float)y / height;

                        result[i] = SampleBilinear(seamlessPixels, seamlessWidth, seamlessHeight, u, v);
                    }
                    else
                    {
                        result[i] = capturedPixel;
                    }
                }
            }

            return result;
        });

        Texture2D processedTexture = new(width, height, TextureFormat.ARGB32, false);
        processedTexture.SetPixels(processedPixels);
        processedTexture.Apply();

        // 8. Create Sprite from processedTexture
        Sprite newSprite = Sprite.Create(processedTexture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));

        // 9. Create a composite ink prefab by instantiating the prefab.
        GameObject synthesizedInk = Instantiate(prefab, inkCenter, Quaternion.Euler(90, 0, 0));

        // 10. Assign a new Sprite to a Sprite Renderer
        SpriteRenderer sr = synthesizedInk.GetComponentInChildren<SpriteRenderer>();
        sr.sprite = newSprite;

        // 11. Assign a new Sprite to a Sprite Mask
        SpriteMask sm = synthesizedInk.GetComponentInChildren<SpriteMask>();
        sm.sprite = newSprite;

        // 12. Set Unique Sorting Order (inkId based)
        int baseOrder = InkMarkSetter.inkId * 10;
        InkMarkSetter.inkId++;
        sr.sortingOrder = baseOrder + 1;
        sm.frontSortingOrder = baseOrder + 1;
        sm.backSortingOrder = baseOrder;
        sm.isCustomRangeActive = true;

        InkMark ink = synthesizedInk.GetComponent<InkMark>();
        ink.SetSynthesizedInkMarkData(InkMarkType.SYNTHESIZED, newInkType);
        
        // 13. Add a Collider
        SphereCollider sphereColl = synthesizedInk.AddComponent<SphereCollider>();
        sphereColl.isTrigger = true;
        sphereColl.radius = 2.5f;
    }

    // A function that recursively sets the layers of a GameObject and all of its children.
    void SetLayerRecursively(GameObject obj, int layer)
    {
        if (obj == null) return;

        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }

    Color SampleBilinear(Color[] pixels, int width, int height, float u, float v)
    {
        float x = u * (width - 1);
        float y = v * (height - 1);

        int x1 = Mathf.FloorToInt(x);
        int y1 = Mathf.FloorToInt(y);
        int x2 = Mathf.Min(x1 + 1, width - 1);
        int y2 = Mathf.Min(y1 + 1, height - 1);

        float tx = x - x1;
        float ty = y - y1;

        Color c00 = pixels[y1 * width + x1];
        Color c10 = pixels[y1 * width + x2];
        Color c01 = pixels[y2 * width + x1];
        Color c11 = pixels[y2 * width + x2];

        return Color.Lerp(Color.Lerp(c00, c10, tx), Color.Lerp(c01, c11, tx), ty);
    }
}