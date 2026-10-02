using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private InventoryData inventory;
    private InventorySlotUI[] inventorySlots;

    private void Awake() 
    {
        inventorySlots = GetComponentsInChildren<InventorySlotUI>();
        this.RefreshUI();
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
            inventorySlots[i].UpdateUI(inventory.slots[i]);
        }
    }
}
