using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text progressText;
    [SerializeField] private Slider loadingBar;

    void Start()
    {
        LoadSceneAsync("ProceduralMap");
    }

    private async Task LoadSceneAsync(string sceneName)
    {
        var operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            if (loadingBar != null) loadingBar.value += progress * 0.05f + Time.deltaTime * 0.01f;
            if (progressText != null) progressText.text = $"{loadingBar.value * 100:F0}%";
            if (progress >= 1 && loadingBar.value >= 1)
            {
                operation.allowSceneActivation = true;
                await Task.Yield();
            }
            await Task.Delay(100);
        }
    }
}