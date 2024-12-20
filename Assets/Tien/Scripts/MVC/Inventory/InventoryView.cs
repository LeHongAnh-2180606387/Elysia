using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class InventoryView : CategoryInventoryView
{
    [Header("Weight Text")]
    [SerializeField] protected TextMeshProUGUI currentWeightText;
    [SerializeField] protected TextMeshProUGUI maxWeightText;

    [Header("Value Text")]
    [SerializeField] protected TextMeshProUGUI currentValueText;

    [Header("Sounds")]
    [SerializeField] protected AudioSource coinAudioSource;


    private InventoryController GetController() => (InventoryController)controller;

    protected override void Awake()
    {
        base.Awake();
        buyButtom.onClick.AddListener(OnSellButtomClicked);
    }

    public void UpdateInventoryWeightText(float currentWeight, float maxWeight)
    {
        currentWeightText.text = currentWeight.ToString();
        maxWeightText.text = $"{maxWeight} KG";
    }

    public void UpdateValueText(int currentValue)
    {
        currentValueText.text = $"{currentValue} GOLD";
    }

    private void OnSellButtomClicked()
    {
        GetController().TrySellSelectedItem();
    }


    public void PlayCoinsSound()
    {
        coinAudioSource.Play();
    }
}
