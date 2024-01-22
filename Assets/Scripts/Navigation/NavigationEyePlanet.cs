using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class NavigationEyePlanet : MonoBehaviour
{
    Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        StartCoroutine( WaitToCallAnimation() );
    }

    IEnumerator WaitToCallAnimation()
    {
        while (true) 
        {
            float random = Random.Range(5f, 40f);
            yield return new WaitForSeconds( random );

            animator.SetTrigger("Show");
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
