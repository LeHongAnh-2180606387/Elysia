using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemAmountText;
    [SerializeField] private TextMeshProUGUI weightAmountText;
    [SerializeField] private Image itemIcon;
    [SerializeField] private Button slotButton;
    [SerializeField] private Image rarityBorder;
    [SerializeField] private List<RarityColors> colors;

    [Serializable]
    private struct RarityColors
    {
        public ItemRarity Rarity;
        public Color RarityColor;
    }

    private ItemSlotController controller;

    private void Awake()
    {
        slotButton.onClick.AddListener(OnSlotButtomClicked);
    }

    private void OnSlotButtomClicked()
    {
        controller.SelectItem();
    }

    public void SetRarityBorder(ItemRarity rarity)
    {
        var rarityColor = colors.Find(c => c.Rarity == rarity);

        rarityBorder.color = rarityColor.RarityColor;
    }

    public void SetController(ItemSlotController controller)
    {
        this.controller = controller;
    }

    public void SetIcon(Sprite icon)
    {
        itemIcon.sprite = icon;
    }

    public void SetAmount(int amount)
    {
        itemAmountText.text = amount.ToString();
    }

    public void SetWeight(float amount)
    {
        weightAmountText.text = amount.ToString();
    }
}
