using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBasicAttackCollider : MonoBehaviour
{
    private PlayerAttackController playerAttackControllerScr;
    [SerializeField]
    private GameObject inkMarkObj;
    private Player playerScr;
    private void Start()
    {
        playerScr = DebugUtils.GetComponentWithErrorLogging<Player>(GameObject.FindGameObjectWithTag("PLAYER"), "Player");
        playerAttackControllerScr = DebugUtils.GetComponentWithErrorLogging<PlayerAttackController>(GameObject.FindGameObjectWithTag("PLAYER"), "PlayerAttackController");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ENEMY"))
        {
            Debug.Log("Ãæµ¹");
            Entity enemyScr = DebugUtils.GetComponentWithErrorLogging<Entity>(other.transform, "Enemy");
            if(!DebugUtils.CheckIsNullWithErrorLogging<Entity>(enemyScr, this.gameObject))
            {
                enemyScr.HP -= 100;
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
                    inkMark.CurrType = playerScr.BasicAttackInkType;
                    inkMark.SetSprites();

                }
                instantiatedMark.transform.Rotate(90, 0, 0);
                instantiatedMark.transform.localScale = new Vector3(1.0f, 1.0f, 0.1f);
            }
        }
        return null;
    }
}
