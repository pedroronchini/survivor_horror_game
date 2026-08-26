using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;

public class PlayerAiming : MonoBehaviour
{
    public bool isAiming;

    [SerializeField] private CinemachineCamera aimCamera;

    [SerializeField] private GameObject crosshair;

    [SerializeField] private float crosshairSpreadMax = 25f; // porcentagem do tamanho da mira

    [SerializeField] private float crosshairSpreadMin = 7f; // porcentagem do tamanho da mira

    [SerializeField] private float shrinkDuration = 0.5f;

    [SerializeField] private float aimTimer = 0f;

    private Mouse currentMouse;

    private Ray ray;

    private Transform cameraTransform;

    [SerializeField] private RectTransform segmentoCima;

    [SerializeField] private RectTransform segmentoBaixo;
    
    [SerializeField] private RectTransform segmentoEsquerda;
    
    [SerializeField] private RectTransform segmentoDireita;


    private void Awake() {
        if (Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    private void Update() {
        currentMouse = Mouse.current;

        if (currentMouse.rightButton.isPressed)
        {
            aimCamera.Priority = 15;
            isAiming = true;
            crosshair.SetActive(true);

            // Aumentando a mira conforme segura pro mais tempo
            aimTimer += Time.deltaTime;

            float progress = Mathf.Clamp01(aimTimer / shrinkDuration);

            float currentDistance = Mathf.Lerp(crosshairSpreadMax, crosshairSpreadMin, progress);

            segmentoCima.anchoredPosition = new Vector2(0, currentDistance);
            segmentoBaixo.anchoredPosition = new Vector2(0, -currentDistance);
            segmentoEsquerda.anchoredPosition = new Vector2(-currentDistance, 0);
            segmentoDireita.anchoredPosition = new Vector2(currentDistance, 0);
        } else
        {
            aimCamera.Priority = 5;
            isAiming = false;
            crosshair.SetActive(false);


            // Voltando a mira padrão
            aimTimer = 0f;
        }

        if (currentMouse.leftButton.wasPressedThisFrame && currentMouse.rightButton.isPressed)
        {
            ray = new Ray(cameraTransform.position, cameraTransform.forward);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, ~LayerMask.GetMask("Player")))
            {
                Debug.Log(hit.collider.gameObject.name);
            }
        }
    }
}
