using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUIManager : MonoBehaviour
{
    [SerializeField]
    private Canvas BattleUICanvas;

    public void SetBattleUICanvasState(bool value)
    {
        BattleUICanvas.gameObject.SetActive(value);
    }
}
