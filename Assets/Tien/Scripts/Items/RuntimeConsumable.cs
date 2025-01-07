using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    // Lớp này đại diện cho một đối tượng tiêu thụ (consumable) trong runtime.
    // Chứa các thuộc tính và thông tin liên quan đến đối tượng tiêu thụ.
    public class RuntimeConsumable : MonoBehaviour
    {
        // Số lượng đối tượng tiêu thụ hiện tại. Mặc định là 1.
        public int itemCount = 1;

        // Cờ cho biết liệu số lượng đối tượng tiêu thụ có phải là vô hạn không.
        public bool unlimitedCount;

        // Tham chiếu đến đối tượng tiêu thụ (Consumable) mà đối tượng này đại diện.
        public Consumable instance;

        // Mô hình đại diện cho đối tượng tiêu thụ trong game.
        public GameObject itemModel;
    }
}
