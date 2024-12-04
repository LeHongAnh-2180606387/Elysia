using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusUIController : MonoBehaviour
{
    public GameObject statusPanel; // Tham chiếu đến Panel của bảng trạng thái

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            // Chuyển đổi trạng thái hoạt động của bảng trạng thái
            statusPanel.SetActive(!statusPanel.activeSelf);

            // In ra trạng thái của Panel để kiểm tra
            Debug.Log("Status Panel Active: " + statusPanel.activeSelf);
        }
    }
}
