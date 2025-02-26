using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInputAction : MonoBehaviour
{
    private PlayerInput input;

    public InputAction MoveAction { get; private set; }
    public InputAction DashAction { get; private set; }

    private void Awake()
    {
        input = GetComponent<PlayerInput>();

        MoveAction = input.actions.FindAction("Move");
        DashAction = input.actions.FindAction("Dash");


    }

    public void OnEnable()
    {
        MoveAction.Enable();
        DashAction.Enable();
    }

    public void OnDisable()
    {
        MoveAction.Disable();
        DashAction.Disable();
    }
}
