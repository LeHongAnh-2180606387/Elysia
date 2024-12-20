using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDoor : MonoBehaviour
{
    private Animator _doorAnimator;

    // Tạo biến string để nhập trigger từ Unity Editor
    public string doorOpenTrigger;
    public string doorCloseTrigger;

    // Start is called before the first frame update
    void Start()
    {
        _doorAnimator = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Sử dụng trigger được gán từ Unity Editor
            _doorAnimator.SetTrigger(doorOpenTrigger);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Sử dụng trigger được gán từ Unity Editor
            _doorAnimator.SetTrigger(doorCloseTrigger);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
