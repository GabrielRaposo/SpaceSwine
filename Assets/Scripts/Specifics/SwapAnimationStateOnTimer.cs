using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapAnimationStateOnTimer : MonoBehaviour
{
    [SerializeField] float duration;
    [SerializeField] List<int> stateValues;
    
    int index;

    Animator animator;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        if (!animator || stateValues.Count < 1 || duration <= 0)
        {
            Debug.Log("Please enter valid values.");
            enabled = false;
            return;
        }

        StartCoroutine( AnimationLoop() );
    }

    IEnumerator AnimationLoop()
    {
        index = 0;
        
        while (true)
        {
            yield return new WaitForSeconds(duration);

            index = (index + 1) % stateValues.Count;
            animator.SetInteger("State", stateValues[index]);
        }
    }
}
