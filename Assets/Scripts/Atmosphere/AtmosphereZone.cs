using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class AtmosphereZone : MonoBehaviour
{
    [Header("Atmosfera")]
    [Tooltip("Atmosfera aplicada enquanto o jogador está dentro desta área.")]
    [SerializeField] private AtmosphereProfile profile;

    private const string PlayerTag = "Player";

    private AtmosphereController controller;

    private void Awake()
    {
        controller = FindAnyObjectByType<AtmosphereController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsPlayer(other)) return;

        if (controller == null) return;

        controller.EnterZone(profile);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsPlayer(other)) return;

        if (controller == null) return;

        controller.ExitZone(profile);
    }

    private bool IsPlayer(Collider other) => other.CompareTag(PlayerTag);
}
