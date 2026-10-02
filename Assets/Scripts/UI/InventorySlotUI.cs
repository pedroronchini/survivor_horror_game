using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textName;
    [SerializeField] private TextMeshProUGUI textQuantity;
    [SerializeField] private Image icon;

    public void UpdateUI(InventorySlot slot)
    {
        if (slot != null && slot.item != null)
        {
            textName.text = slot.item.itemName;
            textQuantity.text = slot.item.isStackable ? slot.quantity.ToString() : "";
            icon.sprite = slot.item.icon;
            icon.enabled = true;
        } else
        {
            textName.text = "";
            textQuantity.text = "";
            icon.sprite = null;
            icon.enabled = false;
        }


    }
}
