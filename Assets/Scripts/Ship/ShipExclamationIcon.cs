using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class ShipExclamationIcon : MonoBehaviour
{
    Animator animator;
    bool shown;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Show ()
    {
        if (shown)
            return;

        if (animator)
            animator.SetTrigger("Show");
        shown = true;
    }

    public void Hide () 
    {
        if (!shown)
            return;

        if (animator)
            animator.SetTrigger("Reset");
        shown = false;
    }
}
