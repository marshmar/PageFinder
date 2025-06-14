using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    private Player player;
    private PlayerInputAction input;
    private PlayerUI playerUI;
    private PlayerUtils playerUtils;

    private Collider[] interactableObjColls = new Collider[3];
    private float interactableRange = 2.0f;
    private int interactableLayer = 1 << 12;
    private bool isInteractable;
    private float inkElapsedTime = 0f;
    private Action<InputAction.CallbackContext> cachedAction;
    private InputAction interactAction;

    public bool IsInteractable { get => isInteractable; set => isInteractable = value; }

    private void Awake()
    {
        playerUtils = DebugUtils.GetComponentWithErrorLogging<PlayerUtils>(this.gameObject, "PlayerUtils");
        playerUI = DebugUtils.GetComponentWithErrorLogging<PlayerUI>(this.gameObject, "PlayerUI");
        input = DebugUtils.GetComponentWithErrorLogging<PlayerInputAction>(this.gameObject, "PlayerInputAction");
        player = this.GetComponentSafe<Player>();
    }

    private void Start()
    {
        InitializeInteractAction();
    }



    // Update is called once per frame
    void Update()
    {
        if (FindInteractableObjects())
        {
            isInteractable = true;
            SetIconInteractable(true);
            AddInteractAction();
        }
        else
        {
            isInteractable = false;
            SetIconInteractable(false);

            if(interactAction != null)
                interactAction.canceled -= cachedAction;
        }
    }

    private void SetIconInteractable(bool active)
    {
        playerUI.SetInteractButton(active);
    }

    private void InitializeInteractAction()
    {
        interactAction = player.InputAction.GetInputAction(PlayerInputActionType.Interact);
        if (interactAction == null)
        {
            Debug.LogError("Interact Action is null");
            return;
        }
    }

    private void AddInteractAction()
    {
        IInteractable interactableObj = interactableObjColls[0].GetComponent<IInteractable>();
        cachedAction = interactableObj.InteractAction();
        interactAction.canceled += cachedAction;
    }

    private bool FindInteractableObjects()
    {
        int count = Physics.OverlapSphereNonAlloc(playerUtils.Tr.position, interactableRange, interactableObjColls, interactableLayer);
        return count > 0;
    }
}
