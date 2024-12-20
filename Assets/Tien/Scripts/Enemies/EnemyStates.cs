using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

namespace SA
{
    public class EnemyStates : MonoBehaviour
    {
        [Header("Stats")]
        public int health;
        public int maxHealth;
        public CharacterStats characterStats;

        [Header("Value")]
        public float delta;
        public float vertical;
        public float horizontal;
        public float poiseDegrade = 2f;

        [Header("States")]
        public bool canBeParried = true;
        public bool parryIsOn = true;
        public bool isInvincible;
        public bool dontDoAnything;
        public bool canMove;
        public bool isDead;
        public bool hasDestination;
        public Vector3 targetDestionation;
        public Vector3 dirToTarget;
        public bool rotateToTarget;
        public bool isGreatSword;
        public bool damaged = false;

        //References
        public Animator anim;
        public Rigidbody rigid;
        EnemyTarget enTarget;
        AnimatorHook a_hook;
        public StateManager parriedBy;
        public LayerMask ignoreLayers;
        public NavMeshAgent agent;
        public StateManager player;
        public GameObject lockOnGameObject;
        public Canvas enemyCanvas;
        public GameObject dropGameObject;
        public AudioSource audioSource;

        //Attacks
        AIAttacks curAttack;
        public void SetCurrentAttack(AIAttacks a)
        {
            curAttack = a;
        }
        public AIAttacks GetCurrentAttack()
        {
            return curAttack;
        }

        public GameObject[] defaultDamageCollider;


        List<Rigidbody> ragdollRigids = new List<Rigidbody>();
        List<Collider> ragdollColliders = new List<Collider>();

        float timer;
        Image healthBar;

        public delegate void SpellEffect_Loop();
        public SpellEffect_Loop spellEffect_loop;



        public void Init()
        {
            anim = GetComponentInChildren<Animator>(); // Lấy Animator từ đối tượng con
            audioSource = this.gameObject.AddComponent<AudioSource>(); // Thêm AudioSource
            audioSource.maxDistance = 3.5f; // Thiết lập khoảng cách âm thanh phát ra
            enTarget = GetComponent<EnemyTarget>(); // Lấy EnemyTarget
            enTarget.Init(this); // Khởi tạo EnemyTarget với chính nó

            rigid = GetComponent<Rigidbody>(); // Lấy Rigidbody
            agent = GetComponent<NavMeshAgent>(); // Lấy NavMeshAgent
            rigid.isKinematic = true; // Đặt Rigidbody là kinematic

            a_hook = anim.GetComponent<AnimatorHook>(); // Lấy AnimatorHook từ Animator
            if (a_hook == null)
                a_hook = anim.gameObject.AddComponent<AnimatorHook>(); // Thêm AnimatorHook nếu không tồn tại

            enemyCanvas = GetComponentInChildren<Canvas>(); // Lấy Canvas từ đối tượng con

            a_hook.Init(null, this); // Khởi tạo AnimatorHook
            InitRagDoll(); // Khởi tạo ragdoll
            parryIsOn = false; // Tắt parry mặc định

            gameObject.layer = 8; // Đặt lớp của game object
            ignoreLayers = ~(1 << 9); // Cài đặt các lớp mà kẻ thù bỏ qua

            lockOnGameObject.SetActive(false); // Tắt hiển thị đối tượng khóa mục tiêu
            healthBar = enemyCanvas.transform.Find("HealthBG").Find("Health").GetComponent<Image>(); // Lấy thanh sức khỏe từ Canvas
            enemyCanvas.gameObject.SetActive(false); // Tắt Canvas
            health = maxHealth; // Đặt sức khỏe bắt đầu
        }


        void InitRagDoll()
        {
            Rigidbody[] rigs = GetComponentsInChildren<Rigidbody>(); // Lấy tất cả các Rigidbody từ đối tượng con
            for (int i = 0; i < rigs.Length; i++)
            {
                if (rigs[i] == rigid)
                    continue;

                ragdollRigids.Add(rigs[i]); // Thêm Rigidbody vào danh sách ragdoll
                rigs[i].isKinematic = true; // Đặt Rigidbody là kinematic

                Collider col = rigs[i].gameObject.GetComponent<Collider>(); // Lấy Collider
                col.isTrigger = true; // Đặt Collider thành trigger
                ragdollColliders.Add(col); // Thêm Collider vào danh sách ragdoll
            }
        }

        // Enables ragdoll physics
        public void EnableRagdoll()
        {
            for (int i = 0; i < ragdollRigids.Count; i++)
            {
                ragdollRigids[i].isKinematic = false; // Bỏ kinematic khỏi Rigidbody
                ragdollColliders[i].isTrigger = false; // Bỏ trigger khỏi Collider
                ragdollRigids[i].detectCollisions = false; // Tắt phát hiện va chạm
            }

            Collider controllerCollider = rigid.gameObject.GetComponent<Collider>();
            controllerCollider.enabled = false; // Tắt Collider của Rigidbody chính
            rigid.isKinematic = true; // Đặt Rigidbody chính là kinematic

            StartCoroutine("CloseAnimator"); // Ngừng hoạt ảnh sau khi kết thúc khung hình
        }

        IEnumerator CloseAnimator()
        {
            yield return new WaitForEndOfFrame(); // Đợi một khung hình kết thúc
            anim.enabled = false; // Tắt Animator
            this.enabled = false; // Tắt script
        }

        // Update method for each frame
        public void Tick(float d)
        {
            if (isGreatSword)
                anim.Play("gs_oh_idle_r"); // Chạy hoạt ảnh nếu sử dụng kiếm lớn

            delta = d; // Cập nhật delta time
            delta = Time.deltaTime; // Thay đổi thời gian giữa các khung hình
            canMove = anim.GetBool("OnEmpty"); // Kiểm tra khả năng di chuyển

            if (enTarget != null)
            {
                if (player.lockOnTarget == null)
                    enTarget.isLockOn = false;

                if (enTarget.isLockOn)
                {
                    lockOnGameObject.SetActive(true); // Hiển thị đối tượng khóa mục tiêu
                }
                else
                {
                    lockOnGameObject.SetActive(false); // Ẩn đối tượng khóa mục tiêu
                }
            }

            if (damaged)
                enemyCanvas.gameObject.SetActive(true); // Hiển thị Canvas khi bị hư hại

            UpdateEnemyHealthUI(health, maxHealth); // Cập nhật thanh sức khỏe

            if (spellEffect_loop != null)
                spellEffect_loop(); // Gọi hiệu ứng phép thuật liên tục

            if (dontDoAnything)
            {
                dontDoAnything = !canMove; // Đổi trạng thái không làm gì
                return;
            }

            if (rotateToTarget)
            {
                LookTowardsTarget(); // Xoay về phía mục tiêu
            }

            if (health <= 0)
            {
                if (!isDead)
                {
                    isDead = true; // Đánh dấu kẻ thù đã chết
                    enemyCanvas.gameObject.SetActive(false); // Ẩn Canvas
                    audioSource.PlayOneShot(ResourceManager.singleton.GetAudio("die").audio_clip); // Phát âm thanh chết
                    EnableRagdoll(); // Kích hoạt ragdoll
                    StartCoroutine(StartSinking()); // Bắt đầu quá trình chìm
                }
            }

            if (isInvincible)
            {
                isInvincible = !canMove; // Đổi trạng thái bất khả xâm phạm
            }

            if (parriedBy != null && parryIsOn == false)
            {
                parriedBy = null; // Xóa đối tượng đã parry nếu không còn parry
            }

            if (canMove)
            {
                parryIsOn = false; // Tắt trạng thái parry khi di chuyển
                anim.applyRootMotion = false; // Không áp dụng root motion

                MovementAnimation(); // Cập nhật hoạt ảnh di chuyển
            }
            else
            {
                if (anim.applyRootMotion == false)
                    anim.applyRootMotion = true; // Bật root motion khi không di chuyển
            }

            characterStats.poise -= delta * poiseDegrade; // Giảm poise
            if (characterStats.poise < 0)
                characterStats.poise = 0; // Đảm bảo poise không âm
            damaged = false; // Đặt trạng thái bị hư hại thành false
        }

        // Updates movement animation
        public void MovementAnimation()
        {
            float square = agent.desiredVelocity.sqrMagnitude; // Lấy tốc độ bình phương của agent
            float v = Mathf.Clamp(square, 0, 0.5f); // Giới hạn giá trị tốc độ di chuyển
            anim.SetFloat("vertical", v, 0.2f, delta); // Cập nhật tham số "vertical" trong Animator
        }

        // Makes the enemy look towards the target
        void LookTowardsTarget()
        {
            Vector3 dir = dirToTarget; // Hướng đến mục tiêu
            dir.y = 0; // Đặt thành 0 để không thay đổi trục Y
            Quaternion targetRotation = Quaternion.LookRotation(dir); // Tạo góc quay để nhìn về mục tiêu
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, delta * 5); // Xoay về hướng mục tiêu
        }

        // Sets the destination for the NavMeshAgent
        public void SetDestination(Vector3 d)
        {
            if (agent)
            {
                if (!hasDestination)
                {
                    hasDestination = true; // Đánh dấu có điểm đến
                    agent.isStopped = false; // Bỏ dừng NavMeshAgent
                    agent.SetDestination(d); // Đặt điểm đến mới
                    targetDestionation = d; // Lưu điểm đến
                }
            }
        }

        // Applies damage to the enemy
        public void DoDamage()
        {
            if (isInvincible)
                return; // Không nhận sát thương nếu bất khả xâm phạm

            damaged = true; // Đánh dấu bị hư hại
            rotateToTarget = true; // Xoay về phía mục tiêu
            int damage = 20; // Sát thương cơ bản
            health -= damage; // Giảm sức khỏe
            audioSource.PlayOneShot(ResourceManager.singleton.GetAudio("slash_impact").audio_clip); // Phát âm thanh tấn công

            if (canMove)
            {
                int ran = Random.Range(0, 100);
                string tA = (ran > 50) ? "damage_1" : "damage_2"; // Chọn hoạt ảnh ngẫu nhiên
                anim.Play(tA); // Chạy hoạt ảnh tấn công
            }

            isInvincible = true; // Bật trạng thái bất khả xâm phạm
            anim.applyRootMotion = true; // Bật root motion
            anim.SetBool("canMove", false); // Tắt khả năng di chuyển
        }

        // Applies spell damage to the enemy
        public void DoDamageSpell()
        {
            if (isInvincible)
                return; // Không nhận sát thương nếu bất khả xâm phạm

            health -= 50; // Giảm sức khỏe
            audioSource.PlayOneShot(ResourceManager.singleton.GetAudio("damage_3").audio_clip); // Phát âm thanh phép thuật
            damaged = true; // Đánh dấu bị hư hại
            rotateToTarget = true; // Xoay về phía mục tiêu
            anim.Play("damage_3"); // Chạy hoạt ảnh bị phép thuật tấn công
        }

        // Handles the case when the enemy's attack is blocked
        public void HandleBlocked()
        {
            audioSource.PlayOneShot(ResourceManager.singleton.GetAudio("shield_impact").audio_clip); // Phát âm thanh khi bị chặn
            anim.Play("attack_interrupt"); // Chạy hoạt ảnh bị gián đoạn
            anim.SetFloat("interruptSpeed", 1.2f); // Đặt tốc độ gián đoạn
            player.characterStats._stamina -= 40; // Giảm stamina của người chơi
            Vector3 targetDir = transform.position - player.transform.position; // Hướng từ kẻ thù đến người chơi
            player.SnapToRotation(targetDir); // Xoay người chơi về hướng kẻ thù
            CloseDamageCollider(); // Đóng collider gây sát thương
        }

        // Checks if the enemy can be parried
        public void CheckForParry(Transform target, StateManager states)
        {
            if (canBeParried == false || parryIsOn == false || isInvincible)
                return; // Không thể parry nếu không cho phép, không bật hoặc bất khả xâm phạm

            Vector3 dir = transform.position - target.position; // Hướng từ kẻ thù đến mục tiêu
            dir.Normalize(); // Chuẩn hóa hướng
            float dot = Vector3.Dot(target.forward, dir); // Tính toán góc giữa mục tiêu và kẻ thù

            if (dot < 0)
                return; // Nếu không phải mặt đối diện thì không parry

            isInvincible = true; // Bật trạng thái bất khả xâm phạm
            anim.Play("attack_interrupt"); // Chạy hoạt ảnh bị gián đoạn
            anim.SetFloat("interruptSpeed", 0.5f); // Đặt tốc độ gián đoạn
            anim.applyRootMotion = true; // Bật root motion
            anim.SetBool("canMove", false); // Tắt khả năng di chuyển
            parriedBy = states; // Lưu trạng thái parry
        }

        // Handles the enemy being parried
        public void IsGettingParried(Action a, Weapon curWeapon)
        {
            float damage = 80; // Sát thương nhận được từ parry
            health -= Mathf.RoundToInt(damage); // Giảm sức khỏe
            dontDoAnything = true; // Đặt trạng thái không làm gì
            anim.SetBool("canMove", false); // Tắt khả năng di chuyển
            anim.Play("parry_received"); // Chạy hoạt ảnh bị parry
        }

        // Handles the enemy being backstabbed
        public void IsGettingBackStabbed(Action a, Weapon curWeapon)
        {
            dontDoAnything = true; // Đặt trạng thái không làm gì
            anim.SetBool("canMove", false); // Tắt khả năng di chuyển
            anim.Play("backstab_received"); // Chạy hoạt ảnh bị backstab
            StartCoroutine(PlaySlashImpact()); // Chạy hiệu ứng âm thanh
            StartCoroutine(SetHealth()); // Thay đổi sức khỏe sau một khoảng thời gian
        }

        IEnumerator SetHealth()
        {
            yield return new WaitForSeconds(2.07f); // Đợi một khoảng thời gian
            health = 0; // Đặt sức khỏe về 0
        }

        IEnumerator PlaySlashImpact()
        {
            yield return new WaitForSeconds(0.5f); // Đợi một khoảng thời gian
            audioSource.PlayOneShot(ResourceManager.singleton.GetAudio("slash_impact").audio_clip); // Phát âm thanh tấn công
        }

        public ParticleSystem fireParticle; // Hệ thống hạt lửa
        float _t; // Thay đổi thời gian cho hiệu ứng lửa

        // Emits fire particles
        public void OnFire()
        {
            if (fireParticle == null)
                return; // Không có hệ thống hạt lửa thì thoát

            if (_t < 3)
            {
                _t += Time.deltaTime; // Cập nhật thời gian
                fireParticle.Emit(1); // Phát ra một hạt lửa
            }
            else
            {
                _t = 0; // Đặt lại thời gian
                spellEffect_loop = null; // Xóa hiệu ứng phép thuật
            }
        }

        // Opens damage colliders
        public void OpenDamageCollier()
        {
            if (curAttack == null)
                return; // Không có tấn công hiện tại thì thoát

            if (curAttack.isDefaultDamageCollider || curAttack.damageCollider.Length == 0)
            {
                ObjectListStatus(defaultDamageCollider, true); // Mở collider mặc định
            }
            else
            {
                ObjectListStatus(curAttack.damageCollider, true); // Mở collider của tấn công hiện tại
            }
        }

        // Closes damage colliders
        public void CloseDamageCollider()
        {
            if (curAttack == null)
                return; // Không có tấn công hiện tại thì thoát

            if (curAttack.isDefaultDamageCollider || curAttack.damageCollider.Length == 0)
            {
                ObjectListStatus(defaultDamageCollider, false); // Đóng collider mặc định
            }
            else
            {
                ObjectListStatus(curAttack.damageCollider, false); // Đóng collider của tấn công hiện tại
            }
        }

        // Sets the status of a list of game objects
        void ObjectListStatus(GameObject[] l, bool status)
        {
            for (int i = 0; i < l.Length; i++)
            {
                l[i].SetActive(status); // Cập nhật trạng thái của từng game object
            }
        }

        // Starts sinking and handles the drop item
        IEnumerator StartSinking()
        {
            this.GetComponent<CapsuleCollider>().enabled = false; // Tắt CapsuleCollider
            player.lockOnTarget = null; // Xóa mục tiêu khóa của người chơi
            EnemyManager.singleton.enemyTargets.Remove(enTarget); // Xóa kẻ thù khỏi danh sách mục tiêu
            yield return new WaitForSeconds(0.8f); // Đợi một khoảng thời gian
            HandleDropItem(); // Xử lý rơi item
            Destroy(this.gameObject); // Hủy đối tượng kẻ thù
        }

        // Handles dropping an item
        void HandleDropItem()
        {
            GameObject go = Instantiate(dropGameObject) as GameObject; // Tạo đối tượng item mới
            go.transform.position = this.transform.position; // Đặt vị trí của item tại vị trí của kẻ thù
            player.pickManager.pick_items.Add(go.GetComponent<PickableItem>()); // Thêm item vào danh sách pickable
        }

        void UpdateEnemyHealthUI(int curHealth, int maxHealth)
        {
            healthBar.fillAmount = (float)curHealth / (float)maxHealth;
        }
    }
}