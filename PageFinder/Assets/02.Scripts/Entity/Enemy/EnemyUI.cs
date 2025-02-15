using System.Collections;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Experimental.AI;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class EnemyUI : MonoBehaviour
{
    private const float upDist = 80f;
    bool damageFlashIsRunning;

    [SerializeField]
    private bool isBoss;

    [SerializeField] private SliderBar hpBar;
    [SerializeField] private SliderBar shieldBar;
    [SerializeField] private SliderBar damageFlashBar;

    [SerializeField] private Image perceiveImg;

    [SerializeField]
    private GameObject damageTxtPrefab;
    private GameObject[] damageTxts = new GameObject[5];
    [SerializeField]
    private Vector3[] damagePos = new Vector3[] 
    {   new Vector3(-26, 50, 0 ),  
        new Vector3( 0, 40, 0),
        new Vector3( 26, 45, 0),
        new Vector3(-20, 20, 0),
        new Vector3(15, 20, 0)};

    [SerializeField]
    private RectTransform enemyUITr;

    private RectTransform perceiveImgTr;

    void Start()
    {
        // ������ ��� ����
        for (int i = 0; i < damageTxts.Length; i++)
        {
            damageTxts[i] = Instantiate(damageTxtPrefab, hpBar.transform);
            damageTxts[i].SetActive(false);
        }

        damageFlashBar.SetMaxValueUI(1);
        damageFlashBar.SetCurrValueUI(0);

        damageFlashIsRunning = false;

        if(perceiveImg)
        {
            perceiveImg.gameObject.SetActive(false);
            perceiveImgTr = DebugUtils.GetComponentWithErrorLogging<RectTransform>(perceiveImg.gameObject, "RectTransform");
        }
    }

    private void Update()
    {
        if (isBoss)
            return;

        // ���� ���� ��ǥ�� ��ũ�� ��ǥ�� ��ȯ
        Vector3 screenPos = Camera.main.WorldToScreenPoint(gameObject.transform.position) + Vector3.up * upDist;
        enemyUITr.position = screenPos;

        if (perceiveImg)
            perceiveImgTr.position = screenPos + Vector3.up * (upDist + 0.5f) ;

        // ���� �÷��̾� ȭ�� �ȿ� ���� ��쿡�� UI�� ǥ�õ� �� �ֵ��� �ؾ���
    }

    public void SetCurrHPBarUI(float maxHP, float currHP, float maxShield, float currShield)
    {
        if (hpBar == null)
        {
            Debug.LogError("hpBar is not assignment");
            return;
        }

        // ü�� ���� �����ϴ� ��츸
        //if (hpBar.bar.value - currHP > 0)
        //{
        //    if (damageFlashIsRunning)
        //    {
        //        StopCoroutine(DamageFlash(hpBar.bar.value, currHP + currShield, maxHP));
        //        damageFlashIsRunning = false;
        //    }

        //    StartCoroutine(DamageFlash(hpBar.bar.value, currHP + currShield, maxHP));
        //}

        hpBar.SetCurrValueUI(currHP);
        Debug.Log($"Hp : {currHP}");
    }

    public void SetMaxHPBarUI(float value)
    {
        if (hpBar == null)
        {
            Debug.LogError("hpBar is not assignment");
            return;
        }
        hpBar.SetMaxValueUI(value);
        shieldBar.SetMaxValueUI(value);
    }

    public void SetStateBarUIForCurValue(float maxHP, float curHP, float shieldValue)
    {
        //Debug.Log($"maxHP : {maxHP}     currHp : {curHP}    shieldValue : {shieldValue}");
        if (curHP + shieldValue >= maxHP)
        {
            float hpRatio = curHP * (curHP / (curHP + shieldValue));
            hpBar.SetCurrValueUI(hpRatio);
            shieldBar.SetCurrValueUI(maxHP);
        }
        else
        {
            shieldBar.SetCurrValueUI(curHP + shieldValue);
            hpBar.SetCurrValueUI(curHP);
        }
    }

    //public void SetCurrShieldUI(float maxHP, float currHP, float maxShield, float currShield)
    //{
    //    if (shieldBar == null)
    //    {
    //        Debug.LogError("shieldBar is not assignment");
    //        return;
    //    }

    //    // ���� ü�� + ���� �ǵ尡 �ִ� ü�� ���� �ʰ��ϴ� ���
    //    if (currHP + currShield > maxHP)
    //        currShield = (maxHP + currShield) * (currShield / (currShield + currHP));

    //    if (currShield == 0)
    //    {
    //        shieldBar.SetCurrValueUI(currShield);

    //        // ���尡 ���� ���ŵ� ������ UI�� MaxHp, CurrHp ���� �ٽ� ������
    //        hpBar.SetMaxValueUI(maxHP);
    //        hpBar.SetCurrValueUI(currHP);
    //    }
    //    else
    //    {
    //        shieldBar.SetMaxValueUI(currHP + currShield);
    //        Debug.Log($"currHp :{currHP}  currShield : {currShield}");
    //        shieldBar.SetCurrValueUI(currHP + currShield);
    //    }

    //    Debug.Log($"UI CurrShield : {currShield}");

    //    // ���� ���� �����ϴ� ��츸
    //    //if (shieldBar.bar.value - currShield > 0)
    //    //{
    //    //    if (damageFlashIsRunning)
    //    //    {
    //    //        StopCoroutine(DamageFlash(shieldBar.bar.value, currHP + currShield, maxHP));
    //    //        damageFlashIsRunning = false;
    //    //    }

    //    //    StartCoroutine(DamageFlash(shieldBar.bar.value, currHP + currShield, maxHP));
    //    //}
    //}

    //public void SetMaxShieldUI(float maxHP, float maxShield)
    //{
    //    if (shieldBar == null)
    //    {
    //        Debug.LogError("shieldBar is not assignment");
    //        return;
    //    }

    //    shieldBar.SetMaxValueUI(maxHP + maxShield);
    //}

    public IEnumerator DamagePopUp(InkType inkType, float damage)
    {
        int damageTxtIndex = GetDamageTxtIndexToCanUse();
        GameObject damageTxtObj = damageTxts[damageTxtIndex];
        TMP_Text damageTmpTxt = DebugUtils.GetComponentWithErrorLogging<TMP_Text>(damageTxtObj, "TMP_Text");
        RectTransform damageTxtRect = DebugUtils.GetComponentWithErrorLogging<RectTransform>(damageTxtObj, "RectTransform");
        float activeTime = 1;
        float deleteTime = 1;
        float runTime = 0.0f;
        float upDist = 50.0f;

        Vector3 dmageUIPos = damagePos[damageTxtIndex];

        damageTxtRect.localPosition = dmageUIPos;
        damageTxtRect.localScale = Vector3.one * 0.8f;
        damageTmpTxt.text = damage.ToString();
        damageTxtObj.SetActive(true);

        switch (inkType)
        {
            case InkType.RED:
                damageTmpTxt.color = Color.red;
                break;

            case InkType.BLUE:
                damageTmpTxt.color = Color.blue;
                break;

            case InkType.GREEN:
                damageTmpTxt.color = Color.green;
                break;
        }

        // Ȱ��ȭ
        while (runTime < activeTime)
        {
            runTime += Time.deltaTime;

            damageTxtRect.localPosition = Vector3.Lerp(dmageUIPos, dmageUIPos + Vector3.up * upDist, runTime / activeTime);
            damageTxtRect.localScale = Vector3.Lerp(Vector3.one * 0.8f, Vector3.one * 1.5f, runTime / activeTime);

            yield return null;
        }

        // ��Ȱ��ȭ
        Color color = damageTmpTxt.color;
        while (color.a > 0)
        {
            color.a -= Time.deltaTime / deleteTime;
            damageTmpTxt.color = color;
            yield return null;
        }

        damageTxtObj.SetActive(false);
    }

    private int GetDamageTxtIndexToCanUse()
    {
        for(int i=0; i< damageTxts.Length; i++)
        {
            if (!damageTxts[i].activeSelf)
                return i;
        }

        return -1;
    }

    private IEnumerator DamageFlash(float currValue, float targetValue, float maxHP)
    {
        damageFlashIsRunning = true;

        damageFlashBar.SetMaxValueUI(maxHP);
        damageFlashBar.SetCurrValueUI(currValue);

        Debug.Log($"DamageFlash ���� : {shieldBar.bar.value}");
        yield return new WaitForSeconds(0.5f);

        float elapsed = 0f;
        float time = 0.3f;
        float value = currValue;

        while(elapsed < time)
        {
            elapsed += Time.deltaTime;
            value = Mathf.Lerp(currValue, targetValue, elapsed / time);
            damageFlashBar.SetCurrValueUI(value);
            yield return null;
        }

        damageFlashBar.SetCurrValueUI(0);

        damageFlashIsRunning = false;
        Debug.Log($"DamageFlash �� : {shieldBar.bar.value}");
    }

    public void ActivatePerceiveImg()
    {
        if(!isBoss)
            StartCoroutine(Perceive());
    }

    private IEnumerator Perceive()
    {
        perceiveImg.gameObject.SetActive(true);

        yield return new WaitForSeconds(1f);

        perceiveImg.gameObject.SetActive(false);
    }
}

