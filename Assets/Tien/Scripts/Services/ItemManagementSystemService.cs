using UnityEngine;

public class ItemManagementSystemService
{
    private ShopController shopController;
    private InventoryController inventoryController;
    private CurrencyService currencyService;
    private UIService uiService;
    private ResourceGatherConfig resourceGatherConfig;
    private EventService eventService;

    public void Init(CurrencyService currencyService, UIService uiService, 
        ResourceGatherConfig resourceGatherConfig, EventService eventService)
    {
        this.currencyService = currencyService;
        this.uiService = uiService;
        this.resourceGatherConfig = resourceGatherConfig;
        this.eventService = eventService;

        eventService.OnResourceGather.AddListener(OnResourceGathered);
    }

    ~ItemManagementSystemService()
    {
        eventService.OnResourceGather.RemoveListener(OnResourceGathered);
    }

    private void OnResourceGathered()
    {
        var resorucePool = resourceGatherConfig.GetDecrescentOrderedResourcesOdds();
        var currentInventoryValue = inventoryController.GetInventoryValue();

        foreach (var res in resorucePool)
        {
            if(currentInventoryValue >= res.MinGoldRequirement)
            {
                var sortedEntry = res.GetRandomEntryBasedOnWeight();

                if(inventoryController.GetRemainingWeight() < sortedEntry.Item.Weight)
                {
                    uiService.OpenGenericPopup($"Sorry! You found {sortedEntry.Item.Name}, but you are carrying too much, try sell some stuff.");
                    return;
                }

                AddItemToInventory(new Item(sortedEntry.Item), 1);
                return;
            }
        }
    }

    public void SetControllers(ShopController shopController, InventoryController inventoryController)
    {
        this.shopController = shopController;
        this.inventoryController = inventoryController;

    }

    public void TryBuyItem(Item item, int amount, AudioSource failed, AudioSource success)
    {
        int currencyNeeded = item.BuyingPrice * amount;
        float weightOfTheItems = item.Weight * amount;

        if(currencyNeeded > currencyService.GetCurrency())
        {
            uiService.OpenGenericPopup("Sorry! You don't have enough Gold, try sell some stuff.");
            failed.Play();
            return;
        }

        if(weightOfTheItems > inventoryController.GetRemainingWeight())
        {
            uiService.OpenGenericPopup("Sorry! You are carrying too much, try sell some stuff.");
            failed.Play();
            return;
        }

        uiService.OpenGenericPopup($"You want to buy {amount} {item.Name} for {currencyNeeded}",
            () =>
            {
                success.Play();
                currencyService.SubtractCurrency(currencyNeeded);
                shopController.RemoveItem(item, amount);    
                AddItemToInventory(item, amount);
            });
    }

    public void AddItemToInventory(Item item, int amount)
    {
        inventoryController.AddItem(item, amount, false);
    }

    public void SellItem(Item item, int amount)
    {
        int sellingPrice = item.SellingPrice * amount;

        currencyService.AddCurrency(sellingPrice);
        inventoryController.RemoveItem(item,amount);
        shopController.AddItem(item,amount, true);
    }
}
