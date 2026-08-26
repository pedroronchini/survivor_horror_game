using UnityEngine;
using Unity.Cinemachine;

public class CameraSensitivity : MonoBehaviour
{
    [SerializeField] private float sensitivity = 0.4f;

    private CinemachineInputAxisController axisController;

    private void Awake() {
        axisController = GetComponent<CinemachineInputAxisController>();

        foreach (var axis in axisController.Controllers)
        {
            axis.Input.Gain = sensitivity;
        }
    }
}
