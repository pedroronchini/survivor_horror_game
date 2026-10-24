using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private GameObject interactionPrompt;

    private IInteractable currentInteractable;
    private InputAction interactAction;

    private void Awake()
    {
        interactAction = InputSystem.actions.FindAction("Player/Interact", throwIfNotFound: true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IInteractable>(out var interactable))
        {
            currentInteractable = interactable;
            SetPromptVisible(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<IInteractable>(out var interactable) && currentInteractable == interactable)
        {
            currentInteractable = null;
            SetPromptVisible(false);
        }
    }

    private void Update()
    {
        if (interactAction.WasPressedThisFrame() && currentInteractable != null)
        {
            currentInteractable.Interact();
            SetPromptVisible(false);

            currentInteractable = null;
        }
    }

    private void SetPromptVisible(bool visible)
    {
        if (interactionPrompt == null) return;

        interactionPrompt.SetActive(visible);
    }
}
