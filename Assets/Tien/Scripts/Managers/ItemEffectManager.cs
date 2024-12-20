using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    // Quản lý và áp dụng các hiệu ứng từ item (vật phẩm) trong trò chơi.
    public class ItemEffectManager : MonoBehaviour
    {
        // Từ điển lưu trữ các hiệu ứng với ID của chúng và giá trị tương ứng.
        Dictionary<string, int> effects = new Dictionary<string, int>();

        // Áp dụng hiệu ứng dựa trên ID của hiệu ứng và trạng thái của nhân vật.
        public void CastEffect(string effectId, StateManager states)
        {
            // Lấy giá trị tương ứng với ID của hiệu ứng.
            int i = GetIntFromId(effectId);
            if (i < 0)
                return; // Nếu ID không hợp lệ, không thực hiện hiệu ứng.

            // Áp dụng hiệu ứng dựa trên giá trị lấy được.
            switch (i)
            {
                case 0: // "bestus" - tăng cường sức khỏe.
                    AddHealth(states);
                    break;
                case 1: // "focus" - tăng cường sự tập trung.
                    AddFocus(states);
                    break;
                case 2: // "souls" - tăng cường điểm linh hồn.
                    AddSouls(states);
                    break;
            }
        }

        #region Effects Actual
        // Tăng cường sức khỏe của nhân vật.
        void AddHealth(StateManager states)
        {
            states.characterStats._health += states.characterStats._healthRecoverValue;
        }

        // Tăng cường sự tập trung của nhân vật.
        void AddFocus(StateManager states)
        {
            states.characterStats._focus += states.characterStats._focusRecoverValue;
        }

        // Tăng cường điểm linh hồn của nhân vật.
        void AddSouls(StateManager states)
        {
            states.characterStats._souls += 100;
        }
        #endregion

        // Lấy giá trị tương ứng với ID của hiệu ứng từ từ điển.
        int GetIntFromId(string id)
        {
            int index = -1;
            if (effects.TryGetValue(id, out index))
            {
                return index; // Trả về giá trị nếu ID có trong từ điển.
            }

            return index; // Trả về -1 nếu ID không có trong từ điển.
        }

        // Khởi tạo ID của các hiệu ứng và giá trị tương ứng.
        void InitEffectsId()
        {
            effects.Add("bestus", 0);
            effects.Add("focus", 1);
            effects.Add("souls", 2);
        }

        // Singleton pattern để truy cập ItemEffectManager từ các lớp khác.
        public static ItemEffectManager singleton;
        private void Awake()
        {
            InitEffectsId(); // Khởi tạo các ID hiệu ứng khi đối tượng được khởi tạo.
            singleton = this; // Đặt singleton thành instance hiện tại của ItemEffectManager.
        }
    }
}
