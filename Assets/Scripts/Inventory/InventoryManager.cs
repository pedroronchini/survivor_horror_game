using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public InventoryData inventory;

    private void Awake() {
        inventory.InicializarSlots();
    }
}
