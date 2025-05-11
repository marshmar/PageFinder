using UnityEngine;
using TMPro;
public class BossUIManager : MonoBehaviour, IUIElement
{
    [Header("Boss Panel")]
    [SerializeField] private GameObject hud_boss;

    [Header("SliderBar")]
    [SerializeField] private SliderBar hpBar;
    [SerializeField] private SliderBar shieldBar;
    [SerializeField] private SliderBar damageFlashBar;

    [Header("BossText")]
    [SerializeField] private TMP_Text bossNameTxt;

    public void BindBossUI(EnemyData enemyData, EnemyUI enemyUi)
    {
        Open();
        bossNameTxt.text = "À§Ä¡µå";
        enemyUi.BindBossUi(hpBar, shieldBar, damageFlashBar);
    }

    public void Open()
    {
        hud_boss.gameObject.SetActive(true);
    }
    
    public void Close()
    {
        hud_boss.gameObject.SetActive(false);
    }

    public void Refresh()
    {
        
    }
}
