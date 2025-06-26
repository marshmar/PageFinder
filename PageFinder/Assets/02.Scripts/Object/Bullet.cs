using System;
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
    private InkType bulletInkType;
    protected Transform tr;
    protected float damage;
    protected MeshRenderer meshRenderer;

    public BulletType bulletType;
    public float duration;
    public float bulletSpeed;
    public bool isCreateInkMarkObj;


    public float Damage { get => damage; set => damage = value; }
    public InkType BulletInkType { get => bulletInkType; set 
        { 
            bulletInkType = value;
            //SetMaterial();
        }
    }

    private void SetMaterial()
    {
        Color color;
        if (!DebugUtils.CheckIsNullWithErrorLogging<MeshRenderer>(meshRenderer))
        {
            switch (bulletInkType)
            {
                case InkType.Red:
                    if(ColorUtility.TryParseHtmlString("#D54A2C", out color))
                    {
                        meshRenderer.material.color = color;
                    }
                    break;
                case InkType.Green:
                    if (ColorUtility.TryParseHtmlString("#65A539", out color))
                    {
                        meshRenderer.material.color = color;
                    }
                    break;
                case InkType.Blue:
                    if (ColorUtility.TryParseHtmlString("#1E9BC5", out color))
                    {
                        meshRenderer.material.color = color;
                    }
                    break;

            }
        }
    }

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
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        if(bulletType == BulletType.PLAYER)
        {

        }
        else if(bulletType == BulletType.ENEMY)
        {
            if(other.CompareTag("PLAYER"))
            {
                if(isCreateInkMarkObj)
                    GenerateInkMark(other.ClosestPoint(tr.position));
                PlayerState playerState = DebugUtils.GetComponentWithErrorLogging<PlayerState>(other.gameObject, "Player");
                playerState.CurHp -= damage;
                Destroy(gameObject);
            }
            else if(other.CompareTag("MAP"))
            {
                if (isCreateInkMarkObj)
                    GenerateInkMark(other.ClosestPoint(tr.position));
                Destroy(gameObject);
            }
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

        tr.position = new Vector3(tr.position.x, fixedYPosition, tr.position.z);
        Vector3 dir = (direction - tr.position);
        tr.rotation = Quaternion.LookRotation(dir) * Quaternion.Euler(0f, 180f, 0f);
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

    public virtual void GenerateInkMark(Vector3 spawnPosition)
    {
        InkMark inkMark = InkMarkPooler.Instance.Pool.Get();
        if(!DebugUtils.CheckIsNullWithErrorLogging<InkMark>(inkMark, this.gameObject))
        {
            inkMark.SetInkMarkData(InkMarkType.INKSKILL, bulletInkType);
            inkMark.transform.position = spawnPosition;
            inkMark.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        }
    }
}
