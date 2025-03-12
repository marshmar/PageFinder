using System;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IInteractable
{
    public Action<InputAction.CallbackContext> InteractAction();
}
