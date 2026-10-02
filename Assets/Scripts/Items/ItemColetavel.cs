using UnityEngine;

public class ItemColetavel : MonoBehaviour, IInteractable
{
    public ItemData itemData;
    public InventoryData inventory;
    private bool itemHasAdded;

    public void Interact()
    {
        itemHasAdded = inventory.AdicionarItem(itemData);

        if (itemHasAdded)
        {
            Destroy(gameObject);
        } else
        {
            Debug.Log("Inventário Cheio!");
        }
        
    }
}
