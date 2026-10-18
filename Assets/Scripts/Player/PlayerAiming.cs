using UnityEngine;
using Unity.Cinemachine;
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
    [SerializeField] private Vector3 normalShoulderOffset = new Vector3(0.4f, -1.1f, 0.1f);
    [SerializeField] private Vector3 aimShoulderOffset = new Vector3(0.6f, -1.1f, 0.1f);

    [SerializeField] private GameObject crosshair;

    [SerializeField] private float crosshairSpreadMax = 25f;
    [SerializeField] private float crosshairSpreadMin = 7f;
    [SerializeField] private float shrinkDuration = 0.5f;
    [SerializeField] private float aimTimer = 0f;

    private InputAction aimAction;
    private InputAction attackAction;
    private Ray ray;
    private Transform cameraTransform;

    [SerializeField] private RectTransform segmentoCima;
    [SerializeField] private RectTransform segmentoBaixo;
    [SerializeField] private RectTransform segmentoEsquerda;
    [SerializeField] private RectTransform segmentoDireita;

    private void Awake()
    {
        aimAction = InputSystem.actions.FindAction("Player/Aim", throwIfNotFound: true);
        attackAction = InputSystem.actions.FindAction("Player/Attack", throwIfNotFound: true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (Camera.main != null)
            cameraTransform = Camera.main.transform;

        if (playerCamera != null)
            thirdPersonFollow = playerCamera.GetComponent<CinemachineThirdPersonFollow>();
    }

    private void Update()
    {
        if (aimAction.IsPressed())
        {
            isAiming = true;
            crosshair.SetActive(true);

            aimTimer += Time.deltaTime;
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

        if (isAiming && attackAction.WasPressedThisFrame())
        {
            ray = new Ray(cameraTransform.position, cameraTransform.forward);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, ~LayerMask.GetMask("Player")))
            {
                Debug.Log(hit.collider.gameObject.name);
            }
        }
    }
}