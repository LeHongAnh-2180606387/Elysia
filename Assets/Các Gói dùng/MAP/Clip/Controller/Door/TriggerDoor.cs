using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDoor : MonoBehaviour
{
    private Animator _doorAnimator;

    // Tạo biến string để nhập trigger từ Unity Editor
    public string doorOpenTrigger;
    public string doorCloseTrigger;

    // Thêm các biến để cấu hình âm thanh
    public AudioClip doorOpenSound;
    public AudioClip doorCloseSound;
    private AudioSource _audioSource;

    // Start is called before the first frame update
    void Start()
    {
        _doorAnimator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();

        // Đảm bảo rằng AudioSource đã được gắn
        if (_audioSource == null)
        {
            Debug.LogError("AudioSource component is missing!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Sử dụng trigger được gán từ Unity Editor
            _doorAnimator.SetTrigger(doorOpenTrigger);

            // Phát âm thanh mở cửa
            PlaySound(doorOpenSound);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Sử dụng trigger được gán từ Unity Editor
            _doorAnimator.SetTrigger(doorCloseTrigger);

            // Phát âm thanh đóng cửa
            PlaySound(doorCloseSound);
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (_audioSource != null && clip != null)
        {
            _audioSource.PlayOneShot(clip);
        }
    }
}