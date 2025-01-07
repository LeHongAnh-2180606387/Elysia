using UnityEngine;

public class Item
{
    public ItemType Type;
    public ItemRarity Rarity;
    public Sprite Icon;
    public string Name;
    public string Description;
    public int BuyingPrice;
    public int SellingPrice;
    public float Weight;

    public Item(ItemSO itemSo)
    {
        Type = itemSo.Type;
        Rarity = itemSo.Rarity;
        Icon = itemSo.Icon;
        Name = itemSo.Name;
        Description = itemSo.Description;
        BuyingPrice = itemSo.BuyingPrice;
        SellingPrice = itemSo.SellingPrice;
        Weight = itemSo.Weight;
    }
}
