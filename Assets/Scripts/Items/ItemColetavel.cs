using UnityEngine;

public class ItemColetavel : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        Debug.Log("Coletou: " + gameObject.name);

        Destroy(gameObject);
    }
}
