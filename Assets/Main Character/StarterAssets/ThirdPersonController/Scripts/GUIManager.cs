    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class GUIManager : MonoBehaviour
    {
        // Start is called before the first frame update
        public float currentHealth = 1000f;
        public float maxHealth = 1000f;

        public float currentStamina = 200f;
        public float maxStamina = 200f;

    //exp
        public int currentLevel = 1;       // Cấp độ hiện tại
        public float currentXP = 0f;       // Kinh nghiệm hiện tại
        public float xpToNextLevel = 100f; // Kinh nghiệm cần để lên cấp
        public float xpMultiplier = 1.5f;

        float barlength = 0.0f;
        float barWidth;    // Chiều rộng của thanh kinh nghiệm
        float barHeight = 20f;    // Chiều cao của thanh kinh nghiệm
        float x = 10f;            // Vị trí X của thanh
        float y = 10f;            // Vị trí Y của thanh


    public float baseBarWidth = 200f; // Kích thước chuẩn của thanh kinh nghiệm

    void Start()
    {
        barlength = Screen.width / 8;
    }

    private void OnGUI()
    {
        //icon
        GUI.Box(new Rect(5, 30, 40, 20), "HP");
        GUI.Box(new Rect(5, 70, 40, 20), "Stamina");

        //health/stamina
        GUI.Box(new Rect(45, 30, barlength, 20), currentHealth.ToString("0") + "/" + maxHealth);
        GUI.Box(new Rect(45, 70, barlength, 20), currentStamina.ToString("0") + "/" + maxStamina);

        // --- Tính toán chiều rộng thanh kinh nghiệm ---
        barWidth = baseBarWidth * (xpToNextLevel / 100f); // 100f là XP cơ bản để làm chuẩn

        float xpRatio = currentXP / xpToNextLevel;

        // Vẽ khung thanh kinh nghiệm
        GUI.Box(new Rect(x, y, barWidth, barHeight), "XP Bar");

        // Vẽ phần đầy của thanh kinh nghiệm
        GUI.Box(new Rect(x, y, barWidth * xpRatio, barHeight), "", GUI.skin.button);

        // --- Hiển thị Level ---
        float circleSize = 50f; // Kích thước hình tròn level
        float circleX = x + barWidth + 20f; // Vị trí X bên phải thanh XP
        float circleY = y; // Vị trí Y bằng với thanh XP

        // Vẽ khung hình tròn giả lập bằng Box
        GUI.Box(new Rect(circleX, circleY, circleSize, circleSize), "");

        // Hiển thị text level bên trong hình tròn
        GUIStyle levelStyle = new GUIStyle(GUI.skin.label);
        levelStyle.alignment = TextAnchor.MiddleCenter;
        levelStyle.fontSize = 16;
        levelStyle.normal.textColor = Color.white;

        GUI.Label(new Rect(circleX, circleY, circleSize, circleSize), "Lv " + currentLevel, levelStyle);
    }


}
