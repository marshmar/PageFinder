using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBasicAttackCollider : MonoBehaviour
{
    #region Variables
    private Player _player;
    private float _extraDamageMultiplier = 0f;
    private float _durabilityLoss        = 30.0f;
    private float[] _damageMultipliers;
    private short[] _baSfxIndexs;
    private InkType _baInkType;


    [SerializeField] private GameObject[] attackEffects;
    #endregion

    #region Properties
    public InkType BaInkType { get => _baInkType; set => _baInkType = value; }
    public float ExtraDamageMultiplier { get => _extraDamageMultiplier; set => _extraDamageMultiplier = value; }
    #endregion

    #region Unity Lifecycle
    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("PLAYER");
        _player = playerObj.GetComponentSafe<Player>();

        Initialize();
    }

    #endregion

    #region Initialization
    private void Initialize()
    {
        _damageMultipliers = new float[3] { 1.0f, 0.9f, 1.3f };
        _baSfxIndexs = new short[3] { Sound.hit3Sfx, Sound.hit1Sfx, Sound.hit2Sfx };

        this.gameObject.SetActive(false);
    }
    #endregion

    #region Actions
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ENEMY"))
        {
            // For Attack Tutorial
            EventManager.Instance.PostNotification(EVENT_TYPE.FirstBasicAttack, this);

            Enemy enemy = other.transform.GetComponentSafe<Enemy>();
            if (enemy.IsNull()) return;

            float damageAmount = _player.State.CalculateDamageAmount(_damageMultipliers[_player.AttackController.ComboCount]);
            enemy.Hit(_baInkType, damageAmount);

            // Play hit sfx
            AudioManager.Instance.Play(_baSfxIndexs[_player.AttackController.ComboCount], AudioClipType.HitSfx);

            // Block execution if basic attack effects for red, green, and blue are not present.
            if (attackEffects.Length < 3) return;
            float inkMarkScale = 0.2f;
            GameObject instantiatedEffect = Instantiate(attackEffects[(int)_baInkType], other.transform.position, Quaternion.identity);
            instantiatedEffect.transform.localScale = new Vector3(inkMarkScale, inkMarkScale, inkMarkScale);
            Destroy(instantiatedEffect, 1.0f);


            if(_player.AttackController.ComboCount == 0)
            {
                GenerateInkMark(Utils.GetSpawnPosRayCast(other.transform.position));
            }

        }

        else if (other.CompareTag("OBJECT"))
        {
            PaperBox paperBox = other.GetComponentSafe<PaperBox>();
            if (paperBox.IsNull()) return;

            paperBox.SetDurability(_baInkType, _durabilityLoss); // 페이퍼박스 내구도 감소시키기
        }
    }

    public virtual void GenerateInkMark(Vector3 spawnPosition)
    {
        InkMark inkMark = InkMarkPooler.Instance.Pool.Get();
        if(inkMark.IsNull()) return;

        inkMark.SetInkMarkData(InkMarkType.BASICATTACK, _baInkType);
        inkMark.transform.position = spawnPosition;
    }
    #endregion

    #region Getter
    #endregion

    #region Setter
    #endregion

    #region Utilities
    #endregion

    #region Events
    #endregion




}