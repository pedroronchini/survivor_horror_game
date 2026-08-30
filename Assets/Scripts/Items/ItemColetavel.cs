using UnityEngine;

public class ItemColetavel : MonoBehaviour, IInteractable
{
    public ItemData itemData;
    public InventoryData inventory;

    public void Interact()
    {
        // Debug.Log("Coletou: " + gameObject.name);

        // Destroy(gameObject);

        if (itemData.isStackable)
        {
            foreach (var slot in inventory.slots)
            {
                if (slot.item == itemData && slot.quantity < itemData.maxStackSize)
                {
                    slot.quantity += 1;
                    Destroy(gameObject);
                    return;
                }
            }
        }

        foreach (var slot in inventory.slots)
        {
            if (slot.item == null)
            {
                slot.item = itemData;
                slot.quantity = 1;
                Destroy(gameObject);
                return;
            }
        }

        Debug.Log("Inventário Cheio!");
    }
}
