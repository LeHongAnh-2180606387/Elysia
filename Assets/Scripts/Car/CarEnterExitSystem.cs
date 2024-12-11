using System.Collections;
using UnityEngine;

public class CarEnterExitSystem : MonoBehaviour
{
    public MonoBehaviour CarController;
    public Transform Car;
    public Transform Player;

    [Header("Cameras")]
    public GameObject PlayerCam;
    public GameObject CarCam;

    public GameObject DriveUi;

    private bool Candrive;
    private bool isDriving; // Biến kiểm tra trạng thái lái xe

    public GameObject CamCar; // GameObject của camera xe
    private bool isInCar = false; // Kiểm tra xem nhân vật có đang trong xe không

    // Start is called before the first frame update
    void Start()
    {
        CarController.enabled = false;
        DriveUi.gameObject.SetActive(false);
        CarCam.SetActive(false); // Đảm bảo camera xe không hiển thị khi bắt đầu
        PlayerCam.SetActive(true); // Đảm bảo camera của player hiển thị khi bắt đầu
        isDriving = false; // Ban đầu người chơi không lái xe
        CamCar.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && Candrive)
        {
            if (!isDriving)
            {
                EnterCar(); // Người chơi vào xe
            }
            else
            {
                ExitCar(); // Người chơi ra khỏi xe
            }
        }
    }

    private void EnterCar()
    {
        // Bật camera xe
        CamCar.SetActive(true);
        isInCar = true; // Cập nhật trạng thái là đang trong xe

        CarController.enabled = true; // Kích hoạt điều khiển xe
        DriveUi.gameObject.SetActive(false);

        // Gắn nhân vật vào xe
        Player.transform.SetParent(Car);
        Player.gameObject.SetActive(false);

        // Chuyển đổi camera
        PlayerCam.gameObject.SetActive(false);
        CarCam.gameObject.SetActive(true); // Bật camera xe khi vào xe

        isDriving = true; // Cập nhật trạng thái lái xe
    }

    private void ExitCar()
    {
        // Tắt camera xe
        CamCar.SetActive(false);
        isInCar = false; // Cập nhật trạng thái là không còn trong xe

        CarController.enabled = false; // Vô hiệu hóa điều khiển xe

        // Hủy gắn nhân vật khỏi xe
        Player.transform.SetParent(null);
        Player.gameObject.SetActive(true);

        // Chuyển đổi camera
        PlayerCam.gameObject.SetActive(true); // Bật camera của player khi ra khỏi xe
        CarCam.gameObject.SetActive(false); // Tắt camera xe khi ra khỏi xe

        isDriving = false; // Cập nhật trạng thái không lái xe
    }

    void OnTriggerStay(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            DriveUi.gameObject.SetActive(true);
            Candrive = true;
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            DriveUi.gameObject.SetActive(false);
            Candrive = false;
        }
    }
}
