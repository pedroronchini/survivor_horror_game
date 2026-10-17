using UnityEngine;
using UnityEngine.Rendering;

public class PlayerCameraFade : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform cameraTarget;

    [Tooltip("Abaixo desta distância o personagem some")]
    [SerializeField] private float limitDistance = 0.5f;

    [Tooltip("Folga extra para reaparecer, evita piscar")]
    [SerializeField] private float margin = 0.1f;

    private SkinnedMeshRenderer[] renderers;
    private bool isHidden = false;              // como o personagem ESTÁ agora

    private void Awake()
    {
        if (mainCamera == null && Camera.main != null)
        {
            mainCamera = Camera.main;
        }

        renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
    }

    private void LateUpdate()
    {
        // Proteção: sem câmera ou sem alvo, não há o que fazer
        if (mainCamera == null || cameraTarget == null) return;

        float distance = Vector3.Distance(mainCamera.transform.position, cameraTarget.position);

        // Histerese: se já está escondido, exige uma distância maior para reaparecer
        float currentLimit = isHidden ? limitDistance + margin : limitDistance;

        // Como o personagem DEVERIA estar neste frame
        bool shouldHide = distance < currentLimit;

        // Nada mudou desde o frame anterior: não faz nada
        if (shouldHide == isHidden) return;

        // Mudou: guarda o novo estado e aplica
        isHidden = shouldHide;
        SetHidden(isHidden);
    }

    private void SetHidden(bool hide)
    {
        ShadowCastingMode mode = hide ? ShadowCastingMode.ShadowsOnly : ShadowCastingMode.On;

        foreach (SkinnedMeshRenderer render in renderers)
        {
            render.shadowCastingMode = mode;
        }
    }
}