using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LocationDisplay : MonoBehaviour
{
    [SerializeField] float holdDuration;

    Animator animator;
    TextMeshProUGUI textDisplay;

    public static LocationDisplay Instance;

    private void Awake() 
    {
        if (Instance != null)
        {
            return;
        }

        Instance = this;

        animator = GetComponentInChildren<Animator>();
        textDisplay = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void DisplayLocation(string text)
    {
        if (textDisplay)    
            textDisplay.text = text;
        if (animator)
            animator.SetTrigger("Reset");

        StopAllCoroutines();
        StartCoroutine( ShowSequence() );
    }

    IEnumerator ShowSequence()
    {
        if (animator)
            animator.SetBool("Open", true);

        yield return new WaitForSeconds(holdDuration);

        if (animator)
            animator.SetBool("Open", false);
    }
}
