using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SA{
	public class InputHandler : MonoBehaviour {
		float vertical;
		float horizontal;
		bool b_input;
		bool a_input;
		bool x_input;
		bool y_input;

		bool rb_input;
		bool lb_input;

		float rt_axis;
		bool rt_input;

		float lt_axis;
		bool lt_input;

        float d_y;
        float d_x;

        bool d_up;
        bool d_down;
        bool d_right;
        bool d_left;

        bool p_d_up;
        bool p_d_down;
        bool p_d_left;
        bool p_d_right;

		bool leftAxis_down;
		bool rightAxis_down;

		float b_timer;
		float rt_timer;
		float lt_timer;
        float close_timer = 0;
        float a_input_count = 1.5f;

		StateManager states;
		CameraManager camManager;
        UIManager uiManager;
        DialogueManager dialogueManager;

		float delta;


        //-----------------------------------------------------------------------


        void Start()
        {
            states = GetComponent<StateManager>(); 
            states.Init(); 

            camManager = CameraManager.singleton; 
            camManager.Init(states); // Khởi tạo CameraManager với trạng thái hiện tại của nhân vật.

            uiManager = UIManager.singleton; // Lấy đối tượng UIManager đơn lẻ.

            dialogueManager = DialogueManager.singleton; // Lấy đối tượng DialogueManager đơn lẻ.

            Cursor.lockState = CursorLockMode.Locked; // Khóa trạng thái của con trỏ chuột.
            Cursor.visible = false; // Ẩn con trỏ chuột.
        }

        // Khởi tạo các chức năng di chuyển và camera.
        void FixedUpdate()
        {
            delta = Time.fixedDeltaTime; 
            GetInput();
            UpdateStates(); 
            states.FixedTick(delta); 
            camManager.Tick(delta); 
        }

        bool preferItem; 

        void Update()
        {
            delta = Time.deltaTime; 
            if (a_input)
                a_input_count++; // Tăng bộ đếm khi nút A được nhấn.
            Debug.Log(delta);
            states.Tick(delta); // Gọi hàm Tick trong StateManager.

            // Nếu không có đối thoại đang diễn ra, xử lý tương tác với vật phẩm hoặc đối tượng.
            if (!dialogueManager.dialogueActive)
            {
                if (states.pickManager.itemCandidate != null || states.pickManager.interactionCandidate != null)
                {
                    if (states.pickManager.itemCandidate && states.pickManager.interactionCandidate)
                    {
                        if (preferItem)
                        {
                            PickupItem();
                        }
                        else
                            Interact();
                    }

                    if (states.pickManager.itemCandidate && !states.pickManager.interactionCandidate)
                    {
                        PickupItem();
                    }

                    if (!states.pickManager.itemCandidate && states.pickManager.interactionCandidate)
                    {
                        Interact();
                    }
                }
                else
                {
                    uiManager.CloseInteractCard(); // Đóng bảng tương tác nếu không có vật phẩm hoặc đối tượng nào.
                    if (uiManager.announceCard[0].gameObject.activeSelf == true
                        || uiManager.announceCard[1].gameObject.activeSelf == true
                        || uiManager.announceCard[2].gameObject.activeSelf == true
                        || uiManager.announceCard[3].gameObject.activeSelf == true
                        || uiManager.announceCard[4].gameObject.activeSelf == true)
                        close_timer += 1; // Tăng bộ đếm thời gian đóng thông báo.
                    if (close_timer > 190)
                    {
                        close_timer = 0;
                        uiManager.CloseAnnounceCard(); // Đóng thông báo sau khi thời gian kết thúc.
                        a_input = false;
                    }
                }
            }
            else
            {
                uiManager.CloseInteractCard(); // Đóng bảng tương tác nếu có đối thoại đang diễn ra.
            }

            if (a_input_count > 1f)
            {
                a_input = false; 
                a_input_count = 0; 
            }

            dialogueManager.Tick(a_input);
            states.MonitorStats(); // Theo dõi các chỉ số của nhân vật.
            ResetInputNState(); // Đặt lại đầu vào và trạng thái.
            uiManager.Tick(states.characterStats, delta, states); 
        }

        void PickupItem()
        {
            uiManager.OpenInteractCard(UIActionType.pickup); // Mở bảng tương tác để nhặt vật phẩm.
            if (a_input)
            {
                Vector3 targetDir = states.pickManager.itemCandidate.transform.position - transform.position; 
                states.SnapToRotation(targetDir); // Xoay nhân vật về hướng vật phẩm.
                states.pickManager.PickCandidate(states); // Gọi hàm nhặt vật phẩm.
                states.PlayAnimation("pick_up"); // Chơi hoạt ảnh nhặt vật phẩm.
                a_input = false; // Đặt lại biến khi nút A không được nhấn.
            }
        }

        void Interact()
        {
            uiManager.OpenInteractCard(states.pickManager.interactionCandidate.actionType); // Mở bảng tương tác với loại hành động tương ứng.
            if (a_input)
            {
                states.audio_source.PlayOneShot(ResourceManager.singleton.GetAudio("interact").audio_clip); // Phát âm thanh tương tác.
                states.InteractLogic(); // Xử lý logic tương tác.
                a_input = false; // Đặt lại biến khi nút A không được nhấn.
            }
        }

        void GetInput()
        {
            vertical = Input.GetAxis(StaticStrings.Vertical); 
            horizontal = Input.GetAxis(StaticStrings.Horizontal);

            b_input = Input.GetButton(StaticStrings.B);
            a_input = Input.GetButton("A"); 
            x_input = Input.GetButton(StaticStrings.X); 
            y_input = Input.GetButtonUp(StaticStrings.Y); 

            rb_input = Input.GetButton(StaticStrings.RB); 
            lb_input = Input.GetButton(StaticStrings.LB); 

            rt_input = Input.GetButton(StaticStrings.RT);
            rt_axis = Input.GetAxis(StaticStrings.RT); 

            if (rt_axis != 0)
                rt_input = true; // Nếu trục RT không bằng 0, đặt rt_input là true.

            lt_input = Input.GetButton(StaticStrings.LT); // Kiểm tra xem nút LT có được nhấn không.
            lt_axis = Input.GetAxis(StaticStrings.LT); // Lấy giá trị trục LT từ người dùng.
            if (lt_axis != 0)
                lt_input = true; // Nếu trục LT không bằng 0, đặt lt_input là true.

            rightAxis_down = Input.GetButtonUp(StaticStrings.L); // Kiểm tra xem nút nhấn trục analog phải có được nhấn không.

            if (b_input)
                b_timer += delta; // Tăng bộ đếm thời gian cho nút B.

            d_x = Input.GetAxis("Pad X");
            d_y = Input.GetAxis("Pad Y"); 

            d_up = Input.GetKeyUp(KeyCode.Alpha1) || d_y > 0; 
            d_down = Input.GetKeyUp(KeyCode.Alpha2) || d_y < 0; 
            d_left = Input.GetKeyUp(KeyCode.Alpha3) || d_x < 0; 
            d_right = Input.GetKeyUp(KeyCode.Alpha4) || d_x > 0; 
        }

        // Truyền giá trị vào các biến và hàm của StateManager.
        void UpdateStates()
        {
            states.vertical = vertical; // Cập nhật giá trị trục dọc trong StateManager.
            states.horizontal = horizontal; 

            states.itemInput = x_input; // Cập nhật giá trị đầu vào vật phẩm.
            states.rt = rt_input; // Cập nhật giá trị đầu vào RT.
            states.lt = lt_input; 
            states.rb = rb_input; 
            states.lb = lb_input; 

            // Hướng di chuyển của nhân vật dựa trên trục camera.
            Vector3 v = states.vertical * camManager.transform.forward;
            Vector3 h = states.horizontal * camManager.transform.right;
            states.moveDir = (v + h).normalized; // Chuẩn hóa hướng di chuyển.

            // Tính toán lượng di chuyển.
            float m = Mathf.Abs(states.horizontal) + Mathf.Abs(states.vertical);
            states.moveAmount = Mathf.Clamp01(m); // Giới hạn giá trị từ 0 đến 1.

            // Xử lý nút B: chạy khi giữ nút B lâu hơn 0.5 giây và có đủ stamina.
            if (b_input && b_timer > 0.5f)
            {
                states.run = (states.moveAmount > 0) && states.characterStats._stamina > 0;
            }
            // Xử lý lăn khi nhấn nút B nhanh.
            if (b_input == false && b_timer > 0 && b_timer < 0.5f)
                states.rollInput = true;

            // Xử lý khi nhấn nút Y: chuyển đổi giữa hai chế độ sử dụng vũ khí hoặc tương tác với vật phẩm.
            if (y_input)
            {
                if (states.pickManager.itemCandidate && states.pickManager.interactionCandidate)
                {
                    preferItem = !preferItem;
                }
                else
                {
                    states.isTwoHanded = !states.isTwoHanded;
                    states.HandleTwoHanded();
                }
            }

            // Xử lý khi mục tiêu bị khóa bị tiêu diệt.
            if (states.lockOnTarget != null)
            {
                if (states.lockOnTarget.eStates.isDead)
                {
                    states.lockOn = false;
                    states.lockOnTarget = null;
                    states.lockOnTransform = null;
                    camManager.lockOn = false;
                    camManager.lockOnTarget = null;
                }
            }
            else
            {
                states.lockOn = false;
                states.lockOnTarget = null;
                states.lockOnTransform = null;
                camManager.lockOn = false;
                camManager.lockOnTarget = null;
            }

            // Xử lý khóa mục tiêu khi nhấn nút nhấn của trục analog phải.
            if (rightAxis_down)
            {
                states.lockOn = !states.lockOn;
                states.lockOnTarget = EnemyManager.singleton.GetEnemy(transform.position);
                if (states.lockOnTarget == null)
                    states.lockOn = false;

                camManager.lockOnTarget = states.lockOnTarget;
                states.lockOnTransform = states.lockOnTarget.GetTarget();
                camManager.lockOnTransform = states.lockOnTransform;
                camManager.lockOn = states.lockOn;
            }

            if (x_input)
                b_input = false; // Đặt lại biến khi nhấn nút X.

            HandleQuickSlotChanges(); // Gọi hàm xử lý thay đổi nhanh giữa các vật phẩm.
        }

        void HandleQuickSlotChanges()
        {
            if (states.isSpellCasting || states.usingItem)
                return; // Không thay đổi khi đang niệm phép hoặc sử dụng vật phẩm.

            if (d_up)
            {
                if (!p_d_up)
                {
                    p_d_up = true;
                    states.inventoryManager.ChangeToNextSpell(); // Chuyển sang phép tiếp theo.
                }
            }

            if (!d_up)
                p_d_up = false;

            if (d_down)
            {
                if (!p_d_down)
                {
                    p_d_down = true;
                    states.inventoryManager.ChangeToNextConsumable(); // Chuyển sang vật phẩm tiêu hao tiếp theo.
                }
            }

            if (!d_up)
                p_d_down = false;

            if (states.onEmpty == false)
                return;

            if (states.isTwoHanded)
                return;

            if (d_left)
            {
                if (!p_d_left)
                {
                    states.inventoryManager.ChangeToNextWeapon(true); // Chuyển sang vũ khí tiếp theo (tay trái).
                    p_d_left = true;
                }
            }
            if (d_right)
            {
                if (!p_d_right)
                {
                    states.inventoryManager.ChangeToNextWeapon(false); // Chuyển sang vũ khí tiếp theo (tay phải).
                    p_d_right = true;
                }
            }

            if (!d_down)
                p_d_down = false;
            if (!d_left)
                p_d_left = false;
            if (!d_right)
                p_d_right = false;
        }

        void ResetInputNState()
        {
            if (b_input == false)
                b_timer = 0; // Đặt lại bộ đếm thời gian cho nút B khi nút này không được nhấn.

            if (states.rollInput)
                states.rollInput = false; // Đặt lại trạng thái lăn sau khi lăn xong.

            if (states.run)
                states.run = false; // Đặt lại trạng thái chạy sau khi chạy xong.
        }


    }
}

