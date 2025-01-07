using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    // Lớp này quản lý các collider gây sát thương (damage colliders) gắn với vũ khí.
    // Bao gồm các phương thức để bật/tắt các collider và khởi tạo chúng.
    public class WeaponHook : MonoBehaviour
    {
        // Mảng chứa các đối tượng collider gây sát thương liên quan đến vũ khí.
        public GameObject[] damageCollider;

        // Kích hoạt tất cả các damage colliders trong mảng.
        public void OpenDamageColliders()
        {
            for (int i = 0; i < damageCollider.Length; i++)
            {
                damageCollider[i].SetActive(true);
            }
        }

        // Vô hiệu hóa tất cả các damage colliders trong mảng.
        public void CloseDamageColliders()
        {
            for (int i = 0; i < damageCollider.Length; i++)
            {
                damageCollider[i].SetActive(false);
            }
        }

        // Khởi tạo tất cả các damage colliders bằng cách truyền vào một đối tượng StateManager.
        // Phương thức này thiết lập các collider để chúng hoạt động với thông tin trạng thái của người chơi.
        public void InitDamageColliders(StateManager states)
        {
            for (int i = 0; i < damageCollider.Length; i++)
            {
                damageCollider[i].GetComponent<DamageCollider>().InitPlayer(states);
            }
        }
    }
}
