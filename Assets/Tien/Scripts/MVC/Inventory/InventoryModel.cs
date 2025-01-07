using System.Collections.Generic;

public class InventoryModel : CategoryInventoryModel
{
    public float CurrentWeight { get; private set; }
    public float MaxWeight { get; private set; }

    public int CurrentValue { get; private set; }

    private InventoryController controller;

    public InventoryModel(InventorySO inventorySO)
    {
        foreach (InventoryItemConfig item in inventorySO.Items)
        {
            Items.Add(new InventoryItem(new Item(item.ItemSO), item.Amount));
        }

        this.slotViewPrefab = inventorySO.SlotViewPrefab;
        this.MaxWeight = inventorySO.MaxWeight;

        CalculateCurrentWeight();
        CalculateInventoryValue();
    }

    public void CalculateCurrentWeight()
    {
        float weight = 0;

        foreach(var slot in Items)
        {
            weight += slot.Amount * slot.Item.Weight;
        }

        CurrentWeight = weight;
    }

    public void CalculateInventoryValue()
    {
        int currentValue = 0;

        foreach (var item in Items)
        {
            currentValue += item.Amount * item.Item.SellingPrice;
        }

        CurrentValue = currentValue;
    }

    public void SetController(InventoryController controller)
    {
        this.controller = controller;
    }
}
