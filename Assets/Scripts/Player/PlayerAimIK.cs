using UnityEngine;

public class PlayerAimIK : MonoBehaviour
{
    [Header("Pesos do IK")]
    [SerializeField, Range(0f, 1f)] private float bodyWeight = 0.7f;
    [SerializeField, Range(0f, 1f)] private float headWeight = 0.5f;
    [Tooltip("Limita o ângulo máximo de inclinação (0 = sem limite, 1 = travado)")]
    [SerializeField, Range(0f, 1f)] private float clampWeight = 0.5f;

    [Header("Ajustes")]
    [Tooltip("Velocidade de entrada e saída do IK ao começar ou parar de mirar")]
    [SerializeField, Min(0f)] private float blendSpeed = 6f;
    [Tooltip("Distância do ponto de mira à frente da câmera")]
    [SerializeField, Min(0f)] private float lookDistance = 20f;

    private Animator animator;
    private PlayerAiming playerAiming;
    private Transform cameraTransform;
    private float currentWeight;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerAiming = GetComponentInParent<PlayerAiming>();

        if (Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (animator == null || cameraTransform == null) return;

        bool isAiming = playerAiming != null && playerAiming.IsAiming;
        float target = isAiming ? 1f : 0f;

        currentWeight = Mathf.MoveTowards(currentWeight, target, blendSpeed * Time.deltaTime);

        Vector3 lookAtPosition = cameraTransform.position + cameraTransform.forward * lookDistance;

        animator.SetLookAtPosition(lookAtPosition);

        animator.SetLookAtWeight(currentWeight, bodyWeight, headWeight, 0f, clampWeight); // sem olhos no modelo
    }
}
