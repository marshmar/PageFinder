using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text progressText;

    void Start()
    {
        StartCoroutine(LoadSceneAsync("ProceduralMap"));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;
        var loadingBar = GetComponentInChildren<Slider>();

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            if (loadingBar != null) loadingBar.value += progress * 0.05f + Time.deltaTime * 0.01f;
            if (progressText != null) progressText.text = $"{loadingBar.value * 100:F0}%";
            if (progress >= 1 && loadingBar.value >= 1)
            {
                operation.allowSceneActivation = true;
                yield return new WaitUntil(() => operation.isDone);
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
}