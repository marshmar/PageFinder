using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BlinkingText : MonoBehaviour
{
    private TMP_Text text;
    private bool decreasing;
    [SerializeField, Range(0, 1.0f)]
    private float blinkingValue;
    // Start is called before the first frame update
    void Start()
    {
        decreasing = true;
        text = DebugUtils.GetComponentWithErrorLogging<TMP_Text>(this.gameObject, "TMP_Text");
        StartCoroutine(BlinkText());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator BlinkText()
    {
        float time = 0.0f;
        float alpha = 0;
        while (true)
        {
            time += Time.deltaTime * blinkingValue;

            if (decreasing)
            {
                alpha = 1.0f - time;
                if (alpha <= 0)
                {
                    time = 0.0f;
                    alpha = 0.0f;
                    decreasing = false;
                }
            }
            else
            {
                alpha = time;
                if (alpha >= 1)
                {
                    time = 0.0f;
                    alpha = 1.0f;
                    decreasing = true;

                }
            }

            text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);

            yield return null;

        }
    }
}
