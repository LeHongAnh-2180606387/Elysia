using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    // Start is called before the first frame update
    private Animator mAnimator;
    void Start()
    {
        mAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(mAnimator != null) {
            if (Input.GetKey(KeyCode.Mouse0)) {
                mAnimator.SetTrigger("TrSwitchKick");
            }
            if (Input.GetKey(KeyCode.Mouse1))
            {
                mAnimator.SetTrigger("TrRightTeep");
            }
            if (Input.GetKey(KeyCode.W))
            {
                mAnimator.SetTrigger("TrBack");
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            if (Input.GetKey(KeyCode.S))
            {
                mAnimator.SetTrigger("TrForward");
                transform.rotation = Quaternion.Euler(0, 180, 0);
            }
            if (Input.GetKey(KeyCode.A))
            {
                mAnimator.SetTrigger("TrLeft");
                transform.rotation = Quaternion.Euler(0, -90, 0);
            }
            if (Input.GetKey(KeyCode.D))
            {
                mAnimator.SetTrigger("TrRight");
                transform.rotation = Quaternion.Euler(0, 90, 0);
            }
        }
    }
}
