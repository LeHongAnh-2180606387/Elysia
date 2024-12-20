using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    // Lớp này lưu trữ thông tin về các phép thuật (spell) như là các item trong trò chơi.
    // Được kế thừa từ ScriptableObject, cho phép tạo ra và cấu hình các phép thuật trong Unity Editor.
    public class SpellItemScriptableObject : ScriptableObject
    {
        // Danh sách các phép thuật (spell) như là item.
        // Mỗi phép thuật có thể bao gồm các thuộc tính và hành vi riêng biệt.
        public List<Spell> spell_items = new List<Spell>();
    }

}

