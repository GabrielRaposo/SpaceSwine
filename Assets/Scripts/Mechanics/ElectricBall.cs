using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricBall : MonoBehaviour
{
    Animator animator;
    private void Awake() 
    {
        animator = GetComponentInChildren<Animator>();    
    }

    public void SetActivation(bool value) 
    { 
        animator.SetBool("Active", value);
    }
}
