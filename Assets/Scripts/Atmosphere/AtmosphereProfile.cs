using UnityEngine;

[CreateAssetMenu(fileName = "AtmosphereProfile", menuName = "Game/Atmosphere Profile")]
public class AtmosphereProfile : ScriptableObject
{
    [Header("Neblina")]
    [Tooltip("Cor da Neblina")]
    [SerializeField] private Color fogColor;
    [Tooltip("Densidade (modo Exponential Squared), 0,04 ≈ visibilidade de 25m")]
    [SerializeField, Min(0f)] private float fogDensity;

    public Color FogColor => fogColor;
    public float FogDensity => fogDensity;
}
