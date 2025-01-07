public class ItemSlotModel
{
    public InventoryItem InventoryItem;
    
    private ItemSlotController controller;
    public ItemSlotModel(InventoryItem inventoryItem)
    {
        this.InventoryItem = inventoryItem;
    }

    public void SetController(ItemSlotController controller)
    {
        this.controller = controller;
    }
}