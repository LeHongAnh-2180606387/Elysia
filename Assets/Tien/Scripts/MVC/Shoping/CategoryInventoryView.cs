using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class CategoryInventoryView : MonoBehaviour
{
    [field: SerializeField] public Transform ItemGridRoot { get; private set; }

    [Header("Category Buttons")]
    [SerializeField] protected Button allCategoryButton;
    [SerializeField] protected Button materialCategoryButton;
    [SerializeField] protected Button equipmentCategoryButton;
    [SerializeField] protected Button consumableCategoryButton;
    [SerializeField] protected Button treasureCategoryButton;


    [Header("Item Description Panel")]
    [SerializeField] protected GameObject itemDescriptionPanel;
    [SerializeField] protected Image itemIcon;
    [SerializeField] protected TextMeshProUGUI itemNameText;
    [SerializeField] protected TextMeshProUGUI itemDescriptionText;
    [SerializeField] protected TextMeshProUGUI itemWeightText;
    [SerializeField] protected TextMeshProUGUI itemAmountText;
    [SerializeField] protected TextMeshProUGUI itemCostText;
    [SerializeField] protected Button decreaseAmoutButtom;
    [SerializeField] protected Button increaseAmoutButtom;
    [SerializeField] protected Button buyButtom;



    protected virtual void Awake()
    {
        allCategoryButton.onClick.AddListener(OnAllCategoryButtonSelected);
        materialCategoryButton.onClick.AddListener(OnMaterialCategoryButtonSelected);
        equipmentCategoryButton.onClick.AddListener(OnEquipmentCategoryButtonSelected);
        consumableCategoryButton.onClick.AddListener(OnConsumableCategoryButtonSelected);
        treasureCategoryButton.onClick.AddListener(OnTreasureCategoryButtonSelected);

        decreaseAmoutButtom.onClick.AddListener(OnDecreaseAmountButtomClicked);
        increaseAmoutButtom.onClick.AddListener(OnIncreaseAmountButtomClicked);
    }

    public void SetupDescriptionPanel(Sprite icon, string name, string description, float weight, int amount, int cost, int itemMaxAmount)
    {
        itemDescriptionPanel.SetActive(true);
        itemIcon.sprite = icon;
        itemNameText.text = name;
        itemDescriptionText.text = description;
        itemWeightText.text = $"{weight} KG";
        itemAmountText.text = amount.ToString();
        itemCostText.text = $"{cost}  GOLD";

        SetupAmountButtons(amount, itemMaxAmount);
    }

    private void OnIncreaseAmountButtomClicked()
    {
        controller.IncreaseSelectedItemAmount();
    }

    private void OnDecreaseAmountButtomClicked()
    {
        controller.DecreaseSelectedItemAmount();
    }

    public void SetController(CategoryInventoryController controller)
    {
        this.controller = controller;
    }

    public void HideDescriptionPanel()
    {
        itemDescriptionPanel.SetActive(false);
    }

    public void SetupAmountButtons(int itemAmount, int itemMaxAmount)
    {
        itemAmountText.text = itemAmount.ToString();
        if (itemAmount <= 1)
        {
            decreaseAmoutButtom.gameObject.SetActive(false);
        }
        else
        {
            decreaseAmoutButtom.gameObject.SetActive(true);
        }

        if (itemAmount >= itemMaxAmount)
        {
            increaseAmoutButtom.gameObject.SetActive(false);
        }
        else
        {
            increaseAmoutButtom.gameObject.SetActive(true);
        }
    }

    protected CategoryInventoryController controller;

    protected void OnTreasureCategoryButtonSelected()
    {
        controller.SelectTreasureCategory();
    }

    protected void OnConsumableCategoryButtonSelected()
    {
        controller.SelectConsumablesCategory();
    }

    protected void OnEquipmentCategoryButtonSelected()
    {
        controller.SelectEquipmentCategory();
    }

    protected void OnMaterialCategoryButtonSelected()
    {
        controller.SelectMaterialCategory();
    }

    protected void OnAllCategoryButtonSelected()
    {
        controller.SelectAllCategory();
    }
}
