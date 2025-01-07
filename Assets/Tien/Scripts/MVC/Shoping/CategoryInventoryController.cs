using System;
using System.Collections.Generic;

public abstract class CategoryInventoryController
{
    protected CategoryInventoryView view;
    protected CategoryInventoryModel model;

    protected UIService uiService;
    protected EventService eventService = new EventService();

    protected ItemSlotController selectedSlot;
    protected int chosenAmount = 0;

    protected List<ItemSlotController> materialSlotItems = new List<ItemSlotController>();
    protected List<ItemSlotController> equipmentSlotItems = new List<ItemSlotController>();
    protected List<ItemSlotController> consumableSlotItems = new List<ItemSlotController>();
    protected List<ItemSlotController> treasureSlotItems = new List<ItemSlotController>();

    protected void OnSlotSelected(ItemSlotController controller)
    {
        selectedSlot = controller;
        chosenAmount = 1;
        UpdateDetailsPanel();
    }

    protected virtual void UpdateDetailsPanel()
    {
        if (selectedSlot == null)
        {
            view.HideDescriptionPanel();
            return;
        }

        var item = selectedSlot.GetItem();

        var inventoryItem = model.GetInventoryItem(item);

        view.SetupDescriptionPanel(item.Icon, item.Name, item.Description, item.Weight * chosenAmount, chosenAmount, item.BuyingPrice * chosenAmount, inventoryItem.Amount);
    }

    public void DecreaseSelectedItemAmount()
    {
        var inventoryItem = model.GetInventoryItem(selectedSlot.GetItem());
        chosenAmount = Math.Max(chosenAmount - 1, 0);

        view.SetupAmountButtons(chosenAmount, inventoryItem.Amount);
        UpdateDetailsPanel();
    }

    public void IncreaseSelectedItemAmount()
    {
        var inventoryItem = model.GetInventoryItem(selectedSlot.GetItem());
        chosenAmount = Math.Min(chosenAmount + 1, inventoryItem.Amount);

        view.SetupAmountButtons(chosenAmount, inventoryItem.Amount);
        UpdateDetailsPanel();
    }

    public virtual void AddItem(Item item, int amount, bool isShop)
    {
        
        ItemSlotController itemSlotControler = null;

        if (model.HasItem(item))
        {
            switch (item.Type)
            {
                case ItemType.None:
                    break;
                case ItemType.Materials:
                    itemSlotControler = materialSlotItems.Find(i => i.GetItem().Name.Equals(item.Name));
                    break;
                case ItemType.Equipment:
                    itemSlotControler = equipmentSlotItems.Find(i => i.GetItem().Name.Equals(item.Name));
                    break;
                case ItemType.Consumables:
                    itemSlotControler = consumableSlotItems.Find(i => i.GetItem().Name.Equals(item.Name));
                    break;
                case ItemType.Treasure:
                    itemSlotControler = treasureSlotItems.Find(i => i.GetItem().Name.Equals(item.Name));
                    break;
            }
        }
        else
        {
            ItemSlotModel itemSlotModel = new ItemSlotModel(new InventoryItem(item, amount));
            itemSlotControler = new ItemSlotController(model.slotViewPrefab, itemSlotModel, view.ItemGridRoot, eventService, isShop);
            switch (item.Type)
            {
                case ItemType.None:
                    break;
                case ItemType.Materials:
                    materialSlotItems.Add(itemSlotControler);
                    break;
                case ItemType.Equipment:
                    equipmentSlotItems.Add(itemSlotControler);
                    break;
                case ItemType.Consumables:
                    consumableSlotItems.Add(itemSlotControler);
                    break;
                case ItemType.Treasure:
                    treasureSlotItems.Add(itemSlotControler);
                    break;
            }
        }
        model.AddItem(item, amount);

        itemSlotControler.ItemAdded(model.GetInventoryItem(item).Amount);

        selectedSlot = null;

        UpdateDetailsPanel();
    }

    public virtual void RemoveItem(Item item, int amount)
    {
        ItemSlotController itemSlotControler = null;

        if (model.HasItem(item))
        {
            model.RemoveItem(item, amount);

            var itemSlot = model.GetInventoryItem(item);

            int remainingAmount = 0;

            if (itemSlot != null)
            {
                remainingAmount = itemSlot.Amount;  
            }
            else
            {
                remainingAmount = 0;
            }

            

            switch (item.Type)
            {
                case ItemType.None:
                    break;
                case ItemType.Materials:
                    itemSlotControler = materialSlotItems.Find(i => i.GetItem().Name.Equals(item.Name));
                    itemSlotControler.ItemRemoved(remainingAmount);

                    if (itemSlotControler.GetItemAmout() <= 0)
                    {
                        materialSlotItems.Remove(itemSlotControler);
                    }
                    break;
                case ItemType.Equipment:
                    itemSlotControler = equipmentSlotItems.Find(i => i.GetItem().Name.Equals(item.Name));
                    itemSlotControler.ItemRemoved(remainingAmount);

                    if (itemSlotControler.GetItemAmout() <= 0)
                    {
                        equipmentSlotItems.Remove(itemSlotControler);
                    }
                    break;
                case ItemType.Consumables:
                    itemSlotControler = consumableSlotItems.Find(i => i.GetItem().Name.Equals(item.Name));
                    itemSlotControler.ItemRemoved(remainingAmount);

                    if (itemSlotControler.GetItemAmout() <= 0)
                    {
                        consumableSlotItems.Remove(itemSlotControler);
                    }
                    break;
                case ItemType.Treasure:
                    itemSlotControler = treasureSlotItems.Find(i => i.GetItem().Name.Equals(item.Name));
                    itemSlotControler.ItemRemoved(remainingAmount);

                    if (itemSlotControler.GetItemAmout() <= 0)
                    {
                        treasureSlotItems.Remove(itemSlotControler);
                    }
                    break;
            }
        }

        if(model.HasItem(item))
        {
            chosenAmount = 1;
        }
        else
        {
            selectedSlot = null;
            chosenAmount = 0;
        }

        UpdateDetailsPanel();
    }

    public void SelectAllCategory()
    {
        selectedSlot = null;
        UpdateDetailsPanel();
        ToggleConsumableCategory(true);
        ToggleEquipmentCategory(true);
        ToggleMaterialCategory(true);
        ToggleTreasureCategory(true);
    }

    public void SelectMaterialCategory()
    {
        selectedSlot = null;
        UpdateDetailsPanel();
        ToggleConsumableCategory(false);
        ToggleEquipmentCategory(false);
        ToggleMaterialCategory(true);
        ToggleTreasureCategory(false);
    }

    public void SelectEquipmentCategory()
    {
        selectedSlot = null;
        UpdateDetailsPanel();
        ToggleConsumableCategory(false);
        ToggleEquipmentCategory(true);
        ToggleMaterialCategory(false);
        ToggleTreasureCategory(false);
    }

    public void SelectConsumablesCategory()
    {
        selectedSlot = null;
        UpdateDetailsPanel();
        ToggleConsumableCategory(true);
        ToggleEquipmentCategory(false);
        ToggleMaterialCategory(false);
        ToggleTreasureCategory(false);
    }

    public void SelectTreasureCategory()
    {
        selectedSlot = null;
        UpdateDetailsPanel();
        ToggleConsumableCategory(false);
        ToggleEquipmentCategory(false);
        ToggleMaterialCategory(false);
        ToggleTreasureCategory(true);
    }

    protected void ToggleMaterialCategory(bool state)
    {
        foreach (var item in materialSlotItems)
        {
            item.ToggleView(state);
        }
    }

    protected void ToggleEquipmentCategory(bool state)
    {
        foreach (var item in equipmentSlotItems)
        {
            item.ToggleView(state);
        }
    }

    protected void ToggleConsumableCategory(bool state)
    {
        foreach (var item in consumableSlotItems)
        {
            item.ToggleView(state);
        }
    }

    protected void ToggleTreasureCategory(bool state)
    {
        foreach (var item in treasureSlotItems)
        {
            item.ToggleView(state);
        }
    }
}