using System;
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

    [SerializeField] private bool isBoss;

    [SerializeField] private SliderBar hpBar;
    [SerializeField] private SliderBar shieldBar;
    [SerializeField] private SliderBar damageFlashBar;

    [SerializeField] private Image perceiveImg;
    [SerializeField] private Image confuseImg;
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
    private RectTransform confuseImgTr;

    private Enemy enemy;
    private void Awake()
    {
        enemy = GetComponent<Enemy>();

        // ������ ��� ����
        for (int i = 0; i < damageTxts.Length; i++)
        {
            damageTxts[i] = Instantiate(damageTxtPrefab, hpBar.transform);
            damageTxts[i].SetActive(false);

            TMP_Text text = damageTxts[i].GetComponent<TMP_Text>();
            text.text = "";
            text.color = Color.red;
        }

        if (perceiveImg)
        {
            perceiveImg.gameObject.SetActive(false);
            perceiveImgTr = DebugUtils.GetComponentWithErrorLogging<RectTransform>(perceiveImg.gameObject, "RectTransform");
        }

        if (confuseImg)
        {
            confuseImg.gameObject.SetActive(false);
            confuseImgTr = DebugUtils.GetComponentWithErrorLogging<RectTransform>(confuseImg.gameObject, "RectTransform");
        }
    }

    public void BindEnemyStatsToUi()
    {
        enemy.MaxHp.OnModified += SetMaxHPBarUI;
    }

    private void OnEnable()
    {
        // ������ ��� ����
        for (int i = 0; i < damageTxts.Length; i++)
        {
            damageTxts[i].SetActive(false);
            TMP_Text text = damageTxts[i].GetComponent<TMP_Text>();
            text.text = "";
            text.color = Color.red;
        }

        if (perceiveImg)
        {
            perceiveImg.gameObject.SetActive(false);
        }

        if (confuseImg)
        {
            confuseImg.gameObject.SetActive(false);
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

        if (confuseImg)
            confuseImgTr.position = screenPos + Vector3.up * (upDist + 0.5f);

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

    public void SetMaxHPBarUI()
    {
        if (hpBar == null)
        {
            Debug.LogError("hpBar is not assignment");
            return;
        }

        float value = enemy.MaxHp.Value;
        hpBar.SetMaxValueUI(value);
        shieldBar.SetMaxValueUI(value);
        damageFlashBar.SetMaxValueUI(value);
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


    public IEnumerator DamagePopUp(InkType inkType, float damage)
    {
        if (isBoss)
            yield break;

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
        damageTmpTxt.text = ((int) damage).ToString();
        damageTmpTxt.transform.SetAsLastSibling();
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

    internal void StartDamageFlash(float curHp, float damage, float maxHp)
    {
        StartCoroutine(DamageFlash(curHp, damage, maxHp));
    }

    private IEnumerator DamageFlash(float curHp, float damage, float maxHp)
    {
        float elapsed = 0f;
        float time = 0.3f;

        while (elapsed < time)
        {
            elapsed += Time.deltaTime;
            damageFlashBar.SetCurrValueUI(Mathf.Lerp(curHp, curHp - damage, elapsed / time));
            yield return null;
        }
        //SetStateBarUIForCurValue(maxHp, Mathf.Lerp(curHp, curHp - damage, elapsed / time), 0);
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

    public void SetConfuseImg(bool state)
    {
        confuseImg.gameObject.SetActive(state);
    }
}

