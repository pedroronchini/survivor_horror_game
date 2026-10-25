using UnityEngine;

[RequireComponent(typeof(PlayerShooting))]
public class WeaponAudio : MonoBehaviour
{
    [Header("Fontes de áudio")]
    [SerializeField] private AudioSource shotSource;
    [SerializeField] private AudioSource reloadSource;

    [Header("Sons")]
    [SerializeField] private AudioClip fireClip;
    [SerializeField] private AudioClip dryFireClip;
    [SerializeField] private AudioClip reloadClip;

    [Header("Variação")]
    [Tooltip("Variação aleatória de tom no tiro, evita som repetitivo")]
    [SerializeField, Range(0f, 0.2f)] private float pitchVariation = 0.05f;

    private PlayerShooting playerShooting;

    private void Awake()
    {
        playerShooting = GetComponent<PlayerShooting>();
    }

    private void OnEnable()
    {
        if (playerShooting == null) return;

        playerShooting.Fired += HandleFired;
        playerShooting.DryFired += HandleDryFired;
        playerShooting.ReloadStarted += HandleReloadStarted;
        playerShooting.ReloadCancelled += HandleReloadCancelled;
    }

    private void OnDisable()
    {
        if (playerShooting == null) return;

        playerShooting.Fired -= HandleFired;
        playerShooting.DryFired -= HandleDryFired;
        playerShooting.ReloadStarted -= HandleReloadStarted;
        playerShooting.ReloadCancelled -= HandleReloadCancelled;
    }

    private void HandleFired()
    {
        if (shotSource == null) return;

        shotSource.pitch = Random.Range(1f - pitchVariation, 1f + pitchVariation);
        PlayShotSound(fireClip);
    }

    private void HandleDryFired()
    {
        if (shotSource == null) return;

        shotSource.pitch = 1f;
        PlayShotSound(dryFireClip);
    }

    private void HandleReloadStarted()
    {
        if (reloadSource == null || reloadClip == null) return;

        reloadSource.clip = reloadClip;
        reloadSource.Play();
    }

    private void HandleReloadCancelled()
    {
        if (reloadSource == null) return;

        reloadSource.Stop();
    }

    private void PlayShotSound(AudioClip clip)
    {
        if (shotSource == null || clip == null) return;

        shotSource.PlayOneShot(clip);
    }
}
