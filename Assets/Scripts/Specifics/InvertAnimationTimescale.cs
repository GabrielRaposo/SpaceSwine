using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvertAnimationTimescale : MonoBehaviour
{
    [SerializeField] string boolName;

    void OnEnable()
    {
        Animator animator = GetComponent<Animator>();
        if (!animator)
            return;

        animator.SetBool(boolName, true);        
    }

}
