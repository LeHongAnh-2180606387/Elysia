using System.Collections;
using DialogueEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Elevator : MonoBehaviour
{
    private ScenePositionSpawn scenePositionSpawn = new ScenePositionSpawn();
    // Đang xem xét về việc lưu trữ giá trị này dưới dạng Singleton
    public bool isFloor1 = false;
    public bool isFloor2 = false;
    public bool isFloor3 = false;
    public bool isRoom = false;
    public bool isFloorTraining = false;
    UIManager uIManager = new UIManager();

    public GameObject Player;
    public void Start()
    {
        if (Player == null)
        {
            StartCoroutine(FindPlayer());
        }
    }

    IEnumerator FindPlayer()
    {
        yield return new WaitForSeconds(4f);
        if (Player == null)
            Player = GameObject.FindGameObjectWithTag("Player");
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (SceneManager.GetActiveScene().name == "City" || SceneManager.GetActiveScene().name == "Training")
            {
                if (ConversationManager.Instance == null)
                {
                    Debug.LogError("ConversationManager.Instance is null.");
                    return;
                }
                isFloor1 = false;
                isFloor2 = false;
                isFloor3 = false;
                isRoom = false;
                isFloorTraining = false;

            }
        }
    }
    private void OnEnable()
    {
        ConversationManager.OnConversationStarted += ConversationStart;
        ConversationManager.OnConversationEnded += ConversationEnd;
    }

    private void OnDisable()
    {
        ConversationManager.OnConversationStarted -= ConversationStart;
        ConversationManager.OnConversationEnded -= ConversationEnd;
    }

    private void ConversationStart()
    {
        if (SceneManager.GetActiveScene().name == "City" || SceneManager.GetActiveScene().name == "Training")
        {
            if (SceneManager.GetActiveScene().name != "City" || Player.transform.position.y > 0 && Player.transform.position.y <= 5)
            {
                isFloor1 = true;
                ConversationManager.Instance.SetBool("isFloor1", isFloor1);
            }
            else if (SceneManager.GetActiveScene().name != "City" || Player.transform.position.y > 5 && Player.transform.position.y <= 11)
            {
                isFloor2 = true;
                ConversationManager.Instance.SetBool("isFloor2", isFloor2);
            }
            else if (SceneManager.GetActiveScene().name != "City" || Player.transform.position.y > 11 && Player.transform.position.y <= 17)
            {
                isFloor3 = true;
                ConversationManager.Instance.SetBool("isFloor3", isFloor3);
            }
            else if (SceneManager.GetActiveScene().name == "Room")
            {
                isRoom = true;
                ConversationManager.Instance.SetBool("isRoom", isRoom);
            }
            else if (SceneManager.GetActiveScene().name == "Training")
            {
                isFloorTraining = true;
                ConversationManager.Instance.SetBool("isFloorTraining", isFloorTraining);
            }
        }
        Debug.Log("A conversation has Start.");
    }

    private void ConversationEnd()
    {
        if (SceneManager.GetActiveScene().name == "City" || SceneManager.GetActiveScene().name == "Training")
        {
            ConversationManager.Instance.SetBool("isFloor1", false);
            ConversationManager.Instance.SetBool("isFloor2", false);
            ConversationManager.Instance.SetBool("isFloor3", false);
            ConversationManager.Instance.SetBool("isRoom", false);
            ConversationManager.Instance.SetBool("isFloorTraining", false);
        }
        Debug.Log("A conversation has ended.");
    }
    public void Update()
    {


    }
    public void TransformFloor1()
    {
        if (SceneManager.GetActiveScene().name != "City")
        {
            //Destroy(Player);
            uIManager.SwapSceneAtPosition("City", scenePositionSpawn.getCityFloorOnePosition());
        }
        else
            Player.transform.position = scenePositionSpawn.getCityFloorOnePosition();
    }
    public void TransformFloor2()
    {
        if (SceneManager.GetActiveScene().name != "City")
        {
            //Destroy(Player);
            uIManager.SwapSceneAtPosition("City", scenePositionSpawn.getCityFloorTwoPosition());
        }
        Player.transform.position = scenePositionSpawn.getCityFloorTwoPosition();
    }
    public void TransformFloor3()
    {
        if (SceneManager.GetActiveScene().name != "City")
        {
            //Destroy(Player);
            uIManager.SwapSceneAtPosition("City", scenePositionSpawn.getCityFloorThreePosition());
        }
        Player.transform.position = scenePositionSpawn.getCityFloorThreePosition();
    }
    public void TransformRoom()
    {
        //Destroy(Player);

        uIManager.SwapSceneAtPosition("Room", scenePositionSpawn.getRoomPosition());
    }
    public void TransformTraining()
    {
        //Destroy(Player);

        uIManager.SwapSceneAtPosition("Training", scenePositionSpawn.getTrainingPosition());
    }
}