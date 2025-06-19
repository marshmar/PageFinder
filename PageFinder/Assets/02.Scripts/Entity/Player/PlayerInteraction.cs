using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    #region Variables
    private const float _interactableRange       = 2.0f;
    private float _inkElapsedTime                = 0f;
    private bool _isInteractable                 = true;
    private Collider[] _interactableObjColls     = new Collider[3];
    private int _interactableLayer;

    // Hashing
    private Player _player;
    private Action<InputAction.CallbackContext> _cachedAction;
    private InputAction _interactAction;
    private IInteractable _previousInteractable;
    #endregion

    #region Properties
    public bool IsInteractable { get => _isInteractable; set => _isInteractable = value; }

    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        _player = this.GetComponentSafe<Player>();
        _interactableLayer = LayerMask.GetMask("INTERACTABLEOBJECT");
    }

    private void Start()
    {
        InitializeInteractAction();
    }

    private void Update()
    {
        if (FindInteractableObjects())
        {
            _isInteractable = true;
            SetIconInteractable(true);
            AddInteractAction();
        }
        else
        {
            _isInteractable = false;
            SetIconInteractable(false);

            if (_interactAction != null)
                _interactAction.canceled -= _cachedAction;
        }
    }

    private void OnDestroy()
    {
        _interactAction.canceled -= _cachedAction;
    }
    #endregion

    #region Initialization

    private void InitializeInteractAction()
    {
        _interactAction = _player.InputAction.GetInputAction(PlayerInputActionType.Interact);
        if (_interactAction == null)
        {
            Debug.LogError("Interact Action is null");
            return;
        }
    }


    #endregion

    #region Actions
    private void SetIconInteractable(bool active)
    {
        _player.UI.SetInteractButton(active);
    }

    private void AddInteractAction()
    {
        IInteractable interactableObj = _interactableObjColls[0].GetComponent<IInteractable>();
        if (_previousInteractable == interactableObj) return;
        _previousInteractable = interactableObj;

        _cachedAction = interactableObj.InteractAction();
        if (_cachedAction == null) return;
        _interactAction.canceled += _cachedAction;
    }

    private bool FindInteractableObjects()
    {
        int count = Physics.OverlapSphereNonAlloc(_player.Utils.Tr.position, _interactableRange, _interactableObjColls, _interactableLayer);
        return count > 0;
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
