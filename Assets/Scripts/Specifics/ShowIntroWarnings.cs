using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowIntroWarnings : MonoBehaviour
{
    [SerializeField] float delayBetween;
    [SerializeField] float speedUpRatio;
    [SerializeField] int speedUpDelay = 1;

    RescaleTween[] warningsSigns;

    void Start()
    {
        warningsSigns = GetComponentsInChildren<RescaleTween>();
    }

    public void CallInOrder()
    {
        if (warningsSigns == null || warningsSigns.Length < 1)
            return;

        StartCoroutine( MainSequence() );
    }

    private IEnumerator MainSequence()
    {
        float d = delayBetween;
        for (int i = 0; i < warningsSigns.Length; i++)
        {
            warningsSigns[i].Call();
            yield return new WaitForSeconds( d );

            if (i > 0 && i % speedUpDelay == 0)
            {
                d *= speedUpRatio;    
                //Debug.Log("d: " + d);
            }
        }
    }
}
