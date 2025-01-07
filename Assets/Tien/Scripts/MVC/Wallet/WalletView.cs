using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WalletView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textAmount;

    private WalletController walletController;

    public void SetController(WalletController walletController)
    {
        this.walletController = walletController;
    }

    public void UpdateAmountText(int amount)
    {
        textAmount.text = amount.ToString();
    }
}
