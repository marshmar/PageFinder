using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleButton : MonoBehaviour
{
    public void GameStart()
    {
        Debug.Log("�� ��ȯ");
        //SceneManager.LoadScene("Prototype_CSP"); //KHD_TEST
        SceneManager.LoadScene("Loading");
    }
}