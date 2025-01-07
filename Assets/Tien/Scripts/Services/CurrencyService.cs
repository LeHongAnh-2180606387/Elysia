using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyService
{
    private int currency = 0;

    private EventService eventService;

    public int GetCurrency() { return currency; }

    public void Init(EventService eventService)
    {
        this.eventService = eventService;
    }

    public void AddCurrency(int amount)
    {
        eventService.OnCurrencyAdd.InvokeEvent(amount);
        UpdateCurrencyValue(currency + amount);
    }

    public void SubtractCurrency(int amount)
    {
        eventService.OnCurrencySubtract.InvokeEvent(amount);
        UpdateCurrencyValue(Mathf.Max(0, currency - amount));
    }

    private void UpdateCurrencyValue(int amount)
    {
        eventService.OnCurrencyUpdated.InvokeEvent(amount);
        currency = amount;
    }
}
