using UnityEngine;
using Systems.SaveLoad.Manager; // Đảm bảo kết nối đúng namespace với PlayerDataManager

namespace Systems.Hero.Manager
{
    public class HeroCollisionHandler : MonoBehaviour
    {
        public int damageAmount = 10; // Số máu mất khi va chạm

        private void OnCollisionEnter(Collision collision)
        {
            // Kiểm tra nếu va chạm với đối tượng gây sát thương
            if (collision.gameObject.CompareTag("hmmm"))
            {
                // Gọi sự kiện mất máu
                // PlayerDataManager.Instance.TakeDamage(damageAmount);
            }
        }
    }
}

