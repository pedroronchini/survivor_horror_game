using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAiming : MonoBehaviour
{
    public bool isAiming;

    [Header("Câmera")]
    [SerializeField] private CinemachineCamera playerCamera;
    private CinemachineThirdPersonFollow thirdPersonFollow;

    [Header("Ajustes ao mirar")]
    [SerializeField] private float normalCameraDistance = 1.6f;
    [SerializeField] private float aimCameraDistance = 1.6f;
    [SerializeField] private float normalFOV = 60f;
    [SerializeField] private float aimFOV = 50f;
    [SerializeField] private float aimTransitionSpeed = 8f;
    [SerializeField] private Vector3 normalShoulderOffset = new Vector3(0.4f, 0f, 0.1f);
    [SerializeField] private Vector3 aimShoulderOffset = new Vector3(0.6f, 0f, 0.1f);

    [SerializeField] private GameObject crosshair;

    [SerializeField] private float crosshairSpreadMax = 25f;
    [SerializeField] private float crosshairSpreadMin = 7f;
    [SerializeField] private float shrinkDuration = 0.5f;
    [SerializeField] private float recoilCrosshairKick = 0.25f;
    private float aimTimer = 0f;

    private InputAction aimAction;

    [SerializeField] private RectTransform segmentoCima;
    [SerializeField] private RectTransform segmentoBaixo;
    [SerializeField] private RectTransform segmentoEsquerda;
    [SerializeField] private RectTransform segmentoDireita;


    private void Awake()
    {
        aimAction = InputSystem.actions.FindAction("Player/Aim", throwIfNotFound: true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (playerCamera != null)
            thirdPersonFollow = playerCamera.GetComponent<CinemachineThirdPersonFollow>();
    }

    private void Update()
    {
        if (aimAction.IsPressed())
        {
            isAiming = true;
            crosshair.SetActive(true);

            aimTimer = Mathf.Min(aimTimer + Time.deltaTime, shrinkDuration);
            float progress = Mathf.Clamp01(aimTimer / shrinkDuration);
            float currentDistance = Mathf.Lerp(crosshairSpreadMax, crosshairSpreadMin, progress);

            segmentoCima.anchoredPosition = new Vector2(0, currentDistance);
            segmentoBaixo.anchoredPosition = new Vector2(0, -currentDistance);
            segmentoEsquerda.anchoredPosition = new Vector2(-currentDistance, 0);
            segmentoDireita.anchoredPosition = new Vector2(currentDistance, 0);
        }
        else
        {
            isAiming = false;
            crosshair.SetActive(false);
            aimTimer = 0f;
        }

        // Transição suave da câmera entre normal e mirando
        if (playerCamera != null)
        {
            float targetFOV = isAiming ? aimFOV : normalFOV;
            var lens = playerCamera.Lens;
            lens.FieldOfView = Mathf.Lerp(lens.FieldOfView, targetFOV, Time.deltaTime * aimTransitionSpeed);
            playerCamera.Lens = lens;
        }

        if (thirdPersonFollow != null)
        {
            float targetDistance = isAiming ? aimCameraDistance : normalCameraDistance;
            thirdPersonFollow.CameraDistance = Mathf.Lerp(thirdPersonFollow.CameraDistance, targetDistance, Time.deltaTime * aimTransitionSpeed);

            Vector3 targetOffset = isAiming ? aimShoulderOffset : normalShoulderOffset;
            thirdPersonFollow.ShoulderOffset = Vector3.Lerp(thirdPersonFollow.ShoulderOffset, targetOffset, Time.deltaTime * aimTransitionSpeed);
        }
    }

    public void ApplyRecoil()
    {
        aimTimer = Mathf.Max(0f, aimTimer - recoilCrosshairKick);
    }
}