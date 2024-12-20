using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueEditor;
using StarterAssets;
public class DialogueEditerIntro : MonoBehaviour
{
    [SerializeField]
    public NPCConversation myConversation;
    private bool dialogueInProgress = false; // Biến để kiểm tra xem cuộc hội thoại đang diễn ra hay không

    // Tham chiếu đến các thành phần của người chơi để vô hiệu hóa/kích hoạt lại
    public GameObject playerGameObject;
    private ThirdPersonController playerController;

    void Start()
    {
        // Bắt đầu cuộc hội thoại khi GameObject được khởi động
        StartConversation();
    }

    void Update()
    {
        // Nếu cuộc hội thoại đang diễn ra, vô hiệu hóa các cơ chế điều khiển khác
        if (dialogueInProgress && playerController != null)
        {
            // Ở đây bạn có thể thực hiện các hành động cụ thể, chẳng hạn như vô hiệu hóa các nút, chuột, hoặc bộ điều khiển
            // Ví dụ:
            // Cursor.lockState = CursorLockMode.Locked;
            // Cursor.visible = false;
            playerController.enabled = false;
        }
    }

    void StartConversation()
    {
        // Bắt đầu cuộc hội thoại
        ConversationManager.Instance.StartConversation(myConversation);

        // Đặt biến dialogueInProgress thành true để chỉ ra rằng cuộc hội thoại đang diễn ra
        dialogueInProgress = true;

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
        // Ở đây bạn có thể thực hiện các hành động cụ thể để kích hoạt lại các cơ chế điều khiển, chẳng hạn như mở khóa chuột, hiển thị con trỏ chuột, kích hoạt lại PlayerController, ...
        // Kích hoạt lại PlayerController nếu đã tham chiếu thành công
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
