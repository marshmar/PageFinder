using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBasicAttackCollider : MonoBehaviour
{
    private PlayerAttackController playerAttackControllerScr;
    [SerializeField]
    private GameObject inkMarkObj;

    private void Start()
    {
        playerAttackControllerScr = DebugUtils.GetComponentWithErrorLogging<PlayerAttackController>(GameObject.FindGameObjectWithTag("PLAYER"), "PlayerAttackController");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ENEMY"))
        {
            Debug.Log("Ãæµ¹");
            Enemy enemyScr = DebugUtils.GetComponentWithErrorLogging<Enemy>(other.transform, "Enemy");
            if(!DebugUtils.CheckIsNullWithErrorLogging<Enemy>(enemyScr, this.gameObject))
            {
                enemyScr.HP -= 10;
                Debug.Log(enemyScr.HP);
                if(playerAttackControllerScr.ComboCount == 0)
                    GenerateInkMark(other.transform.position);
            }

        }
    }

    public virtual GameObject GenerateInkMark(Vector3 position)
    {
        Vector3 spawnPostion = new Vector3(position.x, 1.1f, position.z);
        if (!DebugUtils.CheckIsNullWithErrorLogging<GameObject>(inkMarkObj, this.gameObject))
        {
            GameObject instantiatedMark = Instantiate(inkMarkObj, spawnPostion, Quaternion.identity);
            if (!DebugUtils.CheckIsNullWithErrorLogging<GameObject>(instantiatedMark, this.gameObject))
            {
                InkMark inkMark = DebugUtils.GetComponentWithErrorLogging<InkMark>(instantiatedMark, "Skill");
                if (!DebugUtils.CheckIsNullWithErrorLogging<InkMark>(inkMark, this.gameObject))
                {
                    inkMark.CurrMark = playerAttackControllerScr.BasicAttackInkMark;
                    inkMark.SetSprites();

                }
                instantiatedMark.transform.Rotate(90, 0, 0);
                instantiatedMark.transform.localScale = new Vector3(1.0f, 1.0f, 0.1f);
            }
        }
        return null;
    }
}
