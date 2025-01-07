using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    // Quản lý các mục tiêu kẻ thù trong trò chơi.
    public class EnemyManager : MonoBehaviour
    {
        // Danh sách các mục tiêu kẻ thù đang được quản lý.
        public List<EnemyTarget> enemyTargets = new List<EnemyTarget>();

        // Tìm mục tiêu kẻ thù gần nhất từ vị trí cho trước.
        public EnemyTarget GetEnemy(Vector3 from)
        {
            EnemyTarget r = null; // Biến lưu trữ mục tiêu kẻ thù gần nhất.
            float minDist = float.MaxValue; // Khoảng cách nhỏ nhất hiện tại, khởi tạo với giá trị tối đa.

            // Lặp qua tất cả các mục tiêu kẻ thù để tìm mục tiêu gần nhất.
            for (int i = 0; i < enemyTargets.Count; i++)
            {
                // Tính khoảng cách giữa vị trí cho trước và vị trí của mục tiêu kẻ thù.
                float tDist = Vector3.Distance(from, enemyTargets[i].GetTarget().position);
                // Nếu khoảng cách này nhỏ hơn khoảng cách nhỏ nhất hiện tại, cập nhật mục tiêu gần nhất.
                if (tDist < minDist)
                {
                    minDist = tDist;
                    r = enemyTargets[i];
                }
            }

            return r; // Trả về mục tiêu kẻ thù gần nhất.
        }

        // Singleton pattern để truy cập EnemyManager từ các lớp khác.
        public static EnemyManager singleton;
        private void Awake()
        {
            singleton = this; // Đặt singleton thành instance hiện tại của EnemyManager.
        }
    }
}
