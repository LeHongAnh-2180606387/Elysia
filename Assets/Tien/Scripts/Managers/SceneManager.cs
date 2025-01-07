using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    // Quản lý các cảnh, bao gồm việc chuyển đổi bản đồ và xử lý kết thúc trò chơi.
    public class SceneManager : MonoBehaviour
    {
        // Các đối tượng bản đồ và giao diện kết thúc trò chơi.
        public GameObject map1;
        public GameObject map2;
        public StateManager states;
        public GameObject gameOverUI;

        void Start()
        {
            // Ẩn map2 khi bắt đầu trò chơi.
            map2.SetActive(false);
        }

        // Xử lý sự kiện khi đối tượng va chạm với collider này.
        void OnTriggerEnter(Collider other)
        {
            // Lấy StateManager từ đối tượng va chạm.
            StateManager states = other.GetComponent<StateManager>();
            if (states != null && this.gameObject.tag == "LevelChanger")
            {
                // Kích hoạt map2, di chuyển đối tượng vào vị trí mới và ẩn map1.
                map2.SetActive(true);
                states.gameObject.transform.position = new Vector3(-442.9f, -14.214f, -219.52f);
                map1.SetActive(false);
            }
        }

        // Xử lý tình trạng kết thúc trò chơi.
        public IEnumerator HandleGameOver()
        {
            // Phát âm thanh kết thúc trò chơi.
            states.audio_clip = ResourceManager.singleton.GetAudio("character_die").audio_clip;
            states.audio_source.PlayOneShot(states.audio_clip);

            // Thay đổi trạng thái của nhân vật.
            states.canAttack = false;
            states.canMove = false;
            states.isInvincible = true;
            states.anim.Play("dead");

            // Chờ 3.5 giây trước khi tiếp tục.
            yield return new WaitForSeconds(3.5f);

            // Hiển thị giao diện kết thúc trò chơi và hiển thị con trỏ chuột.
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            gameOverUI.SetActive(true);
        }
    }
}
