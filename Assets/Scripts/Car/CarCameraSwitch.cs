using UnityEngine;

public class CarCameraSwitch : MonoBehaviour
{
    public GameObject CamCar; // GameObject của camera xe
    private bool isInCar = false; // Kiểm tra xem nhân vật có đang trong xe không

    void Start()
    {
        // Đảm bảo camera xe bắt đầu ở trạng thái tắt
        CamCar.SetActive(false);
    }

    void Update()
    {
        // Kiểm tra nếu nhân vật nhấn phím F và thay đổi trạng thái vào/ra xe
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (isInCar)
            {
                ExitCar(); // Nếu nhân vật đang trong xe, thoát ra
            }
            else
            {
                EnterCar(); // Nếu nhân vật chưa vào xe, vào xe
            }
        }
    }

    private void EnterCar()
    {
        // Bật camera xe
        CamCar.SetActive(true);
        isInCar = true; // Cập nhật trạng thái là đang trong xe
    }

    private void ExitCar()
    {
        // Tắt camera xe
        CamCar.SetActive(false);
        isInCar = false; // Cập nhật trạng thái là không còn trong xe
    }
}
