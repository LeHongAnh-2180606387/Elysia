using UnityEngine;

public class ShopController : CategoryInventoryController
{
    private ItemManagementSystemService itemManagementSystemService;
    public ShopController(ShopView shopViewPrefab, ShopModel shopModel, UIService uiService, EventService eventService, ItemManagementSystemService itemManagementSystemService)
    {
        this.model = shopModel;
        this.uiService = uiService;
        this.eventService = eventService;
        this.itemManagementSystemService = itemManagementSystemService;

        this.eventService.OnItemSlotClickedOnShop.AddListener(OnSlotSelected);

        view = GameObject.Instantiate<ShopView>(shopViewPrefab, uiService.UIRoot);
        view.SetController(this);
        SetupInventory();
    }

    ~ShopController() 
    {
        this.eventService.OnItemSlotClickedOnShop.RemoveListener(OnSlotSelected);
    }

    private void SetupInventory()
    {
        foreach(var item in model.Items)
        {
            ItemSlotModel slotModel = new ItemSlotModel(item);
            ItemSlotController itemSlotController = new ItemSlotController(model.slotViewPrefab, slotModel, view.ItemGridRoot, eventService, true);

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

    private ShopView GetView() => (ShopView)view;

    public void TryButItem()
    {
        itemManagementSystemService.TryBuyItem(selectedSlot.GetItem(), chosenAmount, GetView().FailSound, GetView().SuccessSound);
    }

}
