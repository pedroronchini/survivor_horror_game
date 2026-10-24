using Unity.Cinemachine;
using UnityEngine;

public class CameraSensitivity : MonoBehaviour
{
    [SerializeField] private float sensitivity = 0.4f;

    private CinemachineInputAxisController axisController;

    private void Awake()
    {
        axisController = GetComponent<CinemachineInputAxisController>();

        foreach (var axis in axisController.Controllers)
        {
            // Preserva o sinal do Gain (o eixo Y usa -1 para não inverter a câmera)
            axis.Input.Gain = Mathf.Sign(axis.Input.Gain) * sensitivity;
        }
    }
}
