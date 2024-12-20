using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA {
    // Lớp này lưu trữ danh sách các vật phẩm có thể có trong trò chơi hoặc ứng dụng.
    // Được kế thừa từ ScriptableObject, cho phép tạo ra các vật phẩm có thể cấu hình trong Unity Editor.
    public class ItemScriptableObject : ScriptableObject
    {
        // Danh sách chứa các đối tượng Item.
        public List<Item> items = new List<Item>();
    }
}

