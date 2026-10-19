using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform cameraTarget;

    [Header("Movimento")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float aimMoveSpeed = 1.2f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float runSpeed = 6f;

    [Header("Multiplicadores de direção")]
    [SerializeField, Range(0f, 1f)] private float strafeMultiplier = 0.75f; // andar de lado
    [SerializeField, Range(0f, 1f)] private float backwardMultiplier = 0.6f; // andar de costas
    

    private CharacterController characterController;
    private Transform cameraTransform;
    private PlayerAiming playerAiming;
    private float verticalVelocity;
    private InputAction moveAction;
    private InputAction sprintAction;

    private void Awake()
    {
        moveAction = InputSystem.actions.FindAction("Player/Move", throwIfNotFound: true);
        sprintAction = InputSystem.actions.FindAction("Player/Sprint", throwIfNotFound: true);
        characterController = GetComponent<CharacterController>();
        playerAiming = GetComponent<PlayerAiming>();

        if (mainCamera == null && Camera.main != null)
        {
            mainCamera = Camera.main;
        }

        if (mainCamera != null)
        {
            cameraTransform = mainCamera.transform;
        }
    }

    private void Update()
    {
        if (cameraTransform == null) return;

        // Input (WASD ou analógico esquerdo); ClampMagnitude mantém a intensidade do analógico
        Vector2 input = moveAction.ReadValue<Vector2>();
        Vector3 inputDir = Vector3.ClampMagnitude(new Vector3(input.x, 0f, input.y), 1f);
        bool hasInput = inputDir.sqrMagnitude > 0.01f;
        
        float cameraYaw = cameraTransform.eulerAngles.y;

        bool isAiming = playerAiming != null && playerAiming.isAiming;
        bool isSprinting = sprintAction.IsPressed() && !isAiming && inputDir.z > 0.5f;

        float targetYaw = cameraYaw;

        if (isSprinting)
        {
            targetYaw += Mathf.Atan2(inputDir.x, inputDir.z) * Mathf.Rad2Deg;
        }

        // O corpo sempre acompanha a direção da câmera (estilo câmera de ombro fixa) quando não está correndo
        float rotationStep = 1f - Mathf.Exp(-rotationSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.Euler(0f, targetYaw, 0f),
            rotationStep
        );

        // --- Movimento horizontal (relativo à câmera, tipo strafe) ---
        Vector3 horizontal = Vector3.zero;
        if (hasInput)
        {
            float speed = GetCurrentSpeed(isAiming, isSprinting);

            if (!isSprinting)
            {
                speed *= GetDirectionMultiplier(inputDir);
            }

            horizontal = Quaternion.Euler(0f, cameraYaw, 0f) * inputDir * speed;
        }

        // --- Gravidade ---
        if (characterController.isGrounded && verticalVelocity < 0f)
            verticalVelocity = -2f;
        else
            verticalVelocity += Physics.gravity.y * Time.deltaTime;

        // --- Aplicação (uma única chamada de Move) ---
        Vector3 motion = horizontal;
        motion.y = verticalVelocity;

        characterController.Move(motion * Time.deltaTime);

        if (cameraTarget != null)
        {
            cameraTarget.rotation = Quaternion.Euler(0f, cameraYaw, 0f);
        }
        
    }

    private float GetDirectionMultiplier(Vector3 direction)
    {
        float forwardAmount = direction.normalized.z;

        if (forwardAmount >= 0f)
            return Mathf.Lerp(strafeMultiplier, 1f, forwardAmount);

        return Mathf.Lerp(strafeMultiplier, backwardMultiplier, -forwardAmount);
    }

    private float GetCurrentSpeed(bool isAiming, bool isSprinting)
    {
        if (isAiming)
        {
            return aimMoveSpeed;
        }

        if (isSprinting)
        {
            return runSpeed;
        }

        return moveSpeed;
    }
}