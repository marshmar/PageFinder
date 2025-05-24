using UnityEngine;


public class Player : MonoBehaviour
{
    public PlayerAnim Anim { get; private set; }
    public PlayerState State { get; private set; }
    public PlayerUtils Utils { get; private set; }
    public PlayerTarget Target { get; private set; }
    public PlayerInputInvoker InputInvoker { get; private set; }
    public PlayerInputAction InputAction { get; private set; }
    public PlayerBuff Buff { get; private set; }
    public PlayerInteraction Interaction { get; private set; }
    public PlayerUI UI { get; private set; }
    public PlayerBasicAttackCollider BasicAttackCollider { get; private set; }
    public NewPlayerAttackController AttackController { get; private set; }
    public NewPlayerDashController DashController { get; private set; }
    public NewPlayerSkillController SkillController { get; private set; }
    public PlayerMoveController MoveController { get; private set; }
    public ScriptInventory ScriptInventory { get; private set; }
    public StickerInventory StickerInventory { get; private set; }
    public TargetObject TargetMarker { get; private set; }

    private void Awake()
    {
        Anim = GetComponent<PlayerAnim>();
        State = GetComponent<PlayerState>();
        Utils = GetComponent<PlayerUtils>();
        Target = GetComponent<PlayerTarget>();
        InputInvoker = GetComponent<PlayerInputInvoker>();
        InputAction = GetComponent<PlayerInputAction>();
        Buff = GetComponent<PlayerBuff>();
        Interaction = GetComponent<PlayerInteraction>();
        UI = GetComponent<PlayerUI>();

        AttackController = GetComponent<NewPlayerAttackController>();
        DashController = GetComponent<NewPlayerDashController>();
        SkillController = GetComponent<NewPlayerSkillController>();
        MoveController = GetComponent<PlayerMoveController>();
        ScriptInventory = GetComponent<ScriptInventory>();
        StickerInventory = GetComponent<StickerInventory>();

        BasicAttackCollider = GetComponentInChildren<PlayerBasicAttackCollider>();
        TargetMarker = GetComponentInChildren<TargetObject>();
    }
}