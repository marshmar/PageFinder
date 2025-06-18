using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDamageIndicator : MonoBehaviour
{
    private Image damageIndicatorImg;
    private float _damageIndicatorShowingTime = 0.5f;
    private WaitForSeconds _damageIndicatorDelay;

    private void Awake()
    {
        damageIndicatorImg = this.GetComponentSafe<Image>();
    }

    private void Start()
    {
        damageIndicatorImg.enabled = false;
        _damageIndicatorDelay = new WaitForSeconds(_damageIndicatorShowingTime);
    }

    public IEnumerator ShowDamageIndicator()
    {
        damageIndicatorImg.enabled = true;
        yield return _damageIndicatorDelay;
        damageIndicatorImg.enabled = false;
    }
}