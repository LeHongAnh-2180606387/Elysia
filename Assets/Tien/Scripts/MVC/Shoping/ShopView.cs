using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopView : CategoryInventoryView
{
    [field: SerializeField] public AudioSource FailSound;
    [field: SerializeField] public AudioSource SuccessSound;

    protected override void Awake()
    {
        base.Awake();
        buyButtom.onClick.AddListener(OnBuyButtomClicked);
    }

    private void OnBuyButtomClicked()
    {
        GetController().TryButItem();
    }

    private ShopController GetController() => (ShopController)controller;
}
