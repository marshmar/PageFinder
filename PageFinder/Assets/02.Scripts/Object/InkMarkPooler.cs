using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// 잉크마크 오브젝트 풀링을 담당하는 클래스
/// </summary>
public class InkMarkPooler : Singleton<InkMarkPooler> 
{
    public GameObject inkMarkPrefab;
    private ObjectPool<InkMark> pool;
    public int maxPoolSize = 40;
    public int defaultPoolCapacity = 10;

    public override void Awake()
    {
        base.Awake();
        pool = new ObjectPool<InkMark>(CreatedPooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, true, defaultPoolCapacity, maxPoolSize);
    }

    // 오브젝트 풀 생성시 초기 객체 할당 이벤트 함수
    private InkMark CreatedPooledItem()
    {
        var inkMarkObj = Instantiate(inkMarkPrefab);

        InkMark inkMark = inkMarkObj.AddComponent<InkMark>();
        inkMarkObj.name = "InkMark";

        return inkMark;
    }

    // 오브젝트 풀에 사용 객체 반환
    private void OnReturnedToPool(InkMark inkMark)
    {
        inkMark.gameObject.SetActive(false);
    }

    // 오브젝트 풀에서 객체 꺼내기
    private void OnTakeFromPool(InkMark inkMark)
    {
        inkMark.gameObject.SetActive(true);
    }

    // 오브젝트 풀 객체 삭제
    private void OnDestroyPoolObject(InkMark inkMark)
    {
        Destroy(inkMark.gameObject);
    }
}
