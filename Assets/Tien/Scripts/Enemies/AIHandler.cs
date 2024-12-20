using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    public class AIHandler : MonoBehaviour
    {
        public AIAttacks[] ai_attacks; // Mảng các cuộc tấn công của AI

        public EnemyStates states; // Trạng thái của kẻ thù

        public StateManager en_states; // Quản lý trạng thái của kẻ thù
        public Transform target; // Mục tiêu mà AI đang nhắm đến

        public float sight; // Tầm nhìn của AI
        public float fov_angle; // Góc nhìn của AI

        public int closeCount = 10; // Số khung hình để kiểm tra gần
        int _close; // Biến đếm số khung hình đã qua khi AI ở gần mục tiêu

        public int frameCount = 30; // Số khung hình để kiểm tra tầm nhìn xa
        int _frame; // Biến đếm số khung hình đã qua khi AI ở xa mục tiêu

        public int attackCount = 30; // Số khung hình để kiểm tra tấn công
        int _attack; // Biến đếm số khung hình đã qua khi AI tấn công

        float dis; // Khoảng cách từ AI đến mục tiêu
        float angle; // Góc nhìn từ AI đến mục tiêu
        float delta; // Thay đổi thời gian giữa các khung hình
        Vector3 dirToTarget; // Hướng từ AI đến mục tiêu

        void Start()
        {
            if (states == null)
                states = GetComponent<EnemyStates>(); // Nếu không có trạng thái, lấy từ thành phần EnemyStates

            states.Init(); // Khởi tạo trạng thái
            InitDamageColliders(); // Khởi tạo các collider gây sát thương
        }

        void InitDamageColliders()
        {
            for (int i = 0; i < ai_attacks.Length; i++)
            {
                for (int j = 0; j < ai_attacks[i].damageCollider.Length; j++)
                {
                    DamageCollider d = ai_attacks[i].damageCollider[j].GetComponent<DamageCollider>();
                    d.InitEnemy(states); // Khởi tạo collider gây sát thương với trạng thái kẻ thù
                }
            }

            for (int i = 0; i < states.defaultDamageCollider.Length; i++)
            {
                DamageCollider d = states.defaultDamageCollider[i].GetComponent<DamageCollider>();
                d.InitEnemy(states); // Khởi tạo các collider gây sát thương mặc định với trạng thái kẻ thù
            }
        }

        public enum AIState
        {
            far, close, inSight, attacking
        }
        public AIState aiState; // Trạng thái của AI

        void Update()
        {
            delta = Time.deltaTime; // Thay đổi thời gian giữa các khung hình
            dis = DistanceFromTarget(); // Khoảng cách từ AI đến mục tiêu
            angle = AngleToTarget(); // Góc nhìn từ AI đến mục tiêu
            if (target)
                dirToTarget = target.position - transform.position; // Tính toán hướng từ AI đến mục tiêu
            states.dirToTarget = dirToTarget; // Cập nhật hướng đến mục tiêu trong trạng thái

            switch (aiState)
            {
                case AIState.far:
                    HandleFarSight(); // Xử lý khi AI ở xa mục tiêu
                    break;
                case AIState.close:
                    HandleCloseSight(); // Xử lý khi AI ở gần mục tiêu
                    break;
                case AIState.inSight:
                    Insight(); // Xử lý khi AI có tầm nhìn vào mục tiêu
                    break;
                case AIState.attacking:
                    if (states.canMove)
                    {
                        states.rotateToTarget = true; // Nếu có thể di chuyển, xoay AI về hướng mục tiêu
                        aiState = AIState.inSight; // Chuyển trạng thái sang tầm nhìn
                    }
                    break;
                default:
                    break;
            }

            states.Tick(delta); // Cập nhật trạng thái
        }

        void GoToTarget()
        {
            states.hasDestination = false; // Đặt đích đến của AI thành không
            states.SetDestination(target.position); // Đặt điểm đến cho AI
        }

        void Insight()
        {
            #region delay handler

            HandleCooldowns(); // Xử lý thời gian hồi chiêu

            float d2 = Vector3.Distance(states.targetDestionation, target.position); // Khoảng cách đến mục tiêu
            if (d2 > 2 || dis > sight * 5)
                GoToTarget(); // Nếu xa mục tiêu, đặt điểm đến mới
            if (dis < 2)
                states.agent.isStopped = true; // Dừng di chuyển khi rất gần mục tiêu

            if (_attack > 0)
            {
                _attack--;
                return; // Giảm số lượng khung hình tấn công và thoát nếu chưa đến lượt
            }
            _attack = attackCount; // Đặt lại số lượng khung hình tấn công

            #endregion

            #region perform attack

            AIAttacks a = WillAttack(); // Xác định cuộc tấn công
            states.SetCurrentAttack(a); // Đặt cuộc tấn công hiện tại

            if (a != null && en_states.isDead == false)
            {
                aiState = AIState.attacking; // Chuyển trạng thái sang tấn công
                states.anim.Play(a.targetAnim); // Phát hoạt ảnh tấn công
                states.anim.SetBool("OnEmpty", false); // Đặt trạng thái không còn vũ khí
                states.canMove = false; // Không cho phép di chuyển khi tấn công
                a._cool = a.cooldown; // Đặt thời gian hồi chiêu
                states.agent.isStopped = true; // Dừng di chuyển của AI
                states.rotateToTarget = false; // Ngừng xoay về hướng mục tiêu
                return;
            }
            #endregion

            return;
        }

        void HandleCooldowns()
        {
            for (int i = 0; i < ai_attacks.Length; i++)
            {
                AIAttacks a = ai_attacks[i];
                if (a._cool > 0)
                {
                    a._cool -= delta; // Giảm thời gian hồi chiêu theo thời gian
                    if (a._cool < 0)
                    {
                        a._cool = 0; // Đảm bảo thời gian hồi chiêu không âm
                    }
                    continue;
                }
            }
        }

        public AIAttacks WillAttack()
        {
            int w = 0;
            List<AIAttacks> l = new List<AIAttacks>(); // Danh sách các cuộc tấn công có thể
            for (int i = 0; i < ai_attacks.Length; i++)
            {
                AIAttacks a = ai_attacks[i];
                if (a._cool > 0)
                    continue; // Bỏ qua nếu đang trong thời gian hồi chiêu
                if (dis > a.minDistance)
                    continue; // Bỏ qua nếu xa hơn khoảng cách tối thiểu
                if (angle < a.minAngle)
                    continue; // Bỏ qua nếu góc nhìn nhỏ hơn góc tối thiểu
                if (angle > a.maxAngle)
                    continue; // Bỏ qua nếu góc nhìn lớn hơn góc tối đa
                if (a.weight == 0)
                    continue; // Bỏ qua nếu trọng số bằng 0

                w += a.weight; // Cộng trọng số
                l.Add(a); // Thêm cuộc tấn công vào danh sách
            }

            if (l.Count == 0)
                return null; // Không có cuộc tấn công khả dụng

            int ran = Random.Range(0, w + 1); // Chọn một cuộc tấn công ngẫu nhiên theo trọng số
            int c_w = 0;
            for (int i = 0; i < l.Count; i++)
            {
                c_w += l[i].weight;
                if (c_w > ran)
                {
                    return l[i]; // Trả về cuộc tấn công được chọn
                }
            }

            return null;
        }

        void HandleFarSight()
        {
            if (target == null)
                return;

            _frame++; // Tăng số lượng khung hình
            if (_frame > frameCount)
            {
                _frame = 0; // Đặt lại số lượng khung hình

                if (dis < sight)
                {
                    if (angle < fov_angle)
                    {
                        aiState = AIState.close; // Chuyển trạng thái sang gần
                    }
                }
            }
        }

        void HandleCloseSight()
        {
            _close++; // Tăng số lượng khung hình
            if (_close > closeCount)
            {
                _close = 0; // Đặt lại số lượng khung hình

                if (dis > sight || angle > fov_angle)
                {
                    aiState = AIState.far; // Chuyển trạng thái sang xa
                    return;
                }
            }
            RaycastToTarget(); // Thực hiện raycast đến mục tiêu
        }

        void RaycastToTarget()
        {
            RaycastHit hit;
            Vector3 origin = transform.position;
            origin.y += 0.5f; // Điều chỉnh vị trí gốc
            Vector3 dir = dirToTarget;
            dir.y += 0.5f; // Điều chỉnh hướng

            if (Physics.Raycast(origin, dir, out hit, sight, states.ignoreLayers))
            {
                StateManager st = hit.transform.GetComponentInParent<StateManager>();
                if (st != null)
                {
                    states.rotateToTarget = true; // Xoay về hướng mục tiêu khi phát hiện
                    aiState = AIState.inSight; // Chuyển trạng thái sang tầm nhìn
                }
            }
        }

        float DistanceFromTarget()
        {
            if (target == null)
                return 100; // Nếu không có mục tiêu, trả về giá trị lớn

            return Vector3.Distance(target.position, transform.position); // Tính khoảng cách
        }

        float AngleToTarget()
        {
            float a = 180;
            if (target)
            {
                Vector3 d = dirToTarget;
                a = Vector3.Angle(d, transform.forward); // Tính góc nhìn từ AI đến mục tiêu
            }
            return a;
        }
    }

    [System.Serializable]
    public class AIAttacks
    {
        public int weight; // Trọng số của cuộc tấn công
        public float minDistance; // Khoảng cách tối thiểu để tấn công
        public float minAngle; // Góc tối thiểu để tấn công
        public float maxAngle; // Góc tối đa để tấn công

        public float cooldown = 2; // Thời gian hồi chiêu
        public float _cool; // Thời gian hồi chiêu hiện tại

        public string targetAnim; // Tên hoạt ảnh tấn công

        public bool isDefaultDamageCollider; // Kiểm tra có phải collider gây sát thương mặc định không
        public GameObject[] damageCollider; // Mảng các collider gây sát thương
    }
}
