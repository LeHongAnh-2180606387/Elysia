using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory Config", menuName = "Configs/InventoryConfig")] 
public class InventorySO: ScriptableObject
{
    public List<InventoryItemConfig> Items;
    public float MaxWeight;
    public ItemSlotView SlotViewPrefab;
}