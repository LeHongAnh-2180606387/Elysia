using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    // Lớp này lưu trữ danh sách các tương tác có thể xảy ra trong trò chơi hoặc ứng dụng.
    // Được kế thừa từ ScriptableObject, cho phép tạo ra các tương tác có thể cấu hình trong Unity Editor.
    public class InteractionScriptableObject : ScriptableObject
    {
        // Danh sách chứa các đối tượng tương tác.
        public List<Interactions> interactions = new List<Interactions>();
    }

    // Lớp này đại diện cho một tương tác cụ thể.
    [System.Serializable]
    public class Interactions
    {
        // ID duy nhất để xác định tương tác.
        public string interactionId;

        // Tên của animation cần thực hiện khi tương tác.
        public string anim;

        // Chỉ định xem tương tác này có được thực hiện chỉ một lần không.
        public bool oneShot;

        // Sự kiện đặc biệt sẽ xảy ra khi tương tác diễn ra.
        public string specialEvent;
    }
}
