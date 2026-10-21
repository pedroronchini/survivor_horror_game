using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private float fireRate = 0.25f;
    [SerializeField] private float maxDistance = 100f;

    private InputAction attackAction;
    private PlayerAiming playerAiming;
    private Transform cameraTransform;
    private float nextFireTime;

    private void Awake()
    {
        attackAction = InputSystem.actions.FindAction("Player/Attack", throwIfNotFound: true);
        playerAiming = GetComponent<PlayerAiming>();

        if (Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        if (cameraTransform == null) return;

        bool isAiming = playerAiming != null && playerAiming.isAiming;

        if (isAiming && attackAction.WasPressedThisFrame() && Time.time >= nextFireTime)
        {
            Shoot();
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
}
