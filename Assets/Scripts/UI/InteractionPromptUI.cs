using UnityEngine;

public class InteractionPromptUI : MonoBehaviour
{
    [Header("Referências")]
    [Tooltip("Ícone mostrado quando há algo para interagir à frente do jogador.")]
    [SerializeField] private GameObject promptIcon;
    [Tooltip("Se vazio, é buscado na cena.")]
    [SerializeField] private PlayerInteraction playerInteraction;

    private void Awake()
    {
        if (playerInteraction == null)
            playerInteraction = FindAnyObjectByType<PlayerInteraction>();

        SetIconVisible(false);
    }

    private void OnEnable()
    {
        if (playerInteraction == null) return;

        playerInteraction.FocusChanged += HandleFocusChanged;
    }

    private void OnDisable()
    {
        if (playerInteraction == null) return;

        playerInteraction.FocusChanged -= HandleFocusChanged;
    }

    private void HandleFocusChanged(IInteractable interactable) => SetIconVisible(interactable != null);

    private void SetIconVisible(bool isVisible)
    {
        if (promptIcon == null) return;

        promptIcon.SetActive(isVisible);
    }
}
