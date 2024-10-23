using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;

public class Gradation : MonoBehaviour
{
    [SerializeField]
    private GameObject Gradation_Prefab;

    [SerializeField]
    private int gradationCntPerHp = 100;

    List<GameObject> gradations = new List<GameObject>();

    private void Start()
    {
        //AddGradation(10);
    }


    private void AddGradation(int cnt)
    {
        GameObject gradation;
        for (int i = 0; i < cnt; i++)
        {
            gradation = Instantiate(Gradation_Prefab, transform.position, Quaternion.identity, transform);
            gradations.Add(gradation);
        }

        for (int i = 0; i < gradations.Count; i++)
            gradations[i].SetActive(false);
    }


    /// <summary>
    /// 눈금을 설정한다.
    /// </summary>
    public void SetGradation(float totalValue)
    {
        int numOfGradationToBeDisplayed = (int)(totalValue / gradationCntPerHp);
        float[] standardX = { -1, 1 };
        float intervalX = (-standardX[0] + standardX[1]) / (numOfGradationToBeDisplayed + 1);
        Vector3 pos;

        // 최소값 : -37.81    최대값 : 39.58

        // 눈금을 더 생성해야하는 경우
        if (gradations.Count < numOfGradationToBeDisplayed)
            AddGradation(numOfGradationToBeDisplayed - gradations.Count);  // 눈금이 생성되어 있는 것보다 부족하면 생성한다. 
        else 
        {
            // 생성되어 있는 눈금 중 비활성화하여 개수를 줄여야하는 경우
            for (int i = gradations.Count - numOfGradationToBeDisplayed; i < gradations.Count; i++)
                gradations[i].SetActive(false);

        }
       

        for (int i = 0; i < numOfGradationToBeDisplayed; i++)
        {
            pos = new Vector3(standardX[0] + intervalX * (i + 1), 0f, 0);
            gradations[i].transform.localPosition = pos;
            gradations[i].SetActive(true);

            if (numOfGradationToBeDisplayed % 10 == 0) // 눈금 개수가 10의 배수일때마다 눈금 크기 줄이기
                gradations[i].transform.localScale = new Vector3(0.005f - 0.001f * (numOfGradationToBeDisplayed / 10), 0.004f, 1);
        }
    }
}
