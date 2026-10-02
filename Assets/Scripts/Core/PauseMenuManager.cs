using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public class PauseMenuManager : MonoBehaviour
{
    public static bool IsPaused { get; private set; }

    [SerializeField] private GameObject painel;

    // O Cinemachine reativa a ação Look sozinho (AutoEnableInputs), então desligamos o controlador
    [SerializeField] private CinemachineInputAxisController cameraInput;

    private InputAction pauseAction;
    private InputActionMap playerMap;

    private void Awake()
    {
        pauseAction = InputSystem.actions.FindAction("System/Pause", throwIfNotFound: true);
        playerMap = InputSystem.actions.FindActionMap("Player", throwIfNotFound: true);

        if (cameraInput == null)
            cameraInput = FindAnyObjectByType<CinemachineInputAxisController>();
    }

    void Update()
    {
        if (pauseAction.WasPressedThisFrame())
        {
            SetPaused(!IsPaused);
        }
    }

    public void SetPaused(bool paused)
    {
        IsPaused = paused;
        painel.SetActive(paused);
        Time.timeScale = paused ? 0f : 1f;

        // Desliga todo o input de gameplay (mover, mirar, atirar, interagir) enquanto pausado
        if (paused) playerMap.Disable();
        else playerMap.Enable();

        if (cameraInput != null)
            cameraInput.enabled = !paused;

        // Libera o cursor para poder clicar no painel enquanto pausado
        Cursor.lockState = paused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = paused;
    }

    private void OnDestroy()
    {
        // Evita que o jogo continue pausado ao trocar de cena
        if (IsPaused)
        {
            IsPaused = false;
            Time.timeScale = 1f;
            playerMap.Enable();
        }
    }
}
