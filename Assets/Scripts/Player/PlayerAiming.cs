using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerAiming : MonoBehaviour
{
    [Header("Câmera")]
    [SerializeField] private CinemachineCamera playerCamera;

    [Header("Ajustes ao mirar")]
    [SerializeField] private float normalCameraDistance = 1.6f;
    [SerializeField] private float aimCameraDistance = 1.6f;
    [SerializeField] private float normalFOV = 60f;
    [SerializeField] private float aimFOV = 50f;
    [SerializeField] private float aimTransitionSpeed = 8f;
    [SerializeField] private Vector3 normalShoulderOffset = new Vector3(0.4f, 0f, 0.1f);
    [SerializeField] private Vector3 aimShoulderOffset = new Vector3(0.6f, 0f, 0.1f);

    [Header("Retícula")]
    [SerializeField] private GameObject crosshair;
    [SerializeField] private float crosshairSpreadMax = 25f;
    [SerializeField] private float crosshairSpreadMin = 7f;
    [SerializeField] private float shrinkDuration = 0.5f;
    [SerializeField] private float recoilCrosshairKick = 0.25f;

    [FormerlySerializedAs("segmentoCima")]
    [SerializeField] private RectTransform crosshairTop;
    [FormerlySerializedAs("segmentoBaixo")]
    [SerializeField] private RectTransform crosshairBottom;
    [FormerlySerializedAs("segmentoEsquerda")]
    [SerializeField] private RectTransform crosshairLeft;
    [FormerlySerializedAs("segmentoDireita")]
    [SerializeField] private RectTransform crosshairRight;

    private CinemachineThirdPersonFollow thirdPersonFollow;
    private InputAction aimAction;
    private PlayerShooting playerShooting;
    private float aimTimer;

    public bool IsAiming { get; private set; }

    private void Awake()
    {
        aimAction = InputSystem.actions.FindAction("Player/Aim", throwIfNotFound: true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (playerCamera != null)
            thirdPersonFollow = playerCamera.GetComponent<CinemachineThirdPersonFollow>();


        playerShooting = GetComponent<PlayerShooting>();
    }

    private void OnEnable()
    {
        if (playerShooting == null) return;

        playerShooting.Fired += HandleFired;
    }

    private void Update()
    {
        UpdateAimState();
        UpdateCamera();
    }

    private void OnDisable()
    {
        if (playerShooting == null) return;

        playerShooting.Fired -= HandleFired;
    }

    private void UpdateAimState()
    {
        if (aimAction.IsPressed())
        {
            IsAiming = true;
            SetCrosshairVisible(true);

            aimTimer = Mathf.Min(aimTimer + Time.deltaTime, shrinkDuration);
            float progress = Mathf.Clamp01(aimTimer / shrinkDuration);
            float currentDistance = Mathf.Lerp(crosshairSpreadMax, crosshairSpreadMin, progress);

            SetCrosshairSpread(currentDistance);
        }
        else
        {
            IsAiming = false;
            SetCrosshairVisible(false);
            aimTimer = 0f;
        }
    }

    private void UpdateCamera()
    {
        // Transição suave da câmera entre normal e mirando
        if (playerCamera != null)
        {
            float targetFOV = IsAiming ? aimFOV : normalFOV;
            var lens = playerCamera.Lens;
            lens.FieldOfView = Mathf.Lerp(lens.FieldOfView, targetFOV, Time.deltaTime * aimTransitionSpeed);
            playerCamera.Lens = lens;
        }

        if (thirdPersonFollow != null)
        {
            float targetDistance = IsAiming ? aimCameraDistance : normalCameraDistance;
            thirdPersonFollow.CameraDistance = Mathf.Lerp(thirdPersonFollow.CameraDistance, targetDistance, Time.deltaTime * aimTransitionSpeed);

            Vector3 targetOffset = IsAiming ? aimShoulderOffset : normalShoulderOffset;
            thirdPersonFollow.ShoulderOffset = Vector3.Lerp(thirdPersonFollow.ShoulderOffset, targetOffset, Time.deltaTime * aimTransitionSpeed);
        }
    }

    private void HandleFired()
    {
        aimTimer = Mathf.Max(0f, aimTimer - recoilCrosshairKick);
    }

    private void SetCrosshairVisible(bool visible)
    {
        if (crosshair == null) return;

        crosshair.SetActive(visible);
    }

    private void SetCrosshairSpread(float distance)
    {
        if (crosshairTop == null || crosshairBottom == null || crosshairLeft == null || crosshairRight == null) return;

        crosshairTop.anchoredPosition = new Vector2(0, distance);
        crosshairBottom.anchoredPosition = new Vector2(0, -distance);
        crosshairLeft.anchoredPosition = new Vector2(-distance, 0);
        crosshairRight.anchoredPosition = new Vector2(distance, 0);
    }
}