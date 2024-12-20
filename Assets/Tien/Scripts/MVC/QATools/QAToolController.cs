using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QAToolController
{
    private QAToolView qaToolView;

    private UIService uiService;

    private CurrencyService currencyService;

    public QAToolController(QAToolView qaToolView, UIService uiService, CurrencyService currencyService)
    {
        this.qaToolView = GameObject.Instantiate<QAToolView>(qaToolView, uiService.UIRoot);
        this.qaToolView.SetController(this);
        this.uiService = uiService;
        this.currencyService = currencyService;
    }

    public void AddCurrency(int amount)
    {
        this.currencyService.AddCurrency(amount);
    }

    public void RemoveCurrency(int amount)
    {
        this.currencyService.SubtractCurrency(amount);
    }
}
