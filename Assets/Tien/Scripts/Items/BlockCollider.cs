using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    // Lớp này xử lý các va chạm với các collider có thể gây ra các hành động liên quan đến việc chặn tấn công của kẻ thù.
    // Được gắn vào các đối tượng có thể nhận va chạm từ các đối tượng khác.
    public class BlockCollider : MonoBehaviour
    {
        // Phương thức này được gọi khi collider của đối tượng này va chạm với một collider khác.
        void OnTriggerEnter(Collider other)
        {
            // Kiểm tra xem collider va chạm có chứa một DamageCollider không.
            DamageCollider dc = other.GetComponentInChildren<DamageCollider>();
            if (dc != null)
            {
                // Nếu tìm thấy DamageCollider, lấy thành phần EnemyStates từ đối tượng cha của DamageCollider.
                EnemyStates eStates = dc.GetComponentInParent<EnemyStates>();
                if (eStates != null)
                {
                    // Gọi phương thức HandleBlocked() trên EnemyStates để xử lý tình huống khi bị chặn.
                    eStates.HandleBlocked();
                }
            }
        }
    }
}
