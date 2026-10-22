using TMPro;
using UnityEngine;

public class AmmoHUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private PlayerShooting playerShooting;
    [SerializeField] private PlayerAiming playerAiming;

    private void Awake()
    {
        if (playerAiming == null)
        {
            playerAiming = FindAnyObjectByType<PlayerAiming>();
        }

        if (playerShooting == null)
        {
            playerShooting = FindAnyObjectByType<PlayerShooting>();
        }
    }

    private void Update()
    {
        if (ammoText == null || playerShooting == null) return;

        bool isAiming = playerAiming != null && playerAiming.isAiming;

        ammoText.gameObject.SetActive(isAiming);

        if (!isAiming) return;

        if (playerShooting.IsReloading)
        {
            ammoText.text = "Recarregando...";
        }
        else
        {
            ammoText.text = $"{playerShooting.CurrentAmmo} / {playerShooting.ReserveAmmo}";
        }
    }
}
