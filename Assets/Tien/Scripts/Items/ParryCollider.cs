using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    // Lớp này đại diện cho một collider được sử dụng để xử lý cơ chế parry (chặn đòn).
    public class ParryCollider : MonoBehaviour
    {
        StateManager states; // Quản lý trạng thái của người chơi.
        EnemyStates eStates; // Quản lý trạng thái của kẻ thù.

        public float maxTimer = 0.6f; // Thời gian tối đa mà collider sẽ hoạt động trước khi bị tắt.
        float timer; // Biến để theo dõi thời gian đã trôi qua.

        // Phương thức khởi tạo cho người chơi.
        public void InitPlayer(StateManager st)
        {
            states = st; // Gán quản lý trạng thái của người chơi.
        }

        // Phương thức khởi tạo cho kẻ thù.
        public void InitEnemy(EnemyStates eSt)
        {
            eStates = eSt; // Gán quản lý trạng thái của kẻ thù.
        }

        // Phương thức Update được gọi mỗi khung hình để cập nhật timer và kiểm tra trạng thái.
        void Update()
        {
            if (states)
            {
                timer += states.delta; // Cập nhật timer theo thời gian delta.

                // Nếu thời gian trôi qua vượt quá maxTimer, đặt timer về 0 và tắt collider.
                if (timer > maxTimer)
                {
                    timer = 0;
                    gameObject.SetActive(false);
                }
            }
        }

        // Phương thức OnTriggerEnter được gọi khi collider va chạm với một đối tượng khác.
        void OnTriggerEnter(Collider other)
        {
            // Nếu có quản lý trạng thái của người chơi, kiểm tra kẻ thù.
            if (states)
            {
                EnemyStates e_st = other.transform.GetComponentInParent<EnemyStates>();

                // Nếu có kẻ thù, gọi phương thức CheckForParry để xử lý cơ chế parry.
                if (e_st != null)
                {
                    e_st.CheckForParry(transform.root, states);
                }
            }

            // Nếu có quản lý trạng thái của kẻ thù, kiểm tra người chơi.
            if (eStates)
            {
                // Kiểm tra người chơi, phương thức chưa được thực hiện.
            }
        }
    }
}
