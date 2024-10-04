using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDoor : MonoBehaviour
{

    private Animator _doorAnimator;
    // Start is called before the first frame update
    void Start()
    {
        _doorAnimator = GetComponent<Animator>();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _doorAnimator.SetTrigger("DoorOpen_1");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _doorAnimator.SetTrigger("DoorClose_1");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
