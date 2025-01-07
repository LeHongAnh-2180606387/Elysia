using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GUIManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Slider healthSlider;
    public Slider staminaSlider;
    public Slider xpSlider;

    public TextMeshProUGUI levelText;      // Hiển thị cấp độ
    public TextMeshProUGUI healthText;     // Hiển thị giá trị máu
    public TextMeshProUGUI staminaText;    // Hiển thị giá trị stamina

    [Header("Player Stats")]
    public float currentHealth = 1000f;
    public float maxHealth = 1000f;

    public float currentStamina = 200f;
    public float maxStamina = 200f;

    public int currentLevel = 1;       // Cấp độ hiện tại
    public float currentXP = 0f;       // Kinh nghiệm hiện tại
    public float xpToNextLevel = 100f; // Kinh nghiệm cần để lên cấp
    public float xpMultiplier = 1.5f;  // Tỉ lệ tăng XP cần thiết mỗi cấp

    void Start()
    {
        UpdateUI(); // Cập nhật giao diện ban đầu
    }

    void Update()
    {
        // Cập nhật giao diện mỗi khung hình
        UpdateUI();
    }

    public void UpdateUI()
    {
        // Cập nhật thanh máu
        healthSlider.maxValue = maxHealth;
        healthSlider.value = Mathf.Clamp(currentHealth, 0, maxHealth);

        // Hiển thị giá trị máu
        healthText.text = $"{currentHealth}/{maxHealth}";

        // Cập nhật thanh stamina
        staminaSlider.maxValue = maxStamina;
        staminaSlider.value = Mathf.Clamp(currentStamina, 0, maxStamina);

        // Hiển thị giá trị stamina
        staminaText.text = $"{currentStamina}/{maxStamina}";

        // Cập nhật thanh XP
        xpSlider.maxValue = xpToNextLevel;
        xpSlider.value = Mathf.Clamp(currentXP, 0, xpToNextLevel);

        // Cập nhật văn bản cấp độ
        levelText.text = "" + currentLevel;
    }


}
