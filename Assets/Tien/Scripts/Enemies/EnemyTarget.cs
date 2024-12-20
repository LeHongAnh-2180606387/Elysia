using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    public class EnemyTarget : MonoBehaviour
    {
        public int index; // Chỉ số của mục tiêu hiện tại trong danh sách
        public bool isLockOn; // Trạng thái khóa mục tiêu
        public List<Transform> targets = new List<Transform>(); // Danh sách các mục tiêu
        public List<HumanBodyBones> h_bones = new List<HumanBodyBones>(); // Danh sách các xương cơ thể để làm mục tiêu

        public EnemyStates eStates; // Tham chiếu đến trạng thái của kẻ thù

        Animator anim; // Animator của kẻ thù

        // Initializes the EnemyTarget with the provided EnemyStates
        public void Init(EnemyStates st)
        {
            eStates = st; // Gán EnemyStates
            anim = eStates.anim; // Lấy Animator từ EnemyStates
            if (anim.isHuman == false)
                return; // Nếu Animator không phải là Animator của con người thì thoát

            // Lấp đầy danh sách các mục tiêu với các xương cơ thể
            for (int i = 0; i < h_bones.Count; i++)
            {
                targets.Add(anim.GetBoneTransform(h_bones[i])); // Thêm các xương cơ thể vào danh sách mục tiêu
            }

            EnemyManager.singleton.enemyTargets.Add(this); // Thêm EnemyTarget vào danh sách mục tiêu của EnemyManager
        }

        // Returns the current target from the list, or the transform of the EnemyTarget if the list is empty
        public Transform GetTarget(bool negative = false)
        {
            // Nếu danh sách mục tiêu trống, trả về transform của EnemyTarget
            if (targets.Count == 0)
                return transform;

            // Xử lý việc chọn mục tiêu tiếp theo hoặc trước đó
            if (negative == false)
            { // Nếu không có tham số âm
                if (index < targets.Count - 1)
                {
                    index++; // Chuyển đến mục tiêu kế tiếp
                }
                else
                {
                    index = 0; // Quay lại mục tiêu đầu tiên
                }
            }
            else
            { // Nếu tham số là âm
                if (index == 0)
                {
                    index = targets.Count - 1; // Chuyển đến mục tiêu cuối cùng
                }
                else
                {
                    index--; // Chuyển đến mục tiêu trước đó
                }
            }

            index = Mathf.Clamp(index, 0, targets.Count); // Giới hạn chỉ số mục tiêu hợp lệ

            return targets[index]; // Trả về mục tiêu hiện tại
        }
    }
}
