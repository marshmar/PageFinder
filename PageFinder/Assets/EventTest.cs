using UnityEngine;

public class EventTest : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            EventManager.Instance.PostNotification(EVENT_TYPE.UI_Changed, this, UIType.Reward);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            EventManager.Instance.PostNotification(EVENT_TYPE.UI_Changed, this, UIType.Battle);
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            EventManager.Instance.PostNotification(EVENT_TYPE.Buff, this, 9);
        }
    }
}
