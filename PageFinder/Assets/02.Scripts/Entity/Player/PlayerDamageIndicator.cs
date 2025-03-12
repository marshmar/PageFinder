using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDamageIndicator : MonoBehaviour
{
    [SerializeField]
    private Image damageIndicator_Img;
    private void Start()
    {
        damageIndicator_Img.enabled = false;
    }

    public IEnumerator ShowDamageIndicator()
    {
        damageIndicator_Img.enabled = true;
        yield return new WaitForSeconds(0.5f);
        damageIndicator_Img.enabled = false;
    }
}