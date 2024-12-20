using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueEditor;
using Unity.VisualScripting;
using StarterAssets;
public class DialogueEditorNPC : MonoBehaviour
{

    [SerializeField]
    public NPCConversation myConversation;
    private bool dialogueInProgress = false;
    private bool conversationDone = false;
    // Tham chiếu đến các thành phần của người chơi để vô hiệu hóa/kích hoạt lại
    public GameObject playerGameObject;
    public GameObject secretPlayer;
    private ThirdPersonController playerController;
    private bool playerInRange = false;
    //public TextDisplay textDisplay;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }


    //&& !conversationDone
    void Update()
    {
        // Kiểm tra xem người chơi có ở trong phạm vi, nhấn phím E và không có cuộc hội thoại nào đang diễn ra
        if (playerInRange && Input.GetKeyDown(KeyCode.E) && !dialogueInProgress)
        {
            StartConversation();
            /*if (AttributeManager.Instance.isRunningConversation == true)
            {
                // Thiết lập lại các giá trị thuộc tính từ AttributeManager trước khi bắt đầu cuộc hội thoại
                ConversationManager.Instance.SetBool("isRead", AttributeManager.Instance.isRead);
                ConversationManager.Instance.SetBool("isTrue", AttributeManager.Instance.isTrue);
            }*/
        }
    }
    void StartConversation()
    {
        // Bắt đầu cuộc hội thoại
        ConversationManager.Instance.StartConversation(myConversation);

        // Đặt biến dialogueInProgress thành true để chỉ ra rằng cuộc hội thoại đang diễn ra
        dialogueInProgress = true;

        // Vô hiệu hóa PlayerController nếu đã tham chiếu thành công
        /*if (playerController != null)
        {
            playerController.enabled = false;
            playerController.canMove = false;// Vô hiệu hóa khả năng di chuyển
            playerController.StopMovement(); // Đặt movement về 0 ngay lập tức
        }*/

        // Chờ cho đến khi cuộc hội thoại kết thúc, sau đó sẽ kích hoạt lại các cơ chế điều khiển
        StartCoroutine(WaitForConversationEnd());
    }

    IEnumerator WaitForConversationEnd()
    {
        // Chờ cho đến khi cuộc hội thoại kết thúc
        while (ConversationManager.Instance.IsConversationActive)
        {
            yield return null;
        }

        // Khi cuộc hội thoại kết thúc, đặt lại biến dialogueInProgress và kích hoạt lại các cơ chế điều khiển
        dialogueInProgress = false;
        conversationDone = true;

        // Kích hoạt lại PlayerController nếu đã tham chiếu thành công
       /* if (playerController != null)
        {
            playerController.enabled = true;
            playerController.canMove = true; // Kích hoạt lại khả năng di chuyển
        }*/

        // Cập nhật các thuộc tính sau khi cuộc hội thoại kết thúc
        /*AttributeManager.Instance.isRead = ConversationManager.Instance.GetBool("isRead");
        AttributeManager.Instance.isTrue = ConversationManager.Instance.GetBool("isTrue");
        AttributeManager.Instance.isRunningConversation = true;
        textDisplay.StartDisplayTextAndLoadScene();*/
    }

    void Awake()
    {
        // Lấy tham chiếu đến PlayerController khi GameObject khởi động
        playerController = playerGameObject.GetComponent<ThirdPersonController>();
    }
}


