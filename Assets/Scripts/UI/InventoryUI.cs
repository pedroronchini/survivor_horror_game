using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private InventoryData inventory;
    private InventorySlotUI[] inventorySlots;

    private void Awake() 
    {
        inventorySlots = GetComponentsInChildren<InventorySlotUI>();
    }

    private void OnEnable() 
    {
        this.RefreshUI();
        inventory.hasChanged += this.RefreshUI;
    }

    private void OnDisable() 
    {
        inventory.hasChanged -= this.RefreshUI;
    }

    private void RefreshUI()
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            // A UI pode ter mais slots que o inventário atual (ex.: após upgrade de capacidade)
            var slot = i < inventory.slots.Count ? inventory.slots[i] : null;
            inventorySlots[i].UpdateUI(slot);
        }
    }
}
