using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    // Lớp này xử lý các va chạm của collider với kẻ thù khi xảy ra hiệu ứng thở (breath effect).
    // Được gắn vào các đối tượng có thể gây sát thương khi va chạm với kẻ thù.
    public class BreathCollider : MonoBehaviour
    {
        // Phương thức này được gọi khi collider của đối tượng này va chạm với một collider khác.
        void OnTriggerEnter(Collider other)
        {
            // Tìm kiếm thành phần EnemyStates trong các đối tượng con của collider va chạm.
            EnemyStates eStates = other.GetComponentInChildren<EnemyStates>();
            if (eStates != null)
            {
                // Nếu tìm thấy EnemyStates, gọi phương thức DoDamageSpell() để thực hiện hành động gây sát thương.
                eStates.DoDamageSpell();

                // Sử dụng SpellEffectsManager để áp dụng hiệu ứng phép thuật "onFire" lên kẻ thù.
                SpellEffectsManager.singleton.UseSpellEffect("onFire", null, eStates);
            }
        }
    }
}
