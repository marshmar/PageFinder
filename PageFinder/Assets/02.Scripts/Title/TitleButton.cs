using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleButton : MonoBehaviour
{
    public void GameStart()
    {
        Debug.Log("Scene Transition");
        SceneManager.LoadScene("Loading");
    }
}