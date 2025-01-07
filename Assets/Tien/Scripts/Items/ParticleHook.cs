using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    // Lớp này quản lý hệ thống hạt (particle system) và cung cấp chức năng phát ra các hạt.
    public class ParticleHook : MonoBehaviour
    {
        ParticleSystem[] particles; // Mảng lưu trữ các hệ thống hạt con của đối tượng.

        // Phương thức khởi tạo để lấy tất cả các hệ thống hạt con của đối tượng.
        public void Init()
        {
            // Lấy tất cả các ParticleSystem từ các đối tượng con.
            particles = GetComponentsInChildren<ParticleSystem>();
        }

        // Phương thức phát ra các hạt từ tất cả các hệ thống hạt con.
        // Tham số v là số lượng hạt cần phát ra, mặc định là 1.
        public void Emit(int v = 1)
        {
            // Nếu mảng particles là null (chưa được khởi tạo), không thực hiện gì cả.
            if (particles == null)
                return;

            // Lặp qua tất cả các hệ thống hạt và phát ra số lượng hạt được chỉ định.
            for (int i = 0; i < particles.Length; i++)
            {
                particles[i].Emit(v);
            }
        }
    }
}
