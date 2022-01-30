using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemporaryParticle : MonoBehaviour
{
    public float timeLimit;
    
    private void Start()
    {
        Debug.Log("TemporaryParticle - Start()");
        StartCoroutine(RaposUtil.WaitSeconds(timeLimit, DestroySelf));
    }

    private void DestroySelf()
    {
        Debug.Log("DestroySelf()");
        Destroy(gameObject);
    }
    
}
