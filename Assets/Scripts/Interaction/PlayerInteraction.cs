using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Detecção")]
    [Tooltip("Camadas dos objetos com que o jogador pode interagir.")]
    [SerializeField] private LayerMask interactableMask;
    [Tooltip("Raio da esfera de detecção.")]
    [SerializeField, Min(0f)] private float detectionRadius = 0.8f;
    [Tooltip("Distância do centro da esfera à frente do personagem.")]
    [SerializeField, Min(0f)] private float detectionForwardOffset = 0.7f;
    [Tooltip("Altura do centro da esfera acima do pivô do personagem (o pivô fica no meio do corpo).")]
    [SerializeField, Min(0f)] private float detectionHeight = 0f;

    private const int MaxCandidates = 8; // no máximo 8 objetos considerados por busca.

    private readonly Collider[] hitBuffer = new Collider[MaxCandidates];
    private InputAction interactAction;
    private PlayerAiming playerAiming;
    private IInteractable currentInteractable;

    public event Action<IInteractable> FocusChanged;

    private void Awake()
    {
        interactAction = InputSystem.actions.FindAction("Player/Interact", throwIfNotFound: true);
        playerAiming = GetComponent<PlayerAiming>();
    }

    private void Update()
    {
        UpdateFocus();
        HandleInteractInput();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(GetDetectionCenter(), detectionRadius);
    }

    private void UpdateFocus()
    {
        if (playerAiming != null && playerAiming.IsAiming)
        {
            SetFocus(null);
            return;
        }

        SetFocus(FindClosestInteractable());
    }

    private IInteractable FindClosestInteractable()
    {
        int count = Physics.OverlapSphereNonAlloc(GetDetectionCenter(), detectionRadius, hitBuffer, interactableMask, QueryTriggerInteraction.Collide);
        IInteractable closest = null;
        float closestDistance = float.MaxValue;

        for (int i = 0; i < count; i++)
        {
            if (!hitBuffer[i].TryGetComponent(out IInteractable interactable)) continue;

            float distance = (hitBuffer[i].transform.position - transform.position).sqrMagnitude;

            if (distance < closestDistance)
            {
                closest = interactable;
                closestDistance = distance;
            }
        }

        return closest;
    }

    private Vector3 GetDetectionCenter() => transform.position + transform.forward * detectionForwardOffset + Vector3.up * detectionHeight;

    private void SetFocus(IInteractable newFocus)
    {
        if (newFocus == currentInteractable) return;

        currentInteractable = newFocus;

        FocusChanged?.Invoke(currentInteractable);
    }

    private void HandleInteractInput()
    {
        if (!interactAction.WasPressedThisFrame()) return;

        if (currentInteractable == null) return;

        currentInteractable.Interact();
    }

}
