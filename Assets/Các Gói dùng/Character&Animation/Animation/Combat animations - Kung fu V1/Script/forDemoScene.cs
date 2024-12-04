using System.Collections;
using UnityEngine;

public class forDemoScene : MonoBehaviour
{
    private Animator animator;
    private float timechange;

    public float timeChangeAnimation = 5;
    public int animationNumber = 0;
    public AnimationClip[] animationsClip;
    public TextMesh text;

    void Start()
    {
        animator = GetComponent<Animator>();
        timechange = timeChangeAnimation;

        if (animationsClip.Length > 0)
        {
            ChangeAnimationState(animationsClip[animationNumber].name);
        }
    }

    void ChangeAnimationState(string animationName)
    {
        text.text = animationName;
        animator.Play(animationName);  // Chạy animation qua tên clip
    }

    void Update()
    {
        if (timechange > 0)
        {
            timechange -= Time.deltaTime;
        }
        else
        {
            animationNumber = (animationNumber + 1) % animationsClip.Length;
            ChangeAnimationState(animationsClip[animationNumber].name);

            timechange = timeChangeAnimation;
        }
    }
}
