using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBasicAttackCollider : MonoBehaviour
{
    private PlayerAttackController playerAttackControllerScr;
    private PlayerAudioController playerAudioControllerScr;
    [SerializeField]
    private GameObject inkMarkObj;
    private Player playerScr;
    private bool isInkGained;
    [SerializeField]
    private GameObject[] attackEffects;
    [SerializeField]
    public float inkMarkScale = 2.0f;

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("PLAYER");
        playerScr = DebugUtils.GetComponentWithErrorLogging<Player>(playerObj, "Player");
        playerAttackControllerScr = DebugUtils.GetComponentWithErrorLogging<PlayerAttackController>(playerObj, "PlayerAttackController");
        playerAudioControllerScr = DebugUtils.GetComponentWithErrorLogging<PlayerAudioController>(playerObj, "PlayerAudioController");
        isInkGained = false;
    }

    private void OnEnable()
    {
        isInkGained = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ENEMY"))
        {
           Entity entityScr = DebugUtils.GetComponentWithErrorLogging<Entity>(other.transform, "Entity");
            if(!DebugUtils.CheckIsNullWithErrorLogging<Entity>(entityScr, this.gameObject))
            {
                if (playerScr.BasicAttackInkType == InkType.RED)
                {
                    GameObject instantiatedEffect = Instantiate(attackEffects[0], other.transform);
                    instantiatedEffect.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                    Destroy(instantiatedEffect, 1.0f);
                }
                if (playerScr.BasicAttackInkType == InkType.GREEN)
                {
                    GameObject instantiatedEffect = Instantiate(attackEffects[1], other.transform);
                    instantiatedEffect.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                    Destroy(instantiatedEffect, 1.0f);
                }
                if (playerScr.BasicAttackInkType == InkType.BLUE)
                {
                    GameObject instantiatedEffect = Instantiate(attackEffects[2], other.transform);
                    instantiatedEffect.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                    Destroy(instantiatedEffect, 1.0f);

                    if (!isInkGained)
                    {
                        playerScr.ExtraInkGain();
                        isInkGained = true;
                    }
                }

                if(playerAttackControllerScr.ComboCount == 0)
                {
                    GenerateInkMark(other.transform.position);
                    playerAudioControllerScr.PlayAudio("Attack2");
                    entityScr.HP -= 70;
                }
                else
                {
                    entityScr.HP -= 50;
                    playerAudioControllerScr.PlayAudio("Attack1");
                }


                
                


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
                instantiatedMark.transform.localScale = new Vector3(inkMarkScale, inkMarkScale, inkMarkScale);
            }
        }
        return null;
    }
}
