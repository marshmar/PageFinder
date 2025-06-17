using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System;

// NOTE: The name must match the action name defined in the Input System. 
public enum PlayerInputActionType
{
    Move,
    Dash,
    Skill,
    Attack,
    Cancel,
    Pause,
    Interact
}

[RequireComponent(typeof(PlayerInput))]
public class PlayerInputAction : MonoBehaviour
{
    #region Variables
    private PlayerInput _input;
    private Dictionary<PlayerInputActionType, InputAction> _actions = new Dictionary<PlayerInputActionType, InputAction>();
    #endregion

    #region Properties
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        _input = this.GetComponentSafe<PlayerInput>();
        
        InitializePlayerActions();
    }

    public void OnEnable()
    {
        EnableActions();
    }

    public void OnDisable()
    {
        DisableActions();
    }
    #endregion

    #region Initialization
    private void InitializePlayerActions()
    {
        foreach (PlayerInputActionType key in Enum.GetValues(typeof(PlayerInputActionType)))
        {
            var action = _input.actions.FindAction(key.ToString());
            if (action != null)
            {
                _actions.Add(key, action);
            }
        }
    }
    #endregion

    #region Actions
    #endregion

    #region Getter
    public InputAction GetInputAction(PlayerInputActionType inputActType)
    {
        if (_actions.TryGetValue(inputActType, out var action))
            return action;

        return null;
    }
    #endregion

    #region Setter
    #endregion

    #region Utilities
    private void EnableActions()
    {
        foreach(var action in _actions.Values)
        {
            action.Enable();
        }
    }

    private void DisableActions()
    {
        foreach (var action in _actions.Values)
        {
            action.Disable();
        }
    }
    #endregion

    #region Events
    #endregion


}
