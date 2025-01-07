using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalletModel
{
    public int WalletAmount;

    private WalletController walletController;

    public WalletModel(int walletAmount)
    {
        WalletAmount = walletAmount;
    }

    public void SetWalletController(WalletController walletController)
    {
        this.walletController = walletController;
    }
}
