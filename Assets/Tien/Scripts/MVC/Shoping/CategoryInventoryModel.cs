using System.Collections.Generic;

public abstract class CategoryInventoryModel
{
    public List<InventoryItem> Items = new List<InventoryItem>();
    public ItemSlotView slotViewPrefab;


    public bool HasItem(Item item)
    {
        InventoryItem desiredItem = Items.Find(i => i.Item.Name.Equals(item.Name));

        return desiredItem != null;
    }

    public InventoryItem GetInventoryItem(Item item)
    {
        InventoryItem desiredItem = Items.Find(i => i.Item.Name.Equals(item.Name));

        return desiredItem;
    }

    public int GetItemAmount(Item item)
    {
        InventoryItem desiredItem = Items.Find(i => i.Item.Name.Equals(item.Name));

        if (desiredItem != null)
        {
            return desiredItem.Amount;
        }

        return 0;
    }

    public void AddItem(Item item, int amount)
    {
        InventoryItem desiredItem = Items.Find(i => i.Item.Name.Equals(item.Name));

        if (desiredItem != null)
        {
            desiredItem.Amount += amount;
            return;
        }

        Items.Add(new InventoryItem(item, amount));
    }

    public void RemoveItem(Item item, int amount)
    {
        InventoryItem desiredItem = Items.Find(i => i.Item.Name.Equals(item.Name));

        if (desiredItem != null)
        {
            desiredItem.Amount -= amount;

            if (desiredItem.Amount <= 0)
            {
                Items.Remove(desiredItem);
            }

            return;
        }
    }
}
