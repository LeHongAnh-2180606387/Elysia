using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopInteraction : MonoBehaviour
{
    [SerializeField] private GameObject UI; // Tham chiếu đến UI
    private bool isPlayerInRange = false;

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E)) // Nhấn phím E trong vùng shop
        {
            OpenShop();
        }
        if (Input.GetKeyDown(KeyCode.Escape)) // Nhấn phím Escape để đóng UI
        {
            CloseShop();
        }
    }

    public void OpenShop()
    {
        if (UI != null)
        {
            UI.SetActive(true); // Hiển thị UI
            Time.timeScale = 0; // Tạm dừng toàn bộ game (vật lý, chuyển động, etc)
            // Chặn các thao tác nhập liệu từ scene
            Debug.Log("Shop UI opened, game paused.");
        }
    }

    public void CloseShop()
    {
        if (UI != null)
        {
            UI.SetActive(false); // Ẩn UI
            Time.timeScale = 1; // Khôi phục lại game (tiếp tục các hoạt động)
            // Cho phép các thao tác nhập liệu từ scene
            Debug.Log("Shop UI closed, game resumed.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Kiểm tra nếu đối tượng là nhân vật
        {
            isPlayerInRange = true;
            Debug.Log("Player entered the shop zone.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) // Kiểm tra nếu đối tượng rời khỏi vùng shop
        {
            isPlayerInRange = false;
            Debug.Log("Player exited the shop zone.");
        }
    }
}
