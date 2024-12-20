using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SA
{
    public class StateManager : MonoBehaviour
    {
        [Header("Init")]
        public GameObject activeModel;
        public Image damageImage;

        [Header("Character Stats")]
        public Attributes attributes;
        public CharacterStats characterStats;
        public WeaponStats weaponStats;

        [Header("Inputs")]
        public float vertical;
        public float horizontal;
        public float moveAmount;
        public Vector3 moveDir;
        public bool rt, rb, lt, lb;
        public bool rollInput;
        public bool itemInput;


        [Header("Stats")]
        public float moveSpeed = 3f;
        public float rotateSpeed = 5f;
        public float toGround = 0.5f;
        public float rollSpeed = 1;
        public float parryOffset = 1.4f;
        public float backstabOffset = 1.4f;

        [Header("States")]
        public bool run;
        public bool onGround;
        public bool lockOn;
        public bool inAction;
        public bool canMove;
        public bool canRotate;
        public bool canAttack;
        public bool isSpellCasting;
        public bool enableIK;
        public bool isTwoHanded;
        public bool usingItem;
        public bool isBlocking;
        public bool isLeftHand;
        public bool canBeParried;
        public bool parryIsOn;
        public bool onEmpty;
        public bool isInvincible;
        public bool damaged;
        public bool isDead = false;


        [Header("Others")]
        public EnemyTarget lockOnTarget;
        public Transform lockOnTransform;
        public AnimationCurve roll_curve;

        [HideInInspector]
        public Animator anim;
        [HideInInspector]
        public Rigidbody rigid;
        [HideInInspector]
        public AnimatorHook a_hook;
        [HideInInspector]
        public ActionManager actionManager;
        [HideInInspector]
        public InventoryManager inventoryManager;
        [HideInInspector]
        public PickableItemsManager pickManager;
        [HideInInspector]
        public AudioSource audio_source;
        [HideInInspector]
        public AudioClip audio_clip;
        //public SceneManager sceneController;


        public float delta;
        [HideInInspector]
        public LayerMask ignoreLayers;

        [HideInInspector]
        public Action currentAction;


        [HideInInspector]
        public ActionInput storeActionInput;
        public ActionInput storePreviousAction;

        float _actionDelay;
        float flashSpeed = 5f;
        Color flashColour = new Color(1f, 0f, 0f, 0.1f);

        //-----------------------------------------------------------------------

        //-------------------setters--------------------------
        public void Init()
        {
            SetUpAnimator();
            rigid = GetComponent<Rigidbody>();
            rigid.angularDrag = 999; // Đặt ma sát xoay
            rigid.drag = 4; // Đặt ma sát di chuyển
            rigid.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ; // Khóa quay quanh trục X và Z

            inventoryManager = GetComponent<InventoryManager>(); // Lấy InventoryManager
            inventoryManager.Init(this); // Khởi tạo InventoryManager

            actionManager = GetComponent<ActionManager>(); // Lấy ActionManager
            actionManager.Init(this); // Khởi tạo ActionManager

            pickManager = GetComponent<PickableItemsManager>(); // Lấy PickableItemsManager

            a_hook = activeModel.GetComponent<AnimatorHook>(); // Lấy AnimatorHook từ activeModel
            if (a_hook == null)
                // Thêm component AnimatorHook vào activeModel nếu chưa có
                a_hook = activeModel.AddComponent<AnimatorHook>();

            a_hook.Init(this, null); // Khởi tạo AnimatorHook

            audio_source = activeModel.GetComponent<AudioSource>(); // Lấy AudioSource từ activeModel

            // Bỏ qua LayerMask của collider khi tiếp xúc.
            gameObject.layer = 8; // Đặt layer của gameObject
            ignoreLayers = ~(1 << 9); // Bỏ qua các lớp cụ thể

            anim.SetBool("onGround", true); // Đặt biến boolean "onGround" cho Animator

            characterStats.InitCurrent(); // Khởi tạo thông số nhân vật hiện tại

            UIManager ui = UIManager.singleton; // Lấy UIManager singleton
            ui.AffectAll(characterStats.hp, characterStats.fp, characterStats.stamina); // Cập nhật UI với thông số nhân vật
            ui.InitSouls(characterStats._souls); // Khởi tạo số linh hồn

            DialogueManager.singleton.Init(this.transform); // Khởi tạo DialogueManager
        }

        void SetUpAnimator()
        {
            if (activeModel == null)
            {
                // Lấy Animator của đối tượng mô hình dưới controller
                anim = GetComponentInChildren<Animator>();

                if (anim == null)
                    Debug.Log("No model found"); // Thông báo nếu không tìm thấy mô hình
                else
                    // Lấy game object chứa Animator
                    activeModel = anim.gameObject;
            }

            if (anim == null)
                anim = activeModel.GetComponent<Animator>(); // Lấy Animator từ activeModel nếu chưa có

            anim.applyRootMotion = false; // Không áp dụng root motion
        }

        //--------------------runner------------------------
        public void FixedTick(float d)
        {
            delta = d;
            isBlocking = false;
            rigid.constraints &= ~RigidbodyConstraints.FreezePositionY; // Bỏ khóa chuyển động theo trục Y

            //-----------------Xử lý hành động, tương tác và trạng thái--------------------

            //_________Tấn công (inAction)_______ 
            if (onGround == true)
            {
                usingItem = anim.GetBool("interacting"); // Lấy giá trị boolean "interacting"
                anim.SetBool("spellcasting", isSpellCasting); // Đặt giá trị boolean "spellcasting"
                if (inventoryManager.rightHandWeapon != null)
                    inventoryManager.rightHandWeapon.weaponModel.SetActive(!usingItem); // Kích hoạt hoặc hủy kích hoạt mô hình vũ khí
                if (inventoryManager.curConsumable != null)
                {
                    if (inventoryManager.curConsumable.itemModel != null)
                        inventoryManager.curConsumable.itemModel.SetActive(usingItem); // Kích hoạt hoặc hủy kích hoạt mô hình vật phẩm tiêu dùng
                }

                if (isBlocking == false && isSpellCasting == false)
                    enableIK = false; // Tắt IK nếu không chặn và không sử dụng phép thuật

                if (inAction)
                { //"inAction" evaluation.
                    anim.applyRootMotion = true; // Áp dụng root motion khi đang hành động
                    _actionDelay += delta;
                    if (_actionDelay > 0.3f)
                    { // Để lại khoảng trống: nếu hành động kéo dài hơn 0.3 giây, đặt lại để thực hiện hành động khác
                        inAction = false;
                        _actionDelay = 0;
                    }
                    else
                    {
                        return;
                    }
                }
            }
            //_____Bắt đầu trạng thái______
            onEmpty = anim.GetBool("OnEmpty"); // Lấy giá trị boolean "OnEmpty"
                                               //canMove = anim.GetBool("canMove"); // Lấy giá trị boolean "canMove" (bỏ qua)

            if (onEmpty)
            {
                canMove = true; // Cho phép di chuyển nếu không còn gì
                canAttack = true; // Cho phép tấn công nếu không còn gì
            }

            if (canRotate)
                HandleRotation(); // Xử lý quay

            if (!onEmpty && !canMove && !canAttack) // Dừng cập nhật khi tất cả các biến quản lý trạng thái đều là false (có thể nhân vật đang hành động)
                return;

            if (canMove && !onEmpty)
            {
                if (moveAmount > 0)
                {
                    anim.CrossFade("Empty Override", 0.1f); // Chuyển đổi hoạt ảnh khi di chuyển
                    onEmpty = true;
                }
            }

            if (canAttack)
                DetectAction(); // Xác định hành động tấn công
            if (canMove)
                DetectItemAction(); // Xác định hành động với vật phẩm

            // Tắt RootMotion sau khi hoạt ảnh đã được phát.
            anim.applyRootMotion = false;

            if (inventoryManager.blockCollider.gameObject.activeSelf)
            {
                isInvincible = true; // Bỏ qua sát thương nếu collider đang hoạt động
            }

            //_____Kết thúc trạng thái_____

            // --------Xử lý chuyển động và vật lý-------
            // Vật lý
            if (moveAmount > 0 || !onGround)
            {
                rigid.drag = 0; // Đặt ma sát di chuyển thành 0 nếu đang di chuyển hoặc không trên mặt đất
            }
            else
                rigid.drag = 4; // Đặt ma sát di chuyển

            // Di chuyển
            if (usingItem || isSpellCasting)
            {
                run = false; // Không chạy khi đang sử dụng vật phẩm hoặc phép thuật
                moveAmount = Mathf.Clamp(moveAmount, 0, 0.5f); // Giới hạn giá trị moveAmount
            }

            if (onGround && canMove)
                rigid.velocity = moveDir * (moveSpeed * moveAmount); // Cập nhật vận tốc của rigidbody

            if (run)
            {
                moveSpeed = 5.5f; // Tốc độ di chuyển khi chạy
                lockOn = false; // Không khóa mục tiêu
            }
            else
            {
                moveSpeed = 3f; // Tốc độ di chuyển bình thường
            }

            HandleRotation(); // Xử lý quay

            // ------Xử lý hoạt ảnh chuyển động------
            anim.SetBool("lockon", lockOn); // Đặt giá trị boolean "lockon"
            if (lockOn == false)
                HandleMovementAnimation(); // Xử lý hoạt ảnh di chuyển khi không khóa mục tiêu
            else
                HandleLockOnAnimations(moveDir); // Xử lý hoạt ảnh khóa mục tiêu

            //anim.SetBool("blocking", isBlocking); // (Bỏ qua) Đặt giá trị boolean "blocking"
            anim.SetBool("isLeft", isLeftHand); // Đặt giá trị boolean "isLeft"

            //________________________
            a_hook.useIK = enableIK; // Đặt sử dụng IK cho AnimatorHook
            HandleBlocking(); // Xử lý chặn

            if (isSpellCasting)
            {
                HandleSpellCasting(); // Xử lý phép thuật
                return;
            }
            //_________________________

            // Cuộn (inAction)
            a_hook.CloseRoll(); // Đóng cuộn
            if (onGround == true)
                HandleRolls(); // Xử lý cuộn nếu trên mặt đất

            //_________________________
            if (lockOn == false)
                lockOnTarget = null; // Không có mục tiêu nếu không khóa mục tiêu
            if (lockOnTarget != null)
            {
                lockOnTarget.isLockOn = true; // Đặt trạng thái khóa mục tiêu cho đối tượng mục tiêu
            }
        }

        float i_timer;

        public void Tick(float d)
        {
            delta = d;
            onGround = OnGround(); // Xác định trạng thái trên mặt đất
            anim.SetBool("onGround", onGround); // Đặt giá trị boolean "onGround" cho Animator
            pickManager.Tick(); // Gọi Tick cho PickableItemsManager
            if (isInvincible)
            {
                i_timer += delta;
                if (i_timer > 0.5f)
                {
                    i_timer = 0;
                    isInvincible = false; // Hủy trạng thái bất khả xâm phạm sau 0.5 giây
                }
            }

            if (damaged)
            {
                damageImage.color = flashColour; // Đặt màu của damageImage khi bị thương
            }
            else
            {
                damageImage.color = Color.Lerp(damageImage.color, Color.clear, flashSpeed * Time.deltaTime); // Hiệu ứng màu sắc khi không bị thương
            }
            damaged = false; // Đặt trạng thái bị thương thành false
        }

        public bool IsInput()
        {
            if (rt || rb || lt || lb || rollInput)
                return true; // Kiểm tra xem có bất kỳ đầu vào nào không
            return false;
        }



        //-----------------definer------------------------
        void HandleRotation()
        {
            // Xử lý quay. (dựa trên hướng) (5)
            Vector3 targetDir = (lockOn == false) ? moveDir
                : (lockOnTransform != null) ? lockOnTransform.position - transform.position
                : moveDir;
            targetDir.y = 0; // Đặt giá trị y bằng 0 để quay quanh trục y
            if (targetDir == Vector3.zero)
                targetDir = transform.forward; // Nếu hướng mục tiêu là Vector3.zero, sử dụng hướng hiện tại

            Quaternion targetRot = Quaternion.LookRotation(targetDir); // Tạo quaternion từ hướng mục tiêu
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, delta * moveAmount * rotateSpeed); // Quay từ từ đến hướng mục tiêu
        }

        //************Item**********************
        public void DetectItemAction()
        {
            if (onEmpty == false || usingItem || isBlocking)
                return; // Không xử lý nếu không còn gì, đang sử dụng vật phẩm, hoặc đang chặn

            if (itemInput == false)
                return; // Không xử lý nếu không có đầu vào cho vật phẩm

            if (inventoryManager.curConsumable == null)
                return; // Không xử lý nếu không có vật phẩm tiêu dùng hiện tại

            if (inventoryManager.curConsumable.itemCount < 1 && inventoryManager.curConsumable.unlimitedCount == false)
                return; // Không xử lý nếu số lượng vật phẩm tiêu dùng nhỏ hơn 1 và không có số lượng không giới hạn

            RuntimeConsumable slot = inventoryManager.curConsumable;

            string targetAnim = slot.instance.targetAnim;
            audio_clip = ResourceManager.singleton.GetAudio(slot.instance.audio_id).audio_clip;
            if (string.IsNullOrEmpty(targetAnim))
                return; // Không xử lý nếu không có hoạt ảnh mục tiêu

            usingItem = true; // Đặt trạng thái sử dụng vật phẩm
            anim.Play(targetAnim); // Phát hoạt ảnh
        }

        public void InteractLogic()
        {
            if (pickManager.interactionCandidate.actionType == UIActionType.talk)
            {
                audio_source.PlayOneShot(ResourceManager.singleton.GetAudio("hello").audio_clip); // Phát âm thanh khi nói chuyện
                pickManager.interactionCandidate.InteractActual(); // Thực hiện tương tác thực tế
                return;
            }

            Interactions interaction = ResourceManager.singleton.GetInteraction(pickManager.interactionCandidate.interactionId);

            if (interaction.oneShot)
            {
                if (pickManager.interactions.Contains(pickManager.interactionCandidate))
                {
                    pickManager.interactions.Remove(pickManager.interactionCandidate); // Loại bỏ tương tác nếu một lần
                }
            }

            Vector3 targetDir = pickManager.interactionCandidate.transform.position - transform.position;
            SnapToRotation(targetDir); // Đặt hướng của đối tượng

            pickManager.interactionCandidate.InteractActual(); // Thực hiện tương tác thực tế

            PlayAnimation(interaction.anim); // Phát hoạt ảnh tương tác
            pickManager.interactionCandidate = null; // Xóa đối tượng tương tác
        }

        public void SnapToRotation(Vector3 dir)
        {
            dir.Normalize(); // Chuẩn hóa hướng
            dir.y = 0; // Đặt giá trị y bằng 0
            if (dir == Vector3.zero)
                dir = transform.forward; // Nếu hướng là Vector3.zero, sử dụng hướng hiện tại
            Quaternion t = Quaternion.LookRotation(dir); // Tạo quaternion từ hướng
            transform.rotation = t; // Đặt hướng của đối tượng
        }

        public void PlayAnimation(string targetAnim)
        {
            onEmpty = false; // Đặt trạng thái không còn gì
            canMove = false; // Đặt trạng thái không thể di chuyển
            canAttack = false; // Đặt trạng thái không thể tấn công
            inAction = true; // Đặt trạng thái đang hành động
            isBlocking = false; // Đặt trạng thái không chặn
            anim.CrossFade(targetAnim, 0.2f); // Chuyển đổi hoạt ảnh với độ mờ 0.2 giây
        }

        //**********Actions*********************
        public void DetectAction()
        {
            // Nếu không thể di chuyển, thoát khỏi hàm
            if (canAttack == false && (onEmpty == false || usingItem || isSpellCasting))
                return;

            if (rb == false && rt == false && lt == false && lb == false)
                return; // Không xử lý nếu không có đầu vào hành động

            if (characterStats._stamina <= 8)
                return; // Không xử lý nếu năng lượng thấp

            ActionInput targetInput = actionManager.GetActionInput(this); // Lấy đầu vào hành động
            storeActionInput = targetInput;
            if (onEmpty == false)
            {
                a_hook.killDelta = true;
                targetInput = storePreviousAction; // Sử dụng hành động trước đó nếu không còn gì
            }

            storePreviousAction = targetInput;
            Action slot = actionManager.GetActionFromInput(targetInput); // Lấy hành động từ đầu vào

            if (slot == null)
                return; // Không xử lý nếu không có hành động

            switch (slot.type)
            {
                case ActionType.attack:
                    AttackAction(slot); // Xử lý hành động tấn công
                    break;
                case ActionType.block:
                    BlockAction(slot); // Xử lý hành động chặn
                    break;
                case ActionType.spells:
                    SpellAction(slot); // Xử lý hành động phép thuật
                    break;
                case ActionType.parry:
                    ParryAction(slot); // Xử lý hành động phản công
                    break;
            }
        }

        void AttackAction(Action slot)
        {
            if (characterStats._stamina < 5)
                return; // Không xử lý nếu năng lượng thấp

            if (CheckForParry(slot))
                return; // Không xử lý nếu đang phản công

            if (CheckForBackStab(slot))
                return; // Không xử lý nếu đang đánh lén

            string targetAnim = null;
            ActionAnim branch = slot.GetActionStep(ref actionManager.actionIndex).GetBranch(storeActionInput);
            targetAnim = branch.targetAnim;
            audio_clip = ResourceManager.singleton.GetAudio(branch.audio_ids).audio_clip;


            if (string.IsNullOrEmpty(targetAnim))
                return; // Không xử lý nếu không có hoạt ảnh mục tiêu

            currentAction = slot;
            canBeParried = slot.canBeParried;

            canAttack = false; // Đặt trạng thái không thể tấn công
            onEmpty = false; // Đặt trạng thái không còn gì
            canMove = false; // Đặt trạng thái không thể di chuyển
            inAction = true; // Đặt trạng thái đang hành động

            float targetSpeed = 1;
            if (slot.changeSpeed)
            {
                targetSpeed = slot.animSpeed;
                if (targetSpeed == 0)
                    targetSpeed = 1;
            }

            anim.SetFloat("animSpeed", targetSpeed); // Đặt tốc độ hoạt ảnh

            anim.SetBool("mirror", slot.mirror); // Đặt trạng thái gương cho hoạt ảnh
            anim.CrossFade(targetAnim, 0.2f); // Chuyển đổi hoạt ảnh với độ mờ 0.2 giây
            characterStats._stamina -= slot.staminaCost; // Giảm năng lượng theo chi phí
                                                         // rigid.velocity = Vector3.zero -> thay vì tắt vận tốc, thêm vận tốc vào AnimatorHook.cs
        }

        bool CheckForParry(Action slot)
        {
            if (slot.canParry == false)
                return false; // Không thể phản công nếu không được phép

            EnemyStates parryTarget = null;
            Vector3 origin = transform.position;
            origin.y += 1;
            Vector3 rayDir = transform.forward;
            RaycastHit hit;
            if (Physics.Raycast(origin, rayDir, out hit, 2, ignoreLayers))
            {
                parryTarget = hit.transform.GetComponentInParent<EnemyStates>();
            }

            if (parryTarget == null)
                return false; // Không xử lý nếu không có mục tiêu phản công

            if (parryTarget.parriedBy == null)
                return false; // Không xử lý nếu mục tiêu không thể bị phản công

            // Hướng được tính từ người chơi đến kẻ thù
            Vector3 dir = parryTarget.transform.position - transform.position;
            dir.Normalize();
            dir.y = 0;
            float angle = Vector3.Angle(transform.forward, dir);

            if (angle < 60)
            {
                Vector3 targetPosition = -dir * parryOffset;
                targetPosition += parryTarget.transform.position;
                transform.position = targetPosition;

                if (dir == Vector3.zero)
                    dir = -parryTarget.transform.forward;

                Quaternion eRotation = Quaternion.LookRotation(-dir); // Quay kẻ thù
                Quaternion ourRot = Quaternion.LookRotation(dir); // Quay người chơi

                parryTarget.transform.rotation = eRotation;
                transform.rotation = ourRot;
                parryTarget.IsGettingParried(slot, inventoryManager.GetCurrentWeapon(slot.mirror)); // Thực hiện phản công
                canAttack = false; // Đặt trạng thái không thể tấn công
                onEmpty = false; // Đặt trạng thái không còn gì
                canMove = false; // Đặt trạng thái không thể di chuyển
                inAction = true; // Đặt trạng thái đang hành động
                anim.SetBool("mirror", slot.mirror); // Đặt trạng thái gương cho hoạt ảnh
                anim.SetFloat("parrySpeed", 1); // Đặt tốc độ phản công
                anim.CrossFade("parry_attack", 0.2f); // Chuyển đổi hoạt ảnh phản công với độ mờ 0.2 giây
                lockOnTarget = null; // Xóa mục tiêu khóa

                return true;
            }

            return false;
        }

        bool CheckForBackStab(Action slot)
        {
            if (slot.canBackStab == false)
                return false; // Không thể đánh lén nếu không được phép

            EnemyStates backstab = null;
            Vector3 origin = transform.position;
            origin.y += 1;
            Vector3 rayDir = transform.forward;
            RaycastHit hit;
            if (Physics.Raycast(origin, rayDir, out hit, 1, ignoreLayers))
            {
                backstab = hit.transform.GetComponentInParent<EnemyStates>();
            }

            if (backstab == null)
                return false; // Không xử lý nếu không có mục tiêu đánh lén

            // Hướng được tính từ kẻ thù đến người chơi
            Vector3 dir = transform.position - backstab.transform.position;
            dir.Normalize();
            dir.y = 0;
            float angle = Vector3.Angle(backstab.transform.forward, dir);

            if (angle > 150)
            {
                Vector3 targetPosition = dir * backstabOffset;
                targetPosition += backstab.transform.position;
                transform.position = targetPosition;

                backstab.transform.rotation = transform.rotation; // Quay kẻ thù về hướng của người chơi
                backstab.IsGettingBackStabbed(slot, inventoryManager.GetCurrentWeapon(slot.mirror)); // Thực hiện đánh lén
                canAttack = false; // Đặt trạng thái không thể tấn công
                onEmpty = false; // Đặt trạng thái không còn gì
                canMove = false; // Đặt trạng thái không thể di chuyển
                inAction = true; // Đặt trạng thái đang hành động
                anim.SetBool("mirror", slot.mirror); // Đặt trạng thái gương cho hoạt ảnh
                anim.CrossFade("parry_attack", 0.2f); // Chuyển đổi hoạt ảnh đánh lén với độ mờ 0.2 giây
                lockOnTarget = null; // Xóa mục tiêu khóa

                return true;
            }

            return false;
        }


        //**************Blocking******************
        bool blockAnim;
        string blockIdleAnim;

        void HandleBlocking()
        {
            if (isBlocking == false)
            {
                if (blockAnim)
                {
                    inventoryManager.CloseBlockCollider(); // Đóng collider của việc chặn
                    anim.CrossFade(blockIdleAnim, 0.1f); // Chuyển đổi hoạt ảnh chặn
                    blockAnim = false;
                }
            }
        }

        void BlockAction(Action slot)
        {
            isBlocking = true; // Đặt trạng thái chặn
            enableIK = true; // Kích hoạt IK
            isLeftHand = slot.mirror; // Xác định tay nào để chặn
            a_hook.currentHand = (slot.mirror) ? AvatarIKGoal.LeftHand : AvatarIKGoal.RightHand;
            a_hook.InitIKForShield(slot.mirror); // Khởi tạo IK cho khiên
            inventoryManager.OpenBlockCollider(); // Mở collider của việc chặn

            if (blockAnim == false)
            {
                blockIdleAnim = (isTwoHanded == false) ? inventoryManager.GetCurrentWeapon(isLeftHand).oh_idle : inventoryManager.GetCurrentWeapon(isLeftHand).th_idle;
                blockIdleAnim += (isLeftHand) ? "_l" : "_r"; // Chọn hoạt ảnh idle cho tay trái hoặc phải
                string targetAnim = slot.targetAnim;
                targetAnim += (isLeftHand) ? "_l" : "_r"; // Thêm suffix cho hoạt ảnh mục tiêu
                anim.CrossFade(targetAnim, 0.1f); // Chuyển đổi hoạt ảnh
                blockAnim = true;
            }
        }

        void ParryAction(Action slot)
        {
            string targetAnim = null;
            targetAnim = slot.GetActionStep(ref actionManager.actionIndex).GetBranch(slot.input).targetAnim; // Lấy hoạt ảnh phản công
            if (string.IsNullOrEmpty(targetAnim))
                return; // Không xử lý nếu không có hoạt ảnh mục tiêu

            float targetSpeed = 1;
            if (slot.changeSpeed)
            {
                targetSpeed = slot.animSpeed;
                if (targetSpeed == 0)
                    targetSpeed = 1;
            }

            anim.SetFloat("animSpeed", targetSpeed); // Đặt tốc độ hoạt ảnh

            // Thoát hàm sau khi phát hoạt ảnh.
            canBeParried = slot.canBeParried; // Cập nhật trạng thái phản công

            canAttack = false;
            onEmpty = false;
            canMove = false; // Không thể di chuyển để sử dụng RootMotion.
            inAction = true; // Đặt trạng thái đang hành động
            anim.SetBool("mirror", slot.mirror); // Đặt trạng thái gương cho hoạt ảnh
            anim.CrossFade(targetAnim, 0.2f); // Chuyển đổi hoạt ảnh với độ mờ 0.2 giây
        }

        //*****************Spells*****************

        float cur_focusCost;
        float cur_staminaCost;
        float spellCastTime;
        float max_spellCastTime;
        string spellTargetAnim;
        bool spellIsMirrored;
        GameObject projectileCandidate;
        SpellType curSpellType;

        public delegate void SpellCast_Start();
        public delegate void SpellCast_Loop();
        public delegate void SpellCast_Stop();
        public SpellCast_Start spellCast_start;
        public SpellCast_Loop spellCast_loop;
        public SpellCast_Stop spellCast_stop;

        void SpellAction(Action slot)
        { // slot từ ActionManager

            if (characterStats._stamina < 1)
                return; // Không xử lý nếu năng lượng thấp

            if (slot.spellClass != inventoryManager.currentSpell.instance.spellClass || characterStats._focus < 3)
            {
                anim.SetBool("mirror", slot.mirror);
                canAttack = false;
                onEmpty = false;
                canMove = false;
                inAction = true;
                anim.CrossFade("cant_spell", 0.2f); // Chuyển đổi hoạt ảnh không thể phép thuật
                return;
            }

            ActionInput inp = actionManager.GetActionInput(this);
            if (inp == ActionInput.lb)
                inp = ActionInput.rb;
            if (inp == ActionInput.lt)
                inp = ActionInput.rt;

            Spell s_inst = inventoryManager.currentSpell.instance; // Lấy phép thuật từ CurrentSpell, không từ actionSlots như với vũ khí vật lý
            SpellAction s_slot = s_inst.GetAction(s_inst.actions, inp); // s_slot từ Resource spell, dẫn xuất từ InventoryManager.
            if (s_slot == null)
                return; // Không xử lý nếu không có phép thuật

            SpellEffectsManager.singleton.UseSpellEffect(s_inst.spell_effect, this); // Sử dụng hiệu ứng phép thuật

            isSpellCasting = true;
            spellCastTime = 0;
            max_spellCastTime = s_slot.castTime;
            spellTargetAnim = s_slot.throwAnim;
            spellIsMirrored = slot.mirror;
            curSpellType = s_inst.spellType;

            string targetAnim = s_slot.targetAnim;
            if (spellIsMirrored)
                targetAnim += "_l";
            else
                targetAnim += "_r";

            projectileCandidate = inventoryManager.currentSpell.instance.projectile;
            inventoryManager.CreateSpellParticle(inventoryManager.currentSpell, spellIsMirrored, (s_inst.spellType == SpellType.looping));

            anim.SetBool("spellcasting", isSpellCasting);
            anim.SetBool("mirror", slot.mirror);
            anim.CrossFade(targetAnim, 0.2f); // Chuyển đổi hoạt ảnh phép thuật

            cur_focusCost = s_slot.focusCost;
            cur_staminaCost = s_slot.staminaCost;

            a_hook.InitIKForBreathSpell(spellIsMirrored); // Khởi tạo IK cho phép thuật thở

            if (curSpellType == SpellType.looping)
            {
                if (spellCast_start != null)
                    spellCast_start(); // Bắt đầu phép thuật looping
            }
        }

        void HandleSpellCasting()
        {
            if (curSpellType == SpellType.looping)
            {
                enableIK = true;
                a_hook.currentHand = (spellIsMirrored) ? AvatarIKGoal.LeftHand : AvatarIKGoal.RightHand;

                if ((rb == false && lb == false) || characterStats._focus < 2)
                {
                    isSpellCasting = false;
                    enableIK = false;

                    if (spellCast_stop != null)
                        spellCast_stop(); // Dừng phép thuật looping
                    return;
                }

                if (spellCast_loop != null)
                    spellCast_loop(); // Tiếp tục phép thuật looping

                return;
            }

            spellCastTime += delta; // Cập nhật thời gian phép thuật

            if (inventoryManager.currentSpell.currentParticle != null)
                inventoryManager.currentSpell.currentParticle.SetActive(true); // Kích hoạt hạt phép thuật

            if (spellCastTime > max_spellCastTime)
            {
                canAttack = false;
                onEmpty = false;
                canMove = false;
                inAction = true;
                isSpellCasting = false;

                string targetAnim = spellTargetAnim;
                anim.SetBool("mirror", spellIsMirrored);
                anim.CrossFade(targetAnim, 0.2f); // Chuyển đổi hoạt ảnh phép thuật
            }
        }

        public void ThrowProjectile()
        { // Sẽ được gọi trong AnimatorHook.cs
            if (projectileCandidate == null)
                return; // Không xử lý nếu không có viên đạn

            GameObject g0 = Instantiate(projectileCandidate) as GameObject; // Tạo đối tượng viên đạn
            Transform p = anim.GetBoneTransform((spellIsMirrored) ? HumanBodyBones.LeftHand : HumanBodyBones.RightHand);
            g0.transform.position = p.position;

            if (lockOnTransform && lockOn)
            {
                Vector3 v = lockOnTransform.position;
                v.y += 1f;
                g0.transform.LookAt(v); // Hướng viên đạn đến mục tiêu
            }
            else
                g0.transform.rotation = transform.rotation;

            Projectile proj = g0.GetComponent<Projectile>();
            proj.Init(); // Khởi tạo viên đạn
            characterStats._stamina -= cur_staminaCost;
            characterStats._focus -= cur_focusCost;
        }

        //************Locomotions*****************

        void HandleRolls()
        {
            if (!rollInput || usingItem || characterStats._stamina < 10)
                return; // Không xử lý nếu không có đầu vào lăn, đang sử dụng vật phẩm, hoặc năng lượng thấp

            float v = vertical;
            float h = horizontal;
            v = (moveAmount > 0.3f) ? 1 : 0;
            h = 0;

            if (v != 0)
            {
                if (moveDir == Vector3.zero)
                    moveDir = transform.forward;

                transform.rotation = Quaternion.LookRotation(moveDir); // Quay đối tượng theo hướng lăn

                a_hook.InitForRoll();
                a_hook.rm_multi = rollSpeed; // Thiết lập tốc độ lăn
            }
            else
            {
                a_hook.rm_multi = 1.3f;
            }

            anim.SetFloat("vertical", v); // Đặt giá trị thẳng đứng
            anim.SetFloat("horizontal", h); // Đặt giá trị ngang

            canAttack = false;
            onEmpty = false;
            canMove = false;
            inAction = true; // Đặt trạng thái đang hành động
            anim.CrossFade("Rolls", 0.2f); // Chuyển đổi hoạt ảnh lăn
            isInvincible = true;
            isBlocking = false; // Ngừng trạng thái chặn
            characterStats._stamina -= 25f; // Giảm năng lượng
        }

        void HandleMovementAnimation()
        {
            anim.SetBool("run", run); // Đặt trạng thái chạy
            anim.SetFloat("vertical", moveAmount, 0.4f, delta); // Đặt giá trị thẳng đứng
            anim.SetBool("onGround", onGround); // Đặt trạng thái trên mặt đất
        }

        void HandleLockOnAnimations(Vector3 moveDir)
        {
            Vector3 relativeDir = transform.InverseTransformDirection(moveDir);
            float h = relativeDir.x;
            float v = relativeDir.z;

            if (usingItem || isSpellCasting)
            {
                run = false;
                v = Mathf.Clamp(v, -0.7f, 0.6f);
                h = Mathf.Clamp(h, -0.6f, 0.6f);
            }

            anim.SetFloat("vertical", v, 0.2f, delta); // Đặt giá trị thẳng đứng trong trạng thái khóa mục tiêu
            anim.SetFloat("horizontal", h, 0.2f, delta); // Đặt giá trị ngang trong trạng thái khóa mục tiêu
        }

        public bool OnGround()
        {
            bool r = false;

            Vector3 origin = transform.position + (Vector3.up * toGround);
            Vector3 dir = -Vector3.up;
            float dis = toGround + 0.2f;
            RaycastHit hit;

            Debug.DrawRay(origin, dir * dis, Color.cyan);

            if (Physics.Raycast(origin, dir, out hit, dis, ignoreLayers))
            {
                r = true;
                Vector3 targetPosition = hit.point;
                transform.position = targetPosition; // Đặt vị trí của đối tượng lên mặt đất

            }

            return r;
        }

        //***********TwoHanded********************

        public void HandleTwoHanded()
        {
            bool isRight = true;
            if (inventoryManager.rightHandWeapon == null)
                return;

            Weapon w = inventoryManager.rightHandWeapon.instance;
            if (w == null)
            {
                w = inventoryManager.leftHandWeapon.instance;
                isRight = false;
            }
            if (w == null)
                return;

            if (isTwoHanded)
            {
                anim.CrossFade(w.th_idle, 0.2f); // Chuyển đổi hoạt ảnh idle cho hai tay
                actionManager.UpdateActionsTwoHanded();
                if (isRight)
                {
                    if (inventoryManager.leftHandWeapon)
                        inventoryManager.leftHandWeapon.weaponModel.SetActive(false); // Ẩn mô hình vũ khí tay trái
                }
                else
                {
                    if (inventoryManager.rightHandWeapon)
                        inventoryManager.rightHandWeapon.weaponModel.SetActive(false); // Ẩn mô hình vũ khí tay phải
                }
            }
            else
            {
                anim.Play("Equip Weapon"); // Phát hoạt ảnh trang bị vũ khí
                actionManager.UpdateActionsOneHanded();
                if (isRight)
                {
                    if (inventoryManager.leftHandWeapon)
                        inventoryManager.leftHandWeapon.weaponModel.SetActive(true); // Hiện mô hình vũ khí tay trái
                }
                else
                {
                    if (inventoryManager.rightHandWeapon)
                        inventoryManager.rightHandWeapon.weaponModel.SetActive(true); // Hiện mô hình vũ khí tay phải
                }
            }
        }

        //**********StatsMonitor*****************

        public void AddHealth()
        {
            characterStats.fp++; // Thêm điểm sức mạnh
        }

        public void MonitorStats()
        {
            if (run & moveAmount > 0)
            {
                characterStats._stamina -= delta * 10; // Giảm năng lượng khi chạy
            }
            else
            {
                characterStats._stamina += delta * 9; // Tăng năng lượng khi không chạy
            }

            characterStats._health = Mathf.Clamp(characterStats._health, 0, characterStats.hp); // Giới hạn sức khỏe
            characterStats._focus = Mathf.Clamp(characterStats._focus, 0, characterStats.fp); // Giới hạn điểm tập trung
            characterStats._stamina = Mathf.Clamp(characterStats._stamina, 0, characterStats.stamina); // Giới hạn năng lượng
        }

        public void SubstractStaminaOverTime()
        {
            characterStats._stamina -= cur_staminaCost; // Giảm năng lượng theo thời gian
        }

        public void SubstractFocusOverTime()
        {
            characterStats._focus -= cur_focusCost; // Giảm điểm tập trung theo thời gian
        }

        public void EffectBlocking()
        {
            isBlocking = true; // Bắt đầu trạng thái chặn
        }

        public void StopEffectBlocking()
        {
            isBlocking = false; // Ngừng trạng thái chặn
        }

        public void DoDamage(AIAttacks a)
        {
            if (isInvincible)
                return; // Không xử lý nếu đang bất tử
            damaged = true;

            int damage = 20;
            
            characterStats._health -= damage; // Giảm sức khỏe
            if (canMove)
            {
                int ran = Random.Range(0, 100);
                string tA = (ran > 50) ? "damage_1" : "damage_2";
                audio_clip = ResourceManager.singleton.GetAudio("hurt").audio_clip;
                anim.Play(tA); // Phát hoạt ảnh nhận sát thương
            }
            anim.SetBool("OnEmpty", false);
            onEmpty = false;
            isInvincible = true; // Đặt trạng thái bất tử
            anim.applyRootMotion = true; // Kích hoạt RootMotion
            anim.SetBool("canMove", false); // Không thể di chuyển
            if (characterStats._health <= 0 && !isDead)
            {
                Die(); // Thực hiện hành động chết nếu sức khỏe <= 0
            }
        }

        public void Die()
        {
            isDead = true;
            isInvincible = true; // Đặt trạng thái bất tử
            //StartCoroutine(sceneController.HandleGameOver()); // Xử lý kết thúc trò chơi
        }

    }
}

