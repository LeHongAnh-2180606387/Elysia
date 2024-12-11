using UnityEngine;

public class FreeLookCam : MonoBehaviour
{
    [Header("Camera Settings")]
    public Transform target; // Đối tượng mà camera sẽ theo dõi
    public bool autoTargetPlayer = true;
    public UpdateType updateType = UpdateType.FixedUpdate;

    [Header("Movement Settings")]
    public float moveSpeed = 8f;
    public float turnSpeed = 1f;
    public float turnSmoothing = 0f; // Mượt mà khi xoay
    public float tiltMax = 75f; // Góc nghiêng tối đa
    public float tiltMin = 45f; // Góc nghiêng tối thiểu

    [Header("Additional Settings")]
    public bool lockCursor = true;
    public bool verticalAutoReturn = false;

    private float lookAngle; // Góc xoay theo chiều ngang
    private float tiltAngle; // Góc xoay theo chiều dọc
    private const float LookDistance = 10f; // Khoảng cách camera tới target
    private Vector3 pivotOffset; // Offset của pivot camera
    private Transform pivot; // Trục xoay
    private Transform cam; // Camera

    private void Awake()
    {
        cam = GetComponentInChildren<Camera>().transform;
        pivot = cam.parent;

        if (autoTargetPlayer && target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
        }

        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        
    }


    private void Update()
    {
        if (updateType == UpdateType.Update)
        {
            FollowTarget(Time.deltaTime);
            HandleRotationMovement();
        }
    }

    private void FixedUpdate()
    {
        if (updateType == UpdateType.FixedUpdate)
        {
            FollowTarget(Time.fixedDeltaTime);
            HandleRotationMovement();
        }
    }

    private void LateUpdate()
    {
        if (updateType == UpdateType.LateUpdate)
        {
            FollowTarget(Time.deltaTime);
            HandleRotationMovement();
        }
    }

    private void FollowTarget(float deltaTime)
    {
        if (target == null)
            return;

        transform.position = Vector3.Lerp(transform.position, target.position, deltaTime * moveSpeed);
    }

    private void HandleRotationMovement()
    {
        if (target == null)
            return;

        float x = Input.GetAxis("Mouse X");
        float y = Input.GetAxis("Mouse Y");

        lookAngle += x * turnSpeed;

        tiltAngle -= y * turnSpeed;
        tiltAngle = Mathf.Clamp(tiltAngle, -tiltMin, tiltMax);

        Quaternion targetRotation = Quaternion.Euler(0f, lookAngle, 0f);
        if (turnSmoothing > 0f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSmoothing * Time.deltaTime);
        }
        else
        {
            transform.rotation = targetRotation;
        }

        pivot.localRotation = Quaternion.Euler(tiltAngle, 0f, 0f);
    }

    public enum UpdateType
    {
        FixedUpdate,
        Update,
        LateUpdate
    }
}
