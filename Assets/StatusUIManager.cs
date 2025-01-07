using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class StatusUIManager : MonoBehaviour
{
    // Start is called before the first frame update
    private float currenthealth;
    public float armor = 10f;
    private float stamina;
    public float attack = 50f;

    public GUIManager gui;
    public TextMeshProUGUI armortext;      // Hiển thị cấp độ
    public TextMeshProUGUI healthText;     // Hiển thị giá trị máu
    public TextMeshProUGUI staminaText;
    public TextMeshProUGUI attackText;
    void Start()
    {
        UpdateStatus();
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateStatus();   
    }
   public void UpdateStatus()
    {  // Cập nhật thanh máu
       

        // Hiển thị giá trị máu
        healthText.text = "" + gui.currentHealth;

        // Cập nhật thanh stamina
       

        // Hiển thị giá trị stamina
        staminaText.text ="" + gui.currentStamina;

        armortext.text = "" + armor;

        attackText.text = "" + attack;

    }
}
