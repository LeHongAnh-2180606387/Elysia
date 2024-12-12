
using System;
using Systems.Hero.Manager;
using Systems.Hero.Model;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI textMeshPro;

    public void Update()
    {
        if (PlayerDataManager.Instance.playerHero != null)
            textMeshPro.text = PlayerDataManager.Instance.playerHero.transform.position.ToString();;
    }
}