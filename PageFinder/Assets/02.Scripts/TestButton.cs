using UnityEngine;

public class TestButton : MonoBehaviour
{
    public void ButtonAction()
    {
        Debug.Log("실드 생성 이벤트 발생");
        EventManager.Instance.PostNotification(EVENT_TYPE.Generate_Shield_Player, this, new System.Tuple<float, float>(10f, 10f));
    }
}
