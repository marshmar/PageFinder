using UnityEngine;

public class Player : MonoBehaviour
{
    #region Variables
    #endregion

    #region Properties
    public PlayerAnim Anim { get; private set; }
    public PlayerState State { get; private set; }
    public PlayerUtils Utils { get; private set; }
    public PlayerTargetingVisualizer TargetingVisualizer { get; private set; }
    public PlayerInputInvoker InputInvoker { get; private set; }
    public PlayerInputAction InputAction { get; private set; }
    public PlayerBuff Buff { get; private set; }
    public PlayerInteraction Interaction { get; private set; }
    public PlayerUI UI { get; private set; }
    public PlayerBasicAttackCollider BasicAttackCollider { get; private set; }
    public PlayerAttackController AttackController { get; private set; }
    public NewPlayerDashController DashController { get; private set; }
    public NewPlayerSkillController SkillController { get; private set; }
    public PlayerMoveController MoveController { get; private set; }
    public ScriptInventory ScriptInventory { get; private set; }
    public StickerInventory StickerInventory { get; private set; }
    public TargetObject TargetMarker { get; private set; }

    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        Anim = this.GetComponentSafe<PlayerAnim>();
        State = this.GetComponentSafe<PlayerState>();
        Utils = this.GetComponentSafe<PlayerUtils>();
        TargetingVisualizer = this.GetComponentSafe<PlayerTargetingVisualizer>();
        InputInvoker = this.GetComponentSafe<PlayerInputInvoker>();
        InputAction = this.GetComponentSafe<PlayerInputAction>();
        Buff = this.GetComponentSafe<PlayerBuff>();
        Interaction = this.GetComponentSafe<PlayerInteraction>();
        UI = this.GetComponentSafe<PlayerUI>();

        AttackController = this.GetComponentSafe<PlayerAttackController>();
        DashController = this.GetComponentSafe<NewPlayerDashController>();
        SkillController = this.GetComponentSafe<NewPlayerSkillController>();
        MoveController = this.GetComponentSafe<PlayerMoveController>();
        ScriptInventory = this.GetComponentSafe<ScriptInventory>();
        StickerInventory = this.GetComponentSafe<StickerInventory>();

        BasicAttackCollider = this.GetComponentInChildrenSafe<PlayerBasicAttackCollider>();
        TargetMarker = this.GetComponentInChildrenSafe<TargetObject>();
    }
    #endregion

    #region Initialization
    #endregion

    #region Actions
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