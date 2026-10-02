using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory/InventoryData")]
public class InventoryData : ScriptableObject
{
    public List<InventorySlot> slots;
    public int currentMaxSlots = 6;
    public int absoluteMaxSlots = 10;
    public event Action hasChanged;

    public void InicializarSlots()
    {
        slots.Clear();

        for (int i = 0; i < currentMaxSlots; i++)
        {
            slots.Add(new InventorySlot());
        }
    }

    public bool AdicionarItem(ItemData item)
    {
        if (item.isStackable)
        {
            foreach (var slot in slots)
            {
                if (slot.item == item && slot.quantity < item.maxStackSize)
                {
                    slot.quantity += 1;
                    hasChanged?.Invoke();
                    return true;
                }
            }
        }

        foreach (var slot in slots)
        {
            if (slot.item == null)
            {
                slot.item = item;
                slot.quantity = 1;
                hasChanged?.Invoke();
                return true;
            }
        }

        return false;
    }
}
