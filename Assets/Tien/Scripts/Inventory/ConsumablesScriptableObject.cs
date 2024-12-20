using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    // Lớp này dùng để lưu trữ danh sách các đối tượng tiêu thụ (consumables).
    // Được kế thừa từ ScriptableObject, cho phép tạo ra các đối tượng tiêu thụ có thể cấu hình trong Unity Editor.
    public class ConsumablesScriptableObject : ScriptableObject
    {
        // Danh sách chứa các đối tượng tiêu thụ.
        public List<Consumable> consumables = new List<Consumable>();
    }
}
