using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    // Lớp này dùng để lưu trữ danh sách các đối tượng âm thanh.
    // Được kế thừa từ ScriptableObject, cho phép tạo ra các đối tượng âm thanh có thể cấu hình trong Unity Editor.
    public class AudioScriptableObject : ScriptableObject
    {
        public List<Audio> audio_list = new List<Audio>(); // Danh sách các đối tượng âm thanh.

    }

    // Lớp này lưu trữ thông tin về một đối tượng âm thanh.
    [System.Serializable]
    public class Audio
    {
        public string id; // Mã định danh của âm thanh, dùng để tra cứu hoặc phân biệt âm thanh.
        public AudioClip audio_clip; // Đối tượng AudioClip chứa dữ liệu âm thanh thực tế.
    }
}
