using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QAToolView : MonoBehaviour
{
    [SerializeField] private Button AddCurrency;
    [SerializeField] private Button RemoveCurrency;

    private QAToolController controller;

    private void Awake()
    {
        AddCurrency.onClick.AddListener(OnAddCurrencyButtonClicked);
        RemoveCurrency.onClick.AddListener(OnRemoveCurrencyButtonClicked);
    }

    public void SetController(QAToolController controller)
    {
        this.controller = controller;
    }

    private void OnRemoveCurrencyButtonClicked()
    {
        controller.RemoveCurrency(1000);
    }

    private void OnAddCurrencyButtonClicked()
    {
        controller.AddCurrency(1000);
    }
}
