using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SA
{
    public class WinManager : MonoBehaviour
    {
        public EnemyManager enManager; // Tham chiếu đến EnemyManager để quản lý kẻ thù.
        public GameObject winMenu; // Menu hiển thị khi người chơi thắng cuộc.

        void Init()
        {
            // Khởi tạo EnemyManager.
            enManager = GetComponent<EnemyManager>();
        }

        void OnTriggerEnter(Collider other)
        {
            // Lấy StateManager từ đối tượng va chạm.
            StateManager states = other.GetComponent<StateManager>();
            if (states != null)
            {
                // Kiểm tra số lượng kẻ thù còn lại.
                if (enManager.enemyTargets.Count == 0)
                {
                    // Hiển thị thông báo thắng cuộc và bắt đầu xử lý menu thắng cuộc.
                    winMenu.GetComponentInChildren<Text>().text = "YOU HAVE BEATEN ALL ENEMIES. WELCOME HOME CHOSEN ONE !";
                    StartCoroutine(handleWinMenu());
                    this.gameObject.SetActive(false); // Ẩn đối tượng chứa script này.
                }
                else
                {
                    // Hiển thị thông báo chưa hoàn thành nhiệm vụ và bắt đầu xử lý menu thắng cuộc.
                    winMenu.GetComponentInChildren<Text>().text = "TURN BACK, YOU HAVE NOT FINISHED YOUR JOB, YOU STILL HAVE " + enManager.enemyTargets.Count + " ENEMIES LEFT TO SLAY !";
                    StartCoroutine(handleWinMenu());
                }
            }
            else
            {
                return; // Không có StateManager, không làm gì cả.
            }
        }

        IEnumerator handleWinMenu()
        {
            // Kích hoạt menu thắng cuộc.
            winMenu.SetActive(true);
            Time.timeScale = 0f; // Tạm dừng thời gian trong trò chơi.
            yield return new WaitForSecondsRealtime(3); // Chờ 3 giây thực tế.
            Time.timeScale = 1f; // Tiếp tục thời gian trong trò chơi.
            winMenu.SetActive(false); // Ẩn menu thắng cuộc.
        }
    }
}
