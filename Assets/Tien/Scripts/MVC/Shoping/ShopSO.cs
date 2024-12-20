using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Shop Config", menuName = "Configs/Shop")]
public class ShopSO : ScriptableObject
{
    public List<InventoryItemConfig> Items;
}
