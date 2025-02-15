using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBasicAttackCollider : MonoBehaviour
{
    private PlayerAttackController playerAttackControllerScr;
    private PlayerAudioController playerAudioControllerScr;
    private PlayerState playerState;
    [SerializeField]
    private GameObject inkMarkObj;

    private PlayerInkType playerInkType;
    private bool isInkGained;
    [SerializeField]
    private GameObject[] attackEffects;
    [SerializeField]
    public float inkMarkScale = 2.0f;

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("PLAYER");

        playerInkType = DebugUtils.GetComponentWithErrorLogging<PlayerInkType>(playerObj, "PlayerInkType");
        playerAttackControllerScr = DebugUtils.GetComponentWithErrorLogging<PlayerAttackController>(playerObj, "PlayerAttackController");
        playerAudioControllerScr = DebugUtils.GetComponentWithErrorLogging<PlayerAudioController>(playerObj, "PlayerAudioController");
        playerState = DebugUtils.GetComponentWithErrorLogging<PlayerState>(playerObj, "PlayerState");
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
            EnemyAction entityScr = DebugUtils.GetComponentWithErrorLogging<EnemyAction>(other.transform, "Enemy");
            if (!DebugUtils.CheckIsNullWithErrorLogging<EnemyAction>(entityScr, this.gameObject))
            {
                if (playerInkType.BasicAttackInkType == InkType.RED)
                {
                    GameObject instantiatedEffect = Instantiate(attackEffects[0], other.transform);
                    instantiatedEffect.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                    Destroy(instantiatedEffect, 1.0f);
                }
                if (playerInkType.BasicAttackInkType == InkType.GREEN)
                {
                    GameObject instantiatedEffect = Instantiate(attackEffects[1], other.transform);
                    instantiatedEffect.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                    Destroy(instantiatedEffect, 1.0f);
                }
                if (playerInkType.BasicAttackInkType == InkType.BLUE)
                {
                    GameObject instantiatedEffect = Instantiate(attackEffects[2], other.transform);
                    instantiatedEffect.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                    Destroy(instantiatedEffect, 1.0f);

                    /*                    if (!isInkGained)
                                        {
                                            playerInkType.ExtraInkGain();
                                            isInkGained = true;
                                        }*/
                }

                if (playerAttackControllerScr.ComboCount == 0)
                {
                    GenerateInkMark(other.transform.position);
                    //playerAudioControllerScr.PlayAudio("Attack2");
                    AudioManager.Instance.Play(SoundPath.hit2SfxPath);
                    entityScr.Hit(InkType.RED, playerState.CalculateDamageAmount(1.0f), Enemy.DebuffState.STAGGER, 2); //70
                }
                else if (playerAttackControllerScr.ComboCount == 1)
                {
                    AudioManager.Instance.Play(SoundPath.hit3SfxPath);
                    entityScr.Hit(InkType.RED, playerState.CalculateDamageAmount(0.9f), Enemy.DebuffState.STAGGER, 2);
                }
                else
                {
                    //entityScr.Hit(InkType.RED, 1, Enemy.DebuffState.KNOCKBACK, 3, transform.position); //50
                    entityScr.Hit(InkType.RED, playerState.CalculateDamageAmount(1.3f), Enemy.DebuffState.STAGGER, 3); //50
                    //playerAudioControllerScr.PlayAudio("Attack1");
                    AudioManager.Instance.Play(SoundPath.hit1SfxPath);
                }
            }
        }
        // 최승표 추가 코드 : 페이퍼박스와의 상호작용
        else if (other.CompareTag("OBJECT") && other.name.Equals("PaperBox"))
        {
            Debug.Log("PlayerBasicAttackCollider 페이퍼박스와 맞닿음");
            PaperBox paperBoxScr = DebugUtils.GetComponentWithErrorLogging<PaperBox>(other.gameObject, "PaperBox");
            paperBoxScr.SetDurability(playerInkType.BasicAttackInkType, 30); // 페이퍼박스 내구도 감소시키기
        }
    }

    public virtual GameObject GenerateInkMark(Vector3 position)
    {
        Vector3 spawnPostion = new Vector3(position.x, 1.1f, position.z);
        if (!DebugUtils.CheckIsNullWithErrorLogging<GameObject>(inkMarkObj, this.gameObject))
        {
            InkMark inkMark = InkMarkPooler.Instance.Pool.Get();
            if (!DebugUtils.CheckIsNullWithErrorLogging<InkMark>(inkMark, this.gameObject))
            {
                inkMark.SetInkMarkData(InkMarkType.BASICATTACK, playerInkType.BasicAttackInkType);
                inkMark.transform.position = spawnPostion;
            }
        }
        return null;
    }
}