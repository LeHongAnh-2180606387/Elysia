using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCharacter_Base : MonoBehaviour
{
    private Animator mAnimator;
    private CharacterController controller;
    private float speed = 2.0f;
    private Vector3 moveDirection = Vector3.zero;
    public float gravity = 9.81f; // Gravity

    void Start()
    {
        mAnimator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();

        // Kiểm tra null để đảm bảo rằng các component đã được gán
        if (controller == null)
        {
            Debug.LogError("CharacterController is missing from the GameObject. Please add it.");
        }

        if (mAnimator == null)
        {
            Debug.LogError("Animator is missing from the GameObject. Please add it.");
        }

    }

    void Update()
    {
        // Nếu controller hoặc animator là null, không thực hiện gì cả và tránh báo lỗi liên tục
        if (controller == null || mAnimator == null)
        {
            return; // Dừng Update nếu không có controller hoặc animator
        }


        // Khởi tạo biến kiểm tra di chuyển
        bool isMoving = false;
        moveDirection = Vector3.zero; // Reset the movement direction

        // Kiểm tra di chuyển lên xuống
        if (Input.GetKey(KeyCode.W))
        {
            isMoving = true;
            mAnimator.SetBool("isMoving", true);
            mAnimator.SetBool("isWalkingForward", true);
            moveDirection += Vector3.forward;
        }

        if (Input.GetKey(KeyCode.S))
        {
            isMoving = true;
            mAnimator.SetBool("isMoving", true);
            mAnimator.SetBool("isWalkingBack", true);
            moveDirection += Vector3.back;
        }

        // Kiểm tra di chuyển trái phải
        if (Input.GetKey(KeyCode.A))
        {
            isMoving = true;
            mAnimator.SetBool("isMoving", true);
            mAnimator.SetBool("isWalkingLeft", true);
            moveDirection += Vector3.left;
        }

        if (Input.GetKey(KeyCode.D))
        {
            isMoving = true;
            mAnimator.SetBool("isMoving", true);
            mAnimator.SetBool("isWalkingRight", true);
            moveDirection += Vector3.right;
        }

        // Di chuyển trước - trái, trước - phải
        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A))
        {
            isMoving = true;
            mAnimator.SetBool("isWalkingForward_Left", true);
        }

        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D))
        {
            isMoving = true;
            mAnimator.SetBool("isWalkingForward_Right", true);
        }

        // Di chuyển lùi - trái, lùi - phải
        if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A))
        {
            isMoving = true;
            mAnimator.SetBool("isWalkingBack_Left", true);
        }

        if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D))
        {
            isMoving = true;
            mAnimator.SetBool("isWalkingBack_Right", true);
        }

        // Nếu không còn phím nào được nhấn
        if (!isMoving)
        {
            mAnimator.SetBool("isMoving", false);
            mAnimator.SetBool("isWalkingRight", false);
            mAnimator.SetBool("isWalkingForward", false);
            mAnimator.SetBool("isWalkingBack", false);
            mAnimator.SetBool("isWalkingLeft", false);
            mAnimator.SetBool("isWalkingForward_Left", false);
            mAnimator.SetBool("isWalkingForward_Right", false);
            mAnimator.SetBool("isWalkingBack_Left", false);
            mAnimator.SetBool("isWalkingBack_Right", false);
        }

        // Xử lý di chuyển
        if (isMoving)
        {
            moveDirection = moveDirection.normalized * speed * Time.deltaTime;
        }

        // Áp dụng gravity
        moveDirection.y -= gravity * Time.deltaTime;

        // Di chuyển thông qua CharacterController
        controller.Move(transform.TransformDirection(moveDirection));
    }
}
