using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    public class CameraManager : MonoBehaviour
    {

        [Header("States")]
        public bool lockOn;  

        [Header("Stats")]
        public float followSpeed = 9;  
        public float mouseSpeed = 2;   
        public float turnSmoothing = .1f; 
        public float minAngle = -35;  
        public float maxAngle = 35;   

        [Header("Sources")]
        public Transform target; 
        public EnemyTarget lockOnTarget;  
        public Transform lockOnTransform;  

        [Header("Others")]
        public Vector3 targetDir;  
        public float lookAngle;    
        public float tiltAngle; 


        [HideInInspector]
        public Transform pivot; 
        [HideInInspector]
        public Transform camTrans;  
        StateManager states;  

        float smoothX;  
        float smoothY;  
        float smoothXvelocity;  
        float smoothYvelocity;  


        bool usedRightAxis;  

        bool changeTargetLeft; 
        bool changeTargetRight;  


        //-----------------------------------------------------------------------

        // Hàm khởi tạo (được gọi khi khởi động)
        public void Init(StateManager st)
        {
            states = st;
            target = st.transform;

            camTrans = Camera.main.transform;  // Gán camera chính vào biến camTrans
            pivot = camTrans.parent;  // Gán parent của camera chính vào pivot (điểm xoay)
        }

        // Hàm cập nhật trạng thái mỗi khung hình
        public void Tick(float d)
        {
            float h = Input.GetAxis("Mouse X");  
            float v = Input.GetAxis("Mouse Y"); 

            float targetSpeed = mouseSpeed;  

            changeTargetLeft = Input.GetKeyUp(KeyCode.V);  
            changeTargetRight = Input.GetKeyUp(KeyCode.B); 


            // Xử lý việc chọn mục tiêu cho camera khi đang ở chế độ khóa mục tiêu
            if (lockOnTarget != null)
            {
                // Gán mục tiêu ban đầu khi chưa có mục tiêu khóa
                if (lockOnTransform == null)
                {
                    lockOnTransform = lockOnTarget.GetTarget();  // Lấy mục tiêu đầu tiên từ danh sách mục tiêu
                    states.lockOnTransform = lockOnTransform;
                }

                // Đổi mục tiêu khóa khi nhấn phím đổi mục tiêu
                if (changeTargetLeft || changeTargetRight)
                {
                    lockOnTransform = lockOnTarget.GetTarget(changeTargetLeft);
                    states.lockOnTransform = lockOnTransform;
                }
            }

            // Theo dõi mục tiêu và xử lý quay camera
            FollowTarget(d);
            HandleRotations(d, v, h, targetSpeed);
        }

        // Hàm để theo dõi mục tiêu (di chuyển camera theo mục tiêu)
        void FollowTarget(float d)
        {
            float speed = d * followSpeed;
            // Di chuyển camera mượt mà đến vị trí của mục tiêu
            Vector3 targetPosition = Vector3.Lerp(transform.position, target.position, speed);
            transform.position = targetPosition;
        }

        // Hàm xử lý quay camera
        void HandleRotations(float d, float v, float h, float targetSpeed)
        {
            // Xử lý mượt mà cho trục X và Y (quay ngang và dọc)
            if (turnSmoothing > 0)
            {
                smoothX = Mathf.SmoothDamp(smoothX, h, ref smoothXvelocity, turnSmoothing);
                smoothY = Mathf.SmoothDamp(smoothY, v, ref smoothYvelocity, turnSmoothing);
            }
            else
            {
                smoothX = h;
                smoothY = v;
            }

            // Xử lý quay dọc (tilt up and down)
            tiltAngle -= smoothY * targetSpeed;
            tiltAngle = Mathf.Clamp(tiltAngle, minAngle, maxAngle);  // Giới hạn góc quay dọc
            pivot.localRotation = Quaternion.Euler(tiltAngle, 0, 0);  // Cập nhật quay dọc của camera

            // Xử lý quay camera khi đang khóa mục tiêu
            if (lockOn && lockOnTarget != null && states.run == false)
            {
                // Tính toán hướng từ camera đến mục tiêu khóa
                targetDir = lockOnTransform.position - transform.position;
                targetDir.Normalize();

                if (targetDir == Vector3.zero)
                    targetDir = transform.forward;

                Quaternion targetRot = Quaternion.LookRotation(targetDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, d * 9);  // Quay camera mượt mà về hướng mục tiêu

                // Cập nhật góc quay ngang sau khi thoát khỏi chế độ khóa mục tiêu
                lookAngle = transform.eulerAngles.y;
                return;
            }

            // Xử lý quay camera bình thường (khi không khóa mục tiêu)
            lookAngle += smoothX * targetSpeed;
            transform.rotation = Quaternion.Euler(0, lookAngle, 0);
        }

        // Đảm bảo rằng chỉ có một CameraManager trong game
        public static CameraManager singleton;
        void Awake()
        {
            singleton = this;
        }
    }
}
