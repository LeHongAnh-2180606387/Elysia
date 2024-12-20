public class ShopModel : CategoryInventoryModel
{
    public ShopModel(ShopSO shopSO, ItemSlotView prefab)
    {
       
        foreach (InventoryItemConfig item in shopSO.Items)
        {
            Items.Add(new InventoryItem(new Item(item.ItemSO), item.Amount));
        }

        this.slotViewPrefab = prefab;
    }
}
