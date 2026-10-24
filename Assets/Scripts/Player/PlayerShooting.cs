using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    [Header("Tiro")]
    [SerializeField] private float fireRate = 0.25f;
    [SerializeField] private float maxDistance = 100f;

    [Header("Munição")]
    [SerializeField] private int magazineSize = 15; // Capacidade do pente
    [SerializeField] private int reserveAmmo = 30; // Balas na reserva no começo
    [SerializeField] private float reloadDuration = 1.5f;

    [Header("Efeitos")]
    [SerializeField] private Transform muzzlePoint;
    [SerializeField] private GameObject muzzleFlashPrefab;
    [SerializeField] private GameObject impactPrefab;
    [SerializeField] private float flashDuration = 0.05f;
    [SerializeField] private float impactLifetime = 10f;

    [Header("Recuo")]
    [SerializeField] private CinemachinePanTilt panTilt;
    [SerializeField] private float recoilPitch = 1.5f;
    [SerializeField] private float recoilYaw = 0.5f;

    private InputAction attackAction;
    private PlayerAiming playerAiming;
    private Transform cameraTransform;
    private float nextFireTime;
    private int currentAmmo; // Balas no pente agora
    private InputAction reloadAction;
    private bool isReloading;
    private float reloadEndTime;

    public int CurrentAmmo => currentAmmo;
    public int ReserveAmmo => reserveAmmo;
    public bool IsReloading => isReloading;

    private void Awake()
    {
        attackAction = InputSystem.actions.FindAction("Player/Attack", throwIfNotFound: true);
        reloadAction = InputSystem.actions.FindAction("Player/Reload", throwIfNotFound: true);
        playerAiming = GetComponent<PlayerAiming>();

        if (Camera.main != null)
            cameraTransform = Camera.main.transform;

        if (panTilt == null)
            panTilt = FindAnyObjectByType<CinemachinePanTilt>();

    }

    private void Start()
    {
        currentAmmo = magazineSize;
    }

    private void Update()
    {
        if (cameraTransform == null) return;

        bool isAiming = playerAiming != null && playerAiming.isAiming;

        if (reloadAction.WasPressedThisFrame() && !isReloading && currentAmmo < magazineSize && reserveAmmo > 0)
        {
            isReloading = true;
            reloadEndTime = Time.time + reloadDuration;
            Debug.Log("Recarregando...");
        }

        if (isReloading && Time.time >= reloadEndTime)
        {
            FinishReload();
        }

        if (isAiming && attackAction.WasPressedThisFrame() && Time.time >= nextFireTime && !isReloading)
        {
            if (currentAmmo > 0)
            {
                Shoot();
                currentAmmo--;
                ApplyCameraRecoil();

                if (playerAiming != null)
                    playerAiming.ApplyRecoil();

                Debug.Log($"Pente: {currentAmmo} / Reserva: {reserveAmmo}");
            }
            else
            {
                Debug.Log("Sem munição");
            }

            nextFireTime = Time.time + fireRate;
        }


    }

    private void Shoot()
    {
        if (muzzleFlashPrefab != null && muzzlePoint != null)
        {
            GameObject flash = Instantiate(muzzleFlashPrefab, muzzlePoint.position, muzzlePoint.rotation, muzzlePoint);

            Destroy(flash, flashDuration);
        }

        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, ~LayerMask.GetMask("Player")))
        {
            // Acertou algo
            if (impactPrefab != null)
            {
                GameObject impactInstance = Instantiate(impactPrefab, hit.point + hit.normal * 0.01f, Quaternion.LookRotation(-hit.normal));

                Destroy(impactInstance, impactLifetime);
            }

            Debug.Log(hit.collider.gameObject.name);
            Debug.DrawLine(cameraTransform.position, hit.point, Color.red, 1f);
        }
        else
        {
            // Não acertou nada 
            Debug.DrawLine(cameraTransform.position, cameraTransform.position + cameraTransform.forward * maxDistance, Color.yellow, 1f);
        }
    }

    private void FinishReload()
    {
        int missingBullets = magazineSize - currentAmmo;
        int transferBullets = Mathf.Min(missingBullets, reserveAmmo);

        reserveAmmo -= transferBullets;
        currentAmmo += transferBullets;
        isReloading = false;

        Debug.Log($"Contagem nova: Pente: {currentAmmo} / Reserva: {reserveAmmo}");
    }

    private void ApplyCameraRecoil()
    {
        if (panTilt == null) return;

        panTilt.TiltAxis.Value = Mathf.Clamp(panTilt.TiltAxis.Value - recoilPitch, panTilt.TiltAxis.Range.x, panTilt.TiltAxis.Range.y);

        panTilt.PanAxis.Value = panTilt.PanAxis.Value + Random.Range(-recoilYaw, recoilYaw);
    }
}
