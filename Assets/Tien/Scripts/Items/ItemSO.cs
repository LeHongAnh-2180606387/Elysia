using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "New ItemSO", menuName ="Configs/NewItem")] 
public class ItemSO : ScriptableObject
{
    public ItemType Type;
    public ItemRarity Rarity;
    [ShowAssetPreview] public Sprite Icon;
    public string Name;
    [ResizableTextArea] public string Description;
    public int BuyingPrice;
    public int SellingPrice;
    public float Weight;
}