using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Walley Config", menuName = "Configs/WalletConfig")]
public class WalletConfigSO : ScriptableObject
{
    public WalletView walletViewPrefab;
}
