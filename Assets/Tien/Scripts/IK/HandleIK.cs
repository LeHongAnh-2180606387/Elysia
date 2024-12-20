using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    public class HandleIK : MonoBehaviour
    {
        Animator anim; 

        Transform handHelper;
        Transform bodyHelper; 
        Transform headHelper; 
        Transform shoulderHelper; 
        Transform headTrans; 

        public float weight; // Trọng số của IK

        public IKSnapShot[] ikSnapShots; 
        public Vector3 defaultHeadPos;

        // Trả về một IKSnapShot dựa trên loại được chỉ định
        IKSnapShot GetSnapShot(IKSnapShotType type)
        {
            for (int i = 0; i < ikSnapShots.Length; i++)
            {
                if (ikSnapShots[i].type == type)
                {
                    return ikSnapShots[i];
                }
            }
            return null; // Nếu không tìm thấy, trả về null
        }

        // Khởi tạo các thành phần IK với Animator
        public void Init(Animator a)
        {
            anim = a;

            // Tạo các đối tượng trống để hỗ trợ IK
            headHelper = new GameObject().transform;
            headHelper.name = "head_helper";
            handHelper = new GameObject().transform;
            handHelper.name = "hand_helper";
            bodyHelper = new GameObject().transform;
            bodyHelper.name = "body_helper";
            shoulderHelper = new GameObject().transform;
            shoulderHelper.name = "shoulder_helper";

            // Đặt vị trí và xoay của các đối tượng hỗ trợ
            shoulderHelper.parent = transform.parent;
            shoulderHelper.localPosition = Vector3.zero;
            shoulderHelper.localRotation = Quaternion.identity;
            headHelper.parent = shoulderHelper;
            bodyHelper.parent = shoulderHelper;
            handHelper.parent = shoulderHelper;

            headTrans = anim.GetBoneTransform(HumanBodyBones.Head); // Lấy vị trí đầu từ Animator
        }

        // Cập nhật các mục tiêu IK dựa trên loại snap shot
        public void UpdateIKTargets(IKSnapShotType type, bool isLeft)
        {
            IKSnapShot snap = GetSnapShot(type);

            // Cập nhật vị trí và góc của các đối tượng hỗ trợ
            handHelper.localPosition = snap.handPos;
            handHelper.localEulerAngles = snap.hand_eulers;
            bodyHelper.localPosition = snap.bodyPos;

            // Cập nhật vị trí đầu nếu được ghi đè
            if (snap.overwriteHeadPos)
                headHelper.localPosition = snap.headPos;
            else
                headHelper.localPosition = defaultHeadPos;
        }

        // Cập nhật các thông số IK trong mỗi khung hình
        public void IKTick(AvatarIKGoal goal, float w)
        {
            weight = Mathf.Lerp(weight, w, Time.deltaTime * 5); // Làm mịn trọng số

            // Thiết lập trọng số và vị trí IK
            anim.SetIKPositionWeight(goal, weight);
            anim.SetIKRotationWeight(goal, weight);
            anim.SetIKPosition(goal, handHelper.position);
            anim.SetIKRotation(goal, handHelper.rotation);

            // Thiết lập trọng số và vị trí nhìn
            anim.SetLookAtWeight(weight, 0.8f, 1, 1, 1);
            anim.SetLookAtPosition(bodyHelper.position);
        }

        // Cập nhật vị trí vai trong mỗi khung hình
        public void OnAnimatorMoveTick(bool isLeft)
        {
            Transform shoulder = anim.GetBoneTransform(
                (isLeft) ? HumanBodyBones.LeftShoulder : HumanBodyBones.RightShoulder);

            shoulderHelper.transform.position = shoulder.position; // Cập nhật vị trí vai cho shoulderHelper
        }

        // Cập nhật góc đầu trong mỗi khung hình
        public void LateTick()
        {
            if (headTrans == null || headHelper == null)
                return;

            // Tính toán hướng và góc quay của đầu
            Vector3 direction = headHelper.position - headTrans.position;
            if (direction == Vector3.zero)
                direction = headTrans.forward;

            Quaternion targetRot = Quaternion.LookRotation(direction);
            Quaternion curRot = Quaternion.Slerp(headTrans.rotation, targetRot, weight);
            headTrans.rotation = curRot; // Cập nhật góc quay của đầu
        }
    }

    // Các loại snap shot IK
    public enum IKSnapShotType
    {
        breath_r, breath_l, shield_r, shield_l
    }

    // Lớp lưu trữ thông tin snap shot IK
    [System.Serializable]
    public class IKSnapShot
    {
        public IKSnapShotType type; 
        public Vector3 handPos; 
        public Vector3 hand_eulers; 
        public Vector3 bodyPos; // Vị trí cơ thể

        public bool overwriteHeadPos; // Có ghi đè vị trí đầu không
        public Vector3 headPos; // Vị trí đầu
    }
}
