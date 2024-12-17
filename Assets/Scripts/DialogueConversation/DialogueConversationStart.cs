using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueEditor;
using StarterAssets;

public class DialogueEditorStart : MonoBehaviour
{
    public GameObject playerGameObject;
    private bool dialogueInProgress = false;
    private ThirdPersonController playerController;

    private void OnEnable()
    {
        //.EventStartConversation += StartConversation;
    }

    private void OnDisable()
    {
        //.EventStartConversation -= StartConversation;
    }

    void Start()
    {
        playerController = playerGameObject.GetComponent<ThirdPersonController>();
    }

    void Update()
    {
        if (dialogueInProgress && playerController != null)
        {
            playerController.enabled = false;
            
        }
    }

    void StartConversation(NPCConversation conversation)
    {
        if (!dialogueInProgress)
        {
            ConversationManager.Instance.StartConversation(conversation);
            dialogueInProgress = true;
            // Vô hiệu hóa PlayerController nếu đã tham chiếu thành công
            /*if (playerController != null)
            {
                playerController.enabled = false;
                playerController.canMove = false;// Vô hiệu hóa khả năng di chuyển
                playerController.StopMovement(); // Đặt movement về 0 ngay lập tức
            }*/
            StartCoroutine(WaitForConversationEnd());
        }
    }

    IEnumerator WaitForConversationEnd()
    {
        while (ConversationManager.Instance.IsConversationActive)
        {
            yield return null;
        }

        dialogueInProgress = false;
        if (playerController != null)
        {
            playerController.enabled = true;
        }
    }
    void Awake()
    {
        // Lấy tham chiếu đến PlayerController khi GameObject khởi động
        playerController = playerGameObject.GetComponent<ThirdPersonController>();
    }
}
