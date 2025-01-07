using UnityEngine;

public class InventoryController : CategoryInventoryController
{
    private ItemManagementSystemService itemManagementSystemService;
    public InventoryController(EventService eventService, UIService uiService, 
        InventoryModel inventoryModel, InventoryView inventoryView, ItemManagementSystemService itemManagementSystemService)
    {
        this.eventService = eventService;
        this.uiService = uiService;
        this.itemManagementSystemService = itemManagementSystemService;
        model = inventoryModel;
        this.eventService.OnItemSlotClickedOnInventory.AddListener(OnSlotSelected);

        GetModel().SetController(this);
        view = GameObject.Instantiate<InventoryView>(inventoryView, uiService.UIRoot);

        GetView().SetController(this);
        SetupInventory();
        GetView().UpdateInventoryWeightText(GetModel().CurrentWeight, GetModel().MaxWeight);
        GetView().UpdateValueText(GetModel().CurrentValue);

        UpdateDetailsPanel();
    }

    private void SetupInventory()
    {
        foreach (var item in model.Items)
        {
            ItemSlotModel slotModel = new ItemSlotModel(item);
            ItemSlotController itemSlotController = new ItemSlotController(model.slotViewPrefab, slotModel, view.ItemGridRoot, eventService, false);

            switch (item.Item.Type)
            {
                case ItemType.None:
                    break;
                case ItemType.Materials:
                    materialSlotItems.Add(itemSlotController);
                    break;
                case ItemType.Equipment:
                    equipmentSlotItems.Add(itemSlotController);
                    break;
                case ItemType.Consumables:
                    consumableSlotItems.Add(itemSlotController);
                    break;
                case ItemType.Treasure:
                    treasureSlotItems.Add(itemSlotController);
                    break;
            }
        }
    }

    ~InventoryController()
    {
        this.eventService.OnItemSlotClickedOnInventory.RemoveListener(OnSlotSelected);
    }

    private InventoryModel GetModel() => ((InventoryModel)model);

    private InventoryView GetView() => (InventoryView)view;

    public float GetRemainingWeight() => GetModel().MaxWeight - GetModel().CurrentWeight;

    protected override void UpdateDetailsPanel()
    {
        if (selectedSlot == null)
        {
            view.HideDescriptionPanel();
            return;
        }

        var item = selectedSlot.GetItem();

        var inventoryItem = model.GetInventoryItem(item);

        view.SetupDescriptionPanel(item.Icon, item.Name, item.Description, item.Weight * chosenAmount, chosenAmount, item.SellingPrice * chosenAmount, inventoryItem.Amount);
    }

    public int GetInventoryValue() => GetModel().CurrentValue;

    public override void AddItem(Item item, int amount, bool isShop)
    {
        base.AddItem(item, amount, isShop);

        GetModel().CalculateCurrentWeight();
        GetModel().CalculateInventoryValue();
        GetView().UpdateInventoryWeightText(GetModel().CurrentWeight, GetModel().MaxWeight);
        GetView().UpdateValueText(GetModel().CurrentValue);
    }

    public override void RemoveItem(Item item, int amount)
    {
        base.RemoveItem(item, amount);
        GetModel().CalculateCurrentWeight();
        GetModel().CalculateInventoryValue();
        GetView().UpdateInventoryWeightText(GetModel().CurrentWeight, GetModel().MaxWeight);
        GetView().UpdateValueText(GetModel().CurrentValue);
    }

    public void TrySellSelectedItem()
    {
        if(selectedSlot == null)
        {
            return;
        }

        itemManagementSystemService.SellItem(selectedSlot.GetItem(), chosenAmount);
        GetView().PlayCoinsSound();
    }
}
