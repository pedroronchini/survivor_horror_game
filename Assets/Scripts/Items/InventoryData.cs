using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory/InventoryData")]
public class InventoryData : ScriptableObject
{
    public List<InventorySlot> slots;
    public int currentMaxSlots = 6;
    public int absoluteMaxSlots = 10;

    public void InicializarSlots()
    {
        slots.Clear();

        for (int i = 0; i < currentMaxSlots; i++)
        {
            slots.Add(new InventorySlot());
        }
    }
}
