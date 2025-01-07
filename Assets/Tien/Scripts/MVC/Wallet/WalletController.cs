using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalletController
{
    private WalletView walletView;
    private WalletModel walletModel;

    private UIService uiService;
    private EventService eventService;

    public WalletController(WalletView walletView, WalletModel walletModel, 
        UIService uiService, EventService eventService)
    {
        this.walletModel = walletModel;
        this.uiService = uiService;
        this.walletView = GameObject.Instantiate<WalletView>(walletView, uiService.UIRoot);
        
        this.walletModel.SetWalletController(this);
        this.walletView.SetController(this);
        eventService.OnCurrencyUpdated.AddListener(UpdateWalletAmount);
    }

    ~WalletController()
    {
        eventService.OnCurrencyUpdated.RemoveListener(UpdateWalletAmount);
    }

    public void AddCurrency(int amount)
    {

    }

    public void RemoveCurrency(int amount)
    {

    }

    private void UpdateWalletAmount(int amount)
    {
        walletView.UpdateAmountText(amount);
    }
}
