using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA {
    // Lớp này lưu trữ thông tin về tất cả các vũ khí trong trò chơi.
    // Được kế thừa từ ScriptableObject, cho phép tạo ra và cấu hình các vũ khí trong Unity Editor.
    public class WeaponScriptableObject : ScriptableObject
    {
        // Danh sách chứa tất cả các vũ khí (Weapon).
        // Mỗi vũ khí có thể bao gồm các thuộc tính như tên, sức mạnh, loại vũ khí, v.v.
        public List<Weapon> weapons_all = new List<Weapon>();
    }
}

