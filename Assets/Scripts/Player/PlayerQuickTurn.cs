using Unity.Cinemachine;
using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerQuickTurn : MonoBehaviour
{
    [SerializeField] private CinemachinePanTilt panTilt;
    [SerializeField] private float turnDuration = 0.25f;

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
        {
            panTilt = FindAnyObjectByType<CinemachinePanTilt>();
        }
        
    }

    private void Update()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();

        bool isAiming = playerAiming != null && playerAiming.isAiming;
        bool sprintPressed = sprintAction.WasPressedThisFrame();

        if (input.y < -0.5f)
        {
            if (sprintPressed && !isAiming && !isTurning)
            {
                isTurning = true;
                turnedSoFar = 0f;
            }
        }

        if (isTurning)
        {
            float step = 180f / turnDuration * Time.deltaTime;
            step = Mathf.Min(step, 180f - turnedSoFar);

            panTilt.PanAxis.Value += step;

            if (panTilt.PanAxis.Value > 180f)
            {
                panTilt.PanAxis.Value -= 360f;
            }

            turnedSoFar += step;
            isTurning = turnedSoFar < 180f;
        }
    }
}
