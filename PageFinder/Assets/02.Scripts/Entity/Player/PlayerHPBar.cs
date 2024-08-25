using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHPBar : MonoBehaviour
{
    public Slider hpBar;
    public GameObject Gradation_Prefab;

    List<GameObject> gradations = new List<GameObject>();
    int gradationCntPerHp = 20;

    Player player;

    // Start is called before the first frame update
    public void Start()
    {
        player  = GameObject.FindWithTag("PLAYER").GetComponent<Player>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
            SetGradation();
    }

    /// <summary>
    /// HP바의 최대 HP를 조정하는 함수
    /// </summary>
    /// <param name="maxHP"></param>
    public void SetMaxHPUI(float maxHP)
    {
        hpBar.maxValue = maxHP;
    }

    /// <summary>
    /// HP바의 HP를 조정하는 함수
    /// </summary>
    public void SetHPUI(float currHP)
    {
        hpBar.value = currHP;
    }

    /// <summary>
    /// 눈금을 설정한다.
    /// </summary>
    void SetGradation()
    {
        Transform gradationSet = transform.GetChild(2);
        int gradationCnt = (int)(player.MAXHP / gradationCntPerHp); // 
        GameObject gradation;
        float[] standardX = { -1.126f, 1.19f };
        float intervalX = (-standardX[0] + standardX[1]) / (gradationCnt + 1);
        Vector3 pos;

        // 최소값 : -37.81    최대값 : 39.58
        for (int i = 0; i < gradations.Count; i++)
            Destroy(gradations[i]);

        gradations.Clear();
        
        for (int i=0; i< gradationCnt; i++)
        {
            pos = new Vector3(standardX[0] + intervalX * (i+1) , 0f, 0);
            gradation = Instantiate(Gradation_Prefab, transform.position, Quaternion.identity, gradationSet);
            gradation.transform.localPosition = pos;

            if(gradationCnt % 10 == 0) // 눈금 개수가 10의 배수일때마다 눈금 크기 줄이기
                gradation.transform.localScale = new Vector3(0.005f - 0.015f * (gradationCnt / 10), 0.003f, 1);
            gradations.Add(gradation);
        }
    }
}
