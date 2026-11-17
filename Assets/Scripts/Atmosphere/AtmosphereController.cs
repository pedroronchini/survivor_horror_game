using System.Collections;
using UnityEngine;

public class AtmosphereController : MonoBehaviour
{
    [Header("Perfis")]
    [Tooltip("Atmosfera da rua, usada no início e ao sair de qualquer zona.")]
    [SerializeField] private AtmosphereProfile outdoorProfile;

    [Header("Transição")]
    [Tooltip("Segundos para trocar de uma atmosfera para outra.")]
    [SerializeField, Min(0f)] private float transitionDuration = 1f;

    private AtmosphereProfile currentProfile;
    private Coroutine transitionRoutine;

    private void Awake()
    {
        if (outdoorProfile == null) return;

        currentProfile = outdoorProfile;

        ApplyFog(currentProfile.FogColor, currentProfile.FogDensity);
    }

    public void EnterZone(AtmosphereProfile profile)
    {
        TransitionTo(profile);
    }

    public void ExitZone(AtmosphereProfile profile)
    {
        if (profile != currentProfile) return;

        TransitionTo(outdoorProfile);
    }

    private void TransitionTo(AtmosphereProfile target)
    {
        if (target == null) return;

        if (target == currentProfile) return;

        currentProfile = target;

        if (transitionRoutine != null) StopCoroutine(transitionRoutine);

        transitionRoutine = StartCoroutine(FadeTo(target));
    }

    private IEnumerator FadeTo(AtmosphereProfile target)
    {
        Color initialColor = RenderSettings.fogColor;
        float initialDensity = RenderSettings.fogDensity;
        float elapsed = 0f;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / transitionDuration;

            ApplyFog(Color.Lerp(initialColor, target.FogColor, t), Mathf.Lerp(initialDensity, target.FogDensity, t));

            yield return null;
        }

        ApplyFog(target.FogColor, target.FogDensity);

        transitionRoutine = null;
    }

    private void ApplyFog(Color color, float density)
    {
        RenderSettings.fogColor = color;
        RenderSettings.fogDensity = density;
    }
}
