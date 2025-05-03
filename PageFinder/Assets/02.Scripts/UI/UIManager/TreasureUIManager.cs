using UnityEngine;

public class TreasureUIManager : MonoBehaviour
{
    [SerializeField] private Canvas treasureUICanvas;
    [SerializeField] PlayerState playerState;

    public void SetUICanvasState(bool value, bool changeScripts = true)
    {
        treasureUICanvas.gameObject.SetActive(value);

        if (!value) return;

        if (changeScripts)
        {
        }
    }

    public void OnClickHandler(int selection)
    {
        if (selection == 2) playerState.Coin += 80;
        else if (selection == 3) playerState.CurHp *= 0.6f;
        EventManager.Instance.PostNotification(EVENT_TYPE.UI_Changed, this, UIType.PageMap);
    }
}
