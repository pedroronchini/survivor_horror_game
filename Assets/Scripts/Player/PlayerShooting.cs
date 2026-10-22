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
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, ~LayerMask.GetMask("Player")))
        {
            // Acertou algo
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
}
