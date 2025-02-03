using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.AI;
using UnityEngine.UI;

public class EnemyUI : MonoBehaviour
{
    private const float upDist = 80f;
    bool damageFlashIsRunning;

    [SerializeField] private SliderBar hpBar;
    [SerializeField] private SliderBar shieldBar;
    [SerializeField] private SliderBar damageFlashBar;

    [SerializeField]
    private GameObject damageTxtPrefab;
    private GameObject[] damageTxts = new GameObject[5];
    private Vector3[] damagePos = new Vector3[] 
    {   new Vector3(-26, 50, 0 ),  
        new Vector3( 0, 40, 0),
        new Vector3( 26, 45, 0),
        new Vector3(-20, 20, 0),
        new Vector3(15, 20, 0)};

    [SerializeField]
    private RectTransform enemyUITr; 

    void Start()
    {
        // 데미지 출력 관련
        for (int i = 0; i < damageTxts.Length; i++)
        {
            damageTxts[i] = Instantiate(damageTxtPrefab, hpBar.transform);
            damageTxts[i].SetActive(false);
        }

        damageFlashBar.SetMaxValueUI(1);
        damageFlashBar.SetCurrValueUI(0);

        damageFlashIsRunning = false;
    }

    private void Update()
    {
        // 적의 월드 좌표를 스크린 좌표로 변환
        Vector3 screenPos = Camera.main.WorldToScreenPoint(gameObject.transform.position) + Vector3.up * upDist;
        enemyUITr.position = screenPos;

        // 적이 플레이어 화면 안에 들어온 경우에만 UI에 표시될 수 있도록 해야함
    }

    public void SetCurrHPBarUI(float maxHP, float currHP, float currShield)
    {
        if (hpBar == null)
        {
            Debug.LogError("hpBar is not assignment");
            return;
        }
        // 현재 체력 + 현재 실드가 최대 체력 값을 초과하는 경우
        if(currHP + currShield > maxHP)
            currHP = maxHP * (currHP / currShield + currHP);

        // 체력 값을 감소하는 경우만
        if (hpBar.bar.value - currHP > 0)
        {
            if (damageFlashIsRunning)
            {
                StopCoroutine(DamageFlash(hpBar.bar.value, currHP + currShield, maxHP));
                damageFlashIsRunning = false;
            }

            StartCoroutine(DamageFlash(hpBar.bar.value, currHP + currShield, maxHP));
        }

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
        Debug.Log($"MaxHp : {value}");
    }

    public void SetCurrShieldUI(float maxHP, float currHP, float currShield)
    {
        if (shieldBar == null)
        {
            Debug.LogError("shieldBar is not assignment");
            return;
        }

        // 현재 체력 + 현재 실드가 최대 체력 값을 초과하는 경우
        if (currHP + currShield > maxHP)
            currShield = maxHP * (currShield / currShield + currHP);

        // 맨 처음 초기화할 때
        if (currShield == 0)
            shieldBar.SetCurrValueUI(currShield);
        else
        {
            // 쉴드 값을 감소하는 경우만
            if (shieldBar.bar.value - currShield > 0)
            {
                if (damageFlashIsRunning)
                {
                    StopCoroutine(DamageFlash(shieldBar.bar.value, currHP + currShield, maxHP));
                    damageFlashIsRunning = false;
                }

                StartCoroutine(DamageFlash(shieldBar.bar.value, currHP + currShield, maxHP));
            }
           
            shieldBar.SetCurrValueUI(currHP + currShield);
        }
            
        Debug.Log($"UI CurrShield : {currShield}");
    }

    public void SetMaxShieldUI(float maxHP)
    {
        if (shieldBar == null)
        {
            Debug.LogError("shieldBar is not assignment");
            return;
        }

        // Shield 동작 방식
        // Slider는 MaxHp를 기준으로 설정한다.
        // => 수치 값은 쉴드의 값을 계속 변경하지만 Slider의 Max Value와 value는 MaxHp를 기준으로 표시함
        // MaxHp : 200  CurrHp : 100    MaxShield : 50 CurrShield : 50 이라면
        // Shield Slider의 Value : CurrHp + currShield => 150

        shieldBar.SetMaxValueUI(maxHP);
    }

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

        damageTxtRect.localPosition = damagePos[damageTxtIndex];
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

        // 활성화
        while (runTime < activeTime)
        {
            runTime += Time.deltaTime;

            damageTxtRect.localPosition = Vector3.Lerp(damagePos[damageTxtIndex], damagePos[damageTxtIndex] + Vector3.up * upDist, runTime / activeTime);
            damageTxtRect.localScale = Vector3.Lerp(Vector3.one * 0.8f, Vector3.one * 1.5f, runTime / activeTime);

            yield return null;
        }

        // 비활성화
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

        Debug.Log($"DamageFlash 시작 : {shieldBar.bar.value}");
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
        Debug.Log($"DamageFlash 끝 : {shieldBar.bar.value}");
    }
}

