using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    public class Projectile : MonoBehaviour
    {
        Rigidbody rigid; // Tham chiếu đến Rigidbody của projectile.

        public float hSpeed = 5f; // Tốc độ ngang của projectile.
        public float vSpeed = 2f; // Tốc độ dọc của projectile.

        public Transform target; // Mục tiêu của projectile (không được sử dụng trong đoạn mã này).

        public GameObject explosionPrefab; // Prefab của hiệu ứng nổ khi projectile va chạm.

        public void Init()
        {
            // Khởi tạo Rigidbody và thêm lực cho projectile.
            rigid = GetComponent<Rigidbody>();

            // Tính toán lực để đẩy projectile về phía trước và lên trên.
            Vector3 targetForce = transform.forward * hSpeed;
            targetForce += transform.up * vSpeed;

            // Thêm lực vào Rigidbody với kiểu ForceMode là Impulse.
            rigid.AddForce(targetForce, ForceMode.Impulse);
        }

        void OnTriggerEnter(Collider other)
        {
            // Kiểm tra xem projectile va chạm với một đối tượng có EnemyStates không.
            EnemyStates eStates = other.GetComponentInParent<EnemyStates>();

            if (eStates != null)
            {
                // Nếu có, thực hiện hành động gây sát thương và sử dụng hiệu ứng spell.
                eStates.DoDamageSpell();
                SpellEffectsManager.singleton.UseSpellEffect("onFire", null, eStates);
            }

            // Tạo hiệu ứng nổ tại vị trí của projectile và hủy projectile.
            GameObject g0 = Instantiate(explosionPrefab, transform.position, transform.rotation) as GameObject;
            Destroy(this.gameObject);
        }
    }
}
