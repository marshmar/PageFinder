using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBasicAttackCollider : MonoBehaviour
{
    private Player player;
    //private PlayerAttackController playerAttackControllerScr;
    //private NewPlayerAttackController newPlayerAttackController;
    //private PlayerState playerState;
    //private PlayerInkType playerInkType;
    private bool isInkGained;
    [SerializeField] private GameObject[] attackEffects;
    [SerializeField] public float inkMarkScale = 2.0f;
    public InkType baInkType;

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("PLAYER");
        player = DebugUtils.GetComponentWithErrorLogging<Player>(playerObj, "Player");

        //playerInkType = DebugUtils.GetComponentWithErrorLogging<PlayerInkType>(playerObj, "PlayerInkType");
        //playerAttackControllerScr = DebugUtils.GetComponentWithErrorLogging<PlayerAttackController>(playerObj, "PlayerAttackController");
        //newPlayerAttackController = DebugUtils.GetComponentWithErrorLogging<NewPlayerAttackController>(playerObj, "NewPlayerAttackController");
        //playerState = DebugUtils.GetComponentWithErrorLogging<PlayerState>(playerObj, "PlayerState");
        isInkGained = false;
    }

    private void OnEnable()
    {
        isInkGained = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        /*
        if (other.CompareTag("ENEMY"))
        {
            Enemy entityScr = DebugUtils.GetComponentWithErrorLogging<Enemy>(other.transform, "Enemy");
            if (!DebugUtils.CheckIsNullWithErrorLogging<Enemy>(entityScr, this.gameObject))
            {
                if (playerInkType.BasicAttackInkType == InkType.RED)
                {
                    GameObject instantiatedEffect = Instantiate(attackEffects[0], other.transform.position, Quaternion.identity);
                    instantiatedEffect.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                    Destroy(instantiatedEffect, 1.0f);
                }
                if (playerInkType.BasicAttackInkType == InkType.GREEN)
                {
                    GameObject instantiatedEffect = Instantiate(attackEffects[1], other.transform.position, Quaternion.identity);
                    instantiatedEffect.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                    Destroy(instantiatedEffect, 1.0f);
                }
                if (playerInkType.BasicAttackInkType == InkType.BLUE)
                {
                    GameObject instantiatedEffect = Instantiate(attackEffects[2], other.transform.position, Quaternion.identity);
                    instantiatedEffect.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                    Destroy(instantiatedEffect, 1.0f);

                    if (!isInkGained)
                    {
                        playerState.ExtraInkGain();
                        isInkGained = true;
                    }
                }

                if (playerAttackControllerScr.ComboCount == 0)
                {
                    GenerateInkMark(other.transform.position);
                    AudioManager.Instance.Play(Sound.hit2Sfx, AudioClipType.BaSfx);

                    // 기본 데미지 감소시킬 경우
                    entityScr.Hit(playerInkType.BasicAttackInkType, playerState.CalculateDamageAmount(1.0f));

                    // 적한테 디버프 걸 경우
                    //entityScr.Hit(InkType.RED, playerState.CalculateDamageAmount(1.0f), Enemy.DebuffState.STAGGER, 2); //70
                }
                else if (playerAttackControllerScr.ComboCount == 1)
                {
                    AudioManager.Instance.Play(Sound.hit3Sfx, AudioClipType.BaSfx);
                    entityScr.Hit(playerInkType.BasicAttackInkType, playerState.CalculateDamageAmount(0.9f));
                }
                else
                {
                    entityScr.Hit(playerInkType.BasicAttackInkType, playerState.CalculateDamageAmount(1.3f));
                    AudioManager.Instance.Play(Sound.hit1Sfx, AudioClipType.BaSfx);
                }
            }
        }
        // 최승표 추가 코드 : 페이퍼박스와의 상호작용
        else if (other.CompareTag("OBJECT") && other.GetComponent<PaperBox>())
        {
            Debug.Log("PlayerBasicAttackCollider 페이퍼박스와 맞닿음");
            PaperBox paperBoxScr = DebugUtils.GetComponentWithErrorLogging<PaperBox>(other.gameObject, "PaperBox");
            paperBoxScr.SetDurability(playerInkType.BasicAttackInkType, 30); // 페이퍼박스 내구도 감소시키기
        }
        */

        if (other.CompareTag("ENEMY"))
        {
            Enemy entityScr = DebugUtils.GetComponentWithErrorLogging<Enemy>(other.transform, "Enemy");
            if (!DebugUtils.CheckIsNullWithErrorLogging<Enemy>(entityScr, this.gameObject))
            {
                if (baInkType == InkType.RED)
                {
                    GameObject instantiatedEffect = Instantiate(attackEffects[0], other.transform.position, Quaternion.identity);
                    instantiatedEffect.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                    Destroy(instantiatedEffect, 1.0f);
                }
                if (baInkType == InkType.GREEN)
                {
                    GameObject instantiatedEffect = Instantiate(attackEffects[1], other.transform.position, Quaternion.identity);
                    instantiatedEffect.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                    Destroy(instantiatedEffect, 1.0f);
                }
                if (baInkType == InkType.BLUE)
                {
                    GameObject instantiatedEffect = Instantiate(attackEffects[2], other.transform.position, Quaternion.identity);
                    instantiatedEffect.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                    Destroy(instantiatedEffect, 1.0f);            
                }

                if (player.AttackController.ComboCount == 0)
                {
                    GenerateInkMark(other.transform.position);
                    AudioManager.Instance.Play(Sound.hit2Sfx, AudioClipType.BaSfx);

                    // 기본 데미지 감소시킬 경우
                    entityScr.Hit(baInkType, player.State.CalculateDamageAmount(1.0f));

                    // 적한테 디버프 걸 경우
                    //entityScr.Hit(InkType.RED, playerState.CalculateDamageAmount(1.0f), Enemy.DebuffState.STAGGER, 2); //70
                }
                else if (player.AttackController.ComboCount == 1)
                {
                    AudioManager.Instance.Play(Sound.hit3Sfx, AudioClipType.BaSfx);
                    entityScr.Hit(baInkType, player.State.CalculateDamageAmount(0.9f));
                }
                else
                {
                    entityScr.Hit(baInkType, player.State.CalculateDamageAmount(1.3f));
                    AudioManager.Instance.Play(Sound.hit1Sfx, AudioClipType.BaSfx);
                }
            }
        }
        // 최승표 추가 코드 : 페이퍼박스와의 상호작용
        else if (other.CompareTag("OBJECT") && other.GetComponent<PaperBox>())
        {
            Debug.Log("PlayerBasicAttackCollider 페이퍼박스와 맞닿음");
            PaperBox paperBoxScr = DebugUtils.GetComponentWithErrorLogging<PaperBox>(other.gameObject, "PaperBox");
            paperBoxScr.SetDurability(baInkType, 30); // 페이퍼박스 내구도 감소시키기
        }
    }

    public virtual GameObject GenerateInkMark(Vector3 position)
    {
        Vector3 spawnPostion = new Vector3(position.x, player.Utils.Tr.position.y + 0.1f, position.z);
        InkMark inkMark = InkMarkPooler.Instance.Pool.Get();
        if (!DebugUtils.CheckIsNullWithErrorLogging<InkMark>(inkMark, this.gameObject))
        {
            inkMark.SetInkMarkData(InkMarkType.BASICATTACK, baInkType);
            inkMark.transform.position = spawnPostion;
        }
        return null;
    }
}