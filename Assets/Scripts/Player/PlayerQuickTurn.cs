using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerQuickTurn : MonoBehaviour
{
    [SerializeField] private CinemachinePanTilt panTilt;
    [SerializeField] private float turnDuration = 0.25f;
    [Tooltip("Quanto o input precisa estar para trás para permitir o giro de 180°")]
    [SerializeField, Range(0f, 1f)] private float backInputThreshold = 0.5f;

    private const float TurnAngle = 180f;
    private const float FullCircle = 360f;

    private InputAction moveAction;
    private InputAction sprintAction;
    private PlayerAiming playerAiming;
    private bool isTurning;
    private float turnedSoFar;

    private void Awake()
    {
        moveAction = InputSystem.actions.FindAction("Player/Move", throwIfNotFound: true);
        sprintAction = InputSystem.actions.FindAction("Player/Sprint", throwIfNotFound: true);
        playerAiming = GetComponent<PlayerAiming>();

        if (panTilt == null)
            panTilt = FindAnyObjectByType<CinemachinePanTilt>();
    }

    private void Update()
    {
        if (panTilt == null) return;

        Vector2 input = moveAction.ReadValue<Vector2>();

        bool isAiming = playerAiming != null && playerAiming.isAiming;
        bool sprintPressed = sprintAction.WasPressedThisFrame();

        if (input.y < -backInputThreshold && sprintPressed && !isAiming && !isTurning)
        {
            isTurning = true;
            turnedSoFar = 0f;
        }

        if (isTurning)
        {
            float step = Mathf.Min(TurnAngle / turnDuration * Time.deltaTime, TurnAngle - turnedSoFar);

            panTilt.PanAxis.Value += step;

            if (panTilt.PanAxis.Value > panTilt.PanAxis.Range.y)
                panTilt.PanAxis.Value -= FullCircle;

            turnedSoFar += step;
            isTurning = turnedSoFar < TurnAngle;
        }
    }
}
