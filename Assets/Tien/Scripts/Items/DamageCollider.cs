using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    // Lớp này xử lý các va chạm của collider để gây sát thương cho các đối tượng khi va chạm xảy ra.
    public class DamageCollider : MonoBehaviour
    {
        StateManager states; // Quản lý trạng thái của người chơi.
        EnemyStates eStates; // Quản lý trạng thái của kẻ thù.

        // Khởi tạo collider để gây sát thương cho người chơi.
        public void InitPlayer(StateManager st)
        {
            states = st; // Gán quản lý trạng thái người chơi.
            gameObject.layer = 9; // Đặt lớp của collider để phân biệt với các collider khác.
            gameObject.SetActive(false); // Vô hiệu hóa collider ban đầu để không gây sát thương khi không cần thiết.
        }

        // Khởi tạo collider để gây sát thương cho kẻ thù.
        public void InitEnemy(EnemyStates st)
        {
            eStates = st; // Gán quản lý trạng thái kẻ thù.
            gameObject.layer = 9; // Đặt lớp của collider để phân biệt với các collider khác.
            gameObject.SetActive(false); // Vô hiệu hóa collider ban đầu để không gây sát thương khi không cần thiết.
        }

        // Phương thức này được gọi khi collider va chạm với một collider khác.
        void OnTriggerEnter(Collider other)
        {
            // Nếu collider được khởi tạo để gây sát thương cho người chơi.
            if (states)
            {
                // Tìm kiếm thành phần EnemyStates trong các đối tượng cha của collider va chạm.
                EnemyStates es = other.transform.GetComponentInParent<EnemyStates>();

                // Nếu tìm thấy EnemyStates, gọi phương thức DoDamage() để thực hiện hành động gây sát thương.
                if (es != null)
                {
                    es.DoDamage();
                }
                return; // Kết thúc phương thức nếu đã thực hiện hành động sát thương cho kẻ thù.
            }

            // Nếu collider được khởi tạo để gây sát thương cho kẻ thù.
            if (eStates)
            {
                // Tìm kiếm thành phần StateManager trong các đối tượng cha của collider va chạm.
                StateManager st = other.transform.GetComponentInParent<StateManager>();

                // Nếu tìm thấy StateManager, gọi phương thức DoDamage() để thực hiện hành động gây sát thương cho người chơi.
                if (st != null)
                {
                    st.DoDamage(eStates.GetCurrentAttack());
                }
                return; // Kết thúc phương thức nếu đã thực hiện hành động sát thương cho người chơi.
            }
        }
    }
}
