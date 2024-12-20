using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GenericPopup : MonoBehaviour
{
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private TextMeshProUGUI descriptionText;

    private void Awake()
    {
        closeButton.onClick.AddListener(() => Destroy(gameObject));
    }

    public void Setup(string description, Action onConfirmAction = null)
    {
        descriptionText.text = description;

        confirmButton.gameObject.SetActive(onConfirmAction != null);

        if(onConfirmAction != null)
        {
            confirmButton.onClick.AddListener(() =>
            {
                onConfirmAction();
                Destroy(gameObject);
            }
            );
        }
    }
}
