
public class EventService
{
    public EventController<int> OnCurrencyAdd { get; private set; }
    public EventController<int> OnCurrencySubtract { get; private set; }
    public EventController<int> OnCurrencyUpdated { get; private set; }
    public EventController OnResourceGather {  get; private set; }

    public EventController<ItemSlotController> OnItemSlotClickedOnShop { get; private set; }
    public EventController<ItemSlotController> OnItemSlotClickedOnInventory { get; private set; }

    public void Init()
    {
        OnCurrencyAdd = new EventController<int>();
        OnCurrencySubtract = new EventController<int>();
        OnCurrencyUpdated = new EventController<int>();
        OnResourceGather = new EventController();
        OnItemSlotClickedOnShop = new EventController<ItemSlotController>();
        OnItemSlotClickedOnInventory = new EventController<ItemSlotController>();
    }
}
