
using System.Collections;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;
#endif

/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM 
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class ThirdPersonController : MonoBehaviour
    {
        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 2.0f;

        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 5.335f;

        [Tooltip("Walk speed of the character in m/s")]
        public float WalkSpeed = 1.0f;

        public bool isCrouching = false;

        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;

        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;

        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.50f;

        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;

        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;

        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.28f;

        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;

        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 70.0f;

        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -30.0f;

        [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
        public float CameraAngleOverride = 0.0f;

        [Tooltip("For locking the camera position on all axis")]
        public bool LockCameraPosition = false;

        // cinemachine
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        // player
        private float _speed;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        // animation IDs
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;

#if ENABLE_INPUT_SYSTEM 
        private PlayerInput _playerInput;
#endif
        private Animator _animator;
        private CharacterController _controller;
        private StarterAssetsInputs _input;
        private GameObject _mainCamera;

        private const float _threshold = 0.01f;

        private bool _hasAnimator;


        private float _doublePressTime = 0.2f;
        private float _lastPressTime = 0f;
        private bool _isSprinting = false;

        private GUIManager guiManager;

        //player
        private float damagebyplayer = 50f;
        private float armor = 10f;
        private float currenthealth;
        private float maxHealth;
        public Image damageImage;
        public AudioSource audioSource;  // Reference to the AudioSource component
        public AudioClip damageSound;  // The sound effect to play when taking damage
        private bool isTakingDamage = false;
        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
            }
        }


        private void Awake()
        {
            // get a reference to our main camera
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
            guiManager = FindObjectOfType<GUIManager>();

            if (guiManager == null)
            {
                Debug.LogError("GUIManager not found in the scene!");
            }
        }

        private void Start()
        {
            _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;

            _hasAnimator = TryGetComponent(out _animator);
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM 
            _playerInput = GetComponent<PlayerInput>();
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif

            AssignAnimationIDs();

            // reset our timeouts on start
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;
            enemy = GameObject.FindWithTag("Enemy").transform;
            if (guiManager != null)
            {
                currenthealth = guiManager.currentHealth;
                maxHealth = guiManager.maxHealth;
            }

            if (damageImage != null)
            {
                // Đảm bảo bắt đầu với ảnh trong suốt
                damageImage.canvasRenderer.SetAlpha(0f);
            }
            if (audioSource != null)
            {
                audioSource.playOnAwake = false;  // Don't play sound automatically at start
            }
        }

        private void Update()
        {
            _hasAnimator = TryGetComponent(out _animator);

            JumpAndGravity();
            GroundedCheck();
            Move();
            HandleCrouch();
            HandleCombo();
            DealDamage();

        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        }

        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
                transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
                QueryTriggerInteraction.Ignore);

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDGrounded, Grounded);
            }
        }

        private void CameraRotation()
        {
            // if there is an input and camera position is not fixed
            if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
            {
                //Don't multiply mouse input by Time.deltaTime;
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier;
                _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier;
            }

            // clamp our rotations so our values are limited 360 degrees
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            // Cinemachine will follow this target
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
                _cinemachineTargetYaw, 0.0f);
        }

        //stamina
        private float StaminaDrainRate = 5f;
        private float StaminaRegenRate = 3f;
        private float currentStamina;
        private float maxStamina;
        private void Move()
        {
            if (guiManager != null)
            {
                currentStamina = guiManager.currentStamina;
                maxStamina = guiManager.maxStamina;
            }
            // Tính toán tốc độ mục tiêu dựa trên trạng thái chạy nước rút, đi bộ hoặc di chuyển bình thường
            float targetSpeed = (_input.sprint && currentStamina > 0) ? SprintSpeed
                            : _input.walk ? (WalkSpeed + 1.0f)
                            : MoveSpeed;

            if (_input.move == Vector2.zero) targetSpeed = 0.0f;
            // Trừ stamina khi chạy nước rút
            if (_input.sprint && _input.move != Vector2.zero && currentStamina > 0)
            {
                currentStamina -= StaminaDrainRate * Time.deltaTime;
                currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina); // Đảm bảo không dưới 0
            }
            else if (!_input.sprint && currentStamina < maxStamina) // Hồi phục stamina khi không chạy nước rút
            {
                currentStamina += StaminaRegenRate * Time.deltaTime;
                currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina); // Đảm bảo không vượt quá tối đa
            }

            // Vô hiệu hóa chạy nước rút khi stamina bằng 0
            if (currentStamina <= 0)
            {
                _input.sprint = false; // Tắt trạng thái sprint
            }
            if (guiManager != null)
            {
                guiManager.currentStamina = currentStamina;
            }
            // (Phần còn lại của logic di chuyển không thay đổi)
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            // Tăng tốc hoặc giảm tốc để đạt được target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * SpeedChangeRate);
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            if (_input.move != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                    _mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                    RotationSmoothTime);
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }

            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                                new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDSpeed, _animationBlend);
                _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
            }

        }

        private bool isCrouchKeyPressed = false; // Biến để theo dõi trạng thái nhấn phím cúi

        private void HandleCrouch()
        {
            // Kiểm tra nếu phím cúi đã được nhấn
            if (_input.crouch)
            {
                // Nếu phím cúi được nhấn lần đầu và chưa cúi
                if (!isCrouchKeyPressed)
                {
                    isCrouchKeyPressed = true; // Đánh dấu rằng phím cúi đã được nhấn

                    // Chỉ cúi nếu chưa cúi
                    if (!isCrouching)
                    {
                        isCrouching = true; // Cập nhật trạng thái là cúi
                        if (_animator != null)
                        {
                            _animator.SetBool("Crouch", isCrouching); // Kích hoạt animation cúi
                        }
                    }
                }

                // Nếu đang cúi và nhấn các phím di chuyển theo trục Y (tiến/lùi)
                if (isCrouching && _input.move.y != 0)
                {
                    if (_animator != null)
                    {
                        _animator.SetBool("isCrouchMove", true); // Kích hoạt animation di chuyển tiến/lùi khi cúi
                    }
                }
                else if (isCrouching && _input.move.y == 0)
                {
                    if (_animator != null)
                    {
                        _animator.SetBool("isCrouchMove", false); // Tắt animation di chuyển tiến/lùi khi thả phím di chuyển
                    }
                }
            }
            else
            {
                // Nếu phím cúi đã được thả ra
                if (isCrouchKeyPressed)
                {
                    isCrouchKeyPressed = false; // Đánh dấu rằng phím cúi đã được thả ra

                    // Nếu đang cúi, cập nhật trạng thái đứng dậy
                    if (isCrouching)
                    {
                        isCrouching = false; // Đặt trạng thái không cúi
                        if (_animator != null)
                        {
                            _animator.SetBool("Crouch", isCrouching); // Kích hoạt animation đứng dậy
                            _animator.SetBool("isCrouchMove", false); // Đảm bảo tắt animation di chuyển khi cúi
                            //_animator.SetBool("isCrouchMoveLeft", false); // Đảm bảo tắt animation di chuyển trái khi cúi
                        }
                    }
                }
            }
        }






        // Các biến combo cho punch
        private int punchStep = 0;             // Biến đếm combo punch
        private float punchTimer = 0f;         // Bộ đếm thời gian cho combo punch
        private const float punchDelay = 0.5f; // Thời gian tối đa giữa các đòn punch combo

        // Các biến combo cho kick
        private int kickStep = 0;             // Biến đếm combo kick
        private float kickTimer = 0f;         // Bộ đếm thời gian cho combo kick
        private const float kickDelay = 0.6f; // Thời gian tối đa giữa các đòn kick combo

        private bool isAttacking = false;     // Trạng thái đang tấn công (cả punch và kick)
        private bool isCombat = false;        // Trạng thái combat
        private float resetCombatTimer = 0f;  // Bộ đếm thời gian reset combat
        private const float combatResetDelay = 3.0f; // Thời gian chờ để reset combat

        private float lowerKickTimer = 0f;
        private float lowerKickBasicTimer = 0f;
        private float lowerKickBasicDuration = 0.2f;
        private const float lowerKickDuration = 0.5f;
        private float switchKickTimer = 0f;
        private const float switchKickDuration = 0.5f;
        private float spinningBackFirstTimer = 0f;
        private const float spinningBackFirstDuration = 0.5f;
        private float lowerPunchTimer = 0f;
        private float lowerPunchBasicTimer = 0f;
        private float lowerPunchBasicDuration = 0.2f;
        private const float lowerPunchDuration = 0.5f;

        // Hàm xử lý combo cho cả punch và kick
        private void HandleCombo()
        {
            if (_input.kick && _input.crouch)
            {

                if (!isAttacking)
                {
                    isAttacking = true;
                    // Execute lower kick animation
                    _animator.SetBool("isLowerKickBasic", true);
                    lowerKickBasicTimer = lowerKickBasicDuration;

                    if (isCombat == true)
                    {
                        _animator.SetBool("isLowerKick", true);
                        lowerKickTimer = lowerKickDuration;
                        resetCombatTimer = combatResetDelay;
                    }
                    else
                    {
                        // Regular mode behavior (e.g., no combat-specific effects)
                        Debug.Log("Lower kick in regular mode");
                    }
                }
            }

            // Countdown to reset lower kick
            if (lowerKickBasicTimer > 0)
            {
                lowerKickBasicTimer -= Time.deltaTime;
                if (lowerKickBasicTimer <= 0)
                {
                    ResetLowerBasicKick();
                }
            }
            // Countdown to reset lower kick
            if (lowerKickTimer > 0)
            {
                lowerKickTimer -= Time.deltaTime;
                if (lowerKickTimer <= 0)
                {
                    ResetLowerKick();
                }
            }
            if (_input.punch && _input.crouch)
            {
                if (!isAttacking)
                {
                    isAttacking = true;
                    // Execute lower punch animation
                    _animator.SetBool("isLowerPunchBasic", true);
                    lowerPunchBasicTimer = lowerPunchBasicDuration;

                    if (isCombat)
                    {
                        _animator.SetBool("isLowerPunch", true);
                        lowerPunchTimer = lowerPunchDuration;
                        resetCombatTimer = combatResetDelay;
                    }
                    else
                    {
                        Debug.Log("Lower punch in regular mode");
                    }
                }
            }

            // Countdown to reset lower punch in regular mode
            if (lowerPunchBasicTimer > 0)
            {
                lowerPunchBasicTimer -= Time.deltaTime;
                if (lowerPunchBasicTimer <= 0)  // Corrected variable name
                {
                    ResetLowerBasicPunch();
                }
            }

            // Countdown to reset lower punch in combat mode
            if (lowerPunchTimer > 0)
            {
                lowerPunchTimer -= Time.deltaTime;
                if (lowerPunchTimer <= 0)
                {
                    ResetLowerPunch();
                }
            }

            // Handle switch kick
            if (_input.kick && _input.move.y > 0)
            {
                if (!isAttacking)
                {
                    isAttacking = true;
                    isCombat = true;  // Enter combat mode
                    _animator.SetBool("isCombat", true);
                    resetCombatTimer = combatResetDelay;
                    _animator.SetBool("isSwitchKick", true);
                    switchKickTimer = switchKickDuration;
                }
            }

            // Countdown to reset switch kick
            if (switchKickTimer > 0)
            {
                switchKickTimer -= Time.deltaTime;
                if (switchKickTimer <= 0)
                {
                    ResetSwitchKick();
                }
            }
            // Handle switch kick
            if (_input.punch && _input.move.y > 0)
            {
                if (!isAttacking)
                {
                    isAttacking = true;
                    isCombat = true;  // Enter combat mode
                    _animator.SetBool("isCombat", true);
                    resetCombatTimer = combatResetDelay;
                    _animator.SetBool("isSpinningBackFirst", true);
                    spinningBackFirstTimer = spinningBackFirstDuration;
                }
            }

            // Countdown to reset switch kick
            if (spinningBackFirstTimer > 0)
            {
                spinningBackFirstTimer -= Time.deltaTime;
                if (spinningBackFirstTimer <= 0)
                {
                    ResetSpinningPunch();
                }
            }


            // Khi người chơi nhấn chuột trái (cho punch) hoặc chuột phải (cho kick)
            if ((_input.punch || _input.kick) && !isAttacking)
            {
                isAttacking = true;
                isCombat = true;
                _animator.SetBool("isCombat", isCombat);  // Bắt đầu chế độ combat
                resetCombatTimer = combatResetDelay;     // Đặt lại thời gian combat

                // Kiểm tra nếu punchTimer đã hết hạn thì reset combo punch
                if (punchTimer <= 0)
                {
                    punchStep = 0;
                }

                // Kiểm tra nếu kickTimer đã hết hạn thì reset combo kick
                if (kickTimer <= 0)
                {
                    kickStep = 0;
                }

                // Nếu nhấn chuột trái (Punch)
                if (_input.punch && !_input.crouch)
                {
                    punchStep++; // Tăng punchStep để xác định đòn punch
                    punchTimer = punchDelay; // Đặt lại thời gian đếm ngược cho combo punch

                    // Kích hoạt animation dựa trên punchStep
                    if (punchStep == 1)
                    {
                        _animator.SetBool("Punch1", true);
                        _animator.SetBool("Punch2", false);
                        _animator.SetBool("Punch3", false);
                    }
                    else if (punchStep == 2)
                    {
                        _animator.SetBool("Punch1", false);
                        _animator.SetBool("Punch2", true);
                        _animator.SetBool("Punch3", false);
                    }
                    else if (punchStep == 3)
                    {
                        _animator.SetBool("Punch1", false);
                        _animator.SetBool("Punch2", false);
                        _animator.SetBool("Punch3", true);
                        punchStep = 0; // Reset combo punch sau đòn thứ 3
                    }
                }

                // Nếu nhấn chuột phải (Kick)
                else if (_input.kick && (!_input.crouch))
                {
                    kickStep++; // Tăng kickStep để xác định đòn kick
                    kickTimer = kickDelay; // Đặt lại thời gian đếm ngược cho combo kick

                    // Kích hoạt animation dựa trên kickStep
                    if (kickStep == 1)
                    {
                        _animator.SetBool("Kick1", true);
                        _animator.SetBool("Kick2", false);
                        _animator.SetBool("Kick3", false);

                    }
                    else if (kickStep == 2)
                    {
                        _animator.SetBool("Kick1", false);
                        _animator.SetBool("Kick2", true);
                        _animator.SetBool("Kick3", false);


                    }

                    else if (kickStep == 3)
                    {
                        _animator.SetBool("Kick1", false);
                        _animator.SetBool("Kick2", false);
                        _animator.SetBool("Kick3", true);

                        kickStep = 0; // Reset combo kick sau đòn thứ 3
                    }
                }
            }
            else if (!_input.punch && !_input.kick && isAttacking) // Khi chuột được thả ra
            {
                isAttacking = false; // Đánh dấu chuột đã thả
                punchTimer = punchDelay; // Duy trì combo punch trong khoảng thời gian punchDelay
                kickTimer = kickDelay;   // Duy trì combo kick trong khoảng thời gian kickDelay
            }

            // Giảm punchTimer và kickTimer để reset combo nếu hết thời gian
            if (punchTimer > 0)
            {
                punchTimer -= Time.deltaTime;
                if (punchTimer <= 0)
                {
                    ResetPunch();
                }
            }

            if (kickTimer > 0)
            {
                kickTimer -= Time.deltaTime;
                if (kickTimer <= 0)
                {
                    ResetKick();
                }
            }

            // Đếm ngược để reset combat nếu không có hoạt động trong 5 giây
            if (isCombat)
            {
                // Nếu phím W đang được nhấn, bỏ qua việc giảm resetCombatTimer
                if (_input.move.x != 0 || _input.move.y != 0) // Kiểm tra phím W
                {
                    isCombat = false;
                    _animator.SetBool("isCombat", isCombat);
                    resetCombatTimer = combatResetDelay;
                    // Giữ nguyên thời gian reset combat
                }
                else if (_input.move.x == 0 || _input.move.y == 0)
                {
                    // Nếu W không được nhấn, tiếp tục đếm ngược để thoát combat
                    resetCombatTimer -= Time.deltaTime;
                    if (resetCombatTimer <= 0)
                    {
                        isCombat = false;
                        _animator.SetBool("isCombat", isCombat);  // Thoát khỏi chế độ combat
                    }
                }
            }
        }

        // Hàm để reset combo punch
        private void ResetPunch()
        {
            punchStep = 0;
            punchTimer = 0f;
            _animator.SetBool("Punch1", false);
            _animator.SetBool("Punch2", false);
            _animator.SetBool("Punch3", false);
        }

        // Hàm để reset combo kick
        private void ResetKick()
        {
            kickStep = 0;
            kickTimer = 0f;
            _animator.SetBool("Kick1", false);
            _animator.SetBool("Kick2", false);
            _animator.SetBool("Kick3", false);
        }
        private void ResetLowerKick()
        {
            _animator.SetBool("isLowerKick", false);
            isAttacking = false;
        }
        private void ResetLowerBasicKick()
        {
            _animator.SetBool("isLowerKickBasic", false);
            isAttacking = false;
        }
        private void ResetSwitchKick()
        {
            _animator.SetBool("isSwitchKick", false);
            isAttacking = false;
        }
        private void ResetSpinningPunch()
        {
            _animator.SetBool("isSpinningBackFirst", false);
            isAttacking = false;
        }
        private void ResetLowerPunch()
        {
            _animator.SetBool("isLowerPunch", false);
            isAttacking = false;
        }
        private void ResetLowerBasicPunch()
        {
            _animator.SetBool("isLowerPunchBasic", false);
            isAttacking = false;
        }
        public void GainXP(float xpAmount)
        {
            guiManager.currentXP += xpAmount; // Cộng thêm XP
            Debug.Log("Gained XP: " + xpAmount);

            CheckLevelUp();
        }

        // Kiểm tra xem có đủ XP để lên level hay không
        private void CheckLevelUp()
        {
            while (guiManager.currentXP >= guiManager.xpToNextLevel) // Có thể lên nhiều level cùng lúc
            {
                guiManager.currentXP -= guiManager.xpToNextLevel; // Trừ XP đã dùng để lên level
                guiManager.currentXP++;
                guiManager.currentLevel++;// Tăng level
                guiManager.xpToNextLevel *= 1.2f;      // Tăng XP cần thiết cho level tiếp theo (ví dụ tăng 20%)

                Debug.Log("Level Up! New Level: " + guiManager.currentXP);
            }
        }
        private void JumpAndGravity()
        {
            if (Grounded)
            {
                // reset the fall timeout timer
                _fallTimeoutDelta = FallTimeout;

                // update animator if using character
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, false);
                    _animator.SetBool(_animIDFreeFall, false);
                }

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                // Jump
                if (_input.jump && _jumpTimeoutDelta <= 0.0f)
                {
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDJump, true);
                    }
                }

                // jump timeout
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                // reset the jump timeout timer
                _jumpTimeoutDelta = JumpTimeout;

                // fall timeout
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDFreeFall, true);
                    }
                }

                // if we are not grounded, do not jump
                _input.jump = false;
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }

        public bool IsTakingDamage()
        {
            return isTakingDamage; // Trả về trạng thái nhận sát thương
        }

        public void TakeDamage(float damage, GameObject enemy)
        {
            // Lấy tag từ đối tượng enemy
            string enemyTag = enemy.tag;

            Debug.Log($"Incoming Damage: {damage}, Enemy Tag: {enemyTag}, Current Health: {currenthealth}, Armor: {armor}");

            // Áp dụng logic dựa trên tag
            if (enemyTag == "Enemy")
            {
                TakeDamageFromEnemy(damage); // Giảm sát thương bởi giáp
                Debug.Log($"Damage after armor (Enemy): {damage}");
            }
            /*else if (enemyTag == "boss")
            {
                TakeDamageFromBoss(damage); // Giảm ít giáp hơn đối với boss
                Debug.Log($"Damage after armor (Boss): {damage}");
            }*/
            else
            {
                Debug.LogWarning($"Unknown enemy tag: {enemyTag}");
            }

            if (damageImage != null)
            {
                StartCoroutine(ShowDamageEffect());
            }

            if (audioSource != null && damageSound != null)
            {
                audioSource.PlayOneShot(damageSound);  // Play the damage sound effect
            }
        }

        private Coroutine damageEffectCoroutine;
        private IEnumerator ShowDamageEffect()
        {
            // Set damage image color to red and make it fully opaque
            damageImage.color = Color.red;  // Set the color to red
            damageImage.canvasRenderer.SetAlpha(1f);  // Fully visible

            // Wait for a short period to show the effect
            yield return new WaitForSeconds(0.5f);

            // Fade the damage image back to transparent
            float elapsedTime = 0f;
            float fadeDuration = 0.5f;
            while (elapsedTime < fadeDuration)
            {
                float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);  // Fade from visible to invisible
                damageImage.canvasRenderer.SetAlpha(alpha);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Ensure it is fully invisible at the end
            damageImage.canvasRenderer.SetAlpha(0f);
        }
        public void TakeDamageFromEnemy(float damage)
        {
            damage = Mathf.Max(damage - armor, 0); // Giảm sát thương bởi giáp
            isTakingDamage = true; // Đặt trạng thái nhận sát thương là true
            _animator.SetBool("isTakeDamage", true);
            Debug.Log("TakeDamageFromEnemy called, isTakeDamage set to true");

            // Sau một khoảng thời gian, có thể đặt lại trạng thái nhận sát thương
            StartCoroutine(ResetDamageState());


        }

        private IEnumerator ResetDamageState()
        {
            // Ví dụ: Sau 0.5 giây reset lại trạng thái
            yield return new WaitForSeconds(0.1f);
            isTakingDamage = false; // Đặt lại trạng thái nhận sát thương
            _animator.SetBool("isTakeDamage", false); // Cập nhật lại Animator
            Debug.Log("Resetting damage state, isTakingDamage set to false");
        }

        public void TakeDamageFromBoss(float damage)
        {
            //damage = Mathf.Max((damage*2) - armor, 0); // Giảm ít giáp hơn đối với boss
            //ApplyDamage(damage);
            float originalDamage = damage;
            damage = Mathf.Max(damage - (armor * 0.5f), 0); // Tăng sát thương 50% và trừ giáp
            Debug.Log($"Original Damage: {originalDamage}, After Armor: {damage}");
            ApplyDamage(damage);
            _animator.SetBool("isTakeDamage", true);
            Debug.Log("TakeDamageFromEnemy called, isTakeDamage set to true");
        }
        private void ApplyDamage(float damage)
        {
            currenthealth -= damage;
            currenthealth = Mathf.Clamp(currenthealth, 0, maxHealth);

            Debug.Log($"Current Health after damage: {currenthealth}");

            // Cập nhật GUI (nếu có)
            if (guiManager != null)
            {
                guiManager.currentHealth = currenthealth;
            }

            // Kiểm tra nếu máu <= 0
            if (currenthealth <= 0)
            {
                Die();
            }
        }
   

   
        private void Die()
        {
            Debug.Log("Player has died!");
            // Logic thêm vào: ví dụ tắt GameObject, hiển thị màn hình game over, v.v.
            gameObject.SetActive(false);
        }
        private float lastAttackTime = 0f;  // Lưu thời gian lần tấn công cuối
        public float attackCooldown = 1f;
        public float attackRange = 2f;        // Phạm vi tấn công
        public Transform enemy;
        public void DealDamage()
        {
            if (_input.punch || _input.kick)
            {
                // Kiểm tra cooldown
                if (Time.time - lastAttackTime >= attackCooldown)
                {
                    // Tìm tất cả đối tượng với tag "Enemy" hoặc "Boss" trong phạm vi attackRange
                    Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, attackRange);

                    // Duyệt qua tất cả các đối tượng tìm thấy trong phạm vi
                    foreach (var enemyCollider in enemiesInRange)
                    {
                        // Kiểm tra nếu đối tượng có tag "Enemy" hoặc "Boss"
                        if (enemyCollider.CompareTag("Enemy") /*|| enemyCollider.CompareTag("boss")*/)
                        {
                            // Lấy component EnemyController từ enemy
                            EnemyController enemyHealth = enemyCollider.GetComponent<EnemyController>();
                            if (enemyHealth != null)
                            {
                                // Tính hướng từ player đến enemy
                                Vector3 directionToEnemy = (enemyCollider.transform.position - transform.position).normalized;

                                // Tính góc giữa hướng của player và hướng đến enemy
                                float angle = Vector3.Angle(transform.forward, directionToEnemy);

                                // Kiểm tra nếu góc giữa hướng của player và hướng đến enemy <= 45 độ
                                if (angle <= 45f)
                                {
                                    // Gây damage cho enemy hoặc boss tùy theo tag
                                    
                                        // Nếu là enemy, gây damage thông thường
                                        enemyHealth.TakeDamage(damagebyplayer);
                                        Debug.Log("Enemyhealth :" + enemyHealth.enemyHealth);
                                    // In ra thông tin máu của enemy sau khi nhận damage
                                }

                                else
                                {
                                    Debug.Log("Enemy is not within 45 degrees in front of the player.");
                                }
                            }
                        }
                    }

                    // Cập nhật thời gian tấn công
                    lastAttackTime = Time.time;
                }
                else
                {
                    Debug.Log("Attack is on cooldown.");
                }
            }
        }
        // Coroutine để đặt lại trạng thái tấn công sau một khoảng thời gian (tránh việc tính đến sát thương nhiều lần)
        private IEnumerator ResetAttackCooldown()
        {
            yield return new WaitForSeconds(0.5f); // Chờ 0.5s trước khi cho phép tấn công lại
            isAttacking = false;
        }
    
        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(
                new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
                GroundedRadius);
        }

        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (FootstepAudioClips.Length > 0)
                {
                    var index = Random.Range(0, FootstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume);
                }
            }
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
            }
        }
    }
}
