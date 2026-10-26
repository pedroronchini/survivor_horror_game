using UnityEngine;

[RequireComponent(typeof(Health))]
public class DamageTestTarget : MonoBehaviour
{
    [Header("Feedback de dano")]
    [SerializeField] private Color hitColor = Color.red;
    [SerializeField, Min(0f)] private float flashDuration = 0.1f;

    private Health health;
    private Renderer targetRenderer;
    private Color originalColor;
    private float flashEndTime;
    private bool isFlashing;

    private void Awake()
    {
        health = GetComponent<Health>();
        targetRenderer = GetComponent<Renderer>();

        if (targetRenderer != null)
            originalColor = targetRenderer.material.color;
    }

    private void OnEnable()
    {
        health.Damaged += HandleDamaged;
        health.Died += HandleDied;
    }

    private void Update()
    {
        if (isFlashing && Time.time >= flashEndTime)
        {
            SetColor(originalColor);
            isFlashing = false;
        }
    }

    private void OnDisable()
    {
        health.Damaged -= HandleDamaged;
        health.Died -= HandleDied;
    }

    private void HandleDamaged(int amount)
    {
        SetColor(hitColor);
        flashEndTime = Time.time + flashDuration;
        isFlashing = true;
    }

    private void HandleDied()
    {
        Destroy(gameObject);
    }

    private void SetColor(Color color)
    {
        if (targetRenderer == null) return;

        targetRenderer.material.color = color;
    }
}
