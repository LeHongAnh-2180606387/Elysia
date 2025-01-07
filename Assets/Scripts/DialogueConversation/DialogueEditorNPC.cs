using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueEditor;
using Unity.VisualScripting;
using StarterAssets;
using UnityEngine.SceneManagement;
public class DialogueEditorNPC : MonoBehaviour
{
    [SerializeField] public NPCConversation myConversation;
    private bool dialogueInProgress = false;
    private bool conversationDone = false;

    // Tham chiếu đến các thành phần của người chơi để vô hiệu hóa/kích hoạt lại
    public GameObject playerGameObject;
    public ThirdPersonController playerController;
    private bool playerInRange = false;
    public GameObject buttonF;


    private ScenePositionSpawn scenePositionSpawn = new ScenePositionSpawn();

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            buttonF.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            buttonF.SetActive(false);
        }
    }

    public void Start()
    {
        buttonF.SetActive(false);
        if (playerController == null && SceneManager.GetActiveScene().name == "City" || SceneManager.GetActiveScene().name == "Training" || SceneManager.GetActiveScene().name == "Room")
        {
            StartCoroutine(FindPlayer());
        }
    }
    IEnumerator FindPlayer()
    {
        yield return new WaitForSeconds(4f);
        if (playerGameObject == null)
            playerGameObject = GameObject.FindGameObjectWithTag("Player");
        if (playerController == null)
        {
            playerController = playerGameObject.GetComponent<ThirdPersonController>();
        }
    }
    //&& !conversationDone
    void Update()
    {
        // Kiểm tra xem người chơi có ở trong phạm vi, nhấn phím F và không có cuộc hội thoại nào đang diễn ra
        if (myConversation == true && playerInRange && Input.GetKeyDown(KeyCode.F) && !dialogueInProgress)
        {
            StartConversation();
        }
    }
    void StartConversation()
    {
        // Bắt đầu cuộc hội thoại
        ConversationManager.Instance.StartConversation(myConversation);

        // Đặt biến dialogueInProgress thành true để chỉ ra rằng cuộc hội thoại đang diễn ra
        dialogueInProgress = true;

        // Vô hiệu hóa PlayerController nếu đã tham chiếu thành công
        if (playerController != null)
        {
            playerController.enabled = false;
        }

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
        if (playerController != null)
        {
            playerController.enabled = true;
        }

        // Cập nhật các thuộc tính sau khi cuộc hội thoại kết thúc
        /*AttributeManager.Instance.isRead = ConversationManager.Instance.GetBool("isRead");
        AttributeManager.Instance.isTrue = ConversationManager.Instance.GetBool("isTrue");
        AttributeManager.Instance.isRunningConversation = true;
        textDisplay.StartDisplayTextAndLoadScene();*/
    }
}


