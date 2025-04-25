using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    private PlayerInputAction input;
    private PlayerUI playerUI;
    private PlayerUtils playerUtils;

    private Collider[] interactableObjColls = new Collider[3];
    private float interactableRange = 2.0f;
    private int interactableLayer = 1 << 12;
    private bool isInteractable;
    private float inkElapsedTime = 0f;
    private Action<InputAction.CallbackContext> cachedAction;

    public bool IsInteractable { get => isInteractable; set => isInteractable = value; }

    private void Awake()
    {
        playerUtils = DebugUtils.GetComponentWithErrorLogging<PlayerUtils>(this.gameObject, "PlayerUtils");
        playerUI = DebugUtils.GetComponentWithErrorLogging<PlayerUI>(this.gameObject, "PlayerUI");
        input = DebugUtils.GetComponentWithErrorLogging<PlayerInputAction>(this.gameObject, "PlayerInputAction");
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
            input.InteractAction.canceled -= cachedAction;
        }
    }

    private void SetIconInteractable(bool active)
    {
        playerUI.SetInteractButton(active);
    }

    private void AddInteractAction()
    {
        if (input is null)
        {
            Debug.LogError("PlayerInput 컴포넌트가 존재하지 않습니다.");
            return;
        }

        if (input.InteractAction is null)
        {
            Debug.LogError("Interact Action이 존재하지 않습니다.");
            return;
        }

        IInteractable interactableObj = interactableObjColls[0].GetComponent<IInteractable>();
        cachedAction = interactableObj.InteractAction();
        input.InteractAction.canceled += cachedAction;
    }

    private bool FindInteractableObjects()
    {
        int count = Physics.OverlapSphereNonAlloc(playerUtils.Tr.position, interactableRange, interactableObjColls, interactableLayer);
        return count > 0;
    }
}
