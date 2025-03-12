using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInputAction : MonoBehaviour
{
    private PlayerInput input;

    public InputAction MoveAction { get; private set; }
    public InputAction DashAction { get; private set; }
    public InputAction SkillAction { get; private set; }
    public InputAction AttackAction { get; private set; }
    public InputAction CancelAction { get; private set; }
    public InputAction PauseAction { get; private set; }
    public InputAction InteractAction { get; set; }
    private void Awake()
    {
        input = GetComponent<PlayerInput>();

        MoveAction = input.actions.FindAction("Move");
        DashAction = input.actions.FindAction("Dash");
        SkillAction = input.actions.FindAction("Skill");
        AttackAction = input.actions.FindAction("Attack");
        CancelAction = input.actions.FindAction("Cancel");
        PauseAction = input.actions.FindAction("Pause");
        InteractAction = input.actions.FindAction("Interact");
    }

    public void OnEnable()
    {
        MoveAction.Enable();
        DashAction.Enable();
        SkillAction.Enable();
        AttackAction.Enable();
        CancelAction.Enable();
        PauseAction.Enable();
        InteractAction.Enable();
    }
    
    public void OnDisable()
    {
        MoveAction.Disable();
        DashAction.Disable();
        SkillAction.Disable();
        AttackAction.Disable();
        CancelAction.Disable();
        PauseAction.Disable();
        InteractAction.Disable();
    }
}
