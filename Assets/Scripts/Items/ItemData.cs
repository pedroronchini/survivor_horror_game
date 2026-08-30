using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
  public string itemName;
  public string description;
  public bool isStackable;
  public int maxStackSize;
  public Sprite icon;
}

