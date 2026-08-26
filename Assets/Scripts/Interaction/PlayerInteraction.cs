using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
   private IInteractable nextItem;

   private Keyboard keyboard;

   [SerializeField] private GameObject textoInteracao;

   private void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent<IInteractable>(out var interactable))
        {
            nextItem = interactable;
            textoInteracao.SetActive(true);
        }
   }

   private void OnTriggerExit(Collider other) {
        if (other.TryGetComponent<IInteractable>(out var interactable))
        {
            if (nextItem == interactable)
            {
                nextItem = null;
                textoInteracao.SetActive(false);
            }
            
        }
   }

   private void Update()
    {
        keyboard = Keyboard.current;

        if (keyboard.eKey.wasPressedThisFrame && nextItem != null)
        {
            nextItem.Interact();
            textoInteracao.SetActive(false);
            
            nextItem = null;
        }
    }
}
