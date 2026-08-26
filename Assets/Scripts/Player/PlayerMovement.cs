using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Camera mainCamera;

    [Header("Movimento")]
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float aimMoveSpeed = 1.2f;
    [SerializeField] private float rotationSpeed = 10f;

    private CharacterController characterController;
    private Transform cameraTransform;
    private PlayerAiming playerAiming;
    private float verticalVelocity;

    private void Awake()
    {
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

        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        // Input
        Vector2 input = Vector2.zero;

        if (keyboard.wKey.isPressed) input.y += 1f;
        if (keyboard.aKey.isPressed) input.x -= 1f;
        if (keyboard.dKey.isPressed) input.x += 1f;
        if (keyboard.sKey.isPressed) input.y -= 1f;

        Vector3 inputDir = new Vector3(input.x, 0f, input.y).normalized;
        bool hasInput = inputDir.sqrMagnitude > 0.01f;

        float cameraYaw = cameraTransform.eulerAngles.y;
        float targetAngle = Mathf.Atan2(inputDir.x, inputDir.z) * Mathf.Rad2Deg + cameraYaw;

        bool isAiming = playerAiming != null && playerAiming.isAiming;

        if (isAiming || hasInput)
        {
            float desiredYaw = isAiming ? cameraYaw : targetAngle;

            // 1 - Exp(...) torna a interpolação independente de frame
            // Fração do angulo restante a percorrer neste frame.
            // A forma exponencial mantém o giro idêntico em qualquer framerate.
            float rotationStep = 1f - Mathf.Exp(-rotationSpeed * Time.deltaTime);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.Euler(0f, desiredYaw, 0f),
                rotationStep
            );
        }

        // --- Movimento horizontal ---
        Vector3 horizontal = Vector3.zero;
        if (hasInput)
        {
            float speed = isAiming ? aimMoveSpeed : moveSpeed;
            horizontal = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward * speed;
        }

        // --- Gravidade ---
        if (characterController.isGrounded && verticalVelocity < 0f)
            verticalVelocity = -2f;   // cola no chão, mantém isGrounded confiável
        else
            verticalVelocity += Physics.gravity.y * Time.deltaTime;

        // --- Aplicação (uma única chamada de Move) ---
        Vector3 motion = horizontal;
        motion.y = verticalVelocity;

        characterController.Move(motion * Time.deltaTime);

    }
}
