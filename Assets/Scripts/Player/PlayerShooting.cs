using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class PlayerShooting : MonoBehaviour
{
    [Header("Tiro")]
    [SerializeField] private float fireRate = 0.25f;
    [SerializeField] private float maxDistance = 100f;
    [Tooltip("Camadas que o tiro pode acertar (deixe o Player de fora)")]
    [SerializeField] private LayerMask hitMask;
    [Tooltip("Dano causado por cada tiro")]
    [SerializeField, Min(0)] private int damage = 25;

    [Header("Munição")]
    [Tooltip("Capacidade do pente")]
    [SerializeField] private int magazineSize = 15;
    [Tooltip("Balas na reserva no começo")]
    [SerializeField] private int reserveAmmo = 30;
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

    [Header("Debug")]
    [SerializeField] private bool debugDraw;

    private const float ImpactSurfaceOffset = 0.01f;
    private const float DebugLineDuration = 1f;

    private InputAction attackAction;
    private InputAction reloadAction;
    private PlayerAiming playerAiming;
    private Transform cameraTransform;
    private float nextFireTime;
    private int currentAmmo; // Balas no pente agora
    private bool isReloading;
    private float reloadEndTime;

    public int CurrentAmmo => currentAmmo;
    public int ReserveAmmo => reserveAmmo;
    public bool IsReloading => isReloading;

    public event Action Fired;
    public event Action DryFired;
    public event Action ReloadStarted;
    public event Action ReloadCancelled;
    public event Action ReloadFinished;

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

        bool isAiming = playerAiming != null && playerAiming.IsAiming;

        HandleReload(isAiming);
        HandleFire(isAiming);
    }

    private void HandleReload(bool isAiming)
    {
        if (reloadAction.WasPressedThisFrame() && CanStartReload(isAiming))
            StartReload();

        if (isReloading && !isAiming)
            CancelReload();

        if (isReloading && Time.time >= reloadEndTime)
            FinishReload();
    }

    private bool CanStartReload(bool isAiming)
    {
        return isAiming && !isReloading && currentAmmo < magazineSize && reserveAmmo > 0;
    }

    private void StartReload()
    {
        isReloading = true;
        reloadEndTime = Time.time + reloadDuration;

        ReloadStarted?.Invoke();
    }

    private void CancelReload()
    {
        isReloading = false;
        ReloadCancelled?.Invoke();
    }

    private void FinishReload()
    {
        int missingBullets = magazineSize - currentAmmo;
        int transferBullets = Mathf.Min(missingBullets, reserveAmmo);

        reserveAmmo -= transferBullets;
        currentAmmo += transferBullets;
        isReloading = false;

        ReloadFinished?.Invoke();
    }

    private void HandleFire(bool isAiming)
    {
        if (!isAiming || !attackAction.WasPressedThisFrame() || Time.time < nextFireTime || isReloading) return;

        if (currentAmmo > 0)
        {
            Shoot();
            ApplyCameraRecoil();

            currentAmmo--;

            Fired?.Invoke();
        }
        else
        {
            DryFired?.Invoke();
        }

        nextFireTime = Time.time + fireRate;
    }

    private void Shoot()
    {
        SpawnMuzzleFlash();
        CastShot();
    }

    private void SpawnMuzzleFlash()
    {
        if (muzzleFlashPrefab == null || muzzlePoint == null) return;

        GameObject flash = Instantiate(muzzleFlashPrefab, muzzlePoint.position, muzzlePoint.rotation, muzzlePoint);

        Destroy(flash, flashDuration);
    }

    private void CastShot()
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, hitMask))
        {
            SpawnImpact(hit);
            ApplyDamage(hit);

            if (debugDraw)
            {
                Debug.Log(hit.collider.gameObject.name);
                Debug.DrawLine(cameraTransform.position, hit.point, Color.red, DebugLineDuration);
            }
        }
        else
        {
            // Não acertou nada
            if (debugDraw)
                Debug.DrawLine(cameraTransform.position, cameraTransform.position + cameraTransform.forward * maxDistance, Color.yellow, DebugLineDuration);
        }
    }

    private void SpawnImpact(RaycastHit hit)
    {
        if (impactPrefab == null) return;

        GameObject impactInstance = Instantiate(impactPrefab, hit.point + hit.normal * ImpactSurfaceOffset, Quaternion.LookRotation(-hit.normal));

        impactInstance.transform.SetParent(hit.collider.transform, true);

        Destroy(impactInstance, impactLifetime);
    }

    private void ApplyDamage(RaycastHit hit)
    {
        IDamageable target = hit.collider.GetComponentInParent<IDamageable>();

        if (target == null) return;

        target.TakeDamage(damage);
    }

    private void ApplyCameraRecoil()
    {
        if (panTilt == null) return;

        panTilt.TiltAxis.Value = Mathf.Clamp(panTilt.TiltAxis.Value - recoilPitch, panTilt.TiltAxis.Range.x, panTilt.TiltAxis.Range.y);

        panTilt.PanAxis.Value += Random.Range(-recoilYaw, recoilYaw);
    }
}
