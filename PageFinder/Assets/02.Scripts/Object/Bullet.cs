using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BulletType
{
    PLAYER,
    ENEMY
}
public class Bullet : MonoBehaviour
{
    private float currentDuration;
    private INKMARK bulletInkMark;
    protected Transform tr;
    protected float damage;


    public BulletType bulletType;
    public float duration;
    public float bulletSpeed;
    public GameObject inkMarkObj;
    public bool isCreateInkMarkObj;


    public float Damage { get => damage; set => damage = value; }
    public INKMARK BulletInkMark { get => bulletInkMark; set => bulletInkMark = value; }

    // Start is called before the first frame update
    public virtual void Awake()
    {
        currentDuration = 0;
        if (TryGetComponent<Transform>(out Transform transform))
        {
            tr = transform;
        }
        else
        {
            Debug.LogError($"{gameObject.name}: Could not Get Transform Component");
            return; 
        }
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        if(bulletType == BulletType.PLAYER)
        {

        }
        else if(bulletType == BulletType.ENEMY)
        {

        }
    }

    public virtual void Fire(Vector3 direction)
    {
        StartCoroutine(FireCoroutine(direction));
    }

    public virtual IEnumerator FireCoroutine(Vector3 direction)
    {
        direction.y = 0;

        float fixedYPosition = tr.position.y + 0.5f;

        while (currentDuration < duration)
        {
            // 현재 위치에서 목표 위치까지 일정한 속도로 이동
            tr.position = Vector3.MoveTowards(tr.position, direction, bulletSpeed * Time.deltaTime);
            tr.position = new Vector3(tr.position.x, fixedYPosition, tr.position.z);
            yield return null; // 다음 프레임까지 대기

            currentDuration += Time.deltaTime;
        }

        Destroy(this.gameObject);
    }

    public virtual GameObject GenerateInkMark(Vector3 position)
    {
        Vector3 spawnPostion = new Vector3(position.x, 1.1f, position.z);
        if(!DebugUtils.CheckIsNullWithErrorLogging<GameObject>(inkMarkObj, this.gameObject))
        {
            GameObject instantiatedMark = Instantiate(inkMarkObj, spawnPostion, Quaternion.identity);
            if(!DebugUtils.CheckIsNullWithErrorLogging<GameObject>(instantiatedMark, this.gameObject))
            {
                InkMark inkMark = DebugUtils.GetComponentWithErrorLogging<InkMark>(instantiatedMark, "Skill");
                if(!DebugUtils.CheckIsNullWithErrorLogging<InkMark>(inkMark, this.gameObject))
                {
                    inkMark.CurrMark = bulletInkMark;
                    inkMark.SetMaterials();
                    
                }
                instantiatedMark.transform.Rotate(90, 0, 0);
                return instantiatedMark;
            }
        }
        return null;
    }
}
