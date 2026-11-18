using UnityEngine;

public class InteractionTestTarget : MonoBehaviour, IInteractable
{
    [Header("Feedback de interação")]
    [Tooltip("Cor do objeto quando está ativado.")]
    [SerializeField] private Color activeColor = Color.green;

    private Renderer targetRenderer;
    private Color originalColor;
    private bool isActive;

    private void Awake()
    {
        targetRenderer = GetComponent<Renderer>();

        if (targetRenderer != null)
            originalColor = targetRenderer.material.color;
    }

    public void Interact()
    {
        isActive = !isActive;

        SetColor(isActive ? activeColor : originalColor);
    }

    private void SetColor(Color color)
    {
        if (targetRenderer == null) return;

        targetRenderer.material.color = color;
    }
}
