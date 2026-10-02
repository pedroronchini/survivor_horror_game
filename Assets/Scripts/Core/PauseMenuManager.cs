using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenuManager : MonoBehaviour
{

    [SerializeField] private GameObject painel;

    private Keyboard keyboard;


    // Update is called once per frame
    void Update()
    {
        keyboard = Keyboard.current;

        if (keyboard.tabKey.wasPressedThisFrame)
        {
            if (painel.activeSelf)
            {
                painel.SetActive(false);
                Time.timeScale = 1;
            } else
            {
                painel.SetActive(true);
                Time.timeScale = 0;
            }
        }
    }
}
