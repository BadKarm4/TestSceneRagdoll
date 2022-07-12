using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FIMSpace.FProceduralAnimation;

public class AnimationController : MonoBehaviour
{
    public Animator animator;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            animator.SetBool("start", true);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            animator.SetBool("start", false);
        }

    }
}
